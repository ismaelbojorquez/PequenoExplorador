# Gate B — Adenda de reparación de arranque Android

- Fecha: 2026-08-17 (`America/Mexico_City`).
- Commit de entrada: `24e0d2075d78d59e86562195a9e69e0c04640364`.
- Dispositivo: HONOR DNY-NX9, Android 16.
- Resultado del defecto: **RESOLVED**.
- Resultado Gate B: **CONDITIONAL**; esta adenda no sustituye la matriz touch ni el playtest infantil/no lector.

## Incidente y causa raíz

La aplicación cerraba después de “Made with Unity”. `adb logcat` reprodujo `assets/bin/Data/level0 is corrupted!`, `Position out of bounds`, `CachedReader::OutOfBoundsError` y `SIGTRAP` mientras Unity deserializaba un `MonoBehaviour`. El APK local y el extraído del teléfono eran byte a byte idénticos —SHA-256 `a633aace2f34449c69e848ed0adcf73585d88c742953cb30c3d0a3a809d88148`— y `unzip -t` pasó, descartando corrupción de transporte.

`Bootstrap.unity` contenía seis documentos YAML `!u!115 MonoScript` y 191 referencias locales. Cinco clases ya tenían scripts externos; `CustomizationOptionButtonView` compartía archivo con otro `MonoBehaviour` y no tenía asset homónimo. Import, suites y build no detectaban esa forma inválida, pero el Player Android serializaba un `level0` que abortaba al cargar.

## Corrección y prevención

- Se separaron `CustomizationSlotButtonView` y `CustomizationOptionButtonView` en archivos homónimos, conservando el GUID histórico del primero.
- Una reparación Editor resolvió clase/namespace/assembly contra exactamente un `MonoScript` externo, sustituyó las 191 referencias y retiró los seis documentos locales sin reconstruir componentes ni perder campos.
- `RuntimeSceneSerializationValidationService` recorre todas las escenas runtime habilitadas. `SCENE002` bloquea `!u!115`; `SCENE003` bloquea `m_Script` local.
- El gate se ejecuta dentro del pipeline de contenido/compile/build. EditMode prueba la escena real y una fixture inválida que debe producir ambos códigos.

## Evidencia

| Comando o inspección | Resultado |
|---|---|
| APK anterior local vs extraído + `unzip -t` | `PASS`: idénticos e íntegros; el fallo estaba dentro de la escena serializada. |
| Reparación CLI | `PASS`: `PE_RUNTIME_SCENE_REPAIR_OK repairedReferences=191`. |
| Conteo posterior de `!u!115` y `m_Script` local | `PASS`: 0 y 0. |
| `scripts/compile` | `PASS`. |
| `scripts/test-editmode` | `PASS`: 169/169. |
| `scripts/validate` | `PASS`: repository, compile/validadores, Addressables, EditMode 169/169, PlayMode 29/29 y APK Development. |
| APK final de pipeline | `PASS`: 67,444,690 bytes; SHA-256 `5c382e6c3340f569350ef9ee765566fd0f0377d9403b847df5ae411c33253b80`; API 26/36, IL2CPP/ARM64. |
| Instalación `adb install -r` | `PASS`; no se ejecutó `pm clear` y se conservaron datos. |
| Primer arranque físico, 15 s | `PASS` sobre el artefacto runtime-equivalente SHA-256 `a7173c3647b36559d67e41386ee84f744012346dd6c4bcb8ac6a88b4bfe89384`: PID 19641 vivo; `ApplicationReady`, `ServicesReady` y transición completada a Camp; cero fatal/`SIGTRAP`/corrupción/out-of-bounds. El rebuild final solo añadió resolución portable en tooling Editor y regeneró metadata. |
| Segundo force-stop/rearranque | `NOT RUN`: el teléfono se desconectó y `adb devices -l` quedó vacío. |

## Decisión

El incidente de cierre inmediato queda resuelto con evidencia física y una regresión fail-closed. Gate B regresa del `FAIL` temporal de reparación a `CONDITIONAL`, no a `PASS`: falta completar el recorrido touch físico, rotación/Back/audio/focus/suspend/performance, repetir arranques y realizar el playtest consentido 4–9/no lector. Prompt 31 y el escalado de contenido permanecen bloqueados hasta una adenda o reauditoría final.
