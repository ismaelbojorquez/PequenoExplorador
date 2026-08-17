# Camp — hub y progreso visual

Estado: baseline funcional de Prompt 25. El Camp es un hub local y seguro; no es tienda, city builder ni barrera al contenido educativo. Todo lo visual de esta fase lleva prefijo `PH_` y continúa bloqueado para Release.

## Mapa data-driven

| Estación | ID / acción | Estado actual | Responsabilidad |
|---|---|---|---|
| Expedición | `camp-station.expedition` / `camp-action.expedition` | Activa | Abrir la Selva mediante `WorldLoadUseCase`; no carga escenas por string desde la UI. |
| Álbum | `camp-station.album` / `camp-action.album` | Activa | Abrir el álbum read-only ya existente. |
| Personalización | `camp-station.customization` / `camp-action.customization` | Futura, no interactiva | Reserva visual para Prompt 26; no simula tienda ni compra. |
| Personas adultas | `camp-station.parents` / `camp-action.parents` | Futura, no interactiva y `parentRestricted` | No se habilita hasta existir un parental gate real; el badge no se considera seguridad. |

`CampCatalog` indexa estaciones y mejoras por IDs tipados. `CampSceneRoot` asocia anchors y variantes visuales; agregar una estación o mejora no requiere un switch de edificio. Las acciones de navegación se inyectan desde Bootstrap mediante `CampStationActionId`.

## Primera mejora

| ID | Estación | Costo provisional | Antes → después | Requisito | Efecto |
|---|---|---:|---|---|---|
| `camp-upgrade.observation-corner` | Álbum | 3 Estrellas | `PH_CampObservationTable_Before` → `PH_CampObservationCorner_After` | Ninguno | Cambio visual permanente; no desbloquea ni bloquea aprendizaje. |

El costo de 3 permite comprobar el loop actual con recompensas de discovery + misión, pero sigue sujeto a playtest/tuning humano. Preview no muta estado. Confirmación ejecuta `PurchaseCampUpgradeUseCase`, que valida definición, prerequisitos y saldo; construye un único `PlayerProgress` con spend, transaction key, ledger y unlock antes de un solo checkpoint. Commit fallido conserva el snapshot anterior y el mismo transaction ID permite retry sin doble gasto.

## Persistencia y degradación

Save schema v10 introdujo `unlockedCampUpgradeIds`; schema v11 lo conserva y añade únicamente estado de personalización. La configuración y los costos siguen en Content; Save no copia definitions. Una definition retirada conserva el ID histórico sin romper la carga, aunque la vista no lo presenta.

El perfil Release rechaza los assets `PH_`. No hay IAP, enlace a compra, moneda premium, entitlement, red ni parental bypass. Saldo insuficiente muestra una sugerencia localizada amable y deja progreso/saldo intactos.

## Authoring, validación y recuperación

- `scripts/setup-camp`: regenera catálogo, cuatro estaciones, variante visual antes/después, escena y wiring UI preservando GUIDs.
- `scripts/validate-camp`: comprueba catálogo, duplicados/ciclos/referencias, Addressables locales, anchors de escena, safe area, targets `≥64×64`, restricción adulta y fronteras de Application.
- `scripts/validate`: integra Camp en compile, Addressables, tests y APK Development.

Si el setup falla, revisar `artifacts/logs/setup-camp.log`; no editar GUIDs/scene YAML a mano. Si el validator falla con `CAMP005`, el bloqueo Release es intencional hasta sustituir y aprobar arte final.
