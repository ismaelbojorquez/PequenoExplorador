# Requisitos y registro de audio

Contrato técnico y ledger de producción para el MVP Selva. Prompt 12 incluye únicamente tonos placeholder generados por tooling del proyecto: no son voz, música ni ambiente final, no provienen de terceros y permanecen bloqueados para Release.

## Experiencia y controles

- Instrucciones breves, cálidas, repetibles y siempre equivalentes en subtítulo/visual; el progreso nunca depende solo del audio.
- Feedback no punitivo, sin sobresaltos ni presión. Los placeholders usan ganancia por cue `0.20–0.30` y no sustituyen mezcla humana.
- Volúmenes normalizados independientes `Master`, `Music`, `Ambience`, `Effects` y `Voice`; defaults `0.85/0.65/0.65/0.75/0.85`.
- Una voz prioritaria aplica ducking `0.35×` a música/ambiente. La cola admite cuatro pendientes, ordena por prioridad/FIFO y evita solapamiento de instrucciones.
- Subtítulos están activos por defecto y replay no consume moneda, energía ni progreso.

## Buses runtime

| Bus | Owner/source | Concurrencia | Lifecycle |
|---|---|---:|---|
| Master | `PE_Main.mixer` | Suma final | Preferencia global; no tiene clip. |
| Music | source exclusivo | 1 loop | Camp; pause/focus y shutdown seguros. |
| Ambience | source exclusivo | 1 loop | Camp; ducking junto con Music. |
| Effects | pool fijo | 4 | Reemplazo por prioridad; cooldown por cue. |
| Voice | source + queue | 1 + 4 | Nombres/instrucción/narración; subtítulo y replay. |

Siete `AudioSource` viven exclusivamente en el root Bootstrap: 1 Music + 1 Ambience + 4 Effects + 1 Voice. No existe `Resources.Load`, streaming de red, micrófono, reconocimiento de voz ni middleware.

Definitions de contenido referencian `AudioCueId`, no clips, paths o GUID. El validator de catálogo comprueba que cada cue resuelva en `AudioCueCatalog`; reemplazar un audio conserva su cue ID salvo migración explícita.

## Ledger baseline de cues y assets

Todos los WAV son PCM mono, 48 kHz/16-bit, `DecompressOnLoad`, generados por `AudioFoundationSetup` y etiquetados Addressables `audio-local` + `audio-placeholder` dentro de `SharedLocal`.

| Cue ID | Categoría/bus | Duración | Idioma | Emoción/intención | Archivos/estado |
|---|---|---:|---|---|---|
| `audio.music.camp` | Music/Music | 2.00 s loop | neutro | calma/curiosidad | `PH_Music_Camp_es.wav`; `PH_MUSIC_CAMP`; `ReleaseBlocked`. |
| `audio.ambience.camp` | Ambience/Ambience | 2.00 s loop | neutro | fondo suave | `PH_Ambience_Camp_es.wav`; `PH_AMBIENCE_CAMP`; `ReleaseBlocked`. |
| `audio.feedback.confirm` | Feedback/Effects | 0.18 s | neutro | confirmación suave | `PH_Feedback_Confirm_es.wav`; `PH_FEEDBACK_CONFIRM`; `ReleaseBlocked`. |
| `audio.feedback.retry` | Feedback/Effects | 0.18 s | neutro | invitación a reintentar | `PH_Feedback_Retry_es.wav`; `PH_FEEDBACK_RETRY`; `ReleaseBlocked`. |
| `audio.voice.instruction.explore` | VoiceInstruction/Voice | 0.55 s | ES/EN | guía cálida | `PH_Voice_Instruction_Explore_{es,en}.wav`; `PH_VOICE_INSTRUCTION_EXPLORE`; `ReleaseBlocked`. |
| `audio.voice.name.jungle` | VoiceName/Voice | 0.38 s | ES/EN | nombre claro | `PH_Voice_Name_Jungle_{es,en}.wav`; `PH_VOICE_NAME_JUNGLE`; `ReleaseBlocked`. |
| `audio.voice.narration.welcome` | Narration/Voice | 0.60 s | ES/EN | bienvenida tranquila | `PH_Voice_Narration_Welcome_{es,en}.wav`; `PH_VOICE_NARRATION_WELCOME`; `ReleaseBlocked`. |

Los tonos de voz no pretenden sonar humanos y nunca muestran al niño un mensaje técnico. Los keys de subtítulo son `content.audio.instruction.explore`, `content.audio.name.jungle` y `content.audio.narration.welcome`; los slots conceptuales `Voice` usan los IDs del cue.

Prompt 19 reutiliza únicamente `audio.feedback.confirm` para toma aceptada y `audio.feedback.retry` para pista/resultado no listo. No inventa vocalización, shutter real ni narración de especie. Ambos son tonos `PH_` suaves y ReleaseBlocked; la experiencia conserva retícula/copy aun sin audio. Un cue final de shutter/guía deberá registrar ID, derechos, idioma/emoción/duración y prueba auditiva, sin copiar grabaciones científicas referenciadas.

## Entrega final humana

Cada reemplazo debe registrar ID, locale, texto exacto aprobado, actor/compositor, emoción, pronunciación, duración, formato/master, licencia/release, fecha, edición, loop points y owner. Masters finales: WAV PCM sin pérdida, preferentemente 48 kHz/24-bit; import/load/compression se decide con medición móvil en F15. Voz ES/EN no se traduce automáticamente sin revisión lingüística y factual.

Release queda bloqueado mientras cualquier cue sea `IsPlaceholder=true`, su ID no empiece `PH_`, falte licencia/revisión o el asset tenga `ReleaseBlocked`. `scripts/validate-audio` valida estructura, buses, IDs, clips, addresses, mono/48 kHz, clipping y metadata; registra `PE_AUDIO_RELEASE_PENDING` sin convertirlo en PASS comercial.

## Revisión de vocalización VS-D-A01

Cornell eBird/Merlin describe la voz de `Ramphastos sulfuratus` como un croar lejano y repetitivo; el [expediente Sourced](VS_D_A01_TOUCAN_FACTUAL_DOSSIER.md) conserva el claim y su límite. La descripción no concede licencia sobre las grabaciones de Macaulay/eBird. Audio debe obtener o producir un asset con derechos, verificar especie/localidad/calidad, documentar recordista/licencia y someter la analogía, cue, subtítulo y pronunciación regional a especialista factual y Localization.

Ismael Bojórquez declaró `Audio: APPROVED`, `Rights: APPROVED` y referencia `Propia` el 2026-08-16. No se proporcionó todavía un cue/clip no-`PH_`, source master, recordista/actor, pronunciación o ledger técnico; la declaración no convierte los tonos actuales en vocalización del tucán ni permite reutilizar grabaciones de las fuentes.

Cambiar especie, vocalización o nombre regional devuelve a `Reviewed` la grabación, voice name, subtítulo, replay y cualquier pista sonora. Cambiar mezcla/formato sin alterar contenido requiere QA técnico, pero no reabre facts salvo que modifique inteligibilidad o significado.

## Aceptación pendiente

- Sin clipping/clicks/DC offset perceptible ni loops defectuosos en dispositivo físico.
- Inteligibilidad ES/EN en altavoz de teléfono y audífonos, incluyendo interrupciones y background/foreground.
- Derechos territoriales/plataforma y pronunciación aprobados; hechos siguen [`CONTENT_SOURCES.md`](CONTENT_SOURCES.md).
- Budgets de memoria/tamaño y mezcla final medidos antes de Gate D.
