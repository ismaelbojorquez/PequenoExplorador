using System;
using System.Collections.Generic;
using System.Linq;
using PequenoExplorador.Application.Customization;
using PequenoExplorador.Application.Economy;
using PequenoExplorador.Domain.Content;
using PequenoExplorador.Domain.Progress;
using UnityEngine;

namespace PequenoExplorador.Presentation.Customization
{
    [Serializable]
    public sealed class CustomizationVisualVariant
    {
        [SerializeField] private string _visualId;
        [SerializeField] private GameObject _root;
        [SerializeField] private Renderer[] _renderers = Array.Empty<Renderer>();
        public string RawVisualId => _visualId;
        public GameObject Root => _root;
        public Renderer[] Renderers => _renderers ?? Array.Empty<Renderer>();
    }

    [Serializable]
    public sealed class CustomizationSlotVisualBinding
    {
        [SerializeField] private string _slotId;
        [SerializeField] private GameObject _defaultRoot;
        [SerializeField] private Renderer[] _defaultRenderers = Array.Empty<Renderer>();
        [SerializeField] private CustomizationVisualVariant[] _variants = Array.Empty<CustomizationVisualVariant>();
        [NonSerialized] private MaterialPropertyBlock _propertyBlock;
        public string RawSlotId => _slotId;
        public GameObject DefaultRoot => _defaultRoot;
        public Renderer[] DefaultRenderers => _defaultRenderers ?? Array.Empty<Renderer>();
        public CustomizationVisualVariant[] Variants => _variants ?? Array.Empty<CustomizationVisualVariant>();

        public bool Apply(CosmeticDefinition definition)
        {
            if (definition == null || definition.SlotId.Value != _slotId) return false;
            CustomizationVisualVariant selected = Variants.FirstOrDefault(value => value != null && value.RawVisualId == definition.VisualId.Value);
            if (_defaultRoot != null) _defaultRoot.SetActive(selected == null);
            foreach (CustomizationVisualVariant variant in Variants)
                if (variant?.Root != null) variant.Root.SetActive(ReferenceEquals(variant, selected));
            ApplyColor(selected == null ? DefaultRenderers : selected.Renderers, definition.Color);
            return true;
        }

        private void ApplyColor(IEnumerable<Renderer> renderers, CustomizationColor value)
        {
            _propertyBlock ??= new MaterialPropertyBlock();
            Color32 color = new Color32(value.Red, value.Green, value.Blue, value.Alpha);
            foreach (Renderer renderer in renderers)
            {
                if (renderer == null) continue;
                renderer.GetPropertyBlock(_propertyBlock);
                _propertyBlock.SetColor("_BaseColor", color);
                _propertyBlock.SetColor("_Color", color);
                renderer.SetPropertyBlock(_propertyBlock);
                _propertyBlock.Clear();
            }
        }
    }

    [DisallowMultipleComponent]
    public sealed class ExplorerCustomizationRig : MonoBehaviour
    {
        public const string PlaceholderName = "PH_EXPLORER_CUSTOMIZATION_RIG";
        [SerializeField] private CustomizationSlotVisualBinding[] _bindings = Array.Empty<CustomizationSlotVisualBinding>();
        private ICustomizationCatalog _catalog;
        private IEconomyRepository _repository;
        private CustomizationSelectionResolver _resolver;
        public IReadOnlyList<CustomizationSlotVisualBinding> Bindings => _bindings ?? Array.Empty<CustomizationSlotVisualBinding>();
        public int ApplyCount { get; private set; }

        public void Bind(ICustomizationCatalog catalog, IEconomyRepository repository)
        {
            Unbind();
            _catalog = catalog ?? throw new ArgumentNullException(nameof(catalog));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _resolver = new CustomizationSelectionResolver(catalog);
            _repository.Changed += HandleProgressChanged;
            Render(_repository.Current);
        }

        public void Preview(CosmeticDefinition definition)
        {
            if (definition == null) return;
            Bindings.FirstOrDefault(value => value != null && value.RawSlotId == definition.SlotId.Value)?.Apply(definition);
        }

        public void ClearPreview() => Render(_repository?.Current);

        public void Render(PlayerProgress progress)
        {
            if (_resolver == null || progress == null) return;
            foreach (CosmeticDefinition definition in _resolver.Resolve(progress))
                Bindings.FirstOrDefault(value => value != null && value.RawSlotId == definition.SlotId.Value)?.Apply(definition);
            ApplyCount++;
        }

        private void HandleProgressChanged(PlayerProgress progress) => Render(progress);
        public void Unbind()
        {
            if (_repository != null) _repository.Changed -= HandleProgressChanged;
            _catalog = null; _repository = null; _resolver = null;
        }
        private void OnDisable() => Unbind();
        private void OnDestroy() => Unbind();
    }
}
