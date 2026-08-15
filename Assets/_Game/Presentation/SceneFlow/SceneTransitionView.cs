using System;
using PequenoExplorador.Application.SceneFlow;
using UnityEngine;
using UnityEngine.UI;

namespace PequenoExplorador.Presentation.SceneFlow
{
    [DisallowMultipleComponent]
    public sealed class SceneTransitionView : MonoBehaviour
    {
        [SerializeField] private GameObject _transitionPanel;
        [SerializeField] private Text _statusText;
        [SerializeField] private Slider _progress;
        [SerializeField] private Button _enterJungleButton;
        [SerializeField] private Button _returnCampButton;
        [SerializeField] private Button _retryButton;
        [SerializeField] private Button _simulateFailureButton;
        [SerializeField] private GameObject _developmentControls;

        private ISceneFlowService _service;

        public event Action EnterJungleRequested;
        public event Action ReturnCampRequested;
        public event Action RetryRequested;
        public event Action SimulateFailureRequested;

        public string StatusText => _statusText == null ? string.Empty : _statusText.text;

        private void Awake()
        {
            _enterJungleButton?.onClick.AddListener(() => EnterJungleRequested?.Invoke());
            _returnCampButton?.onClick.AddListener(() => ReturnCampRequested?.Invoke());
            _retryButton?.onClick.AddListener(() => RetryRequested?.Invoke());
            _simulateFailureButton?.onClick.AddListener(() => SimulateFailureRequested?.Invoke());
        }

        public void Bind(ISceneFlowService service, bool developmentControlsEnabled)
        {
            Unbind();
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _service.Changed += Render;
            if (_developmentControls != null)
            {
                _developmentControls.SetActive(developmentControlsEnabled);
            }

            Render(_service.Snapshot);
        }

        public void Unbind()
        {
            if (_service != null)
            {
                _service.Changed -= Render;
                _service = null;
            }
        }

        private void Render(SceneFlowSnapshot snapshot)
        {
            bool showTransition = snapshot.IsTransitioning || snapshot.HasRecoverableError;
            _transitionPanel?.SetActive(showTransition);
            if (_progress != null)
            {
                _progress.value = snapshot.Progress;
                _progress.gameObject.SetActive(snapshot.IsTransitioning);
            }

            if (_statusText != null)
            {
                _statusText.text = snapshot.HasRecoverableError
                    ? "No pudimos cambiar de lugar · Puedes intentar otra vez"
                    : snapshot.IsTransitioning
                        ? "Preparando " + FriendlyName(snapshot.Target) + "…"
                        : FriendlyName(snapshot.Current);
            }

            bool interactive = !snapshot.IsTransitioning && !snapshot.HasRecoverableError;
            SetButton(_enterJungleButton, interactive && snapshot.Current == SceneFlowState.Camp);
            SetButton(_returnCampButton, interactive && snapshot.Current == SceneFlowState.Expedition);
            SetButton(_retryButton, snapshot.HasRecoverableError);
            if (_simulateFailureButton != null)
            {
                _simulateFailureButton.interactable = !snapshot.IsTransitioning;
            }
        }

        private static string FriendlyName(SceneFlowState state)
        {
            switch (state)
            {
                case SceneFlowState.Camp:
                    return "Campamento";
                case SceneFlowState.Expedition:
                    return "Expedición Selva";
                default:
                    return "Inicio";
            }
        }

        private static void SetButton(Button button, bool visible)
        {
            if (button != null)
            {
                button.gameObject.SetActive(visible);
                button.interactable = visible;
            }
        }

        private void OnDestroy()
        {
            Unbind();
        }
    }
}
