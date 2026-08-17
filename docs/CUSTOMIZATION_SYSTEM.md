# Sistema de personalización inclusiva

Estado: baseline de Prompt 26 implementada. El sistema es local, data-driven y no selecciona ni infiere género. Todos los visuales actuales son `PH_`, propios y bloqueados para Release hasta sustitución/revisión de arte.

## Contrato infantil y comercial

- La persona elige apariencia por piezas; no existen etiquetas “niño/niña”, cuerpo por género ni perfil de edad.
- Siempre hay una combinación diversa y gratuita. Nada educativo depende de poseer un cosmético.
- Preview no muta progreso. Desbloquear y equipar son decisiones separadas.
- Estrellas de Explorador son la única moneda: se ganan jugando y nunca se compran. No hay IAP, premium, azar, rareza, tiempo limitado o FOMO.
- Una opción retirada degrada al default gratuito del slot sin romper el save.

## Flujo y ownership

```text
Content ScriptableObjects
  → CustomizationCatalog readonly
  → UnlockCosmeticUseCase / EquipCosmeticUseCase
  → IEconomyRepository + PlayerProgress v11
  → CustomizationView / ExplorerCustomizationRig
```

Content posee slots, opciones, nombres localizados, costo, requisito, color, visual ID y tags de compatibilidad. Application valida disponibilidad, compatibilidad y transacciones. Infrastructure solo serializa ownership/equipped. Presentation aplica variantes y `MaterialPropertyBlock`; no lee JSON, IAP ni assets mediante `AssetDatabase` en runtime. Bootstrap compone el catálogo y enlaza el rig de Camp o Selva.

## Slots y catálogo provisional

| Orden | Slot ID | Opciones `PH_` | Default gratuito | Unlock adicional |
|---:|---|---|---|---|
| 0 | `customization-slot.skin-tone` | claro, medio, oscuro, cálido | `cosmetic.skin.light` | Ninguno; 4 tonos iniciales |
| 1 | `customization-slot.hair` | rizos, ondas, dos chonguitos | `cosmetic.hair.curls` | Ninguno; 3 estilos iniciales |
| 2 | `customization-slot.shirt` | verde selva, sol naranja, azul río | `cosmetic.shirt.jungle` | azul río: 3 estrellas |
| 3 | `customization-slot.pants` | arena, azul noche | `cosmetic.pants.sand` | Ninguno |
| 4 | `customization-slot.shoes` | sendero, coral | `cosmetic.shoes.trail` | Ninguno |
| 5 | `customization-slot.hat` | sin sombrero, sombrero de sol | `cosmetic.hat.none` | sombrero: 2 estrellas |
| 6 | `customization-slot.backpack` | campo, hoja | `cosmetic.backpack.field` | hoja: 2 estrellas |
| 7 | `customization-slot.explorer-tool` | cámara, binoculares | `cosmetic.tool.camera` | binoculares: mejora `camp-upgrade.observation-corner` |

Los costos son tuning provisional, no precio comercial. `hair.puffs` y `hat.sun` demuestran un conflicto por `cosmetic-tag.hair.volume-wide`; una combinación inválida conserva el equip previo y propone elegir otra.

## Persistencia y atomicidad

Schema v11 agrega exclusivamente:

- `unlockedCosmeticIds[]`: ownership local de opciones no iniciales;
- `equippedCosmetics[]`: pares únicos `slotId/cosmeticId`.

La migración pura v10→v11 conserva todo el progreso y comienza ambos arrays vacíos. Definitions/defaults/costos no se copian al save. `WithEconomyAndCosmeticUnlock` crea un snapshot con saldo, transaction key, ledger acotado y ownership; si `Commit` falla, no cambia nada y el retry es seguro. Equipar ocurre después y no vuelve a gastar.

## Visuales y rendimiento

`PH_Explorer.prefab` conserva `ExplorerLocomotionRoot` y añade un solo `ExplorerCustomizationRig` con ocho bindings. Camp usa una instancia visual sin `NavMeshAgent`/locomotion; Selva usa el prefab completo. Colores se aplican con `MaterialPropertyBlock` sobre materiales compartidos. Pelo, sombrero y herramienta cambian roots simples; no se crean instancias de material por preview.

Placeholders pendientes de arte: cuerpo/base modular final, cuatro tonos validados bajo iluminación real, mallas/cabello inclusivos, ropa y accesorios sin clipping, iconos 2D, thumbnails, animación/poses de preview, compatibilidad con cámara y mochila, LOD/batching, paleta/contraste y revisión cultural. Ningún placeholder autoriza Release.

## Validación y comandos

```bash
scripts/setup-customization       # autoría idempotente
scripts/validate-customization    # catálogo, localization, rig, Camp, UI, capas y targets
scripts/test-editmode
scripts/test-playmode
scripts/validate
```

EditMode cubre defaults, duplicados, removed fallback, unlock/equip, insuficiente/prerrequisito/read-only, incompatibilidad, commit fallido/retry, idempotencia, migración v10→v11 y rechazo Release de `PH_`. PlayMode cubre preview→unlock→equip→Selva→Camp→reload, 4:3/16:9/20:9 y materiales compartidos. El debug unlock-all solo se asigna bajo `UNITY_EDITOR || PE_DEVELOPMENT_SERVICES`; el catálogo `PH_` hace que Release falle cerrado.

Hardware Android, clipping real, contraste/tonos bajo iluminación final, ergonomía infantil y performance GPU siguen `NOT RUN` hasta disponer de dispositivo y arte de producción.
