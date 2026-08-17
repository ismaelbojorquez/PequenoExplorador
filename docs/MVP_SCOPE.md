# Alcance canónico — Vertical Slice, MVP y post-MVP

Esta es la única fuente de verdad para cantidades y prioridad. Todos los estados significan documentación, no implementación.

## Estados

- `Specified`: contrato documentado; no implementado.
- `Pending validation`: necesita playtest/decisión antes de fijarse.
- `Pending factual approval`: no puede producirse ni liberarse hasta aprobación.
- `Blocked by Vertical Slice`: no escalar contenido antes de que el slice pase Gate B.
- `Human decision`: ingeniería no puede cerrarlo.

## Vertical Slice exacto

| Elemento | Cantidad | Contenido | Dependencia | Estado | Criterio de aceptación |
|---|---:|---|---|---|---|
| Animal | 1 | `Ramphastos sulfuratus`; nombre aprobado `Tucán pico canoa` / `Keel-billed Toucan`. | Expediente y firmas H-007/H-008/H-009; reabrir si cambia claim/asset. | Runtime `Approved`; conservación excluida y audio final pendiente. [Dossier](VS_D_A01_TOUCAN_FACTUAL_DOSSIER.md) | Detectar, fotografiar y mostrar en álbum; actividad todavía pendiente sin claim nuevo. |
| Actividad | 1 tipo / 1 instancia | Asociación visual `Reconoce al tucán`. | Animal aprobado, ambos modos de guía. | Specified | Se completa sin lectura, sin castigo y con pistas graduadas. |
| Misión | 1 | `VS-M01 — Conoce al tucán`. | World, Discovery, Activity, Reward. | Specified | Loop completo en 3–8 min y mismo resultado en ambas guías. |
| Mejora de campamento | 1 | `Mesa de observación`. | Misión y economía determinista. | Specified | Cambio visible, permanente y restaurado al reabrir. |
| Persistencia | 1 perfil local | Discovery, álbum, actividad, misión, Estrellas de Explorador, mejora y guía. | Contratos anteriores. | Specified | Cerrar/reabrir conserva estados confirmados y tolera datos ausentes. |
| Mundo | 1 claro + campamento | Recorrido acotado de ida y vuelta. | Navegación candidata validada. | Pending validation | Objetivo localizable, retorno claro y pausa/salida disponibles. |
| Álbum | 1 ficha | Ficha del discovery aprobado. | Discovery y contenido factual. | Pending factual approval | Audio/imagen funcionan sin texto obligatorio. |

Quedan fuera del Vertical Slice: producción del resto de especies, secretos, múltiples actividades/misiones, catálogo de personalización, monetización y contenido final.

## Matriz completa de funcionalidades

