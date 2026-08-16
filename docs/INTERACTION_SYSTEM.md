# Interacción contextual de mundo

Contrato ejecutable de Prompt 17. Su alcance termina en foco, auto-acercamiento, prompt y resultado técnico; discovery, fotografía, aprendizaje y recompensas se conectan mediante casos de uso posteriores.

## Flujo y fronteras

```text
tap Explorer
  → Presentation.InteractionDetector (RaycastNonAlloc + índice collider→target)
  → Application.InteractionTargetSelector (prioridad, distancia, ID)
  → InteractionCoordinator
      ├─ IInteractionApproach → locomoción existente
      ├─ IInteractable → acción concreta futura
      └─ snapshot/result → prompt localizado + cue de audio
```

`Domain` aporta `InteractionId`; `Application` posee contratos, selección y estados; `Content` mapea `InteractionDefinitionAsset` a definiciones readonly; `Presentation` adapta colliders, puntos, indicador y UI; `Bootstrap` compila el catálogo y enlaza exactamente una raíz al cargar Selva. No hay evento global, lookup por frame ni categoría animal en el núcleo.

## Contratos runtime

| Contrato | Responsabilidad | No hace |
|---|---|---|
| `InteractionDefinition` | ID, copy/audio, rango, cooldown, prioridad y metadata editorial. | No guarda sesión ni referencia assets Unity. |
| `IInteractable` | Disponibilidad, vida y ejecución idempotente de una intención contextual. | No decide locomoción, UI ni discovery. |
| `InteractionContext` | Posición del explorador, punto de interacción y tiempo inyectado. | No expone GameObjects. |
| `InteractionCoordinator` | Foco único, approach, cancelación, suspensión, cooldown y resultado. | No hace raycast ni reproduce feedback. |
| `InteractionDetector` | Candidatos de un tap usando un índice construido al bind. | No usa `GetComponent`/`Find*` por frame. |

Estados: `None → Approaching → Ready → Completed`; ramas controladas `Unavailable`, `CoolingDown`, `Cancelled`, `Missing` y `Suspended`. Un nuevo foco cancela el path anterior. UI, pause, target destruido o unload limpian foco/listeners; una acción repetida en `Completed` no vuelve a ejecutar el target.

## Selección y UX

Entre targets superpuestos se elige prioridad descendente, luego distancia del hit ascendente y finalmente `InteractionId` ordinal. El orden es determinista e independiente de la categoría. El tap en target lejano muestra foco y usa el mismo tap-to-move; al entrar en `1.35 m` aparece una acción `64×64` o mayor dentro de safe area. Cancelar siempre está disponible. Un target no disponible usa icono, copy y voz amable; no expone excepciones ni penaliza.

## Authoring y fixtures

El catálogo actual contiene exactamente tres fixtures `Draft / PH_ / ReleaseBlocked`, todos con `WorldInteractableView`. El compilador exige esos IDs baseline pero acepta definiciones adicionales sin cambiar el núcleo:

| ID | Forma semántica | Prioridad | Disponibilidad |
|---|---|---:|---|
| `interaction.fixture.animal` | animal neutral | 70 | disponible |
| `interaction.fixture.plant` | planta neutral | 60 | disponible |
| `interaction.fixture.object` | objeto neutral | 50 | no disponible, prueba de fallback |

Cada fixture necesita collider trigger amplio, punto sobre NavMesh e indicador visual. Renombrar el asset no cambia el ID. El validator bloquea duplicados, key/cue ausente, punto fuera de NavMesh, collider pequeño/no trigger, wiring múltiple, target UI menor a `64×64`, hardcode animal en Application y cualquier fixture `PH_` en Release.

## Límites y siguiente conexión

- `IInteractable` no concede discovery, estrellas, misión ni hecho educativo.
- Prompt 18 deberá implementar el caso de uso concreto y persistir su resultado; no debe leer el fixture/archivo desde UI.
- Los materiales, geometría, icono y copy son placeholders y no autorizan Release.
- Touch, latencia, ergonomía, audio y target size en Android físico permanecen requeridos antes de Gate C.

La matriz de pruebas y comandos está en [`18_TESTING.md`](18_TESTING.md); assets temporales en [`ART_ASSET_REQUIREMENTS.md`](ART_ASSET_REQUIREMENTS.md).
