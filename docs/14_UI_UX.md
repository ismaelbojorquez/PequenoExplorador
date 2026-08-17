# 14 — UI/UX infantil y adulta

Fuente de verdad de interacción y accesibilidad de producto. Los flujos específicos remiten al [`01_GDD.md`](01_GDD.md) y al [`PLAYTEST_PLAN.md`](PLAYTEST_PLAN.md).

## Principios

- Landscape, objetivos táctiles amplios y una acción principal por pantalla.
- Audio, icono y movimiento apoyan toda instrucción esencial; texto agrega detalle.
- Feedback inmediato, no punitivo y sin color como único significado.
- Navegación reversible; salida y pausa siempre accesibles desde estados seguros.
- Sin contadores ansiógenos, botones falsos, confirmaciones confusas, anuncios o ofertas infantiles.

## Modos de asistencia

En primera entrada se muestran dos tarjetas equivalentes, sin lenguaje de habilidad:

- **Más guía:** más demostración, resaltados tempranos, voz repetible y menos opciones cuando hay atasco.
- **Guía estándar:** menos señalización inicial y pistas bajo demanda.

No se pregunta edad/fecha de nacimiento, no se recomienda modo según comportamiento y el cambio es libre. La elección se guarda localmente como preferencia, no como perfil.

## Navegación candidata

- Tap-to-move con indicador de destino, camino válido y cancelar mediante nuevo toque.
- Interacción contextual con toque: foco visible, auto-acercamiento si hace falta, acción iconográfica grande y cancelación siempre disponible; mantener pulsado no es requisito principal.
- Viewfinder de fotografía con zona de éxito generosa, guía `acércate/centra/listo`, shutter grande y salida reversible.
- Prompt 16 implementa el candidato como `PH_` medible; P-006 aún exige compararlo con control directo simplificado antes de implementación definitiva.

La foundation ejecutable está en [`INPUT_ACCESSIBILITY.md`](INPUT_ACCESSIBILITY.md). `Explorer` mueve un avatar `PH_` solo sobre suelo válido, con marker verde/ámbar y cámara automática; `Photography` sigue siendo el único contexto de producto que acepta pinch. Abrir UI/fotografía, pausar o perder focus detiene movimiento. Android Back abre pausa reversible y checkpoint, no salida destructiva.

El prompt contextual vive en un Canvas dedicado con safe area y targets mínimos `64×64`. Solo existe un foco. En overlaps gobiernan prioridad, distancia e ID estable; nunca “animal primero” por hardcode. `Unavailable` muestra “Todavía no podemos…” y cue suave, no términos técnicos, cruz roja ni castigo. Detalle: [`INTERACTION_SYSTEM.md`](INTERACTION_SYSTEM.md).

Prompt 19 añade un Canvas `PH_UI_PHOTOGRAPHY` sobre safe area: retícula de `620×390`, shutter `170×170`, salida `180×110`, guía ES/EN y tarjeta de resultado. Color acompaña al icono/copy y no es la única señal. Una toma inválida mantiene cámara y ofrece pista; storage fallido conserva discovery y muestra fallback no alarmante. `reduce motion` desactiva flash; pinch sigue reservado, pero zoom visual final no se implementa aún. Contrato: [`PHOTOGRAPHY_SYSTEM.md`](PHOTOGRAPHY_SYSTEM.md).

## Jerarquía de pantallas

1. Inicio/continuar y acceso adulto discreto.
2. Campamento: mundo, misión actual, álbum y personalización.
3. Selva: navegación, pista, interacción y pausa.
4. Actividad: objetivo, área manipulable, repetir audio, pista y salida.
5. Álbum: categorías, ficha y escucha.
6. Área adulta: progreso descriptivo, asistencia, audio, descansos, privacidad y controles futuros de monetización.

## Área adulta

- Acceso tras parental gate apropiado por plataforma; su diseño final requiere revisión de políticas.
- Resume misiones/discoveries y conceptos explorados, sin notas, ranking, inferencias de capacidad o tiempo usado como presión.
- Permite cambiar guía, volumen, reducción de movimiento, captura asistida, descansos y reset con confirmación.
- La “monetización preparada” es solo un espacio/contrato deshabilitado detrás del gate: sin precio, SKU, compra, ads o SDK en MVP.

## Accesibilidad mínima

- Contraste y tamaño validados, safe areas, escalado de texto y foco visible.
- Cada Canvas actual tiene un solo root de safe area; los targets actuales miden al menos `64×64` píxeles de referencia. Los presets 4:3, 16:9, 20:9 y tablet 16:10 no sustituyen hardware.
- Subtítulos/copy para voz; audio repetible; pistas visuales para sonidos.
- Prompt 12 activa subtítulos por defecto, replay gratuito y volúmenes separados; cambiar idioma actualiza texto/cue sin pedir edad ni inferir perfil.
- Reducir movimiento y flashes; cámara estable. El prototipo ofrece snap sin bob y el modo cámara omite flash cuando se activa la preferencia; el control adulto final aún no está persistido/cableado.
- No exigir velocidad, precisión fina, lectura, audio, percepción de color o multitouch.
- Tiempo suficiente y descansos neutrales; reentrada conserva contexto.

## Copy y tono

Verbos de invitación: “mira”, “escucha”, “prueba”, “¿qué notas?”. Evitar “fácil”, “mal”, “fallaste”, “última oportunidad”, “perfecto” como juicio, o cualquier referencia a edad/nivel.

Todo copy runtime usa las keys estables de [`17_LOCALIZATION.md`](17_LOCALIZATION.md); español es predeterminado e inglés no puede quedar vacío. Una key ausente nunca expone IDs técnicos en Release. Pseudo-localización prueba expansión y caracteres en Development. El selector ES/EN actual es diagnóstico; el selector de producto futuro pertenece al área adulta y no pide edad, idioma inferido ni datos personales.

## Aceptación

- Los tres recorridos de papel de [`PLAYTEST_PLAN.md`](PLAYTEST_PLAN.md) completan su intención sin contradicción.
- Toda pantalla tiene acción principal, salida segura, estado sin audio y fallback sin lectura.
- Ambos modos ofrecen mismo contenido/progreso.
- Área adulta no expone compras ni links al niño.
