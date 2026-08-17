# Privacidad de ingeniería

Baseline técnica del MVP offline infantil; no es asesoría legal ni sustituye política de privacidad/revisión por jurisdicción. Las fuentes normativas y fechas viven en [`POLICY_SOURCE_REGISTER.md`](POLICY_SOURCE_REGISTER.md).

## Datos y superficies actuales

| Superficie | Datos locales | Red/permiso | Control |
|---|---|---|---|
| Save v11 | progreso técnico, preferencias ES/EN/audio/guía, discovery, grants, foto, economía/misiones, agregados learning, mejoras Camp y ownership/equipped cosmético | Sin red/permiso | Sin nombre, edad, fecha de nacimiento, género, cuenta, device/advertising ID, respuestas/taps ni raw event log. |
| Foto del explorador | PNG `384×216` del mundo Unity + manifest técnico | Sin CAMERA, galería, compartir o media scanner | Store privado/acotado; save solo referencia relativa. No captura niño/entorno real. |
| Audio/Input | preferencias y eventos efímeros de sesión | Sin micrófono/ubicación/contactos | No persiste taps ni voz; haptics no-op/off. |
| Servicios comerciales | Null/Mock/Unavailable locales | Sin SDK/backend | Release fail-closed; ads iniciales no habilitados. |

Los logs usan códigos/contadores, no contenido de progreso, paths personales ni pixels. Development puede resetear save/photo store y simular un fallo; esas herramientas no entran en Release.

## Control Prompt 19

`PhotographyValidationService` bloquea `ScreenCapture`, `android.permission.CAMERA`, target/bounds incompletos y contratos de captura fuera del budget. `CapturePhotoUseCase` confirma progreso aunque falle storage; `LocalPhotoStore` solo acepta IDs tipados, nombres deterministas y límites explícitos. Borrar el store de fotos no borra discovery: el futuro álbum debe degradar a imagen canónica.

Antes de Release: inspeccionar merged manifest/AAB, tráfico en dispositivo, sandbox/backup del OS, retención/reset adulto, Data safety/App Privacy y legislación de países objetivo. Cualquier cámara física, compartir, cloud, analytics, ads o IAP exige ADR, minimización, parental controls, revisión legal/privacidad y nueva evidencia; no se hereda autorización de este baseline.
