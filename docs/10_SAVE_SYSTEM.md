# Sistema de guardado local

Estado: schema v7 implementado por Prompt 21. Persiste discovery/foto y la economía idempotente de Estrellas de Explorador; los PNG viven en un store local separado. Sigue sin cloud, cuentas ni entitlements.

## Contrato y límites

```text
Domain.PlayerProgress
        ↓ ISaveService / AutosaveCoordinator (Application)
Infrastructure.LocalSaveService
        ├─ UnityJsonSaveSerializer → SaveEnvelope + DTO v1…v7
        ├─ ISaveMigration[] → pasos puros n→n+1
        └─ IFileStore → LocalFileStore(Application.persistentDataPath)
Bootstrap compone; Presentation solo recibe SaveUserNotice.
```

`PlayerProgress` contiene `ExplorerStars` no negativas, transaction keys económicas, ledger reciente acotado, listas world/mission, records `DiscoveryProgress`, grants procesados, metadata `PhotoProgress` y preferencias locales (`Guía estándar`/`Más guía`, cinco volúmenes, subtítulos e idioma ES/EN). No se serializan pixels, `GameObject`, `ScriptableObject`, `AssetReference`, diccionarios, tipos polimórficos ni nombres de assemblies.

`AppConfig` y sus feature flags son runtime inmutable y no se copian al save. Save conserva solo preferencias adultas mutables; perfil de build, budgets, versión técnica y flags se vuelven a resolver desde Content/Bootstrap en cada arranque según [`RUNTIME_CONFIGURATION.md`](RUNTIME_CONFIGURATION.md).

## Formato v7

Serializador: `UnityEngine.JsonUtility`, provisto por el módulo builtin fijado `com.unity.modules.jsonserialize` `1.0.0`. No se añadió paquete. Es compatible con el Editor `6000.3.22f1`/IL2CPP y suficiente porque los DTOs son clases cerradas con campos explícitos y arrays. Cambiar serializador o representación requiere ADR y migración, no una sustitución silenciosa.

Envelope lógico:

```json
{
  "schemaVersion": 7,
  "checksum": "sha256-hex-del-payload-utf8",
  "payload": "json-escapado-del-dto-v7"
}
```

El payload v7 contiene:

| Campo | Propósito | Dato infantil/PII |
|---|---|---|
| `appVersion` | Diagnóstico de compatibilidad de escritura. | No. |
| `stars` | Saldo local de Estrellas de Explorador; entero ≥0. | No. |
| `processedEconomyTransactionIds` | Keys técnicas durables que impiden repetir grants/spends. | No. |
| `economyLedger` | Últimas 32 operaciones: kind, IDs, amount y balance; sin timestamps/sesiones. | No. |
| `worldIds` | IDs técnicos; vacío en esta fase. | No. |
| `discoveries[]` | `id`, count ≥1 y primer día local agregado opcional `yyyy-MM-dd`. | No identifica persona; no guarda hora/zona. |
| `processedDiscoveryGrantIds` | Claves técnicas `grant.*` ya aplicadas para impedir doble count/grant. | No. |
| `photos[]` | ID, referencia relativa, score, ancho/alto y bytes de la mejor thumbnail; nunca pixels. | No. |
| `completedMissionIds` | IDs técnicos; vacío en esta fase. | No. |
| `settings` | Guía, `localeCode` ES/EN, volúmenes Master/Music/Ambience/Effects/Voice normalizados y subtítulos; no edad. | No. |
| `metadata.saveSequence` | Secuencia monotónica local para diagnóstico/recovery. | No identifica persona/dispositivo. |

No se guarda nombre, edad, fecha de nacimiento, voz, ubicación, cuenta, device ID, advertising ID, contacto ni telemetría. SHA-256 detecta corrupción accidental; **no es cifrado, autenticación ni protección frente a una persona con acceso al archivo**. El MVP no guarda datos sensibles que justifiquen afirmar cifrado.

## Archivos y atomicidad

La ruta raíz se resuelve solo en Bootstrap mediante `Application.persistentDataPath/Save`; nunca se registra una ruta personal en configuración o logs. Nombres fijos:

| Slot | Nombre lógico | Uso |
|---|---|---|
| Primary | `player-progress.json` | Último checkpoint comprometido. |
| Backup | `player-progress.backup.json` | Primary anterior válido. |
| Temporary | `player-progress.tmp` | Escritura en curso, nunca fuente de progreso. |

Una escritura normal ejecuta `temp → write → flush/fsync → replace(primary, backup)`. Cancelación se admite antes del replace; desde el replace la sección es deliberadamente no cancelable y breve. Un fallo previo descarta temp y conserva primary/backup. Al iniciar, cualquier temp residual de un cierre interrumpido se descarta.

La restauración de backup usa replace de primary **sin rotar primary sobre backup**. Así, un primary corrupto nunca reemplaza el backup válido. Si no existe backup válido, la inicialización falla de forma recuperable y conserva los archivos para inspección; no crea un default sobre evidencia dañada.

