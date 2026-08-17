using System;
using System.Collections;
using PequenoExplorador.Application.Economy;
using PequenoExplorador.Application.Localization;
using PequenoExplorador.Domain.Content;
using PequenoExplorador.Domain.Economy;
using PequenoExplorador.Domain.Progress;
using UnityEngine;
using UnityEngine.UI;

namespace PequenoExplorador.Presentation.Economy
{
    [DisallowMultipleComponent]
    public sealed class EconomyView : MonoBehaviour
    {
        public const string PlaceholderObjectName = "PH_UI_ECONOMY";
        [SerializeField] private RectTransform _animatedRoot;
        [SerializeField] private Text _balance;
        [SerializeField] private Text _notice;
        [SerializeField] private Button _debugGrant;
        private IEconomyRepository _repository;
        private ILocalizationService _localization;
        private GrantRewardUseCase _grant;
        private bool _reduceMotion;
        private int _debugSequence;
        private Coroutine _pulse;
        public string BalanceText => _balance == null ? string.Empty : _balance.text;
        public bool DebugGrantVisible => _debugGrant != null && _debugGrant.gameObject.activeSelf;
        public bool ReduceMotionEnabled => _reduceMotion;

        public void Bind(IEconomyRepository repository, ILocalizationService localization, GrantRewardUseCase grant,
            bool developmentDiagnostics, bool reduceMotion)
        {
            Unbind();
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _localization = localization ?? throw new ArgumentNullException(nameof(localization));
            _grant = grant ?? throw new ArgumentNullException(nameof(grant));
            _reduceMotion = reduceMotion;
            _debugSequence = _repository.Current.ProcessedEconomyTransactionIds.Count;
            _repository.Changed += HandleChanged;
            _localization.LocaleChanged += HandleLocaleChanged;
#if UNITY_EDITOR || PE_DEVELOPMENT_SERVICES
            if (_debugGrant != null) _debugGrant.gameObject.SetActive(false);
#else
            if (_debugGrant != null) _debugGrant.gameObject.SetActive(false);
#endif
            if (_debugGrant != null) _debugGrant.onClick.AddListener(GrantDebugStar);
            Render(_repository.Current);
        }

        public void SetReduceMotion(bool enabled) => _reduceMotion = enabled;
        private void HandleChanged(PlayerProgress progress)
        {
            Render(progress);
            if (_reduceMotion || _animatedRoot == null) return;
            if (_pulse != null) StopCoroutine(_pulse);
            _pulse = StartCoroutine(Pulse());
        }
        private void HandleLocaleChanged(string _) => Render(_repository?.Current);
        private void Render(PlayerProgress progress)
        {
            if (progress == null || _localization == null) return;
            try
            {
                if (_balance != null) _balance.text = _localization.Resolve(LocalizationKeys.StarsCount, progress.Stars);
                if (_notice != null) _notice.text = _localization.Resolve(LocalizationKeys.EconomyVirtualNotice);
                Text label = _debugGrant == null ? null : _debugGrant.GetComponentInChildren<Text>(true);
                if (label != null) label.text = _localization.Resolve(LocalizationKeys.EconomyDebugGrant);
            }
            catch { }
        }
        private void GrantDebugStar()
        {
#if UNITY_EDITOR || PE_DEVELOPMENT_SERVICES
            _grant?.Execute(RewardId.Parse("reward.debug.explorer-stars"),
                EconomyTransactionId.Parse("economy-tx.debug." + (++_debugSequence).ToString(System.Globalization.CultureInfo.InvariantCulture)),
                RewardSourceKind.Development, "development.debug");
#endif
        }
        private IEnumerator Pulse()
        {
            Vector3 original = _animatedRoot.localScale;
            _animatedRoot.localScale = original * 1.08f;
            yield return null;
            _animatedRoot.localScale = original;
            _pulse = null;
        }
        public void Unbind()
        {
            if (_repository != null) _repository.Changed -= HandleChanged;
            if (_localization != null) _localization.LocaleChanged -= HandleLocaleChanged;
            if (_debugGrant != null) _debugGrant.onClick.RemoveListener(GrantDebugStar);
            if (_pulse != null) { StopCoroutine(_pulse); _pulse = null; }
            if (_animatedRoot != null) _animatedRoot.localScale = Vector3.one;
            _repository = null; _localization = null; _grant = null;
        }
        private void OnDestroy() => Unbind();
    }
}
