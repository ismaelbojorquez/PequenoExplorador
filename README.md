# Pequeño Explorador: Aprende Jugando

Baseline de producto, ingeniería y cumplimiento para un juego educativo Unity 2.5D/3D infantil, en orientación horizontal, Android-first, iOS-ready, offline-first y sin backend en el MVP.

## Producto acotado

- Público principal: niñas y niños de 4 a 9 años, con decisiones comerciales y consentimientos a cargo de una persona adulta.
- MVP: un único mundo, **Selva**, sin ampliar todavía a otros biomas.
- Pilar: `acción → descubrimiento → aprendizaje → recompensa`.
- Vertical Slice y cantidades del MVP: definidos en [`docs/MVP_SCOPE.md`](docs/MVP_SCOPE.md); no se escala contenido antes de validar el slice.
- Asistencia sin pedir edad: `Más guía` y `Guía estándar`, con el mismo contenido, progreso y recompensa.
- Experiencia inicial recomendada: sin publicidad. Ads, IAP y analítica remota permanecen detrás de interfaces nulas y de decisiones humanas futuras.
- Privacidad por defecto: sin cuenta, chat, ubicación, cámara del dispositivo, micrófono, identificadores publicitarios ni transmisión de datos en el MVP.

El repositorio contiene una foundation Unity mínima: URP, composition root explícito, perfiles runtime Development/Release locales y validados, servicios Null/Mock seguros, nueve assemblies, flujo aditivo `Boot → Camp ↔ Jungle`, Addressables solo locales, persistencia local schema v1, BuildTools, tests y smoke Android. No contiene gameplay, remote config, contenido remoto, SDKs de IAP/ads/analytics, backend ni arte final.

## Documentación

El índice y el orden de lectura están en [`docs/README.md`](docs/README.md). Las fuentes temporales fueron verificadas el 2026-08-14; no constituyen asesoría legal.

## Abrir el proyecto

Usar exactamente Unity `6000.3.22f1 (1c726e1fb402)`. Abrir esta raíz desde Unity Hub; no crear un proyecto anidado ni aceptar una migración silenciosa. El Editor y paquetes exactos están en [`ProjectSettings/ProjectVersion.txt`](ProjectSettings/ProjectVersion.txt) y [`Packages/manifest.json`](Packages/manifest.json).

Para la validación completa en macOS o Linux:

```sh
scripts/validate
```

El wrapper localiza la revisión fijada de Unity Hub o usa `UNITY_EDITOR` si se proporciona. Comandos parciales, reportes JUnit y recovery están en [`docs/18_TESTING.md`](docs/18_TESTING.md); Android está en [`docs/20_ANDROID_RELEASE.md`](docs/20_ANDROID_RELEASE.md). Todos los outputs van a `artifacts/`, que Git ignora. No se versionan `Library`, `Logs`, `.utmp`, builds ni credenciales.

`scripts/build-addressables-local` valida perfiles/grupos/labels y construye el catálogo Android local. `Bootstrap` es la única escena de Build Settings; `Camp` y `Jungle` son placeholders Addressable aditivos. Remote catalogs, CDN y URLs permanecen deshabilitados.

El workflow GitHub ejecuta checks estáticos sin secretos. La validación Unity remota es manual y requiere un runner propio explícitamente habilitado; no se ha ejecutado en GitHub porque todavía no existe remoto. Véase [`docs/GITHUB_SETUP.md`](docs/GITHUB_SETUP.md).

## Continuar desde el estado vivo

1. Ejecutar el preflight de [`AGENTS.md`](AGENTS.md), empezar por [`docs/STATUS.md`](docs/STATUS.md) y contrastar Git, Editor, import y tests.
2. Leer [`docs/MVP_SCOPE.md`](docs/MVP_SCOPE.md), [`docs/02_TECHNICAL_ARCHITECTURE.md`](docs/02_TECHNICAL_ARCHITECTURE.md), [`docs/10_SAVE_SYSTEM.md`](docs/10_SAVE_SYSTEM.md), [`docs/RUNTIME_CONFIGURATION.md`](docs/RUNTIME_CONFIGURATION.md) y el alcance vigente en [`docs/ROADMAP.md`](docs/ROADMAP.md).
3. Reejecutar `scripts/validate`; no heredar el PASS de esta fase.
4. Mantener scene flow y save detrás de Application/Infrastructure; Bootstrap solo compone. Ninguna feature puede leer JSON/archivos, cargar AppConfig desde `Resources` ni mutar DTOs/config directamente.
5. Mantener placeholder/metadata, evidencia y revisión infantil; no instalar SDKs, publicar ni aceptar términos.

## Licencia

No se ha seleccionado licencia para el producto. Véase [`LICENSE_NOT_SELECTED.md`](LICENSE_NOT_SELECTED.md).
