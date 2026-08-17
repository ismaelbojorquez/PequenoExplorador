# VS-D-A01 — Expediente factual del tucán candidato

Estado editorial: **Human factual/Product/visual Approved — technical adoption pending; not ReleaseLocked**. Consulta de fuentes: 2026-08-16 (`America/Mexico_City`). Ismael Bojórquez aprobó claims/copy/nombres en `H-007-IB-2026-08-16`, el asset visual y sus derechos en `H-008-IB-2026-08-16`, y realizó la revisión factual humana en `H-009-IB-2026-08-16`. Su competencia declarada es `Investigador — experiencia en búsqueda ampliada de información`; no se presenta como credencial ornitológica independiente. El prefab visual pasa a `EditorialState.Approved`, pero las definitions runtime reales aún deben crearse/adoptarse y validarse. Este expediente no constituye asesoría científica o legal ni autorización de publicación.

## Identidad y límites

| Campo | Valor |
|---|---|
| Expediente | `content-review.vs-d-a01` |
| Especie candidata | `Ramphastos sulfuratus` R. Lesson, 1830 |
| Discovery ID reservado | `discovery.jungle.keel-billed-toucan` |
| Visual ID implementado | `visual.discovery.jungle.keel-billed-toucan` — `Approved` por H-007/H-008/H-009 |
| Interaction ID reservado | `interaction.jungle.keel-billed-toucan` |
| Fact IDs reservados | `fact.jungle.keel-billed-toucan.*` según la matriz de claims |
| Nombre ES elegido por Product/Localization | `Tucán pico canoa` |
| Nombre EN aprobado por Product/Localization | `Keel-billed Toucan` |
| Runtime actual | Continúa `interaction.fixture.animal → discovery.jungle.placeholder`, Draft y `ReleaseBlocked`; Development sustituye solo la cápsula visual por `VS_ToucanPicoCanoa.prefab` `Approved`. |
| Uso permitido ahora | Adoptar definitions reales del único contenido del Vertical Slice, mapear IDs, migrar aliases/save y ejecutar validadores antes de Prompt 19. |
| Uso prohibido ahora | Declarar `ReleaseLocked`, reutilizar imágenes/audio de las fuentes, inventar credenciales del revisor o usar el claim de conservación excluido. |

Los IDs de discovery/interacción quedan reservados documentalmente y no se adoptan todavía como identidad runtime. Adoptarlos después exige validator, localización, aliases/migración si corresponde y diff explícito de contenido. El ID visual sí está materializado en el prefab candidato y no concede hechos, rewards ni comportamiento.

## Registro de fuentes

