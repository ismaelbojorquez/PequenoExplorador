# ExecPlan — bootstrap determinista y servicios transversales seguros

- Fase/Gate: 06 / B
- Estado: Complete
- Creado/actualizado: 2026-08-15 00:00 America/Mexico_City
- Owner: Principal Game Engineer

## Propósito y alcance

Implementar un composition root determinista que inicialice y apague puertos mínimos sin acoplar features a Unity ni SDKs. Application contendrá lifecycle y puertos; Infrastructure, implementaciones locales Null/Mock/seguras; Presentation, una vista de estado; Bootstrap, configuración, registro tipado privado y ensamblado. Incluye cancelación, reloj/azar inyectables, bus acotado, tests y smoke Android. Excluye SDKs, save completo, scene flow, gameplay, cuentas, red, signing y publicación.

Criterios: estado visible `Ready`, fallo recuperable, shutdown inverso/idempotente, cero listeners tras cierre, perfil Release sin mocks ni diagnóstico, un bootstrap tras reload y pipeline completo con código cero.

## Contexto y orientación

Base limpia: commit `d1b008006e27d77ffbef3801ced12cd8b96b5ecf`, Unity `6000.3.22f1`, nueve assemblies y pipeline F05. `DiagnosticBootstrap` es solo un marcador sin lifecycle; `Shared` contiene únicamente `.gitkeep`; Application/Infrastructure/Presentation solo tienen markers. La escena `Bootstrap` contiene un único componente diagnóstico y una UI placeholder.

Fuentes: `AGENTS.md`, `.agent/PLANS.md`, `docs/STATUS.md`, arquitectura, estándares, decisiones, playbook, testing, riesgos, roadmap, `Packages/`, todo Bootstrap/Shared y suites existentes.

## Progreso

- [x] 2026-08-14 23:14 — preflight Git/documentos/código/configuración completo; árbol limpio.
- [x] 2026-08-14 23:16 — baseline wrappers: checks, compile, EditMode 5/5 y PlayMode 1/1 pasan.
- [x] 2026-08-14 23:37 — implementados puertos, lifecycle, adapters, composición y estado visual recuperable.
- [x] 2026-08-14 23:45 — añadidos fixtures/tests de orden, idempotencia, concurrencia, fallo/retry, cancelación, dispose, perfiles, listeners y reload.
- [x] 2026-08-15 00:00 — pipeline completo posterior al último test, APK/manifest/alineación, guard Release, revisión y documentación terminados.

## Hallazgos

- La solicitud vigente introduce composition root/servicios como F06, mientras el roadmap previo asignaba shell UI a F06. Se actualizará la fuente canónica sin ampliar el Vertical Slice; shell/interacción se reordenarán de forma explícita.
- No hay cambios ajenos, remotos ni dependencias nuevas. La escena actual puede reutilizar su panel placeholder para estado sin crear assets finales.
- La primera compilación detectó una ambigüedad real entre `System.AppContext` y el contexto de la aplicación; se resolvió con alias explícito y se recompiló.
- La primera ejecución extendida EditMode falló 1/15 porque `TaskCanceledException` es una subclase válida de `OperationCanceledException`; el test pasó a comprobar el contrato semántico y la suite completa se reejecutó.
- Unity completó tests y build, aunque emitió avisos no bloqueantes al apagar servidores .NET auxiliares; no hubo error de compilación ni fallo de proceso.
- Dos APK Development sucesivos conservaron tamaño pero no SHA-256; el empaquetado/firma de desarrollo incluye variación temporal. El manifest de build registra siempre el hash del artefacto entregado, pero no se afirma reproducibilidad byte a byte.

## Decisiones

- 2026-08-14 — usar `ApplicationHost` C# puro para lifecycle ordenado; MonoBehaviour solo adapta lifecycle Unity.
- 2026-08-14 — usar `AppContext` inmutable con propiedades tipadas y un `ServiceRegistry` interno a Bootstrap; no habrá `Get<T>`, instancia estática ni acceso global.
- 2026-08-14 — perfiles se resuelven con `UNITY_EDITOR` o define de build Development; Release se compone siempre con NullAnalytics/NoAds/UnavailablePurchase.
- 2026-08-14 — el bus en memoria devuelve `IDisposable`, se limpia en shutdown y solo sirve desacoplamiento uno-a-varios; llamadas directas siguen siendo default.

## Plan de implementación

1. Crear lifecycle, contexto y puertos mínimos BCL-only en Application.
2. Crear reloj, random seeded, logger Unity, message bus, Null/Mock/NoAds/Unavailable en Infrastructure.
3. Crear vista Presentation y composition root Bootstrap con configuración inmutable, registro tipado y cancelación.
4. Cablear la escena explícitamente y añadir define solo al BuildPlayer Development.
5. Extender tests/allowlist y verificar Release seguro, orden/cleanup/reload.
6. Actualizar arquitectura, estándares, decisiones, testing, status, roadmap, riesgos, changelog y AGENTS.
7. Ejecutar `scripts/validate`, inspeccionar APK/diff y crear el commit autorizado.

## Comandos y validación

- `git status --short --branch` — PASS, `## main` limpio.
- `git branch --show-current` — PASS, `main`.
- `git log -1 --format=fuller` — PASS, commit F05 esperado.
- `scripts/check-repository` — PASS, 0.295 s.
- `scripts/compile` — PASS, 10.598 s.
- `scripts/test-editmode` — PASS 5/5, 12.373 s.
- `scripts/test-playmode` — PASS 1/1, 14.877 s.
- `scripts/validate` final — PASS, código `0`, 1:14.27 con caché; checks, compile, EditMode 19/19, PlayMode 2/2 y APK Development.
- `aapt2 dump badging/permissions` — PASS; min/target/compile 26/36/36, sin permisos sensibles ni `AD_ID`.
- `zipalign -c -P 16 4` — PASS; SHA-256 registrado por ejecución en el manifest ignorado.
- `scripts/build-android-release` — PASS del guard fail-closed: código esperado `3`, BuildPipeline no invocado y reporte `BLOCKED` sin signing.
- AAB Release/iOS/CI remota — NOT RUN: signing e identidad no autorizados, módulo iOS ausente y sin remoto/runner.

## Recovery y seguridad

No cambiar asmdefs sin actualizar allowlist/tests; no introducir SDKs, permisos, secretos, red, save o gameplay. Si la escena queda sin referencia, restaurar mediante patch acotado y reimportar; no editar `Library`. Los servicios Release son fail-closed. Los artifacts quedan en `artifacts/` ignorado. No remoto, push, signing ni publicación.

## Resultados y retrospectiva

Los criterios locales quedan cumplidos: `Ready` visible, fallo con retry, un único bootstrap tras reload, lifecycle y listeners deterministas, Development con mocks explícitos y Release fail-closed sin compilar simuladores. El APK Development final pesa `57,069,510 bytes` y fue generado en 18.950 s con caché. No se añadieron globals, SDKs, dependencias ni gameplay.

La prueba más útil fue tratar selección de perfil como propiedad tanto de compilación como de composición: el símbolo se inyecta en las opciones del build Development, no persiste en PlayerSettings, y el código Mock completo está protegido por preprocesador. La prueba de binario Release queda necesariamente pendiente del signing humano; la evidencia actual es estática y de tests, no un AAB Release.
