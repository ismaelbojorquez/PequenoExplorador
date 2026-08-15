# Pequeño Explorador: Aprende Jugando

Baseline de ingeniería y cumplimiento para un juego educativo Unity 2.5D/3D infantil, en orientación horizontal, Android-first, iOS-ready, offline-first y sin backend en el MVP.

## Producto acotado

- Público principal: niñas y niños de 4 a 9 años, con decisiones comerciales y consentimientos a cargo de una persona adulta.
- MVP: un único mundo, **Selva**, sin ampliar todavía a otros biomas.
- Loop: elegir una actividad breve → observar una instrucción audiovisual → explorar e interactuar → resolver una identificación, asociación o conteo → recibir feedback positivo y progreso local → volver al mapa de Selva.
- Experiencia inicial recomendada: sin publicidad. Ads, IAP y analítica remota permanecen detrás de interfaces nulas y de decisiones humanas futuras.
- Privacidad por defecto: sin cuenta, chat, ubicación, cámara, micrófono, identificadores publicitarios ni transmisión de datos en el MVP.

Este repositorio aún **no contiene un proyecto Unity**. No existen escenas, código C#, assets, paquetes ni artefactos de build.

## Documentación

El índice y el orden de lectura están en [`docs/README.md`](docs/README.md). Las fuentes temporales fueron verificadas el 2026-08-14; no constituyen asesoría legal.

## Continuar con Fase 01

1. Ejecutar el preflight de [`AGENTS.md`](AGENTS.md), leer completos los documentos marcados para Fase 01 y comprobar que Git esté limpio en `main`.
2. Revalidar en fuentes oficiales que `6000.3.22f1` siga siendo la última revisión parcheada de Unity 6.3 LTS; si cambió, abrir una decisión explícita antes de actualizar el pin.
3. Confirmar licencia activa del Editor y módulos Android/iOS. No usar rutas locales persistentes ni SDK externos cuando los módulos soportados de Unity sean suficientes.
4. Crear el proyecto Unity en la raíz con la plantilla acordada en Fase 01, fijar la versión exacta y ejecutar un smoke build Android vacío. Esa acción está fuera del alcance de Fase 00.
5. Configurar localmente UnityYAMLMerge apuntando al Editor instalado; no versionar su ruta absoluta.

## Licencia

No se ha seleccionado licencia para el producto. Véase [`LICENSE_NOT_SELECTED.md`](LICENSE_NOT_SELECTED.md).
