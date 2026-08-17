# Fuentes y aprobación factual de contenido

Proceso obligatorio para nombres, hábitats, rasgos, conductas, sonidos, relaciones ecológicas, vocabulario y cualquier afirmación educativa. La ausencia de aprobación bloquea Release; una ilustración atractiva o texto generado no es evidencia.

## Fuentes autorizables

En orden preferente:

1. organismos públicos científicos/ambientales y bases taxonómicas oficiales;
2. museos, universidades, jardines botánicos o colecciones científicas con autoría;
3. publicaciones revisadas por pares o libros de referencia con edición/ISBN;
4. organizaciones de conservación reconocidas, con autor/fecha/metodología;
5. especialista humano acreditado que documente revisión y conflictos.

Blogs anónimos, wikis, tiendas, videos sociales, resultados de buscador y contenido generado por IA pueden orientar búsqueda, pero no son fuente final.

## Registro mínimo por claim

| Campo | Requisito |
|---|---|
| Content ID | ID estable enlazado desde Discovery/Activity/Mission/asset/audio. |
| Claim | Una afirmación atómica, redactada para revisión. |
| Fuente | URL o referencia bibliográfica, institución, autor y título. |
| Consulta | Fecha y, si aplica, versión/fecha de publicación. |
| Contexto | Región, especie exacta, etapa de vida y límites relevantes. |
| Copy infantil | Parafraseo propuesto, sin ampliar la fuente. |
| Revisor | Nombre/rol humano y competencia declarada. |
| Estado | `Draft`, `Sourced`, `Reviewed`, `Approved`, `ReleaseLocked` o `Rejected`. |
| Evidencia | Comentarios, conflictos resueltos y fecha de aprobación. |

## Flujo factual

1. **Draft:** diseño propone objetivo y placeholder, claramente marcado.
2. **Sourced:** productor de contenido adjunta fuente autorizable a cada claim.
3. **Reviewed:** revisor factual comprueba taxonomía, contexto, lenguaje y representación visual/sonora.
4. **Approved:** product/education confirma que el copy conserva el hecho y es apropiado para 4–9.
5. **ReleaseLocked:** QA verifica que texto, voz, imagen, actividad y metadata coinciden con la versión aprobada.

Cambiar especie, claim, ilustración que comunica rasgos, voz o actividad después de `Approved` devuelve el ítem a `Reviewed`.

## Gate de Release

- Cualquier Content ID en `Draft`, `Sourced`, `Reviewed` o sin registro: **Blocked for Release**.
- Dos fuentes en conflicto: elegir con especialista y documentar razonamiento; no simplificar ocultando incertidumbre.
- No se permite “aprobar por lote” sin revisar cada claim/asset asociado.
- QA genera una matriz `Content ID → claim → asset/audio/activity → aprobación`; toda referencia debe resolver.

## Vertical Slice

[`VS-D-A01`](VS_D_A01_TOUCAN_FACTUAL_DOSSIER.md) adopta `Ramphastos sulfuratus`. H-007/H-008/H-009 cubren Product/Education/Localization, visual/rights/QA y revisión factual humana, con competencia declarada `Investigador — experiencia en búsqueda ampliada de información`; no se atribuye credencial ornitológica externa. El conflicto de conservación queda excluido. Discovery, siete facts, seis fuentes e interacción son runtime `Approved`; audio final y `ReleaseLocked` siguen pendientes.

Product/Localization eligió `Tucán pico canoa` y aprobó `Keel-billed Toucan` para EN. La variante `pico iris` permanece como contexto regional. La adopción usa IDs no-`PH_`, alias/save v5 y mantiene conservación fuera del runtime. `Approved` no equivale a `ReleaseLocked`: audio final, matriz efectiva y QA de publicación siguen separados.

Prompt 24 no agrega un claim zoológico: reutiliza `fact.jungle.keel-billed-toucan.diet` y vuelve a contrastar Remsen/Hyde/Chapman, Cornell BOW y taxonomía CONABIO el 2026-08-17. La representación nueva —opciones, pistas, reacción y narración— tiene expediente propio [`VS-A01`](VS_A01_TOUCAN_FEEDING_ACTIVITY.md), estado máximo `Sourced` y firma humana vacía. Por tanto funciona solo en Development y falla Release sin ser presentado como Approved.

## Responsabilidades humanas

- **Product/Education:** intención pedagógica y lenguaje apropiado.
- **Especialista factual:** exactitud y contexto.
- **Art/Audio:** fidelidad de representación y procedencia.
- **QA/Release:** trazabilidad y bloqueo efectivo.
- **Legal/rights:** licencias y permisos; aprobación factual no concede derechos de uso.
