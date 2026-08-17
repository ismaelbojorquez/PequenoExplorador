# VS-A01 — Actividad de alimentación del tucán

Estado al 2026-08-17: **`Sourced` / representación `PH_` / `ReleaseBlocked`**. Este expediente no es aprobación científica, editorial, de derechos ni de publicación. El fact subyacente está Approved en [`VS-D-A01`](VS_D_A01_TOUCAN_FACTUAL_DOSSIER.md); la nueva actividad requiere firma humana asset-specific.

## Identidad y alcance

| Campo | Valor |
|---|---|
| Activity ID | `activity.jungle.keel-billed-toucan.choose-food` |
| Discovery | `discovery.jungle.keel-billed-toucan` |
| Especie | `Ramphastos sulfuratus` — Tucán pico canoa / Keel-billed Toucan |
| Claim reutilizado | `fact.jungle.keel-billed-toucan.diet`: “Su dieta es mayormente fruta; artrópodos y pequeños vertebrados son complementos, no la base.” |
| Copy infantil ES/EN | “Come sobre todo frutas.” / “It mostly eats fruit.” |
| Concepto | `concept.nature.diet.fruit-primary` |
| Estrategia | `activity-type.single-choice`; tap sobre tres tarjetas grandes |
| Estado | Fact `Approved`; actividad, opciones, reacción, UI y voces técnicas `Sourced`/`PH_` |

No afirma que el tucán coma exclusivamente fruta, no enseña una fruta concreta y no usa estado de conservación. Piedra y sombrero son distractores visuales obvios; no representan una lista exhaustiva de dieta.

## Fuentes contrastadas

| Source ID | Institución/autores | Publicación/versión | URL | Consulta | Conclusión y límite |
|---|---|---|---|---|---|
| `source.condor.remsen-hyde-chapman-1993` | *The Condor* / J. V. Remsen Jr., Mary Ann Hyde, Angela Chapman | Vol. 95(1), 1993; pp. 178–192 | [Oxford Academic PDF](https://academic.oup.com/condor/article-pdf/95/1/178/28192899/condor0178.pdf), [University of South Florida record](https://digitalcommons.usf.edu/condor/vol95/iss1/18/) | 2026-08-17 | Revisión de 326 individuos/32 especies de tucanes: fruta predomina y la importancia de materia animal suele sobreestimarse. No demuestra dieta exclusivamente frutal. |
| `source.cornell.bow-keel-billed-toucan-v1` | Cornell Lab of Ornithology / Revee Jones, Carole S. Griffiths; ed. T. S. Schulenberg | Birds of the World v1.0, 2020; texto base actualizado 2011 | [DOI](https://doi.org/10.2173/bow.kebtou1.01) | 2026-08-17; ya aprobado en H-009 | Para la especie, dieta mayormente frutal. Acceso puede requerir suscripción; no concede derechos sobre media. |
| `source.conabio.ramphastos-sulfuratus-2025` | CONABIO Enciclovida | Ficha `Ramphastos sulfuratus`, modificada 2026-05-08 | [Ficha PDF](https://enciclovida.mx/especies/36504.pdf) | 2026-08-17 | Confirma taxón/nombre oficial mexicano; no se usa como única fuente del claim dietario. |

## Authoring exacto

| Elemento | ID/tag | Copy/representación | Estado |
|---|---|---|---|
| Opción correcta | `activity-option.jungle.toucan.fruit` / `tag.food.fruit` | tarjeta coral “Fruta/Fruit” | `PH_` pendiente visual/educación |
| Distractor | `activity-option.jungle.toucan.rock` / `tag.object.rock` | tarjeta gris “Piedra/Rock” | `PH_` pendiente visual/educación |
| Distractor | `activity-option.jungle.toucan.hat` / `tag.object.hat` | tarjeta amarilla “Sombrero/Hat” | `PH_` pendiente visual/educación |
| Pista 1 | key `…hint.1` | algo que crece en una planta | `Sourced` pendiente |
| Pista 2 | key `…hint.2` | jugoso y con semillas | `Sourced` pendiente; generalización de “fruta”, no del tucán |
| Pista 3 | key `…hint.3` | invita a probar la fruta | `Sourced` pendiente |
| Reacción correcta | `learning-reaction.toucan.positive` | salto suave placeholder | `PH_`; reduce motion deja solo estado |
| Reacción incorrecta | `learning-reaction.toucan.neutral` | giro curioso, sin tristeza/castigo | `PH_`; reduce motion deja solo estado |
| Instrucción | `audio.voice.instruction.toucan-food` | tono técnico ES/EN + subtítulo | `PH_`, `ReleaseBlocked` |
| Fact/replay | `audio.voice.fact.toucan-fruit` | tono técnico ES/EN + copy canónico | `PH_`, `ReleaseBlocked` |

La opción incorrecta produce `TryAgain`/pista, nunca resta estrellas ni bloquea reintentos. Completion usa la reward idempotente `reward.activity.toucan-choose-food.complete` (1 estrella provisional) y emite el fact técnico `learning-completed` para Missions una vez.

## Impacto de cambios

- Cambiar especie o dieta reabre fact, copy ES/EN, opciones, tags, pistas, reacciones, fact cue, arte y tests.
- Cambiar “principalmente fruta” a “solo fruta” está prohibido sin nueva evidencia y revisión.
- Cambiar colores/objetos/reacción reabre Child UX, accesibilidad y fidelidad de representación, aunque no cambie el claim.
- Reemplazar tonos por voz humana exige actor, derechos, pronunciación, texto exacto, mezcla e inteligibilidad ES/EN.
- El fact Approved no aprueba por arrastre la actividad; el validador Release exige `Approved` no-placeholder.

## Firma humana pendiente

| Área | Reviewer | Competencia/autoridad | Resultado | ApprovedBy | ApprovalDate |
|---|---|---|---|---|---|
| Product/Education — intención, opciones y pistas |  |  | `PENDING` |  |  |
| Factual — actividad conserva límites del claim |  |  | `PENDING` |  |  |
| Localization ES/EN |  |  | `PENDING` |  |  |
| Visual/Child UX/Accessibility — tarjetas y reacción |  |  | `PENDING` |  |  |
| Audio y rights — voces/cues finales |  |  | `PENDING` |  |  |
| QA/Release — asset/version efectiva |  |  | `PENDING` |  |  |

Solo una persona identificada y autorizada puede completar estas celdas. Hasta entonces Development puede probar el watermark; Release permanece bloqueado.
