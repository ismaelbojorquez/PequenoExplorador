# 09 — Sistema de misiones

Las misiones dan intención sin convertir el juego en lista de tareas. Cantidad canónica: [`MVP_SCOPE.md`](MVP_SCOPE.md).

## Contrato Mission

Cada misión data-driven define:

- ID, título audiovisual y motivo dentro de la aventura;
- uno a cuatro objetivos secuenciales o paralelos claramente visibles;
- contratos World/Discovery/Activity requeridos y fallback;
- variantes de presentación para ambos modos de guía;
- condición de cierre y retorno seguro;
- Reward determinista y Camp Upgrade relacionado, si aplica;
- estado persistente (`Disponible`, `En curso`, `Completada`);
- hechos usados y estado de aprobación.

Prompt 14 reserva únicamente `IMissionDefinition`/`MissionId`, `IRewardDefinition`/`RewardId` y los value IDs relacionados. Misiones, objetivos, rewards y save no se implementan hasta existir sus casos de uso; el catálogo base evita APIs ficticias extensas.

## Familias de misión

- **Conocer:** detectar, fotografiar y revisar un descubrimiento.
- **Observar:** encontrar una pista o relación del entorno.
- **Ayudar al campamento:** completar una actividad para una mejora visible.
- **Recorrido:** conectar varios descubrimientos ya aprobados sin añadir examen final.

El MVP puede combinar familias, pero cada misión conserva una intención principal y duración objetivo de tres a ocho minutos.

## Vertical Slice

`VS-M01 — Conoce al tucán`:

1. aceptar objetivo en el campamento;
2. seguir una pista hasta el claro;
3. detectar y fotografiar el discovery candidato;
4. completar `Reconoce al tucán`;
5. volver al campamento;
6. recibir materiales deterministas y construir `Mesa de observación`;
7. guardar misión, discovery, álbum, saldo y mejora.

## Reglas

- No misiones diarias/semanales, timers reales, caducidad o “vuelve mañana”.
- No cadenas obligatorias que bloqueen salida o descanso.
- No perder progreso por abandonar una misión; reiniciar ofrece confirmación adulta solo si borra estado.
- Una misión incompleta comunica próximo paso sin culpa.
- La recompensa reconoce cierre, no velocidad, exactitud inicial ni ausencia de pistas.

## Aceptación

- El niño puede describir qué intenta hacer mediante imagen/voz.
- Dependencias inexistentes tienen fallback y no bloquean el save.
- La misión se completa en ambos modos de guía con igual resultado.
- Todo hecho está Approved; si no, la misión permanece `Blocked for Release`.
