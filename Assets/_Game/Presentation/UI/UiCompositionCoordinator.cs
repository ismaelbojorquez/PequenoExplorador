using System;
using System.Collections.Generic;
using System.Linq;
using PequenoExplorador.Application.UI;
using UnityEngine;
using UnityEngine.UI;

namespace PequenoExplorador.Presentation.UI
{
    [DisallowMultipleComponent]
    public sealed class UiCompositionCoordinator : MonoBehaviour
    {
        [Serializable]
        public sealed class SurfaceBinding
        {
            [SerializeField] private UiSurfaceId _id;
            [SerializeField] private GameObject _root;
            [SerializeField] private Canvas _canvas;
            [SerializeField] private CanvasGroup _canvasGroup;
            [SerializeField] private GraphicRaycaster _raycaster;

            public SurfaceBinding(UiSurfaceId id, GameObject root)
            {
                _id = id;
                _root = root;
                _canvas = root == null ? null : root.GetComponent<Canvas>();
                _canvasGroup = root == null ? null : root.GetComponent<CanvasGroup>();
                _raycaster = root == null ? null : root.GetComponent<GraphicRaycaster>();
            }

            public UiSurfaceId Id => _id;
            public GameObject Root => _root;
            public Canvas Canvas => _canvas;
            public CanvasGroup CanvasGroup => _canvasGroup;
            public GraphicRaycaster Raycaster => _raycaster;

            public void RefreshComponents()
            {
                if (_root == null) return;
                _canvas = _root.GetComponent<Canvas>();
                _canvasGroup = _root.GetComponent<CanvasGroup>();
                _raycaster = _root.GetComponent<GraphicRaycaster>();
            }
        }

        [SerializeField] private SurfaceBinding[] _surfaces = Array.Empty<SurfaceBinding>();
        private readonly Dictionary<UiSurfaceId, SurfaceBinding> _index = new Dictionary<UiSurfaceId, SurfaceBinding>();
        private bool _tutorialVisible;

        public AppUiState CurrentState { get; private set; } = AppUiState.Boot;
        public IReadOnlyList<SurfaceBinding> Surfaces => _surfaces;
        public bool TutorialVisible => _tutorialVisible;

        private void Awake() => RebuildIndex();

        public void Initialize()
        {
            RebuildIndex();
            ValidateOrThrow();
            Apply(AppUiState.Boot);
        }

        public void Apply(AppUiState state)
        {
            CurrentState = state;
            foreach (SurfaceBinding surface in _surfaces)
            {
                if (surface == null || surface.Root == null) continue;
                SetSurface(surface, UiCompositionPolicy.IsVisible(state, surface.Id, _tutorialVisible));
            }
        }

        public void SetTutorialVisible(bool visible)
        {
            _tutorialVisible = visible;
            Apply(CurrentState);
        }

        public bool IsSurfaceVisible(UiSurfaceId id)
        {
            return _index.TryGetValue(id, out SurfaceBinding binding) && binding.CanvasGroup != null &&
                   binding.CanvasGroup.alpha > 0.99f && binding.CanvasGroup.interactable && binding.CanvasGroup.blocksRaycasts &&
                   binding.Raycaster != null && binding.Raycaster.enabled;
        }

        public void ValidateOrThrow()
        {
            Array values = Enum.GetValues(typeof(UiSurfaceId));
            foreach (UiSurfaceId id in values)
            {
                int count = _surfaces.Count(value => value != null && value.Id == id);
                if (count != 1) throw new InvalidOperationException($"UI surface {id} must be registered exactly once; found {count}.");
            }
            foreach (SurfaceBinding binding in _surfaces)
            {
                if (binding.Root == null || binding.Canvas == null || binding.CanvasGroup == null || binding.Raycaster == null)
                    throw new InvalidOperationException($"UI surface {binding.Id} requires a dedicated Canvas, CanvasGroup and GraphicRaycaster on its root.");
            }
            if (_surfaces.Select(value => value.Raycaster).Distinct().Count() != _surfaces.Length)
                throw new InvalidOperationException("Every UI surface requires its own GraphicRaycaster.");
        }

#if UNITY_EDITOR
        public void ConfigureForEditorAndTests(IEnumerable<SurfaceBinding> surfaces)
        {
            _surfaces = surfaces?.ToArray() ?? Array.Empty<SurfaceBinding>();
            foreach (SurfaceBinding surface in _surfaces) surface?.RefreshComponents();
            RebuildIndex();
        }
#endif

        private void RebuildIndex()
        {
            _index.Clear();
            foreach (SurfaceBinding surface in _surfaces)
            {
                if (surface?.Root != null && !_index.ContainsKey(surface.Id)) _index.Add(surface.Id, surface);
            }
        }

        private static void SetSurface(SurfaceBinding surface, bool visible)
        {
            CanvasGroup group = surface.CanvasGroup;
            if (group != null)
            {
                group.alpha = visible ? 1f : 0f;
                group.interactable = visible;
                group.blocksRaycasts = visible;
            }
            if (surface.Raycaster != null) surface.Raycaster.enabled = visible;
            if (surface.Canvas != null)
            {
                surface.Canvas.overrideSorting = true;
                surface.Canvas.sortingOrder = UiCompositionPolicy.SortingOrder(surface.Id);
            }
        }
    }
}
