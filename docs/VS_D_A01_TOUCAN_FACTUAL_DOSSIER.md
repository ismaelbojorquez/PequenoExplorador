# VS-D-A01 — Expediente factual del tucán candidato

Estado editorial: **Sourced + Owner/Product Approved — pending factual specialist and asset-specific signoff**. Consulta de fuentes: 2026-08-16 (`America/Mexico_City`). Ismael Bojórquez registró aprobación humana como Creador el 2026-08-16 para claims, copy, nombres y especificaciones; no declaró competencia de especialista factual. Existe el candidato visual propio no-`PH_` `visual.discovery.jungle.keel-billed-toucan`, pero su firma visual específica sigue pendiente. Este expediente no constituye asesoría científica o legal ni `ReleaseLocked`.

## Identidad y límites

| Campo | Valor |
|---|---|
| Expediente | `content-review.vs-d-a01` |
| Especie candidata | `Ramphastos sulfuratus` R. Lesson, 1830 |
| Discovery ID reservado | `discovery.jungle.keel-billed-toucan` |
| Visual ID implementado | `visual.discovery.jungle.keel-billed-toucan` — candidato `Sourced`, no `Approved` |
| Interaction ID reservado | `interaction.jungle.keel-billed-toucan` |
| Fact IDs reservados | `fact.jungle.keel-billed-toucan.*` según la matriz de claims |
| Nombre ES elegido por Product/Localization | `Tucán pico canoa` |
| Nombre EN aprobado por Product/Localization | `Keel-billed Toucan` |
| Runtime actual | Continúa `interaction.fixture.animal → discovery.jungle.placeholder`, Draft y `ReleaseBlocked`; Development sustituye solo la cápsula visual por `VS_ToucanPicoCanoa.prefab` `Sourced`. |
| Uso permitido ahora | Usar claims/copy/nombres aprobados por el Creador para preparar el único contenido del Vertical Slice, conservando el gate factual y de assets. |
| Uso prohibido ahora | Marcar assets runtime `Approved`, eliminar watermark, declarar revisión de especialista factual o reutilizar imágenes/audio de las fuentes. |

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

El copy de la matriz quedó aprobado por Product/Education en `H-007-IB-2026-08-16`. Los claims permanecen `Sourced` hasta revisión factual especializada; la aprobación de lenguaje no aprueba por sí sola exactitud especializada, representación o media.

| Claim ID / Content ID | Claim atómico | Fuentes | Contexto y límites | Copy ES propuesto | Baseline EN | Conflicto o incertidumbre | Estado |
|---|---|---|---|---|---|---|---|
| `fact.jungle.keel-billed-toucan.identity` | La especie candidata es `Ramphastos sulfuratus`, familia Ramphastidae. | CONABIO, ITIS, Cornell BOW | Especie; no fijar subespecie para el fixture. | “Este tucán es un *Ramphastos sulfuratus*.” El nombre científico será detalle opcional, no instrucción. | “This toucan is *Ramphastos sulfuratus*.” Scientific name remains optional detail. | ITIS URL consultado abre la subespecie nominal; el slice solo reserva especie. | Sourced |
| `fact.jungle.keel-billed-toucan.common-name` | En México el nombre oficial usado es `Tucán pico canoa`; Cornell registra `Tucán pico iris` para Costa Rica/Panamá. | CONABIO, ITIS, Cornell BOW | El locale actual `es` adopta la baseline mexicana por decisión humana del 2026-08-16. | “Tucán pico canoa.” | “Keel-billed Toucan.” | `Pico iris` permanece como variante regional documentada y requeriría locale/decisión territorial propios. | Sourced; Product/Localization Approved |
| `fact.jungle.keel-billed-toucan.range` | Ocurre desde el sur de México por Centroamérica hasta el norte de Colombia y extremo noroeste de Venezuela. | Cornell BOW; CONABIO confirma México nativo | Distribución general; evitar afirmar presencia en cada localidad del juego. | “Vive desde el sur de México, a través de Centroamérica, hasta una parte del norte de Sudamérica.” | “It lives from southern Mexico through Central America to part of northern South America.” | Copy simplifica el extremo venezolano; revisor debe confirmar si el alcance es apropiado para 4–9. | Sourced |
| `fact.jungle.keel-billed-toucan.habitat` | Habita bosques tropicales perennifolios de tierras bajas y bosques secundarios. | Cornell BOW | No implica que toda selva tropical contenga la especie. | “Vive entre los árboles de selvas cálidas y bosques que están creciendo de nuevo.” | “It lives among the trees of warm forests, including forests that are growing back.” | “Bosque secundario” se parafrasea; Education debe validar comprensión. | Sourced |
| `fact.jungle.keel-billed-toucan.diet` | Su dieta es mayormente fruta; artrópodos y pequeños vertebrados son complementos, no la base. | Remsen et al. 1993; Cornell BOW | Dieta silvestre general. No usar dieta de zoológico ni inferir una fruta exclusiva. | “Come sobre todo frutas.” | “It mostly eats fruit.” | Las observaciones llamativas de alimento animal pueden sobrerrepresentar su frecuencia; no diseñar actividad como si fuera carnívoro. | Sourced |
| `fact.jungle.keel-billed-toucan.bill` | Su pico diagnóstico combina verde, naranja, rojo y azul. | Cornell BOW; eBird/Merlin | Adulto genérico; el arte debe contrastarse con referencia autorizada y variación real. | “Su pico grande tiene varios colores: verde, naranja, rojo y azul.” | “Its large bill has several colors: green, orange, red, and blue.” | No se aprueba todavía una paleta, patrón exacto o asset. Las imágenes enlazadas conservan copyright propio. | Sourced |
| `fact.jungle.keel-billed-toucan.voice` | La vocalización se describe como un croar lejano que se repite regularmente. | Cornell eBird/Merlin | Descripción auditiva, no licencia de grabación ni especificación acústica exacta. | “Su llamado se parece a un croar que se repite.” | “Its call sounds like a repeated croak.” | “Parecido a rana” es una analogía; Audio y especialista deben validarla contra una grabación con derechos. | Sourced |
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
| Art / Audio / Rights / QA | Declarados `APPROVED`; arte y audio de referencia declarados `Propia`. El visual con ID/ledger fue creado después de esta declaración y exige signoff asset-specific; audio sigue sin asset final. |
| Cambios solicitados | Ninguno. |

