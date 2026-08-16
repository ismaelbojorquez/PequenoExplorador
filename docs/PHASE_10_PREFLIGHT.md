# Preflight Prompt 10 — configuración runtime local

Fecha: 2026-08-16 09:57 (`America/Mexico_City`).

## Estado observado antes de editar

- Directorio: raíz del repositorio `PequenoExplorador`; `AGENTS.md` y `docs/STATUS.md` existen y se leyeron completos.
- Git: rama `main`, árbol limpio, HEAD `aea45b53cd694813fba2c4c984a94a17f550127f` (`docs(prompts): add master execution prompt catalog`).
- Se leyeron arquitectura, save, decisiones, estándares, validación, testing, content pipeline, roadmap, ExecPlans y Prompt 10; se inspeccionaron Bootstrap, Content, BuildTools, asmdefs y tests relacionados.
- Save v1 y scene flow no se aceptaron por reporte: `scripts/validate` pasó realmente compile/import, Addressables local, EditMode `46/46`, PlayMode `5/5` y APK Development.
- APK baseline: `60,278,339` bytes; SHA-256 `717c65eb16b6ccc3baa13ce170edc2b5efdf097ea60824d81b860e4e1d432cfc`; API 26/36, IL2CPP/ARM64.

## Inventario de hardcodes en alcance

| Valor/decisión actual | Ubicación | Tratamiento |
|---|---|---|
| Development/Release por define | `BuildProfileConfiguration` | Migrar a selección explícita de asset tipado; el define solo elige ID compilado. |
| Seed `20260814` | `BuildProfileConfiguration` | Mover a perfil. |
| Producto y versión técnica | `DiagnosticBootstrap` | Mover a perfil y conservar `PlayerSettings` build-time separado. |
| Timeout scene flow `20 s` | `ServiceRegistry` | Mover a perfil. |
| Debounce autosave `500 ms` | `ServiceRegistry` | Mover a perfil. |
| Diagnóstico, Mock Ads/Purchases y fallo simulado | Bootstrap/registry | Gobernar por flags Development; Release fail-closed. |
| API Android, IL2CPP/ARM64, paths/grupos Addressables | BuildTools/Content pipeline | Permanecen build-time/content-time; no son AppConfig runtime. |
| Modo de guía y audio | Save v1 | Permanecen preferencias parentales mutables; no se copian a AppConfig. |

## Desviación de numeración

El catálogo denomina este trabajo **Prompt 10**, mientras `docs/ROADMAP.md` reserva **Fase 10** para audio. Se ejecuta el prompt humano actual y se documenta como Prompt 10; no se declara completada la Fase 10 histórica ni se altera el Gate por inferencia.

No se observaron cambios ajenos ni colisiones. No se instalaron paquetes, no se usó red y no se modificó el sistema.
