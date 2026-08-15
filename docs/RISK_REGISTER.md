# Registro de riesgos

Escala: probabilidad (P) e impacto (I) de 1–5; severidad = P×I. `Owner humano` indica una decisión que ingeniería no puede asumir.

| ID | Riesgo | P | I | Severidad | Mitigación / evidencia de cierre | Owner | Revisar |
|---|---|---:|---:|---:|---|---|---|
| R-001 | Unity latest-patch cambia o `6000.3.22f1` presenta regresión crítica. | 2 | 4 | 8 | ADR aceptada tras import/tests/smoke; pin exacto y no migración silenciosa. Revalidar mensualmente. | Tech Director | Mensual |
| R-002 | AAB o librería nativa no soporta páginas de 16 KB. | 2 | 5 | 10 | APK F03 pasó zipalign/ELF y emulador 16 KB; repetir AAB/bundletool y todo SDK futuro. | Release Eng | F12, F47 |
| R-003 | Deadline API 36 del 2026-08-31 bloquea publicación. | 2 | 5 | 10 | ProjectSettings y APK reportan target/compile 36; falta AAB/Play pre-launch y revalidación. | Release Eng | F12, F47 |
| R-004 | SDK transitivo recolecta datos o viola Families/Kids. | 3 | 5 | 15 | Sin SDKs en MVP; allowlist, SBOM, revisión de términos/permisos/tráfico por versión. | Privacy + Eng | Antes de cada SDK |
| R-005 | Monetización infantil introduce dark patterns, compras accidentales o ads no aptos. | 3 | 5 | 15 | Lanzamiento recomendado ad-free; ADR humana, parental gate y revisión legal antes de monetizar. | Owner humano | Gate D/E |
| R-006 | Falta Xcode/iOS module impide validar iOS-ready. | 5 | 3 | 15 | Mantener fronteras de plataforma; adquirir/configurar toolchain y smoke build en F48. | Release Eng | F48 |
| R-007 | Público 4–9 cruza capacidades lectoras y bandas Apple Kids. | 4 | 4 | 16 | Instrucción audiovisual, pruebas separadas 4–5/6–8/9 y decisión de metadata humana. | Product/UX | F08, F40 |
| R-008 | Scope creep más allá de Selva. | 4 | 4 | 16 | Roadmap, definición MVP y change control; no crear otros biomas antes de Gate E. | Producer | Cada fase |
| R-009 | Activación/licencia Unity no permite batch build. | 1 | 4 | 4 | Cerrado localmente: import, tests y Android batch build terminaron; no se guardaron credenciales. Reabrir en CI/nueva máquina. | Owner + Release | CI/nueva máquina |
| R-010 | Assets/audio carecen de derechos o atribución. | 3 | 5 | 15 | Provenance ledger, licencia y releases antes de importar; H-001. | Owner humano | F14–F16 |
| R-011 | Rendimiento/memoria en Android de gama baja es insuficiente. | 4 | 4 | 16 | Budgets, Addressables locales, profiling temprano y device matrix. | Principal Eng | F12, F33, F38 |
| R-012 | Estado local se pierde/corrompe o expone información infantil. | 3 | 4 | 12 | Datos mínimos seudónimos, versionado/migración, pruebas de corrupción y reset adulto. | Principal Eng | F08, F34 |
| R-013 | Fichas de tienda no coinciden con tráfico/binario. | 3 | 5 | 15 | Auditoría de permisos/red/SBOM; cuatro ojos para Data safety/App Privacy. | Privacy + Release | F50–F52 |
| R-014 | Contenido educativo o cultural es inexacto/inadecuado. | 3 | 4 | 12 | Revisión pedagógica/cultural y pruebas con adultos responsables; no claims no validados. | Content owner | F17–F30 |
| R-015 | Ausencia de licencia del producto bloquea colaboración/distribución. | 4 | 4 | 16 | Resolver H-001; mantener aviso sin licencia y no incorporar terceros dudosos. | Owner humano | Antes de terceros |
| R-016 | Política cambia después de esta investigación. | 4 | 5 | 20 | Fechas/fuentes explícitas y revalidación F41/F50/F52 y cada release. | Release + Legal | Continua |
| R-017 | Claim, especie, arte o audio factual llega a Release sin aprobación. | 3 | 5 | 15 | Registro por claim y bloqueo `ReleaseLocked`; revisor humano obligatorio. | Content + QA | Slice y cada lote |
| R-018 | Producción masiva antes de validar el slice desperdicia arte/audio/contenido. | 4 | 4 | 16 | Gate de escalado en `MVP_SCOPE.md`; placeholders hasta Gate B. | Game Director | F04–F13 |
| R-019 | Playtest con menores carece de consentimiento o minimización. | 2 | 5 | 10 | Protocolo humano, consentimiento, datos mínimos y derecho a detener. | Research + Legal | Antes de playtest |
| R-020 | Tap-to-move no es comprensible o accesible para 4–9. | 3 | 4 | 12 | Comparar con control directo simplificado antes de fijar implementación. | UX + Eng | F08 |
| R-021 | Una sesión nueva actúa con status/plan obsoleto o memoria de chat. | 3 | 4 | 12 | `AGENTS` + `STATUS`, prueba de reanudación y actualización en cada fase/hito. | Staff Architect | Cada fase |
| R-022 | Dependencia introduce licencia, datos, permisos o binario nativo incompatible. | 3 | 5 | 15 | Intake obligatorio antes de instalación, transitivos/SBOM, 16 KB y rollback. | Tech + Privacy | Antes de dependencia |
| R-023 | Placeholder temporal llega a Release o comunica un hecho no aprobado. | 3 | 4 | 12 | Prefijo/metadata `PH_`, validador futuro y bloqueo explícito de Release. | Content + QA | Desde F03 |
| R-024 | Bundle ID/company placeholder se usan accidentalmente en stores. | 3 | 5 | 15 | Valores `placeholder`, decisión T-014, checklist Release y reemplazo humano antes de consola. | Owner + Release | Antes de F46 |
| R-025 | Manifest Release conserva red/permisos no justificados por un build Development. | 3 | 5 | 15 | El smoke solo mostró `INTERNET` y permiso interno; generar AAB Release, comparar merged manifest y bloquear permisos no aprobados. | Privacy + Release | F12, F49 |
| R-026 | El grafo de assemblies deriva, crea ciclos o se sobrefragmenta. | 2 | 4 | 8 | Allowlist sobre nueve asmdefs reales, cycle detection, fixture inválida y review obligatoria de toda nueva frontera. | Principal Architect | Cada cambio de asmdef |
| R-027 | CI aparenta cobertura Unity aunque falten remoto, runner o activación. | 3 | 4 | 12 | Job Unity manual, condicionado por variable y self-hosted; requerir solo check estático hasta tener evidencia remota. | Release Eng | Al crear remoto/runner |
| R-028 | Action o script CI filtra credenciales o cambia supply chain. | 2 | 5 | 10 | Actions oficiales fijadas por SHA/licencia, `contents: read`, checkout sin credenciales, artifacts breves y secret scan básico; intake para cambios. | DevSecOps | Cada cambio de workflow |

## Riesgo residual tras Fase 04

Import, tests de frontera/escena, APK y emulador reducen R-001/R-002/R-003/R-009/R-026, pero no prueban AAB Release, firma, dispositivo físico, performance, safe area, iOS ni aceptación de tienda. R-006, R-024 y R-025 continúan explícitos. No hubo gameplay, playtest infantil ni validación legal/store.