La aprobación del Creador resuelve intención de producto, copy y nombre regional. No satisface por sí sola el paso `Reviewed` factual definido en [`CONTENT_SOURCES.md`](CONTENT_SOURCES.md), cuyo gate exige competencia de especialista factual declarada. Tampoco convierte el fixture cápsula ni los tonos `PH_` en representación aprobada de la especie.

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
| Especialista factual |  | Competencia factual sobre Aves/Ramphastidae o fuente institucional equivalente. | `PENDING` |  |  |
| Product/Education — copy 4–9 | Ismael Bojórquez — Creador | Intención de producto y copy infantil. | `APPROVED` | 2026-08-16 | `H-007-IB-2026-08-16` |
| Localization — nombre regional ES/EN | Ismael Bojórquez — Creador | Selecciona `Tucán pico canoa` / `Keel-billed Toucan`. | `APPROVED` | 2026-08-16 | `H-007-IB-2026-08-16` |
| Art — fidelidad visual | Ismael Bojórquez — Creador | Declaró arte `Propia` antes del candidato; revisar ahora prefab y seis renders concretos. | `DECLARED APPROVED — ASSET-SPECIFIC SIGNOFF PENDING` | 2026-08-16 | `H-007-IB-2026-08-16` |
| Audio — vocalización/pronunciación | Ismael Bojórquez — Creador | Declara audio `Propia`; no hay cue/clip no-`PH_` ni ledger que inspeccionar. | `DECLARED APPROVED — ASSET PENDING` | 2026-08-16 | `H-007-IB-2026-08-16` |
| Legal/Rights — assets y territorios | Ismael Bojórquez — Creador | Ledger declara tooling/materiales propios y cero media externa; confirmar titularidad/alcance del asset concreto. | `OWNER DECLARATION — ASSET-SPECIFIC SIGNOFF PENDING` | 2026-08-16 | `H-007-IB-2026-08-16` |
| QA/Release — matriz asset/claim | Ismael Bojórquez — Creador | Validator/evidencia existen; specialist/signoff faltan y Release devuelve `TOUCAN019`. | `DECLARED APPROVED — RELEASE BLOCKED` | 2026-08-16 | `H-007-IB-2026-08-16` |

### Revisión visual asset-specific pendiente

Revisar los seis PNG ignorados en `artifacts/review/toucan/` y registrar una nueva referencia humana; la declaración previa no se extiende automáticamente a un asset que aún no existía. Todos los checks deben resolverse explícitamente:

- [ ] La silueta se reconoce como tucán en teléfono landscape y no como otra ave.
- [ ] El pico grande y los bloques verde, naranja, rojo y azul son adecuados sin copiar una referencia concreta.
- [ ] Cuerpo oscuro, garganta amarilla, ojo y expresión resultan amables para 4–9, sin realismo inquietante.
- [ ] Proporción, patas y cola funcionan como estilización y no afirman sexo, edad, subespecie ni tamaño zoológico.
- [ ] Collider táctil, punto de interacción y bounds candidatos cubren la figura sin volumen engañoso.
- [ ] Front, lateral, tres cuartos, 16:9, 20:9 y siluetas clara/oscura conservan lectura.
- [ ] Autoría `Ismael Bojórquez`, creación propia por tooling Unity y ausencia de media externa son correctas.
- [ ] Se acepta como candidato del Vertical Slice o se registran cambios concretos; aprobar arte no aprueba claims factuales.

Ledger versionado: `Assets/_Game/Content/Discoveries/Jungle/KeelBilledToucan/VS_ToucanPicoCanoa.provenance.json`. Métricas actuales: 16 meshes/renderers, 4,931 vértices, 7,132 triángulos, 7 materiales compartidos, bounds `2.973 × 2.425 × 1.100` y estimación Editor de 89,484 bytes para meshes/materiales. Esta cifra no es memoria pico en dispositivo.

Campos editoriales requeridos tras las firmas:

| Campo | Valor actual |
|---|---|
| `Reviewer` | Ismael Bojórquez — Creador; especialista factual pendiente. |
| `ReviewedOn` | 2026-08-16 |
| `ApprovedBy` | Ismael Bojórquez — Creador, para Product/Education, Localization y especificaciones declaradas. |
| `ApprovalDate` | 2026-08-16 |
| `RightsOwner` / licencia | Declaración: `Propia`; evidencia y mapping por asset pendientes. |
| `ReleaseLockedBy` / fecha |  |

## Condición para desbloquear Prompt 19

Product/Education y Localization quedaron aprobados por Ismael Bojórquez. Ya existe una representación propia no-`PH_` con ID, prefab, materiales, generator, hashes y ledger verificables, pero su aprobación asset-specific continúa pendiente. Prompt 19 permanece bloqueado hasta que un especialista factual identificado declare competencia y revise los claims que fotografía mostrará, y hasta que Ismael firme la checklist visual concreta. Después se crean definitions separadas y se valida su mapping; este expediente por sí solo no autoriza cambiar `EditorialState.Approved` ni `ReleaseLocked`.
