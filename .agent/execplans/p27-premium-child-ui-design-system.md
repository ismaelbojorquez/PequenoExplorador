# ExecPlan — design system infantil premium y migración de UI crítica

- Fase/Gate: Prompt 27 / Gate B permanece FAIL
- Estado: Completed
- Creado/actualizado: 2026-08-17 05:28 CST
- Owner: Lead Game UI Engineer / Child UX / Accessibility

## Propósito y alcance

Consolidar la UI baseline en un sistema visual uGUI+TMP local, coherente, táctil y accesible. Incluye tokens, componentes, estados, motion cancelable/reduce-motion, galería y migración de Boot/loading, Camp, fotografía/discovery card, álbum, learning, misión y personalización. No cambia reglas, contenido factual, economía, navegación ni arte ilustrado final.

## Contexto y orientación

HEAD inicial `6247400276ca430a7c880acd2e0a4097b3e3dfbc`, `main`, árbol limpio. `scripts/validate` previo PASS con 155 EditMode, 26 PlayMode, Addressables y APK Development. La UI vive en múltiples Canvas dentro de `Assets/_Game/Bootstrap/Bootstrap.unity`; cada setup histórico crea colores/botones/texto propios. `com.unity.ugui@2.0.0` ya contiene `Unity.TextMeshPro`; no se instala dependencia. Safe area usa un único `SafeAreaFitter` por Canvas y referencia 1920×1080.

## Progreso

- [x] 2026-08-17 05:28 CST — preflight, documentación, pantallas, setups, paquetes, escenas y baseline contrastados.
- [x] 2026-08-17 — capturados 40 PNG baseline (`before`) para diez superficies × cuatro ratios.
- [x] 2026-08-17 — implementados tokens/componentes/TMP/galería, iconos geométricos y migración visual de ocho roots.
- [x] 2026-08-17 — revisión visual, documentación, validación integral y APK Development completados; commit pendiente como último paso Git.

## Hallazgos

- Ocho setups críticos duplican `CreateText`, `CreateButton`, colores, tamaños y estados; no existe fuente visual ejecutable.
- Las vistas críticas usan `UnityEngine.UI.Text`; TMP está disponible en la dependencia ya fijada, pero no hay font asset/configuración del proyecto.
- La dirección canónica ya define campamento/cuaderno de campo, formas redondeadas, feedback suave y poco texto. Esta fase la consolida; no crea una identidad paralela.
- No existe Android físico conectado; ergonomía, densidad y contraste en panel real permanecerán `NOT RUN`.
- Unity no conservó IDs estables al declarar varios MonoBehaviours en un archivo; se separó una clase por archivo y el setup elimina solo duplicados/scripts faltantes del DesignSystem.
- Cambiar ahora los tipos serializados `Text→TMP_Text` rompería wiring/localización de muchas vistas. Se adoptó TMP para componentes nuevos y un bridge temático medible para la migración segura posterior.

## Decisiones

- 2026-08-17 — crear un décimo asmdef `PequenoExplorador.DesignSystem` sin referencia Domain; Presentation lo consume y mantiene reglas fuera del módulo visual.
- 2026-08-17 — dirección “Kit de expedición”: bandejas redondeadas, badges de esmalte, papel claro y acentos selva/mango/cielo, sin copiar IP, sin gradientes ni sobreestimulación.
- 2026-08-17 — versionar TMP Essential Resources oficiales de `com.unity.ugui 2.0.0` y usar `LiberationSans SDF` provisional con su OFL; no descargar fuentes ni añadir paquete.
- 2026-08-17 — targets recomendados 72, mínimo validado 64; color siempre acompañado de icono/texto/forma.

## Plan de implementación

1. Añadir captura PlayMode reproducible y registrar baseline por superficie/ratio en `artifacts/ui-review/before/`.
2. Crear tokens, componentes, iconografía vectorial, motion y estados en `Assets/_Game/DesignSystem/`; authoring/validator/galería en Editor/Content.
3. Aplicar componentes comunes a las vistas críticas; TMP es canónico para nuevos componentes y el bridge preserva referencias legacy hasta una migración serializada separada.
4. Añadir tests de tokens, contraste, targets, safe area, estados, locale/pseudo, escala grande, reduce motion y screenshots `after`.
5. Ejecutar detector, revisión visual en dos rondas máximo, suite completa, APK y documentación.

## Comandos y validación

- `scripts/validate` — baseline previo PASS: 155 EditMode, 26 PlayMode, Addressables y APK Development.
- `scripts/setup-design-system` — PASS, idempotente; ocho roots + galería.
- `scripts/validate-design-system` — PASS; tokens/TMP/AA/targets/Canvas.
- `scripts/capture-ui-review before|after` — PASS; 40 PNG por fase, revisión representativa 4:3/16:9/20:9/16:10.
- `scripts/validate` — PASS final: repository checks, compile, Addressables, EditMode `158/158`, PlayMode `26/26` y APK Development.

## Recovery y seguridad

Todos los assets nuevos son locales, propios o generados desde recursos Unity existentes y se marcan `PH_` cuando no son finales. Las capturas/builds viven en `artifacts/` ignorado. No se instalan paquetes, no se toca gameplay/save, no se publica ni se hace push. Si la migración rompe una referencia, se reejecuta el setup idempotente y se contrasta el diff; no se limpian cambios con comandos destructivos.

## Resultados y retrospectiva

El sistema centraliza ocho roots sin tocar reglas ni Save. `PequenoExplorador.DesignSystem` es el décimo asmdef y solo depende de TMP; Presentation no gana referencia a Infrastructure. La galería, iconos geométricos, tokens, estados y motion son locales. La primera validación encontró targets de 48–62 tras normalizar Canvas; el componente stretch ahora calcula únicamente el delta necesario y el validador de Input volvió a PASS.

Evidencia final: Addressables `4.0.1`, 47 locations/1,289,237 bytes; EditMode `158/158`; PlayMode `26/26`; APK `67,424,654` bytes, SHA-256 `e771d32998e31630937c6df6a56e1487e480819f7a6b9d72de24b523b359a090`, `69.515 s`, API 26/36, IL2CPP/ARM64. Capturas: diez superficies × cuatro ratios en `artifacts/ui-review/before|after`; la revisión final corrigió solapamientos de actividad 4:3 y truncado de personalización 16:10 antes del `PASS`. Android físico, arte/fuente final, escala 1.25 humana y playtest siguen `NOT RUN`. Gate B permanece FAIL y Prompt 28 es la siguiente fase.
