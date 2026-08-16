using System;
using System.Collections.Generic;
using PequenoExplorador.Application.Configuration;
using UnityEngine;

namespace PequenoExplorador.Content.Configuration
{
    [CreateAssetMenu(fileName = "AppConfig", menuName = "Pequeño Explorador/Configuration/App Config")]
    public sealed class AppConfigAsset : ScriptableObject
    {
        [SerializeField] private BuildProfile _profile = BuildProfile.Unknown;
        [SerializeField] private string _productName = AppConfigDefaults.ProductName;
        [SerializeField] private string _appVersion = AppConfigDefaults.DevelopmentAppVersion;
        [SerializeField] private int _randomSeed = AppConfigDefaults.RandomSeed;
        [SerializeField] private int _sceneTransitionTimeoutSeconds = AppConfigDefaults.SceneTransitionTimeoutSeconds;
        [SerializeField] private int _autosaveDebounceMilliseconds = AppConfigDefaults.AutosaveDebounceMilliseconds;
        [SerializeField] private FeatureFlag[] _enabledFeatures = Array.Empty<FeatureFlag>();

        public BuildProfile Profile => _profile;
        public string ProductName => _productName;
        public string AppVersion => _appVersion;
        public int RandomSeed => _randomSeed;
        public int SceneTransitionTimeoutSeconds => _sceneTransitionTimeoutSeconds;
        public int AutosaveDebounceMilliseconds => _autosaveDebounceMilliseconds;
        public IReadOnlyList<FeatureFlag> EnabledFeatures => Array.AsReadOnly(
            _enabledFeatures ?? Array.Empty<FeatureFlag>());

#if UNITY_EDITOR
        public void ConfigureForEditorAndTests(
            BuildProfile profile,
            string productName,
            string appVersion,
            int randomSeed,
            int sceneTransitionTimeoutSeconds,
            int autosaveDebounceMilliseconds,
            params FeatureFlag[] enabledFeatures)
        {
            _profile = profile;
            _productName = productName;
            _appVersion = appVersion;
            _randomSeed = randomSeed;
            _sceneTransitionTimeoutSeconds = sceneTransitionTimeoutSeconds;
            _autosaveDebounceMilliseconds = autosaveDebounceMilliseconds;
            _enabledFeatures = enabledFeatures == null
                ? Array.Empty<FeatureFlag>()
                : (FeatureFlag[])enabledFeatures.Clone();
        }
#endif
    }
}
