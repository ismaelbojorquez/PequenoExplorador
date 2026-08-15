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

Este repositorio aún **no contiene un proyecto Unity**. No existen escenas, código C#, assets, paquetes ni artefactos de build.

## Documentación

El índice y el orden de lectura están en [`docs/README.md`](docs/README.md). Las fuentes temporales fueron verificadas el 2026-08-14; no constituyen asesoría legal.

## Continuar con Fase 02

1. Ejecutar el preflight de [`AGENTS.md`](AGENTS.md), leer completos los documentos marcados para Fase 02 y comprobar que Git esté limpio en `main`.
2. Leer el paquete de producto de Fase 01 y usar [`docs/MVP_SCOPE.md`](docs/MVP_SCOPE.md) como fuente de verdad; la implementación inicial se limita al Vertical Slice.
3. Revalidar en fuentes oficiales que `6000.3.22f1` siga siendo la última revisión parcheada de Unity 6.3 LTS; si cambió, abrir una decisión explícita antes de actualizar el pin.
4. Confirmar licencia activa del Editor y módulos Android/iOS. No usar rutas locales persistentes ni SDK externos cuando los módulos soportados de Unity sean suficientes.
5. Crear el proyecto Unity en la raíz, fijar la versión exacta y ejecutar un smoke build Android vacío. Esa acción está fuera de las Fases 00–01.
6. Configurar localmente UnityYAMLMerge apuntando al Editor instalado; no versionar su ruta absoluta.

## Licencia

No se ha seleccionado licencia para el producto. Véase [`LICENSE_NOT_SELECTED.md`](LICENSE_NOT_SELECTED.md).
