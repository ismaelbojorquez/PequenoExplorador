# Preflight — Prompt 17

Fecha: 2026-08-16 14:56 (`America/Mexico_City`). Resultado inicial: `PASS`; árbol limpio y sin colisiones ajenas.

## Estado observado antes de editar

| Comprobación | Evidencia | Resultado |
|---|---|---|
| Contrato y fuentes | Lectura completa de `AGENTS.md`, `STATUS`, índice, planes, GDD, loop, UI/UX, input/accesibilidad, modelo de contenido, arquitectura, testing, decisiones, riesgos, standards/review y Prompt 17. | `PASS`; el alcance se limita a interacción contextual `PH_`, sin discovery, fotografía ni actividad final. |
| Git | `git status --short --branch`, `git branch --show-current`, `git log -1 --format=fuller` y ambos diffs. | `main`, limpio, `ahead 8`; HEAD `d17935f77d5d65a27750aa82d557642177344c2c`. |
| Implementación | Se inspeccionaron Bootstrap, Explorer, Input, Audio, Localization, Content, scene flow, asmdefs, Jungle y tests relacionados. | Existe tap-to-move/NavMesh, pero no hay `IInteractable`, coordinator, detector, prompt ni fixtures de interacción. |
| Baseline real | `scripts/validate`. | Código `0` en `80.40 s`: repository/shell, compile/validadores, Addressables `41` locations/`893,594` bytes, EditMode `94/94`, PlayMode `14/14` y APK Development. |
| Loop actual | PlayMode registra `60` frames de locomoción y tres ciclos; inspección confirma que cada tap Explorer cae directamente en raycast de suelo/locomoción. | No existe prioridad de interactable; FPS Editor batch `11065.8` es diagnóstico, no perfil de dispositivo. |
| Hardware | `adb devices`. | Lista vacía; touch smoke Android físico queda `NOT RUN` salvo cambio del entorno. |

## Hallazgos y límites

- `02_TECHNICAL_ARCHITECTURE.md` aún enumera referencias `Unity.AI.Navigation` retiradas de Presentation/tests al cierre de Prompt 16. La implementación/asmdefs prevalece; esta fase corregirá el diagrama.
- El prompt autoriza tres fixtures animal/planta/objeto solo como demostración neutral. No son discoveries, no contienen hechos y deben conservar prefijo/metadata `PH_` y bloqueo Release.
- El detector puede resolver colliders en el tap mediante un índice preparado al bind; se prohíben búsquedas `GetComponent`/`Find*` por frame.
- La UI de prompt debe vivir bajo un `SafeAreaFitter`, usar copy ES/EN y mantener acción/cancelación táctil amplia. El audio reutilizará cues semánticos existentes; no se añadirá audio factual/final.
