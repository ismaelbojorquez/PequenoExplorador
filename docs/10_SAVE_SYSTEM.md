# Sistema de guardado local

Estado: schema v2 implementado en Prompt 11 sobre la foundation de Prompt/Fase 09. Es foundation técnica sin gameplay, cloud, cuentas ni entitlements.

## Contrato y límites

```text
Domain.PlayerProgress
        ↓ ISaveService / AutosaveCoordinator (Application)
Infrastructure.LocalSaveService
        ├─ UnityJsonSaveSerializer → SaveEnvelope + DTO v1/v2
        ├─ ISaveMigration[] → pasos puros n→n+1
        └─ IFileStore → LocalFileStore(Application.persistentDataPath)
Bootstrap compone; Presentation solo recibe SaveUserNotice.
```

`PlayerProgress` contiene únicamente estrellas no negativas, listas de IDs técnicos y preferencias locales (`Guía estándar`/`Más guía`, música, SFX, narración e idioma ES/EN). Las listas world/discovery/mission nacen vacías; no implementan sistemas de gameplay. No se serializan `GameObject`, `ScriptableObject`, `AssetReference`, diccionarios, tipos polimórficos ni nombres de assemblies.

`AppConfig` y sus feature flags son runtime inmutable y no se copian al save. Save conserva solo preferencias adultas mutables; perfil de build, budgets, versión técnica y flags se vuelven a resolver desde Content/Bootstrap en cada arranque según [`RUNTIME_CONFIGURATION.md`](RUNTIME_CONFIGURATION.md).

## Formato v2

Serializador: `UnityEngine.JsonUtility`, provisto por el módulo builtin fijado `com.unity.modules.jsonserialize` `1.0.0`. No se añadió paquete. Es compatible con el Editor `6000.3.22f1`/IL2CPP y suficiente porque los DTOs son clases cerradas con campos explícitos y arrays. Cambiar serializador o representación requiere ADR y migración, no una sustitución silenciosa.

Envelope lógico:

```json
{
  "schemaVersion": 2,
  "checksum": "sha256-hex-del-payload-utf8",
  "payload": "json-escapado-del-dto-v2"
}
```

El payload v2 contiene:

| Campo | Propósito | Dato infantil/PII |
|---|---|---|
| `appVersion` | Diagnóstico de compatibilidad de escritura. | No. |
| `stars` | Contador local determinista; inicialmente `0`. | No. |
| `worldIds` | IDs técnicos; vacío en esta fase. | No. |
| `discoveryIds` | IDs técnicos; vacío en esta fase. | No. |
| `completedMissionIds` | IDs técnicos; vacío en esta fase. | No. |
| `settings` | Guía, música, SFX, narración y `localeCode` (`es`/`en`); no edad. | No. |
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

## Carga, migración y downgrade

1. Sin primary/backup: crear `PlayerProgress` default en español y escribir schema v2.
2. Primary v2 válido: validar checksum, DTO/invariantes y cargar.
3. Primary antiguo: aplicar registro ordenado `n→n+1`; existen migraciones reales `v0→v1→v2`. La migración v1→v2 conserva progreso/settings y añade `localeCode=es`; después se reescribe v2 y se conserva el original como backup.
4. Primary corrupto: intentar backup; si pasa, cargarlo, emitir `ProgressRecovered` y reparar primary preservando backup.
5. Schema futuro: entrar en modo read-only, emitir `NewerSaveVersionDetected` y bloquear save/reset. Nunca sobrescribirlo con el schema actual.
6. Primary y backup inválidos: fallo recuperable; no pérdida/sobrescritura silenciosa.

Cada schema nuevo añade DTO, mapper y migración pura; nunca se edita una migración ya liberada para reinterpretar archivos históricos. Tests destructivos completos vuelven a ejecutarse en F34.

## Checkpoints y lifecycle

`AutosaveCoordinator` conserva solo el progreso más reciente durante la ventana de debounce de 500 ms y serializa escrituras mediante `ISaveService`. `FlushAsync` elimina la espera y permite evidencia determinista. Futuras features solicitan checkpoints tras una recompensa confirmada, misión confirmada, cambio de settings o salida segura; no guardan por frame ni escriben archivos directamente.

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

## Matriz automatizada F09

| Caso | Evidencia |
|---|---|
| Default v2, idioma y round-trip | EditMode. |
| JSON determinista y sin campos de perfil personal | EditMode. |
| Fallo write/flush/commit | Failpoints in-memory; primary/backup invariantes. |
| Truncado/checksum | Rechazo de primary y recuperación de backup. |
| Backup no reemplazado por corrupto | Comparación byte a byte tras reparación. |
| v0→v1→v2, v1→v2 y migración ausente | Migración/backup o fallo conservador sin rewrite. |
| Schema futuro | Read-only; save bloqueado y bytes intactos. |
| Cancelación antes de commit | Excepción de cancelación y último primary intacto. |
| Requests múltiples | Coalescing al checkpoint más reciente. |
| Replace físico repetido | `LocalFileStore` en directorio temporal controlado. |
| Recreación/recarga | PlayMode recrea servicio tras recargar escena. |

El build Android prueba compilación IL2CPP/ARM64 del sistema. La lectura/escritura en dispositivo físico permanece separada y debe reportarse `NOT RUN` si no hay hardware soportado.