| Feature | Vertical Slice | MVP | Post-MVP | Prioridad | Dependencia | Estado MVP | Criterio de aceptación MVP |
|---|---:|---:|---|---|---|---|---|
| Mundo Selva | 1 claro + campamento | 1 mundo Selva conectado | Otros biomas, sin cantidad comprometida | Must | World, navegación, arte | Specified | Todo contenido canónico cabe sin saturación; offline y retorno seguro. |
| Campamento | Núcleo + 1 mejora | Hub completo | Expansiones temáticas | Must | World, save, UI | Specified | Misiones, álbum, personalización y ajustes accesibles sin tienda infantil. |
| Animales | 1 | 20 | Sin cantidad comprometida | Must | Fuentes, arte, audio, Discovery | Blocked by Vertical Slice | Cada uno tiene ID, fuente Approved, ficha, aparición y QA. |
| Plantas | 0 discoveries | 10 | Sin cantidad comprometida | Must | Fuentes, arte, Discovery | Blocked by Vertical Slice | Mismos controles de aprobación y legibilidad que animales. |
| Insectos | 0 | 5 | Sin cantidad comprometida | Must | Fuentes, arte, Discovery | Blocked by Vertical Slice | Encuentro accesible sin precisión fina y ficha Approved. |
| Objetos especiales | 0 | 5 | Sin cantidad comprometida | Must | World, misión, arte | Blocked by Vertical Slice | Cada objeto tiene contexto, recompensa fija y no funciona como premium currency. |
| Misiones | 1 | 10+ | Sin cantidad comprometida | Must | Discovery, Activity, Reward | Blocked by Vertical Slice | Cada misión tiene dependencias, fallback, duración y hechos Approved. |
| Tipos de actividad | 1 | 5+ | Tipos adicionales por evidencia | Must | Educación, UI, contenido | Blocked by Vertical Slice | Cada tipo tiene objetivo observable, dos guías, pistas y evidencia. |
| Secretos | 0 | 5+ | Sin cantidad comprometida | Must | World, Discovery | Blocked by Vertical Slice | Opcionales, pista justa, sin horario/caducidad y recompensa determinista. |
| Fotografía ficticia | 1 flujo | Todos los discoveries aplicables | Variantes creativas | Must | Discovery, UI/accessibility | Specified | Sin permisos reales; captura indulgente y asistida. |
| Álbum | 1 ficha | 40 fichas potenciales según discovery | Colecciones futuras | Must | Discovery, fuentes, audio | Blocked by Vertical Slice | Estado persistente, categorías, audio/imagen y cero claims sin aprobar. |
| Mejoras de campamento | 1 | 5+ | Sin cantidad comprometida | Must | Reward, economía, save | Blocked by Vertical Slice | Permanentes, deterministas, visibles y sin compra. |
| Personalización | 0 opciones finales | 6+ opciones cosméticas | Más catálogos | Should | Campamento, arte, save | Blocked by Vertical Slice | Sin género forzado, rareza, azar, comparación o caducidad. |
| Economía blanda | 1 recurso candidato | 1 recurso, sin compra | Revaluar solo por ADR | Must | Reward, misiones, campamento | Pending validation | Se gana determinísticamente, no caduca/pierde y solo mejora campamento. |
| Modos de guía (`Más guía`, `Guía estándar`) | 2 | 2 | Nuevos modos solo por playtest | Must | UI, actividades, misiones | Specified | Selección manual sin edad; mismo contenido/progreso/recompensa. |
| Progreso local | Slice completo | Un perfil local y reset adulto | Múltiples perfiles por decisión | Must | Save, área adulta | Specified | Offline, versionable, recuperable y sin datos personales. |
| Área de padres | No requerida para slice | 1 área protegida | Controles ampliados | Must | UI, políticas, parental gate | Human decision | Muestra progreso descriptivo/ajustes; no califica ni expone links al niño. |
| Monetización preparada | Ninguna | Contrato/espacio deshabilitado tras gate adulto | Modelo por ADR | Must | Decisión comercial/legal | Human decision | Cero SDK, precio, SKU, ads o compra; frontera reemplazable documentada. |
| Lanzamiento ad-free | Ad-free | Recomendado ad-free | Ads solo por ADR | Must | Decisión humana/políticas | Human decision | Binario y UX sin ads; cualquier cambio reabre políticas y playtests. |
| Accesibilidad/descansos | Baseline | Checklist completo | Mejoras por evidencia | Must | UI, audio, arte | Specified | No depende de lectura/audio/color/velocidad; pausa neutral y salida segura. |
| Tap-to-move | Candidato a probar | Solo si validado | Alternativa accesible | Should | Playtest F07 | Pending validation | Comprensión/motricidad supera o iguala control alternativo sin frustración. |

## MoSCoW resumido

| Must | Should | Could | Won’t en MVP |
|---|---|---|---|
| Pilar completo; Selva; campamento; cantidades canónicas; fotografía; álbum; misiones; 5+ actividades; secretos; dos guías; progreso; accesibilidad; área adulta; monetización solo preparada; ad-free recomendado. | Tap-to-move si valida; 6+ cosméticos; captura asistida; variedad audiovisual. | Narración ambiental adicional, animaciones cosméticas y variantes de actividad que no retrasen QA. | Backend, cuentas, social/chat, cámara real, ubicación, ads/SDKs, IAP/SKUs/precios, otros biomas, multiplayer, rachas, energía, gacha, loot boxes, FOMO, dailies y contenido remoto. |

## Gate de escalado

No se producen las otras 39 fichas ni lotes de misiones/arte/audio hasta que el Vertical Slice demuestre: loop comprensible y divertido, evidencia educativa observable, ambos modos equivalentes, persistencia coherente, contenido factual aprobado y ausencia de feedback punitivo.
