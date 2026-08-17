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

[`VS-D-A01`](VS_D_A01_TOUCAN_FACTUAL_DOSSIER.md) reserva `Ramphastos sulfuratus`. Ismael Bojórquez aprobó Product/Education/copy/Localization en `H-007`, el asset visual/rights/QA en `H-008` y la revisión factual humana en `H-009`, con competencia declarada `Investigador — experiencia en búsqueda ampliada de información`. No se atribuye credencial ornitológica externa. El conflicto temporal de conservación queda excluido y bloqueado. El prefab visual es `Approved`; las definitions runtime continúan `Draft`/`PH_` hasta adopción técnica trazable.

Product/Localization eligió `Tucán pico canoa` y aprobó `Keel-billed Toucan` para EN. La variante `pico iris` permanece registrada como contexto regional, no como nombre del locale actual. La adopción debe crear records/facts/discovery/interacción no-`PH_` y conservar conservación fuera del runtime. `Approved` no equivale a `ReleaseLocked`: audio final, matriz efectiva y QA de la integración siguen separados.

## Responsabilidades humanas

- **Product/Education:** intención pedagógica y lenguaje apropiado.
- **Especialista factual:** exactitud y contexto.
- **Art/Audio:** fidelidad de representación y procedencia.
- **QA/Release:** trazabilidad y bloqueo efectivo.
- **Legal/rights:** licencias y permisos; aprobación factual no concede derechos de uso.
