# 16 — Dirección de audio

Dirección de producto y contrato del framework implementado en Prompt 12; masters, ledger y QA técnico viven en [`AUDIO_REQUIREMENTS.md`](AUDIO_REQUIREMENTS.md). No existe audio final ni voz humana.

## Norte sonoro

Audio cálido, curioso y claro que orienta sin sobreestimular. La Selva tiene profundidad ambiental, pero la voz y señales de interacción permanecen inteligibles en altavoz móvil.

## Capas

- **Ambiente:** identidad de zona y pistas espaciales, sin afirmar especies no aprobadas.
- **Música:** loops tranquilos, variación al descubrir y espacio para voz.
- **SFX:** navegación, interacción, fotografía, asociación, álbum y mejora de campamento.
- **Voz:** objetivos breves, nombres/hechos aprobados, pistas graduadas y descansos.

## Tono de voz

Invita y observa; no evalúa. Frases cortas, vocabulario concreto, ritmo suficiente para prelectores y botón de repetición. No usar voces caricaturescas de culturas ni imitar animales de forma que contradiga contenido factual.

## Necesidades humanas del Vertical Slice

- Voz temporal para ambos modos de guía y tres niveles de pista.
- Ambiente provisional de claro/campamento con licencia conocida.
- SFX de toque válido/inválido, fotografía, descubrimiento, actividad, recompensa y mejora.
- Una transición musical breve y control de música/voz/SFX.

## Accesibilidad y descansos

- Ninguna instrucción existe solo en audio; subtítulo, icono o demostración equivalente.
- Repetición ilimitada sin costo y pistas sonoras acompañadas visualmente.
- Rango dinámico prudente, sin sobresaltos; opción de silenciar capas.
- Al sugerir descanso, música y narración bajan de intensidad sin urgencia.

## Aprobación

Actor/compositor/licencias, pronunciación de nombres y claims, mezcla y pruebas de inteligibilidad requieren responsables humanos. Audio factual no entra a Release sin ID Approved en [`CONTENT_SOURCES.md`](CONTENT_SOURCES.md).

## Localización de voz

La tabla asset `Voice` contiene cinco slots conceptuales; tres corresponden a cues de voz placeholder de Prompt 12. Voz y subtítulo comparten concepto, pero el runtime selecciona clips ES/EN desde `AudioCueDefinition` y resuelve el subtítulo por `LocalizedKey`; ningún nombre de archivo entra en Domain o Save. Futuros takes requieren locale, cue, subtítulo aprobado, pronunciación, actor/licencia y fallback según [`17_LOCALIZATION.md`](17_LOCALIZATION.md) y [`AUDIO_REQUIREMENTS.md`](AUDIO_REQUIREMENTS.md).

## Framework implementado

`IAudioService` expresa intención semántica; `UnityAudioService` posee sources, cooldown, cola, ducking y lifecycle; Content posee `AudioCueDefinition`/catálogo/mixer; Bootstrap es el único composition root; Presentation solicita play/replay y presenta subtítulos. Domain no referencia audio Unity ni archivos.

Mixer `PE_Main`: Master→Music/Ambience/Effects/Voice. La baseline usa siete cues y diez WAV internos `PH_`, locales y Addressables-ready. Un cue/clip ausente devuelve `Missing`, registra un código técnico sin datos infantiles y no bloquea gameplay. `scripts/validate-audio` es el gate estructural; los placeholders permanecen `ReleaseBlocked`.
