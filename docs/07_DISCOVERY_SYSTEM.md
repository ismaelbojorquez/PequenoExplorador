# 07 — Sistema de descubrimiento

Discovery es la unión entre exploración, fotografía ficticia, álbum y aprendizaje. Alcance cuantitativo: [`MVP_SCOPE.md`](MVP_SCOPE.md).

## Contrato Discovery

Cada descubrimiento data-driven necesita:

- ID estable, categoría (`animal`, `planta`, `insecto`, `objeto especial`) y nombre aprobado;
- referencia a su entrada factual y estado de aprobación;
- condiciones de aparición/observación y zona propietaria;
- encuadre accesible y criterios de fotografía indulgentes;
- ficha de álbum con imagen, audio, texto opcional y uno o dos hechos aprobados;
- actividad relacionada opcional/obligatoria según misión;
- recompensa determinista y estado persistente;
- fallback si el contenido factual o visual no está aprobado.

El contrato técnico base está en [`CONTENT_MODEL.md`](CONTENT_MODEL.md): `DiscoveryDefinitionAsset` se mapea a `DiscoveryDefinition` readonly y se resuelve mediante `DiscoveryId` en O(1). El único contenido adoptado es `discovery.jungle.keel-billed-toucan`, `Approved`, con siete facts y seis fuentes; todavía no implementa detección, fotografía ni álbum.

## Progresión persistente implementada

`DiscoverUseCase` recibe `DiscoveryId` + `DiscoveryGrantId`, resuelve aliases y devuelve uno de estos outcomes explícitos:

| Outcome | Mutación | Recompensa única futura |
|---|---|---|
| `First` | Crea record con `count=1`, día local agregado `yyyy-MM-dd` y conserva grant. | Sí, exactamente una vez. |
| `Repeated` | Incrementa count y conserva un grant nuevo. | No. |
| `AlreadyProcessed` | Ninguna; devuelve el count vigente. | No. |
| `MissingContent` | Ninguna; un ID retirado permanece en save sin entrar en denominadores. | No. |
| `UnapprovedContent` | Ninguna en perfiles que no admiten Draft. | No. |
| `SaveReadOnly` | Ninguna ante schema futuro protegido. | No. |

La clave `grant.*` pertenece al origen semántico (interacción ahora, captura después). Economy, álbum y UI consumirán `DiscoverResult`; Discovery no conoce estrellas, vistas ni archivos de audio. `DiscoveryProgressQueries` calcula descubierto/total por world/category exclusivamente desde definitions `Approved` del catálogo. No hay números de contenido hardcodeados ni porcentaje guardado.

El fixture animal enlaza `interaction.jungle.keel-billed-toucan → discovery.jungle.keel-billed-toucan` mediante authoring. `DiscoveryInteractionAction` es el adapter de Application; planta/objeto siguen neutrales. El ID retirado se resuelve por alias y la migración save v4→v5 converge records/grants al ID vigente.

## Estados

`No visto → Detectado → Fotografiado → Explorado`

- **Detectado:** el niño provocó o percibió una pista.
- **Fotografiado:** el viewfinder del juego reconoció encuadre suficiente.
- **Explorado:** completó la actividad relacionada o revisó la ficha, según contrato.
- Los estados no bajan ni expiran; pedir pistas no reduce recompensa.

Estos estados ricos siguen siendo contrato futuro. El schema v5 solo distingue existencia, count, primer día local agregado y grants procesados; no inventa `Detectado/Fotografiado/Explorado` antes de implementar sus reglas.

## Fotografía dentro del juego

- No usa cámara real, galería, ubicación ni permisos del dispositivo.
- El objetivo ocupa una zona amplia del encuadre; no se califica composición.
- `Más guía` muestra marco, dirección y autoenfoque más evidentes.
- `Guía estándar` conserva confirmación y pista bajo demanda.
- Si hay dificultad motriz, una opción permite captura asistida al mantener el objetivo visible brevemente.

## Álbum

- Organiza por categorías sin porcentajes que generen presión.
- Fichas bloqueadas no muestran siluetas alarmantes ni fechas límite.
- Una ficha aprobada permite escuchar nombre, ver imagen y revisar hechos; texto nunca es el único canal.
- El área adulta puede ver conceptos explorados y contenido encontrado, no notas o inferencias sobre capacidad.

## Vertical Slice

Discovery adoptado: **`Ramphastos sulfuratus`**, expediente [`VS-D-A01`](VS_D_A01_TOUCAN_FACTUAL_DOSSIER.md), nombre `Tucán pico canoa` / `Keel-billed Toucan`. H-007/H-008/H-009 sustentan los assets runtime `Approved`. Conservación permanece excluida y el nombre científico es detalle opcional. No hay todavía foto, álbum ni recompensa.

## Aceptación del sistema

- Fotografía se comprende sin leer y no requiere precisión frustrante.
- Estado del álbum coincide tras cerrar/reabrir la sesión de papel o futura implementación.
- Ningún hecho llega a estado Release sin `Approved` en [`CONTENT_SOURCES.md`](CONTENT_SOURCES.md).
- Descubrimiento y recompensa siguen funcionando offline y sin monetización.
- Repetir una misma grant key no incrementa count ni vuelve a habilitar la recompensa única.
- Save v3 migra a v4; v4 migra a v5, reemplaza/mezcla el ID placeholder y normaliza grants exactos sin inventar fecha ni duplicar progreso.
