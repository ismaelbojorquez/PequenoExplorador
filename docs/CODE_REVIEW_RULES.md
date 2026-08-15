# Reglas de code review

Aplican a código, configuración, contenido y documentación. Review verifica evidencia y riesgos; no sustituye pruebas ni aprobación humana.

## Bloqueantes generales

- Alcance no corresponde a `STATUS`, roadmap, MVP/Vertical Slice o ExecPlan activo.
- Dirección de dependencias/asmdefs rota, regla compleja en MonoBehaviour o estado de sesión en ScriptableObject.
- Cambio ajeno sobrescrito, secreto/ruta personal, dependencia no aprobada o placeholder liberable.
- Test/build requerido `NOT RUN` sin aceptar explícitamente el riesgo, o afirmación de PASS sin comando/artefacto.
- Documentación, decisión, riesgo, status o recovery quedan falsos después del cambio.

## Checklist de arquitectura y calidad

- [ ] Domain permanece C# puro y la composición ocurre en un root explícito.
- [ ] Serialización/save usan DTO/versionado/migración y fallo recuperable.
- [ ] Lifecycle, suscripciones, async y cancelación están emparejados.
- [ ] Eventos son inmutables, acotados y no forman un bus global oculto.
- [ ] Tests cubren comportamiento/errores/regresión; logs no contienen datos sensibles.
- [ ] Performance se justifica con medición cuando el cambio toca frame, memoria, carga, batería o tamaño.

## Checklist infantil, privacidad y tiendas

- [ ] No se solicita/infiere edad exacta, cumpleaños, identidad, ubicación, cámara, micrófono o contacto.
- [ ] Inventario de datos sigue siendo cero/mínimo o documenta cada nuevo dato, propósito, retención, acceso y borrado.
- [ ] Permisos Android/iOS y tráfico de red se compararon antes/después; cualquier alta tiene decisión y fallback.
- [ ] SDK y transitivos tienen fuente, licencia, pin, mantenimiento, datos/permisos, 16 KB, soporte móvil y revisión Families/Kids.
- [ ] Ads, IAP, links y compras siguen ausentes; cualquier propuesta tiene ADR humana, parental UX y revalidación de políticas.
- [ ] No dark patterns, rachas, energía, FOMO, azar pagado, castigo, temporizador coercitivo ni presión para compartir/comprar.
- [ ] Ambos modos de guía conservan contenido/progreso/recompensa; feedback y descansos siguen no punitivos.
- [ ] Claims, arte y audio factual tienen Content ID aprobado o quedan `Blocked for Release`.

## Checklist save y resiliencia

- [ ] Save no contiene PII; esquema y migración son explícitos.
- [ ] Escritura atómica, corrupción, datos ausentes, downgrade no soportado y almacenamiento insuficiente tienen resultado definido.
- [ ] Reset/borrado sensible requiere área adulta/confirmación y no borra silenciosamente.
- [ ] Background/foreground/cierre conservan último hito confirmado sin duplicar recompensa.

## Checklist performance y release

- [ ] No allocations/búsquedas/logs repetidos en hot paths sin medición.
- [ ] Addressables y contenido funcionan local-first; no apareció catálogo remoto.
- [ ] Librerías nativas del artefacto objetivo tienen evidencia 16 KB cuando aplique.
- [ ] Build, tamaño, warnings, tests en dispositivo y store checks se reportan por separado; lo no ejecutado es `NOT RUN`.
- [ ] Placeholders, debug UI, flags temporales y datos de prueba están ausentes o bloquean Release.

## Resultado de review

Cada hallazgo cita archivo/línea, riesgo, evidencia y cambio esperado. Clasificar como `BLOCKER`, `MAJOR`, `MINOR` o `QUESTION`; una duda de política/decisión humana no se autocierra como cumplimiento.
