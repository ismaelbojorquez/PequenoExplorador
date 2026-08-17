# Economía y recompensas — Estrellas de Explorador

Estado: Prompt 21 implementado. Una sola moneda virtual ganable, local y sin relación con dinero real. No es una oferta comercial ni asesoría legal.

## Contrato de producto

**Estrellas de Explorador** sustituye al nombre candidato “materiales de exploración”. El saldo es un entero no negativo. Las estrellas se ganan jugando; no se compran, venden, convierten, pierden, caducan ni se entregan por rachas, retorno diario, azar o tiempo conectado. No existe moneda premium.

| Fuente permitida | Regla |
|---|---|
| Discovery | Una reward única por discovery/condición definida; VS-D-A01 concede 1 estrella provisional. |
| Misión | Una vez al completar automáticamente una misión no expirable; la fixture del tucán concede 2 estrellas provisionales. |
| Actividad educativa | Una vez según definition; no reduce por intentos/pistas. La fixture Prompt 23 concede 1 estrella provisional. |
| Colección | Hito data-driven explícito, sin temporizador/FOMO; no implementado todavía. |

| Uso permitido | Estado/límite |
|---|---|
| Mejora visual de Camp | Futuro Prompt 25; nunca bloquea aprendizaje esencial. |
| Cosmético del explorador | Futuro Prompt 26; inclusivo y sin IAP. |
| Compra, moneda premium, energía, racha, loot box, gacha, timer | **Prohibido**. |

La cantidad `1` del reward del tucán es tuning provisional del Vertical Slice, no precio ni compromiso del MVP. Cualquier cantidad nueva vive en `RewardDefinitionAsset` y requiere revisión de economía infantil.

## Arquitectura y atomicidad lógica

```text
Feature → RewardIntent/transaction key
                ↓
Application.GrantRewardUseCase / SpendStarsUseCase
                ↓
Domain.ExplorerStars + PlayerProgress
                ↓
IEconomyRepository → AutosaveCoordinator.Latest → Save v9
```

- `ExplorerStars` impide negativos y detecta overflow.
- `RewardDefinition` es runtime readonly; `RewardDefinitionAsset`/`RewardCatalogAsset` son authoring Content.
- Una `EconomyTransactionId` persistida es la autoridad durable de idempotencia. Un retry/crash no vuelve a aplicar grant/spend.
- El ledger conserva solo las 32 transacciones recientes para diagnóstico; no guarda timestamps, taps, sesiones ni comportamiento granular. No sustituye el set durable de transaction keys.
- Spend y unlock futuro deberán construir un único `PlayerProgress` antes de un checkpoint; este prompt implementa el spend wallet, no una compra de Camp.
- Economy no referencia UI, IAP, ads, analytics, red ni UnityEngine.

Fotografía intenta la reward determinista `economy-tx.discovery.discovery.jungle.keel-billed-toucan` después de registrar el discovery. La misión usa `economy-tx.mission.mission.vertical-slice.photograph-toucan`. Si el proceso cae entre estado y grant, cualquier repetición intenta la misma transaction key: aplica la reward faltante o devuelve `AlreadyProcessed`, nunca duplica.

## UI, Development y Release

`PH_UI_ECONOMY` muestra saldo ES/EN y aclara que las estrellas son virtuales, se ganan jugando y no se compran. El pulso se omite con reduce-motion. El botón `DEBUG +1` solo se habilita en Editor/Development; su definition se compone bajo define Development y no existe en el asset runtime Release. El validator exige una sola moneda/definition de producto y bloquea acoplamientos a compra/ads/azar.

## Persistencia y privacidad

Schema v9 conserva el wallet/keys/ledger introducido en v7 sin reinterpretarlo; v8 añadió misiones y v9 learning mediante migraciones consecutivas. Son IDs técnicos y cantidades agregadas locales, sin PII ni red. El checksum detecta corrupción, no es cifrado.

## Validación

- EditMode: invariantes, grant, spend, insuficiente, overflow, source mismatch, idempotencia, retry tras commit fallido, ledger 32 y migración v6→v7.
- PlayMode: fotografía→discovery (1) + misión (2) → 3 estrellas→segunda captura sin duplicado→flush→recrear servicio; reduce-motion explícito.
- `scripts/validate-economy`: catálogo, reward fixture, safe area/targets y límites de dependencias.
- `scripts/validate`: compile, Addressables, suites y APK Android Development.
