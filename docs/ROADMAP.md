# Roadmap — Gates A–F y Fases 00–57

El roadmap conserva el MVP Selva. Una fase no está aprobada por existir en esta tabla: necesita preflight, evidencia, documentación y commit propios. Los gates son decisiones de continuar; no fechas prometidas.

## Gates

| Gate | Fases | Criterio de salida |
|---|---|---|
| **A — Foundation ready** | 00–03 | Producto/Vertical Slice especificados; repo y proyecto reproducibles; ADR Editor cerrada; fronteras/entry point y calidad mínima compilando; Android smoke APK o AAB vacío. |
| **B — Vertical slice playable** | 04–13 | Un loop Selva representativo es jugable offline en Android objetivo, con progreso local, audio/UI provisional y budgets medidos. |
| **C — MVP content complete** | 14–30 | Todo el contenido Selva acordado está integrado, revisado y accesible para 4–9; no hay otros biomas ni features fuera de alcance. |
| **D — Feature complete / beta ready** | 31–43 | Tests, dispositivos, privacidad y UX infantil respaldan beta; modelo comercial y Apple Kids tienen decisión humana explícita. |
| **E — Release candidate compliant** | 44–52 | AAB/API/16 KB, firma, permisos, metadata y auditorías de store pasan con artefactos; iOS-ready tiene smoke build o bloqueo declarado. |
| **F — Launch and learn** | 53–57 | Testing cerrado, aprobación humana, rollout controlado y revisión post-lanzamiento con riesgos/roadmap actualizados. |

## Fases 00–57