El binario de fotos usa `Application.persistentDataPath/Photos`: PNG determinista por discovery, `photos-index.json` y temps. Sus límites son 512 KiB/archivo, 64 entradas y 32 MiB; no comparte el backup/checksum del save. Si falta o falla, el progreso v7 sigue válido y Presentation usa imagen canónica. Detalle: [`PHOTOGRAPHY_SYSTEM.md`](PHOTOGRAPHY_SYSTEM.md).

## Carga, migración y downgrade

1. Sin primary/backup: crear `PlayerProgress` default en español y escribir schema v7.
2. Primary v7 válido: validar checksum, DTO/invariantes y cargar.
3. Primary antiguo: aplicar `v0→v1→v2→v3→v4→v5→v6→v7`. v5→v6 añade `photos=[]`; v6→v7 conserva `stars` y añade transaction keys/ledger vacíos sin inventar grants. Se reescribe v7 y se conserva el original como backup.
4. Primary corrupto: intentar backup; si pasa, cargarlo, emitir `ProgressRecovered` y reparar primary preservando backup.
5. Schema futuro: entrar en modo read-only, emitir `NewerSaveVersionDetected` y bloquear save/reset. Nunca sobrescribirlo con el schema actual.
6. Primary y backup inválidos: fallo recuperable; no pérdida/sobrescritura silenciosa.

Cada schema nuevo añade DTO, mapper y migración pura; nunca se edita una migración ya liberada para reinterpretar archivos históricos. Tests destructivos completos vuelven a ejecutarse en F34.

## Checkpoints y lifecycle

`AutosaveCoordinator` conserva solo el progreso más reciente durante la ventana de debounce de 500 ms, expone ese snapshot como `Latest` para que varios productores no partan de un `Current` obsoleto y serializa escrituras mediante `ISaveService`. `FlushAsync` elimina la espera y permite evidencia determinista. Discovery solicita checkpoint tras aplicar una grant; no guarda por frame ni lee/escribe archivos directamente.

En pause se solicita el estado actual y se espera hasta 1 segundo. En quit se inicia un flush best-effort con presupuesto de 250 ms y nunca se bloquea indefinidamente. El último checkpoint ya comprometido siempre prevalece; cerrar durante una escritura no duplica recompensa porque los sistemas de recompensa futuros deberán confirmar estado antes de solicitar el checkpoint.

## Reset e inspección

Release no incluye menú ni bypass. En Editor existe:

- `Pequeño Explorador/Development/Save/Inspect Files`;
- `Pequeño Explorador/Development/Save/Reset Local Progress`.

Reset muestra confirmación explícita y elimina solo los tres nombres conocidos. Una UI runtime de reset deberá vivir en el área adulta; no se implementa aquí. `ISaveService.ResetAsync` existe para esa futura fachada, pero una versión futura read-only sigue bloqueada incluso ante reset técnico ordinario.

## Recuperación manual

1. Cerrar app/Play Mode antes de intervenir.
2. Usar `Inspect Files`; copiar primary/backup fuera del proyecto como evidencia privada y no subirlos a Git/issues.
3. No editar JSON ni recalcular checksum manualmente para aparentar recuperación.
4. Si el estado mostró `Progress restored safely`, reiniciar y verificar; el backup válido se conservó.
5. Si el estado indica versión nueva, abrir con la versión de aplicación que creó el archivo; **no resetear ni hacer downgrade**.
6. Si primary y backup fallan, conservar ambos y usar Reset solo tras decisión adulta consciente de que elimina el progreso local.

No hay soporte cloud ni recuperación remota. Una copia manual puede contener progreso de juego aunque no contenga PII; se trata como dato privado local.

## Matriz automatizada Prompt 21

| Caso | Evidencia |
|---|---|
| Default v7, idioma, audio settings, discovery/foto, wallet, transaction keys/ledger y round-trip | EditMode. |
| JSON determinista y sin campos de perfil personal | EditMode. |
| Fallo write/flush/commit | Failpoints in-memory; primary/backup invariantes. |
| Truncado/checksum | Rechazo de primary y recuperación de backup. |
| Backup no reemplazado por corrupto | Comparación byte a byte tras reparación. |
| v0→…→v7, v4→v5, v5→v6, v6→v7 y migración ausente | Migración/backup o fallo conservador sin rewrite. |
| Grant/spend, insuficiente, overflow, retry y ledger 32 | EditMode; sin duplicación ni saldo negativo. |
| Fotografía→discovery→estrella→reload | PlayMode; retry devuelve `AlreadyProcessed`. |
| Schema futuro | Read-only; save bloqueado y bytes intactos. |
| Cancelación antes de commit | Excepción de cancelación y último primary intacto. |
| Requests múltiples | Coalescing al checkpoint más reciente. |
| Replace físico repetido | `LocalFileStore` en directorio temporal controlado. |
| Discovery first/repeat/reload | PlayMode interactúa, flush, recarga Selva y repite sin segundo grant único. |
| Foto válida/inválida, best-photo, storage fallback y unload | EditMode/PlayMode; progreso prevalece y recursos temporales vuelven a cero. |

El build Android prueba compilación IL2CPP/ARM64 del sistema. La lectura/escritura en dispositivo físico permanece separada y debe reportarse `NOT RUN` si no hay hardware soportado.
