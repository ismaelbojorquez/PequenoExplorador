using System;
using System.Collections.Generic;
using System.Linq;
using PequenoExplorador.Application.Economy;
using PequenoExplorador.Domain.Content;
using PequenoExplorador.Domain.Economy;
using PequenoExplorador.Domain.Progress;

namespace PequenoExplorador.Application.Customization
{
    public enum UnlockCosmeticOutcome { Unlocked, AlreadyAvailable, MissingDefinition, PrerequisiteLocked, InsufficientStars, ReadOnly }
    public enum EquipCosmeticOutcome { Equipped, AlreadyEquipped, MissingDefinition, Locked, Incompatible, ReadOnly }

    public readonly struct UnlockCosmeticResult
    {
        public UnlockCosmeticResult(UnlockCosmeticOutcome outcome, CosmeticDefinition definition, ExplorerStars balance)
        { Outcome = outcome; Definition = definition; Balance = balance; }
        public UnlockCosmeticOutcome Outcome { get; }
        public CosmeticDefinition Definition { get; }
        public ExplorerStars Balance { get; }
    }

    public readonly struct EquipCosmeticResult
    {
        public EquipCosmeticResult(EquipCosmeticOutcome outcome, CosmeticDefinition definition)
        { Outcome = outcome; Definition = definition; }
        public EquipCosmeticOutcome Outcome { get; }
        public CosmeticDefinition Definition { get; }
    }

    public sealed class UnlockCosmeticUseCase
    {
        private readonly ICustomizationCatalog _catalog;
        private readonly IEconomyRepository _repository;
        public UnlockCosmeticUseCase(ICustomizationCatalog catalog, IEconomyRepository repository)
        { _catalog = catalog ?? throw new ArgumentNullException(nameof(catalog)); _repository = repository ?? throw new ArgumentNullException(nameof(repository)); }

        public UnlockCosmeticResult Execute(CosmeticId id)
        {
            PlayerProgress current = _repository.Current;
            if (!_catalog.TryGetCosmetic(id, out CosmeticDefinition definition)) return new UnlockCosmeticResult(UnlockCosmeticOutcome.MissingDefinition, null, current.Wallet);
            if (_repository.IsReadOnly) return new UnlockCosmeticResult(UnlockCosmeticOutcome.ReadOnly, definition, current.Wallet);
            if (definition.IsInitiallyUnlocked || current.UnlockedCosmeticIds.Contains(id.Value, StringComparer.Ordinal))
                return new UnlockCosmeticResult(UnlockCosmeticOutcome.AlreadyAvailable, definition, current.Wallet);
            if (definition.RequiredCampUpgradeId.IsValid && !current.UnlockedCampUpgradeIds.Contains(definition.RequiredCampUpgradeId.Value, StringComparer.Ordinal))
                return new UnlockCosmeticResult(UnlockCosmeticOutcome.PrerequisiteLocked, definition, current.Wallet);
            if (!current.Wallet.TrySpend(definition.StarCost, out ExplorerStars balance))
                return new UnlockCosmeticResult(UnlockCosmeticOutcome.InsufficientStars, definition, current.Wallet);
            var unlocked = new List<string>(current.UnlockedCosmeticIds) { id.Value };
            PlayerProgress next;
            if (definition.StarCost.Value == 0)
                next = current.WithCustomizationState(unlocked, current.EquippedCosmetics);
            else
            {
                if (current.ProcessedEconomyTransactionIds.Contains(definition.TransactionId.Value, StringComparer.Ordinal))
                    return new UnlockCosmeticResult(UnlockCosmeticOutcome.AlreadyAvailable, definition, current.Wallet);
                var transactions = new List<string>(current.ProcessedEconomyTransactionIds) { definition.TransactionId.Value };
                EconomyLedgerEntry[] ledger = GrantRewardUseCase.Append(current.EconomyLedger,
                    new EconomyLedgerEntry(definition.TransactionId, EconomyTransactionKind.Spend, definition.SpendReasonId,
                        definition.StarCost, balance));
                next = current.WithEconomyAndCosmeticUnlock(balance, transactions, ledger, unlocked);
            }
            _repository.Commit(next);
            return new UnlockCosmeticResult(UnlockCosmeticOutcome.Unlocked, definition, balance);
        }
    }

    public sealed class EquipCosmeticUseCase
    {
        private readonly ICustomizationCatalog _catalog;
        private readonly IEconomyRepository _repository;
        private readonly CustomizationSelectionResolver _resolver;
        public EquipCosmeticUseCase(ICustomizationCatalog catalog, IEconomyRepository repository)
        { _catalog = catalog ?? throw new ArgumentNullException(nameof(catalog)); _repository = repository ?? throw new ArgumentNullException(nameof(repository)); _resolver = new CustomizationSelectionResolver(catalog); }

        public EquipCosmeticResult Execute(CosmeticId id)
        {
            PlayerProgress current = _repository.Current;
            if (!_catalog.TryGetCosmetic(id, out CosmeticDefinition definition)) return new EquipCosmeticResult(EquipCosmeticOutcome.MissingDefinition, null);
            if (_repository.IsReadOnly) return new EquipCosmeticResult(EquipCosmeticOutcome.ReadOnly, definition);
            if (!_resolver.IsAvailable(definition, current)) return new EquipCosmeticResult(EquipCosmeticOutcome.Locked, definition);
            EquippedCosmetic existing = current.EquippedCosmetics.FirstOrDefault(value => value.SlotId == definition.SlotId);
            if (existing != null && existing.CosmeticId == id) return new EquipCosmeticResult(EquipCosmeticOutcome.AlreadyEquipped, definition);
            CosmeticDefinition[] selected = _resolver.Resolve(current).Where(value => value.SlotId != definition.SlotId).ToArray();
            if (!CustomizationSelectionResolver.IsCompatible(definition, selected)) return new EquipCosmeticResult(EquipCosmeticOutcome.Incompatible, definition);
            var equipped = current.EquippedCosmetics.Where(value => value.SlotId != definition.SlotId).ToList();
            equipped.Add(new EquippedCosmetic(definition.SlotId, definition.Id));
            _repository.Commit(current.WithCustomizationState(current.UnlockedCosmeticIds, equipped));
            return new EquipCosmeticResult(EquipCosmeticOutcome.Equipped, definition);
        }
    }
}