| Fase | Entregable acotado | Gate |
|---:|---|:---:|
| 00 | Inicialización Git, investigación oficial y baseline documental; sin proyecto Unity. | A |
| 01 | Visión, GDD, loops, educación, UX, contratos data-driven y alcance canónico de Vertical Slice/MVP; sin Unity. | A |
| 02 | Contrato operativo de agentes, planes vivos, estándares, review, validación y status; sin Unity. | A |
| 03 | Foundation Unity reproducible: Editor/módulos, proyecto URP, estructura/asmdefs mínimos, Bootstrap futuro, tests y smoke Android APK/AAB vacío; capas de gameplay se crean con casos reales. | A |
| 04 | Fronteras físicas de Domain/Application/adaptadores/Bootstrap, Editor y tests; grafo validado, sin gameplay. | B |
| 05 | Pipeline reproducible local/CI: import, compile, validadores, JUnit, smoke Android y Release bloqueado sin signing. | B |
| 06 | Composition root, lifecycle y puertos/adapters locales seguros para reloj, azar, logs, mensajes, analytics, ads e IAP; sin SDKs ni gameplay. | B |
| 07 | Shell landscape, navegación uGUI/TMP, safe areas y prototipos/playtests de input 4–9: tap-to-move vs alternativa; sin fijar controles antes de evidencia. | B |
| 08 | Dominio puro del loop, actividad, resultado y progreso, con tests. | B |
| 09 | Persistencia local versionada, reset adulto y recuperación de corrupción. | B |
| 10 | Sistema de audio, mezcla, voz/SFX y controles locales. | B |
| 11 | Pipeline ScriptableObject de authoring y validación de contenido. | B |
| 12 | Pipeline Android AAB ARM64/IL2CPP, inspección 16 KB y budgets medidos de frame, memoria, tamaño, carga, batería y térmicas. | B |
| 13 | Vertical Slice canónico Selva end-to-end en dispositivo; gate explícito antes de escalar contenido. | B |
| 14 | Pipeline de arte, naming, import presets, provenance y validadores. | C |
| 15 | Pipeline de audio, masters, compresión, loudness, licencias y validadores. | C |
| 16 | Localización base, fallback, fuentes/glifos y expansión de layouts. | C |
| 17 | Hub/mapa de Selva y selección de actividad, sin otros biomas. | C |
| 18 | Patrón de observar/escuchar instrucciones y ejemplo guiado. | C |
| 19 | Actividad Selva de asociación/clasificación. | C |
| 20 | Actividad Selva de conteo. | C |
| 21 | Actividad Selva de identificación. | C |
| 22 | Feedback positivo, recompensa cosmética local y retorno al loop sin coerción. | C |
| 23 | Tutorial adaptativo y reentrada para prelectores. | C |
| 24 | Balance y contenido para 4–5 años. | C |
| 25 | Balance y contenido para 6–7 años. | C |
| 26 | Balance y contenido para 8–9 años. | C |
| 27 | Lote de contenido Selva 1 revisado pedagógica/culturalmente. | C |
| 28 | Lote de contenido Selva 2; solo si pertenece al alcance MVP aprobado. | C |
| 29 | Pase de accesibilidad: contraste, tamaño, audio, motion y no dependencia de color/texto. | C |
| 30 | Content complete Selva y auditoría de ausencia de scope creep. | C |
| 31 | Estrategia integral de pruebas y fixtures deterministas. | D |
| 32 | Cobertura automatizada de dominio, aplicación, persistencia y validadores. | D |
| 33 | Optimización con profiler contra budgets y dispositivos representativos. | D |
| 34 | Migraciones de save, compatibilidad de upgrades y pruebas destructivas controladas. | D |
| 35 | Resiliencia offline, primera ejecución, interrupciones y falta de almacenamiento. | D |
| 36 | Matriz de aspect ratios, safe areas, orientación, input y lifecycle móvil. | D |
| 37 | QA de audio, inteligibilidad de voz, mezcla, interrupciones y audífonos. | D |
| 38 | Matriz Android low/mid/high, `minSdk` comercial y sesiones sostenidas. | D |
| 39 | Validación UX con adultos responsables/menores bajo protocolo aprobado y acciones. | D |
| 40 | Área parental y decisión humana Apple Kids/age bands; gates sin patrones engañosos. | D |
| 41 | Data inventory, threat/privacy review, territorios y borradores legales humanos. | D |
| 42 | Decisión de modelo comercial; mantener null ads/IAP/analytics si no se aprueba. | D |
| 43 | Alpha/beta readiness review y cierre explícito del Gate D. | D |
| 44 | Feature/dependency freeze, SBOM, licencias y provenance completos. | E |
| 45 | QA de localización, créditos, avisos y copy legal aprobado. | E |
| 46 | Versionado, firma, upload key, backups y builds reproducibles. | E |
| 47 | Compliance Android: API 36, AAB, Play App Signing, 16 KB y pre-launch report. | E |
| 48 | Smoke build iOS, Xcode soportado, export, privacy manifests y device test. | E |
| 49 | Auditoría final de permisos, tráfico, secretos, dependencias y superficie de ataque. | E |
| 50 | Play Console: Families, target audience, content rating, Data safety y listing. | E |
| 51 | App Store Connect: Kids/age rating, privacy, metadata y review notes. | E |
| 52 | Release candidate firmado, matriz de evidencias y go/no-go de envío. | E |
| 53 | Closed testing/TestFlight según alcance, crash/feedback y fixes solo aprobados. | F |
| 54 | Go/no-go humano, estrategia de rollout, soporte y plan de rollback. | F |
| 55 | Publicación controlada en Android; iOS solo si fue aprobada y validada. | F |
| 56 | Observación de store health y soporte con datos mínimos; respuesta a incidentes. | F |
| 57 | Postmortem, métricas apropiadas, deuda, políticas revalidadas y roadmap siguiente. | F |

## Reglas de control

- Agregar un bioma o backend requiere decisión de alcance posterior al MVP, no renombrar una fase existente.
- Ads, analytics o IAP requieren ADR propia, revisión de políticas por versión/territorio y aprobación humana.
- Un gate externo fallido se reporta como fallo o bloqueo, nunca como PASS documental.
- Antes de cada fase se contrasta el estado real con Git, archivos, tests, builds y fuentes; no se hereda confianza del reporte anterior.
