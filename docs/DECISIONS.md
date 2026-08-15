# Registro de decisiones

Estados: **Provisional** requiere validación de fase; **Aceptada** gobierna el alcance; **Humana pendiente** no puede resolverse solo con ingeniería.

## ADR-0001 — Selección de Unity 6.3 LTS

- Estado: **Aceptada**, 2026-08-14.
- Decisión: fijar `Unity 6000.3.22f1 (1c726e1fb402)` para el proyecto.
- Contexto: es Unity 6.3 LTS, la última revisión que devolvió la API oficial de releases el 2026-08-14; se publicó el 2026-08-13 y está instalada localmente con módulos Android. Unity anuncia dos años de soporte para 6.3 LTS.
- Evidencia: Release API revalidada; licencia batch; creación/import sin errores; paquetes exactos; EditMode `2/2`; APK Development API 36, IL2CPP/ARM64; inspección de manifest/ELF/zipalign; ejecución visual en emulador 16 KB. El AAB Release sigue diferido a F11.
- Consecuencias: `ProjectVersion.txt` gobierna; no se usan otras revisiones ni se migra silenciosamente. Cada cambio de Editor requiere ADR/plan, backup y repetición de import, tests y smoke.
- Alternativa: si hay bloqueo reproducible, evaluar explícitamente otra revisión parcheada de 6.3 LTS; solo después, una LTS soportada alternativa. No instalar ni migrar silenciosamente.
- Rollback: antes de contenido, recrear el proyecto vacío con la revisión aprobada; después de contenido, exigir plan de migración y backup/branch.
- Fuentes: [release 6000.3.22f1](https://unity.com/releases/editor/whats-new/6000.3.22f1), [anuncio 6.3 LTS](https://unity.com/blog/unity-6-3-lts-is-now-available), verificadas 2026-08-14.

## Decisiones técnicas aceptadas

| ID | Decisión | Razón | Reabrir |
|---|---|---|---|
| T-001 | Android-first, landscape; iOS-ready. | Prioriza el MVP sin acoplar dominio a plataforma. | Si cambia mercado objetivo. |
| T-002 | Offline-first, sin backend en MVP. | Reduce datos infantiles, operación y riesgo. | Solo con caso de uso, DPIA/privacidad y presupuesto aprobados. |
| T-003 | Capas `Domain` C# puro → `Application` → `Infrastructure`/`Presentation`/`Content`. | Testabilidad y fronteras claras con Unity/SDKs. | En revisión de arquitectura Fase 03. |
| T-004 | Composition root explícito; sin service locator global. | Dependencias visibles y reemplazables. | Fase 03. |
| T-005 | ScriptableObjects para authoring, no como estado mutable de sesión. | Flujo de contenido amigable sin contaminar dominio. | Tras prueba del pipeline de contenido. |
| T-006 | uGUI + TextMeshPro para runtime del MVP. | Madurez en UI móvil Unity y texto localizado. | Prototipo de UI Fase 05. |
| T-007 | Addressables local-first; catálogo/contenido remoto deshabilitado en MVP. | Organización y memoria sin dependencia de red. | Si tamaño o live content lo exige. |
| T-008 | Puertos con implementaciones mock/null para analytics, ads e IAP. | No integrar SDKs ni transmitir datos por accidente. | Tras aprobación comercial/legal. |
| T-009 | AAB, ARM64 e IL2CPP como objetivo de release Android. | Alineación con Play y native libraries; debe validarse 16 KB. | Build pipeline Android. |
| T-010 | ExecPlan vivo solo para trabajo complejo, transversal, riesgoso o multisesión. | Conserva contexto/recovery donde aporta; evitarlo en cambios triviales reduce duplicación y planes obsoletos. | Si la reanudación o trazabilidad resultan insuficientes. |
| T-011 | Evidencia con estados `PASS`, `FAIL`, `BLOCKED` y `NOT RUN`. | Impide convertir intención, documentación o bloqueo externo en resultado ejecutado. | No se relaja; puede ampliarse el playbook. |
| T-012 | Placeholders `PH_` con metadata y bloqueo de Release. | Permite prototipar sin confundir material temporal con contenido aprobado. | Tras implementar validador de contenido. |
| T-013 | Paquetes directos F03: Input System `1.20.0`, URP `17.3.0`, Test Framework `1.6.0`, uGUI `2.0.0`. | Es la baseline oficial mínima que compila y construye; Addressables/Localization no tienen necesidad todavía. | Intake y evidencia completa antes de cualquier cambio. |
| T-014 | Bundle ID técnico `com.placeholder.pequenoexplorador` y company `Placeholder Studio`. | Permiten builds reproducibles sin asumir titularidad comercial. | Obligatorio reemplazar mediante decisión humana antes de crear registros en stores. |
| T-015 | Nueve assemblies con allowlist ejecutable: Domain, Application, Content, Infrastructure, Presentation, Bootstrap, Editor y dos Tests. | Hace físicas las fronteras sin fragmentar por features inexistentes; Bootstrap es el único composition root. | Solo con evidencia medida y actualización de validador, tests, arquitectura y riesgo. |

## Decisiones de producto aceptadas

| ID | Decisión | Límite |
|---|---|---|
| P-001 | Público de 4–9 años. | UX y contenido deben funcionar para prelectores y lectores tempranos. |
| P-002 | MVP limitado al mundo Selva. | Otros biomas, multiplayer, cuentas y contenido remoto quedan fuera. |
| P-003 | Sesiones breves y feedback positivo, sin castigo ni dark patterns. | No rachas coercitivas, loot boxes, chat ni presión de compra. |
| P-004 | Experiencia inicial sin publicidad. | Ads es una decisión posterior condicionada, no un backlog implícito. |
| P-005 | Dos modos manuales: `Más guía` y `Guía estándar`. | No se pide edad/fecha; mismo contenido, progreso y recompensa. |
| P-006 | Tap-to-move es candidato, no control fijado. | Comparar con alternativa simplificada en F07 antes de implementar definitivamente. |
| P-007 | Economía blanda determinista para campamento. | Sin pérdida, compra, azar, caducidad, rachas o ventaja. |
| P-008 | Vertical Slice bloquea escalado de contenido. | No producir catálogo MVP hasta pasar Gate B y aprobación factual. |

## Decisiones humanas/comerciales pendientes

| ID | Decisión pendiente | Quién decide | Fecha límite sugerida |
|---|---|---|---|
| H-001 | Titular legal, licencia del producto y cadena de derechos. | Propietario + asesoría legal | Antes de incorporar terceros. |
| H-002 | Modelo de negocio: pago único, gratis, IAP o sin monetización. | Producto/negocio + legal | Antes del Gate D. |
| H-003 | Participación en Apple Kids Category y bandas 5-under/6–8/9–11. | Producto + legal | Antes de metadata iOS. |
| H-004 | Países de lanzamiento y revisión COPPA/GDPR/leyes locales. | Negocio + privacidad/legal | Antes de pruebas externas con menores. |
| H-005 | Política de privacidad pública y datos de contacto. | Titular + legal | Antes de fichas de tienda. |
| H-006 | Si alguna vez habrá ads/analytics/IAP y proveedores aprobados. | Producto + legal + ingeniería | ADR separada; no antes de Gate D. |
| H-007 | Especialista factual y responsables de aprobación de contenido. | Producto + educación | Antes de producir contenido del slice. |
| H-008 | Protocolo, consentimiento y reclutamiento de playtests con menores. | Research + legal/privacidad | Antes de playtest con participantes. |
