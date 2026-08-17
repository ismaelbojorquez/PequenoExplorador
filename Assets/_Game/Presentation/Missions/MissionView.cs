using System;
using System.Linq;
using PequenoExplorador.Application.Localization;
using PequenoExplorador.Application.Missions;
using PequenoExplorador.Domain.Content;
using PequenoExplorador.Domain.Progress;
using UnityEngine;
using UnityEngine.UI;

namespace PequenoExplorador.Presentation.Missions
{
    [DisallowMultipleComponent]
    public sealed class MissionView : MonoBehaviour
    {
        public const string PlaceholderObjectName = "PH_UI_MISSIONS";
        public static readonly MissionId FixtureMissionId = MissionId.Parse("mission.vertical-slice.photograph-toucan");
        [SerializeField] private Text _title;
        [SerializeField] private Text _body;
        [SerializeField] private Button _activate;
        private IMissionCatalog _catalog;
        private IMissionRepository _repository;
        private MissionCoordinator _coordinator;
        private ILocalizationService _localization;
        public string TitleText => _title == null ? string.Empty : _title.text;
        public string BodyText => _body == null ? string.Empty : _body.text;
        public bool ActivateVisible => _activate != null && _activate.gameObject.activeSelf;

        public void Bind(IMissionCatalog catalog, IMissionRepository repository, MissionCoordinator coordinator,
            ILocalizationService localization)
        {
            Unbind();
            _catalog = catalog ?? throw new ArgumentNullException(nameof(catalog));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _coordinator = coordinator ?? throw new ArgumentNullException(nameof(coordinator));
            _localization = localization ?? throw new ArgumentNullException(nameof(localization));
            _repository.Changed += HandleChanged;
            _localization.LocaleChanged += HandleLocaleChanged;
            if (_activate != null) _activate.onClick.AddListener(HandleActivate);
            Render(_repository.Current);
        }

        public MissionActivationResult ActivateFixture()
        {
            MissionActivationResult result = _coordinator == null
                ? new MissionActivationResult(MissionActivationOutcome.Missing, null)
                : _coordinator.Activate(FixtureMissionId);
            Render(_repository?.Current);
            return result;
        }
        public void Refresh() => Render(_repository?.Current);
        private void HandleActivate() => ActivateFixture();

        private void HandleChanged(PlayerProgress progress) => Render(progress);
        private void HandleLocaleChanged(string _) => Render(_repository?.Current);
        private void Render(PlayerProgress progress)
        {
            if (progress == null || _localization == null || !_catalog.TryGet(FixtureMissionId, out MissionDefinition definition)) return;
            MissionProgress state = progress.Missions.FirstOrDefault(item => item.Id.Equals(FixtureMissionId));
            try
            {
                if (_title != null) _title.text = _localization.Resolve(definition.Title);
                Text buttonLabel = _activate == null ? null : _activate.GetComponentInChildren<Text>(true);
                if (buttonLabel != null) buttonLabel.text = _localization.Resolve(LocalizationKeys.MissionActivate);
                if (_activate != null) _activate.gameObject.SetActive(state == null);
                if (_body == null) return;
                if (state == null) { _body.text = _localization.Resolve(definition.Summary); return; }
                if (state.IsCompleted) { _body.text = _localization.Resolve(definition.Completion); return; }
                MissionObjectiveDefinition objective = definition.Objectives[0];
                MissionObjectiveProgress value = state.Objectives.First(item => item.Id.Equals(objective.Id));
                _body.text = _localization.Resolve(LocalizationKeys.MissionProgress,
                    _localization.Resolve(objective.Label), value.Count, objective.TargetCount);
            }
            catch { }
        }

        public void Unbind()
        {
            if (_repository != null) _repository.Changed -= HandleChanged;
            if (_localization != null) _localization.LocaleChanged -= HandleLocaleChanged;
            if (_activate != null) _activate.onClick.RemoveListener(HandleActivate);
            _catalog = null; _repository = null; _coordinator = null; _localization = null;
        }
        private void OnDestroy() => Unbind();
    }
}