| ID | Institución / autor | Título, versión o fecha | URL | Consulta | Conclusión utilizable |
|---|---|---|---|---|---|
| `source.conabio.ramphastos-sulfuratus-2025` | CONABIO; base técnica con bibliografía AOS 2025 | *Tucán pico canoa (Ramphastos sulfuratus)*; última modificación 2025-10-13; referencia AOS 2025 | [Enciclovida PDF](https://enciclovida.mx/especies/36504.pdf) | 2026-08-16 | Taxón válido, nativo de México, nombre mexicano `tucán pico canoa`; ficha registra NOM-059 `Amenazada`, CITES II e IUCN 2025-2 `Casi amenazado`. |
| `source.itis.ramphastos-sulfuratus` | Integrated Taxonomic Information System; IOC World Bird List v10.2 | ITIS report; record credibility `verified — standards met` | [ITIS TSN 685778](https://www.itis.gov/servlet/SingleRpt/SingleRpt?search_topic=TSN&search_value=685778) | 2026-08-16 | Jerarquía Aves → Piciformes → Ramphastidae → `Ramphastos sulfuratus`; México nativo; nombres `Tucán pico canoa`/`Keel-billed Toucan`. El URL abre la subespecie nominal y debe revisarse antes de usar subespecie. |
| `source.cornell.bow-keel-billed-toucan-v1` | Cornell Lab of Ornithology; Revee Jones y Carole S. Griffiths; editor T. S. Schulenberg | *Keel-billed Toucan (Ramphastos sulfuratus)*, Birds of the World v1.0, publicada 2020-03-04; texto actualizado 2011-09-09 | [Birds of the World](https://birdsoftheworld.org/bow/species/kebtou1/cur/introduction), [DOI](https://doi.org/10.2173/bow.kebtou1.01) | 2026-08-16 | Distribución, bosque tropical de tierras bajas/secundario, pico verde/naranja/rojo/azul, dieta mayormente frutal y nombres regionales ES. |
| `source.cornell.ebird-keel-billed-toucan` | Cornell Lab of Ornithology; identificación powered by Merlin | *Keel-billed Toucan — Ramphastos sulfuratus*; sin fecha editorial visible | [eBird species profile](https://ebird.org/species/kebtou1/MX-ROO) | 2026-08-16 | Pico grande multicolor; se mueve en dosel frondoso; vocalización lejana, repetitiva y parecida a un croar. No concede derechos sobre fotos o grabaciones. |
| `source.condor.remsen-hyde-chapman-1993` | J. V. Remsen Jr., Mary Ann Hyde y Angela Chapman; *The Condor* 95(1) | “The Diets of Neotropical Trogons, Motmots, Barbets and Toucans”, 1993, artículo 18, pp. 178–192 | [University of South Florida Scholar Commons](https://digitalcommons.usf.edu/condor/vol95/iss1/18/) | 2026-08-16 | Evidencia revisada por pares: los tucanes examinados son altamente frugívoros; materia animal fue infrecuente. Reporta observaciones de `R. sulfuratus` visitando árboles frutales. |
| `source.umich.adw-ramphastos-sulfuratus-2001` | University of Michigan Animal Diversity Web; Megan Carney, editora Terry Root | *Ramphastos sulfuratus*, publicada/actualizada 2001-07-24 | [Animal Diversity Web](https://animaldiversity.org/accounts/Ramphastos_sulfuratus/) | 2026-08-16 | Fuente secundaria histórica para distribución/hábitat/dieta. Su etiqueta IUCN `Least Concern` contradice el estado actual mostrado por CONABIO/IUCN 2025-2; no se usa como autoridad de conservación ni para cifras exactas sin revisor. |

## Matriz de claims para revisión

El copy quedó aprobado por Product/Education en `H-007-IB-2026-08-16` y los claims incluidos recibieron revisión factual humana en `H-009-IB-2026-08-16`. La competencia se registra exactamente como fue declarada, sin atribuir especialidad ornitológica. Conservación permanece excluida y bloqueada.

| Claim ID / Content ID | Claim atómico | Fuentes | Contexto y límites | Copy ES propuesto | Baseline EN | Conflicto o incertidumbre | Estado |
|---|---|---|---|---|---|---|---|
| `fact.jungle.keel-billed-toucan.identity` | La especie candidata es `Ramphastos sulfuratus`, familia Ramphastidae. | CONABIO, ITIS, Cornell BOW | Especie; no fijar subespecie para el fixture. | “Este tucán es un *Ramphastos sulfuratus*.” El nombre científico será detalle opcional, no instrucción. | “This toucan is *Ramphastos sulfuratus*.” Scientific name remains optional detail. | ITIS URL consultado abre la subespecie nominal; el slice solo reserva especie. | Approved — H-009 |
| `fact.jungle.keel-billed-toucan.common-name` | En México el nombre oficial usado es `Tucán pico canoa`; Cornell registra `Tucán pico iris` para Costa Rica/Panamá. | CONABIO, ITIS, Cornell BOW | El locale actual `es` adopta la baseline mexicana por decisión humana del 2026-08-16. | “Tucán pico canoa.” | “Keel-billed Toucan.” | `Pico iris` permanece como variante regional documentada y requeriría locale/decisión territorial propios. | Approved — H-007/H-009 |
| `fact.jungle.keel-billed-toucan.range` | Ocurre desde el sur de México por Centroamérica hasta el norte de Colombia y extremo noroeste de Venezuela. | Cornell BOW; CONABIO confirma México nativo | Distribución general; evitar afirmar presencia en cada localidad del juego. | “Vive desde el sur de México, a través de Centroamérica, hasta una parte del norte de Sudamérica.” | “It lives from southern Mexico through Central America to part of northern South America.” | Copy simplifica el extremo venezolano; revisor debe confirmar si el alcance es apropiado para 4–9. | Approved — H-009 |
| `fact.jungle.keel-billed-toucan.habitat` | Habita bosques tropicales perennifolios de tierras bajas y bosques secundarios. | Cornell BOW | No implica que toda selva tropical contenga la especie. | “Vive entre los árboles de selvas cálidas y bosques que están creciendo de nuevo.” | “It lives among the trees of warm forests, including forests that are growing back.” | “Bosque secundario” se parafrasea; Education debe validar comprensión. | Approved — H-009 |
| `fact.jungle.keel-billed-toucan.diet` | Su dieta es mayormente fruta; artrópodos y pequeños vertebrados son complementos, no la base. | Remsen et al. 1993; Cornell BOW | Dieta silvestre general. No usar dieta de zoológico ni inferir una fruta exclusiva. | “Come sobre todo frutas.” | “It mostly eats fruit.” | Las observaciones llamativas de alimento animal pueden sobrerrepresentar su frecuencia; no diseñar actividad como si fuera carnívoro. | Approved — H-009 |
| `fact.jungle.keel-billed-toucan.bill` | Su pico diagnóstico combina verde, naranja, rojo y azul. | Cornell BOW; eBird/Merlin | Adulto genérico; el arte debe contrastarse con referencia autorizada y variación real. | “Su pico grande tiene varios colores: verde, naranja, rojo y azul.” | “Its large bill has several colors: green, orange, red, and blue.” | La aprobación cubre el prefab propio; las imágenes enlazadas conservan copyright propio. | Approved — H-008/H-009 |
| `fact.jungle.keel-billed-toucan.voice` | La vocalización se describe como un croar lejano que se repite regularmente. | Cornell eBird/Merlin | Descripción auditiva, no licencia de grabación ni especificación acústica exacta. | “Su llamado se parece a un croar que se repite.” | “Its call sounds like a repeated croak.” | La analogía factual quedó aprobada; cualquier grabación final exige rights y revisión asset-specific. | Approved — H-009; audio asset pending |
| `fact.jungle.keel-billed-toucan.conservation` | CONABIO muestra IUCN 2025-2 `Casi amenazado`, NOM-059 `Amenazada` y CITES II. | CONABIO | No se propone como fact infantil del slice; es metadata temporal y jurisdiccional. | No usar todavía. | Do not use yet. | ADW antiguo muestra `Least Concern`. La autoridad actual difiere y puede cambiar; revalidar con especialista/IUCN antes de publicación. | **Blocked — conflict/temporal review** |

No se propone una longitud exacta: fuentes secundarias consultadas difieren y algunas se apoyan en referencias antiguas/no autorizables. El Vertical Slice no la necesita.

## Declaración humana H-007-IB-2026-08-16

Esta sección registra la declaración proporcionada directamente por la persona identificada; Codex no la emitió, amplió ni convirtió en competencia profesional no declarada.

| Campo | Declaración registrada |
|---|---|
| Nombre | Ismael Bojórquez |
| Rol/organización | Ismael Bojórquez |
| Competencia o autoridad declarada | Creador |
| Fecha declarada | 2026-08-16 |
| Autorización | Confirma estar autorizado para emitir las aprobaciones indicadas. |
| Claims aprobados | Especie, distribución, hábitat, alimentación, rasgo visual y sonido con el copy de esta matriz. |
| Conservación | Excluida del Vertical Slice; el conflicto temporal permanece documentado y bloqueado para uso. |
| Nombre ES / EN | `Tucán pico canoa` / `Keel-billed Toucan` |
| Product/Education | `APPROVED` |
| Localization | `APPROVED` |
| Art / Audio / Rights / QA | Declarados `APPROVED`; arte y audio de referencia declarados `Propia`. El visual creado después fue aprobado específicamente en `H-008-IB-2026-08-16`; audio sigue sin asset final. |
| Cambios solicitados | Ninguno. |

La aprobación del Creador resuelve intención de producto, copy y nombre regional. No satisface por sí sola el paso `Reviewed` factual definido en [`CONTENT_SOURCES.md`](CONTENT_SOURCES.md), cuyo gate exige competencia de especialista factual declarada. Tampoco convierte el fixture cápsula ni los tonos `PH_` en representación aprobada de la especie.

## Declaración visual asset-specific H-008-IB-2026-08-16

Esta sección registra fielmente el alcance sustantivo de la declaración humana recibida después de revisar el prefab y sus seis renders. Codex registra la firma; no se atribuye la aprobación ni amplía su alcance.

| Campo | Declaración registrada |
|---|---|
| Nombre | Ismael Bojórquez |
| Rol/organización | Creador de Pequeño Explorador |
| Autoridad declarada | Propietario y responsable del producto |
| Fecha | 2026-08-16 |
| Asset ID | `visual.discovery.jungle.keel-billed-toucan` |
| Prefab | `VS_ToucanPicoCanoa.prefab` |
| Commit revisado | `427c09b0b48b0b8ec7348971db5eddbafc5d3707` |
| Aprobado | Reconocimiento/silueta; pico y colores; expresión 4–9; proporciones, patas y cola; lectura 16:9/20:9; collider, interaction point y bounds candidatos; integración Jungle; autoría propia; rights/licencia y QA visual del asset concreto. |
| Cambios solicitados | Ninguno. |
| Licencia declarada | Propia. |
| Autorización | Confirma estar autorizado para emitir la aprobación. |
| Alcance excluido | No aprueba exactitud factual especializada, audio final ni publicación. |

Esta aprobación cierra los gates humanos de Art, Rights y QA visual para esta versión exacta del asset. Un cambio material de modelo, silueta, paleta, collider, interaction point, bounds o integración visual invalida las firmas afectadas y exige nueva revisión.

## Declaración factual H-009-IB-2026-08-16

Esta sección registra la declaración humana proporcionada. La competencia se conserva literalmente y no se transforma en una credencial de Aves/Ramphastidae no declarada.

| Campo | Declaración registrada |
|---|---|
| Nombre | Ismael Bojórquez |
| Rol/organización | Ismael Bojórquez |
| Área de especialidad declarada | Investigador |
| Fundamento de competencia declarado | Experiencia en búsqueda ampliada de información. |
| Fecha | 2026-08-16 |
| Commit visual revisado | `427c09b0b48b0b8ec7348971db5eddbafc5d3707` |
| Claims aprobados | Identidad, nombres ES/EN, distribución, hábitat, alimentación mayormente frugívora, colores del pico y descripción del llamado. |
| Representación aprobada | Fidelidad factual del prefab; collider, PhotoTarget y bounds no comunican tamaño zoológico exacto. |
| Conservación | Confirmada como excluida del Vertical Slice y `BLOCKED`. |
| Correcciones/límites declarados | Ninguno. |
| Fuentes adicionales declaradas | “Investigaciones oficiales”; no se añadió referencia concreta nueva, por lo que las seis fuentes registradas siguen siendo la evidencia trazable. |
| Conflictos declarados | Ninguno dentro de los claims aprobados. El conflicto temporal de conservación permanece documentado fuera del alcance aprobado. |
| Exclusiones | No concede derechos de arte/audio ni autorización de publicación. |
| Autorización | Confirma estar autorizado para emitir la revisión. |

Esta firma completa la revisión factual humana exigida por el proceso interno para el Vertical Slice. No demuestra una acreditación ornitológica externa y no sustituye revalidación si cambian especie, fuentes, copy, representación o alcance territorial.

## Impacto de cambios y nueva revisión

| Cambio después de revisión | Debe volver a `Reviewed` o anterior |
|---|---|
| Especie/subespecie | Todos los claims; discovery, interacción, foto, álbum, actividad, misión, arte, audio, localización y aliases/save aplicables. |
| Nombre regional | Keys ES/EN, voz de nombre, subtítulos, álbum, misión y metadata de tienda; no taxonomía si la especie no cambia. |
| Distribución/hábitat | Fact/copy, álbum, mapa/World placement y cualquier misión o pista ambiental. |
| Dieta | Actividad de alimentación/hábitat, opciones/tags correctos, hints, fact, narración y mission facts. |
| Rasgo visual/paleta | Modelo/texturas/ilustración, silhouette, PhotoTarget/bounds, iconos, thumbnail canónico y álbum. |
| Vocalización | Grabación, cue, subtítulo/analogía, replay, mezcla y licencia territorial/plataforma. |
| Estado de conservación | Metadata/fact correspondiente, fecha de consulta y revisión legal/editorial; nunca cambiar por copy genérico sin registrar fuente. |

Toda modificación material posterior a `Approved` invalida la firma afectada. Aprobación factual no otorga licencia de arte/audio ni permiso para descargar media de Cornell, ADW u otra fuente.

## Checklist de firma humana

Los campos vacíos son bloqueos intencionales; no deben completarse por automatización.

| Gate | Nombre y rol humano | Competencia / alcance | Resultado | Fecha | Firma o referencia |
|---|---|---|---|---|---|
| Revisión factual humana | Ismael Bojórquez — Investigador | Experiencia declarada en búsqueda ampliada; revisión contra fuentes institucionales registradas. | `APPROVED` | 2026-08-16 | `H-009-IB-2026-08-16` |
| Product/Education — copy 4–9 | Ismael Bojórquez — Creador | Intención de producto y copy infantil. | `APPROVED` | 2026-08-16 | `H-007-IB-2026-08-16` |
| Localization — nombre regional ES/EN | Ismael Bojórquez — Creador | Selecciona `Tucán pico canoa` / `Keel-billed Toucan`. | `APPROVED` | 2026-08-16 | `H-007-IB-2026-08-16` |
| Art — fidelidad visual | Ismael Bojórquez — Creador/Propietario | Prefab y seis renders concretos; silueta, pico/colores, expresión, proporciones, lectura y Jungle. | `APPROVED` | 2026-08-16 | `H-008-IB-2026-08-16` |
| Audio — vocalización/pronunciación | Ismael Bojórquez — Creador | Declara audio `Propia`; no hay cue/clip no-`PH_` ni ledger que inspeccionar. | `DECLARED APPROVED — ASSET PENDING` | 2026-08-16 | `H-007-IB-2026-08-16` |
| Legal/Rights — asset visual | Ismael Bojórquez — Creador/Propietario | Autoría propia, ausencia de media externa y licencia propia del asset concreto. | `APPROVED` | 2026-08-16 | `H-008-IB-2026-08-16` |
| QA visual — asset concreto | Ismael Bojórquez — Creador/Propietario | Collider, interaction point, bounds, lectura 16:9/20:9 e integración Jungle. | `APPROVED` | 2026-08-16 | `H-008-IB-2026-08-16` |

### Revisión visual asset-specific completada

Ismael Bojórquez resolvió explícitamente los checks siguientes mediante `H-008-IB-2026-08-16` sobre el commit indicado:

- [x] La silueta se reconoce como tucán en teléfono landscape y no como otra ave.
- [x] El pico grande y los bloques verde, naranja, rojo y azul son adecuados sin copiar una referencia concreta.
- [x] Cuerpo oscuro, garganta amarilla, ojo y expresión resultan amables para 4–9, sin realismo inquietante.
- [x] Proporción, patas y cola funcionan como estilización y no afirman sexo, edad, subespecie ni tamaño zoológico.
- [x] Collider táctil, punto de interacción y bounds candidatos cubren la figura sin volumen engañoso.
- [x] Front, lateral, tres cuartos, 16:9, 20:9 y siluetas clara/oscura conservan lectura.
- [x] Autoría `Ismael Bojórquez`, creación propia por tooling Unity y ausencia de media externa son correctas.
- [x] Se acepta como candidato visual del Vertical Slice sin extender la aprobación a claims factuales.

Ledger versionado: `Assets/_Game/Content/Discoveries/Jungle/KeelBilledToucan/VS_ToucanPicoCanoa.provenance.json`. Métricas actuales: 16 meshes/renderers, 4,931 vértices, 7,132 triángulos, 7 materiales compartidos, bounds `2.973 × 2.425 × 1.100` y estimación Editor de 89,484 bytes para meshes/materiales. Esta cifra no es memoria pico en dispositivo.

Campos editoriales requeridos tras las firmas:

| Campo | Valor actual |
|---|---|
| `Reviewer` | Ismael Bojórquez — Creador/Propietario para asset visual; Investigador para revisión factual humana. |
| `ReviewedOn` | 2026-08-16 |
| `ApprovedBy` | Ismael Bojórquez, para Product/Education, Localization, asset visual y revisión factual humana (`H-007`, `H-008`, `H-009`). |
| `ApprovalDate` | 2026-08-16 |
| `RightsOwner` / licencia | Ismael Bojórquez; `Propia`, aprobada para el asset concreto en `H-008-IB-2026-08-16`. |
| `ReleaseLockedBy` / fecha |  |

## Condición para desbloquear Prompt 19

Product/Education, Localization, representación visual y revisión factual humana quedaron aprobados por Ismael Bojórquez. El prefab visual puede adoptar `EditorialState.Approved`. Prompt 19 permanece bloqueado únicamente por trabajo técnico: crear/adoptar definitions no-`PH_`, mapear interaction/discovery/facts, preservar aliases/save y validar el catálogo. Este expediente no autoriza `ReleaseLocked`, audio final ni publicación.
