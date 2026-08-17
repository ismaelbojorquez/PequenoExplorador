# Tutorial contextual de la primera expedición

Estado: implementado en Prompt 28 como FTUE local y versionado. Es un sistema de guía, no un requisito punitivo ni una fuente de analytics remoto. Gate B permanece `FAIL` hasta integrar y auditar el journey de Prompt 29.

## Contrato y ownership

`TutorialDefinitionAsset` pertenece a Content y compila a una definición readonly. `TutorialCoordinator` en Application observa triggers semánticos, decide el paso y expone el conjunto mínimo de acciones permitido. `TutorialView` en Presentation resuelve copy/voz, spotlight y gesto. Bootstrap conecta outcomes ya existentes; ninguna feature busca objetos por nombre ni contiene una rama de tutorial.

Save schema v12 conserva únicamente `tutorialId`, `contentVersion`, `stepIndex` y `status`, además de la preferencia `GuidanceMode`. No se guardan taps, errores, tiempos, edad, fecha de nacimiento ni historial conductual. Cambiar la definición exige incrementar `contentVersion`; una versión distinta reinicia solo el tutorial y preserva el resto del progreso.

## Secuencia Vertical Slice v1

| # | Trigger semántico | Acción permitida | Spotlight | Copy ES resumido | Ayuda Más guía / estándar |
|---:|---|---|---|---|---|
| 1 | `ExpeditionEntered` | entrar a expedición | mapa | Vamos a la Selva | 6 s / 12 s |
| 2 | `MovementAccepted` | moverse | suelo | Toca el camino | 6 s / 12 s |
| 3 | `InteractionCompleted` | mover/interactuar | tucán | Acércate y saluda | 6 s / 12 s |
| 4 | `PhotoCaptured` | fotografiar | shutter | Toma una foto | 6 s / 12 s |
| 5 | `Continue` | continuar | discovery/estrella | Mira lo que descubriste | 6 s / 12 s |
| 6 | `CampReturned` | volver | back/Camp | Regresa al Campamento | 6 s / 12 s |
| 7 | `AlbumOpened` | abrir álbum | álbum | Abre tu álbum | 6 s / 12 s |

Solo existe una instrucción activa. El tiempo revela ayuda visual o replay; nunca completa pasos. Eventos duplicados o fuera de orden se ignoran. Skip tiene la misma accesibilidad que replay y no quita progreso/recompensa; el tutorial completo puede repetirse desde Camp. Back y pausa siempre permanecen disponibles.

## Presentación y accesibilidad

La primera ejecución ofrece `Más guía` y `Guía estándar` sin pedir edad. El selector bloquea el resto de la pantalla hasta elegir, pero no oculta salida/pausa del sistema. El overlay usa safe area, TMP, targets de al menos 64 unidades, icono+subtítulo y gesto geométrico propio. Reduce-motion elimina el pulso sin eliminar la pista. ES/EN están completos; pseudo permanece Development.

Los siete cues tienen pares ES/EN mono/48 kHz generados internamente para probar cola, ducking, replay y subtítulos. Son `PH_`, no narración humana, y bloquean Release. Se requiere voz humana licenciada, pronunciación/Child UX, mezcla e inteligibilidad en dispositivo antes de aprobarlos.

## Validación y recovery

```sh
scripts/setup-localization
scripts/setup-audio
scripts/setup-tutorial
scripts/setup-design-system
scripts/validate-tutorial
scripts/test-editmode
scripts/test-playmode
scripts/validate
```

EditMode cubre orden, eventos duplicados, ayudas 6/12, gating, skip/replay, versión, reanudación y migración v11→v12. PlayMode cubre elección no lectora, acción equivocada, checkpoint/instancia nueva, pausa, ES/EN, recorrido completo y 4:3/16:9/20:9/16:10. Si un asset/cue/key falta, el validator falla con `TUTORIALnnn`; Release falla mientras la definición o narración sean placeholder.
