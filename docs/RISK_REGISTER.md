# Registro de riesgos

Escala: probabilidad (P) e impacto (I) de 1–5; severidad = P×I. `Owner humano` indica una decisión que ingeniería no puede asumir.

| ID | Riesgo | P | I | Severidad | Mitigación / evidencia de cierre | Owner | Revisar |
|---|---|---:|---:|---:|---|---|---|
| R-001 | Unity latest-patch cambia o `6000.3.22f1` presenta regresión crítica. | 3 | 4 | 12 | Pin provisional, smoke build y ADR antes de contenido; no migración silenciosa. | Tech Director | F01, mensual |
| R-002 | AAB o librería nativa no soporta páginas de 16 KB. | 3 | 5 | 15 | ARM64/IL2CPP, inspección de `.so`, bundletool y prueba 16 KB sobre artefacto real. | Release Eng | F11, F47 |
| R-003 | Deadline API 36 del 2026-08-31 bloquea publicación. | 4 | 5 | 20 | Target/compile 36 desde F01; Play pre-launch y revalidación. | Release Eng | F01, F47 |
| R-004 | SDK transitivo recolecta datos o viola Families/Kids. | 3 | 5 | 15 | Sin SDKs en MVP; allowlist, SBOM, revisión de términos/permisos/tráfico por versión. | Privacy + Eng | Antes de cada SDK |
| R-005 | Monetización infantil introduce dark patterns, compras accidentales o ads no aptos. | 3 | 5 | 15 | Lanzamiento recomendado ad-free; ADR humana, parental gate y revisión legal antes de monetizar. | Owner humano | Gate D/E |
| R-006 | Falta Xcode/iOS module impide validar iOS-ready. | 5 | 3 | 15 | Mantener fronteras de plataforma; adquirir/configurar toolchain y smoke build en F48. | Release Eng | F48 |
| R-007 | Público 4–9 cruza capacidades lectoras y bandas Apple Kids. | 4 | 4 | 16 | Instrucción audiovisual, pruebas separadas 4–5/6–8/9 y decisión de metadata humana. | Product/UX | F04, F40 |
| R-008 | Scope creep más allá de Selva. | 4 | 4 | 16 | Roadmap, definición MVP y change control; no crear otros biomas antes de Gate E. | Producer | Cada fase |
| R-009 | Activación/licencia Unity no permite batch build. | 2 | 4 | 8 | Verificar activación en F01 sin guardar credenciales. | Owner + Release | F01 |
| R-010 | Assets/audio carecen de derechos o atribución. | 3 | 5 | 15 | Provenance ledger, licencia y releases antes de importar; H-001. | Owner humano | F14–F16 |
| R-011 | Rendimiento/memoria en Android de gama baja es insuficiente. | 4 | 4 | 16 | Budgets, Addressables locales, profiling temprano y device matrix. | Principal Eng | F12, F33, F38 |
| R-012 | Estado local se pierde/corrompe o expone información infantil. | 3 | 4 | 12 | Datos mínimos seudónimos, versionado/migración, pruebas de corrupción y reset adulto. | Principal Eng | F08, F34 |
| R-013 | Fichas de tienda no coinciden con tráfico/binario. | 3 | 5 | 15 | Auditoría de permisos/red/SBOM; cuatro ojos para Data safety/App Privacy. | Privacy + Release | F50–F52 |
| R-014 | Contenido educativo o cultural es inexacto/inadecuado. | 3 | 4 | 12 | Revisión pedagógica/cultural y pruebas con adultos responsables; no claims no validados. | Content owner | F17–F30 |
| R-015 | Ausencia de licencia del producto bloquea colaboración/distribución. | 4 | 4 | 16 | Resolver H-001; mantener aviso sin licencia y no incorporar terceros dudosos. | Owner humano | Antes de terceros |
| R-016 | Política cambia después de esta investigación. | 4 | 5 | 20 | Fechas/fuentes explícitas y revalidación F41/F50/F52 y cada release. | Release + Legal | Continua |

## Riesgo residual aceptado en Fase 00

No se ejecutó build ni validación de tienda porque no existe proyecto. Por tanto R-001/R-002/R-003/R-006/R-009 siguen abiertos: la documentación reduce incertidumbre, pero no los convierte en PASS.
