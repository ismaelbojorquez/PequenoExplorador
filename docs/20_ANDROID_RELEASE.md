# Android — builds reproducibles y Release bloqueado

Fecha de baseline: 2026-08-14. Este pipeline no firma, sube ni publica.

## Toolchain y perfil Development

| Elemento | Valor fijado |
|---|---|
| Unity | `6000.3.22f1 (1c726e1fb402)` |
| SDK / Build Tools | bundled; API/Build Tools `36` / `36.0.0` |
| NDK / JDK | bundled `r27c (27.2.12479018)` / Temurin `17.0.18+8` |
| Gradle | bundled `9.1.0` |
| min / target API | `26` provisional / `36` |
| backend / ABI | IL2CPP / ARM64 (`arm64-v8a`) |
| formato | APK Development; sin firma externa |

`scripts/build-android-development` reafirma esos settings, construye primero Addressables con `LocalDevelopment`, usa la escena Bootstrap habilitada y escribe exclusivamente bajo `artifacts/`. `artifacts/reports/android-development.json` registra UTC, Editor, commit de entrada, path relativo, bytes, SHA-256, duración, APIs, backend, ABI y estado de firma. BuildTools inyecta `PE_DEVELOPMENT_SERVICES` únicamente en `BuildPlayerOptions.extraScriptingDefines` de este perfil; no lo persiste en PlayerSettings. El catálogo y bundles locales deben existir en `assets/aa/`; no se configura remote catalog.

## Release

```sh
scripts/build-android-release
```

Es un placeholder seguro: crea un reporte `BLOCKED`, imprime `PE_RELEASE_SIGNING_REQUIRED` y sale con código `3` antes de invocar BuildPipeline. El perfil Release no recibe `PE_DEVELOPMENT_SERVICES`; las clases Mock quedan excluidas por preprocesador y la composición selecciona `NullAnalytics`/`NoAds`/`UnavailablePurchase`. Habilitar Release requiere decisión humana sobre titular/bundle ID y un mecanismo externo de signing que no exponga keystore, passwords ni rutas personales. AAB, Play App Signing, upload key y publicación siguen `NOT RUN`; se abordan en F12/F46–F52.

## Controles exigidos al habilitar AAB

- AAB Release IL2CPP/ARM64, target/compile API vigente y bundle identity aprobada;
- secretos inyectados y enmascarados fuera del repositorio, sin eco en logs;
- merged manifest comparado: bloquear cámara, micrófono, ubicación, contactos, `AD_ID` y cualquier permiso no aprobado;
- `bundletool`, ELF y page alignment 16 KB sobre el artefacto Release;
- hash/tamaño/versionado, SBOM/licencias y prueba en dispositivo representativo;
- ninguna operación automática de upload/publicación dentro del comando de build.

## Evidencia histórica

La evidencia F03/F04 (APK de aproximadamente 57 MB, API 36, ARM64, IL2CPP, zipalign/ELF/emulador 16 KB) se conserva en [`ANDROID_RELEASE.md`](ANDROID_RELEASE.md). Es histórica: no sustituye el build actual ni demuestra AAB Release.
