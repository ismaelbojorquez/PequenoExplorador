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

El repositorio contiene una foundation Unity mínima: URP, escena temporal `Bootstrap`, configuración móvil, build CLI y dos tests EditMode. No contiene gameplay, save, Addressables, IAP, ads, backend ni arte final.

## Documentación

El índice y el orden de lectura están en [`docs/README.md`](docs/README.md). Las fuentes temporales fueron verificadas el 2026-08-14; no constituyen asesoría legal.

## Abrir el proyecto

Usar exactamente Unity `6000.3.22f1 (1c726e1fb402)`. Abrir esta raíz desde Unity Hub; no crear un proyecto anidado ni aceptar una migración silenciosa. El Editor y paquetes exactos están en [`ProjectSettings/ProjectVersion.txt`](ProjectSettings/ProjectVersion.txt) y [`Packages/manifest.json`](Packages/manifest.json).

Para validar en batch desde macOS:

```sh
UNITY_EDITOR="/Applications/Unity/Hub/Editor/6000.3.22f1/Unity.app/Contents/MacOS/Unity"

"$UNITY_EDITOR" -batchmode -nographics -quit \
  -projectPath "$(pwd)" \
  -logFile /tmp/pequeno-explorador-compile.log

"$UNITY_EDITOR" -batchmode -nographics \
  -projectPath "$(pwd)" \
  -runTests -testPlatform EditMode \
  -testResults /tmp/pequeno-explorador-editmode.xml \
  -logFile /tmp/pequeno-explorador-editmode.log
```

El smoke Android y sus perfiles están en [`docs/ANDROID_RELEASE.md`](docs/ANDROID_RELEASE.md). No se versionan `Library`, `Logs`, `.utmp`, builds ni credenciales.

## Continuar con Fase 04

1. Ejecutar el preflight de [`AGENTS.md`](AGENTS.md), empezar por [`docs/STATUS.md`](docs/STATUS.md) y contrastar Git, Editor, import y tests.
2. Leer [`docs/MVP_SCOPE.md`](docs/MVP_SCOPE.md), [`docs/TECHNICAL_ARCHITECTURE.md`](docs/TECHNICAL_ARCHITECTURE.md) y el alcance de Fase 04 en [`docs/ROADMAP.md`](docs/ROADMAP.md).
3. Crear un ExecPlan si el prototipo de interacción cruza input, escena, UX y playtest; no escalar contenido ni implementar sistemas fuera del Vertical Slice.
4. Comparar tap-to-move con una alternativa simplificada para prelectores y ambos modos de guía. Safe area y shell UI pertenecen a Fase 05.
5. Mantener placeholder/metadata, evidencia y revisión infantil; no instalar SDKs, publicar ni aceptar términos.

## Licencia

No se ha seleccionado licencia para el producto. Véase [`LICENSE_NOT_SELECTED.md`](LICENSE_NOT_SELECTED.md).
