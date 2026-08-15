# Requisitos de audio

Contrato inicial para voz, música y SFX del MVP Selva. No contiene audio ni autoriza proveedores.

## Experiencia

- Instrucciones comprensibles para prelectores, con frases breves, tono cálido y repetición bajo demanda; el progreso nunca depende solo del audio.
- Feedback positivo y no punitivo. Evitar sobresaltos, sonidos de error agresivos, frecuencias molestas y presión sonora excesiva.
- Música de Selva calmada y loopeable, sin caricaturizar culturas. SFX distinguen selección, interacción, acierto y reintento sin saturación.
- Controles locales separados para música, voz y efectos, con valores seguros por defecto; responder a mute, interrupciones y audio focus del sistema.

## Masters y metadatos

- Entregar masters WAV PCM sin pérdida, preferentemente 48 kHz/24-bit; compresión runtime se decide con pruebas auditivas y de memoria en F15.
- Cada archivo registra ID, idioma/locale, texto exacto, actor/compositor, dirección, licencia/release, fecha, edición, loop points y propietario de contenido.
- Voz localizada no se obtiene por traducción automática no revisada. Pronunciación de fauna/flora y español regional requiere aprobación de contenido.
- Conservar masters grandes con Git LFS; no versionar sesiones DAW, caches o stems innecesarios sin acuerdo.

## Implementación futura

- F09 creará puertos y mixer; F15 establecerá import presets, formatos/load type y budgets medidos.
- Addressables local-first: voz, música y SFX del MVP deben funcionar sin red desde la primera ejecución.
- Evitar streaming de red, micrófono, reconocimiento de voz y SDKs externos en el MVP.
- Variaciones aleatorias deben ser deterministas en tests cuando afecten gameplay y no elevar el volumen acumulado.

## Criterios de aceptación

- Derechos de voz/música/SFX documentados para territorios y plataformas aprobados.
- Sin clipping, clicks, DC offset perceptible ni loops defectuosos; inteligibilidad probada en altavoz de teléfono y audífonos.
- Mezcla consistente entre actividades, accesible con audio desactivado y dentro de budgets de memoria/tamaño/carga.
- QA con interrupciones, cambio de salida, background/foreground y sesiones sostenidas antes del Gate D.
