using System;
using System.Linq;
using PequenoExplorador.Application.Audio;
using PequenoExplorador.Application.Localization;
using PequenoExplorador.Application.Tutorial;
using UnityEngine;

namespace PequenoExplorador.Content.Tutorial
{
    [Serializable]
    public sealed class TutorialStepAsset
    {
        [SerializeField] private string _id;
        [SerializeField] private TutorialTrigger _trigger;
        [SerializeField] private TutorialAction _allowedActions;
        [SerializeField] private TutorialSpotlight _spotlight;
        [SerializeField] private string _instructionKey;
        [SerializeField] private string _voiceCueId;
        [SerializeField, Min(1f)] private float _standardHelpSeconds = 12f;
        [SerializeField, Min(1f)] private float _moreGuidanceHelpSeconds = 6f;

        public TutorialStepDefinition ToRuntime() => new TutorialStepDefinition(
            _id, _trigger, _allowedActions, _spotlight,
            new LocalizedKey(LocalizationKeys.UiTable, _instructionKey), new AudioCueId(_voiceCueId),
            _standardHelpSeconds, _moreGuidanceHelpSeconds);
    }

    [CreateAssetMenu(fileName = "PH_Tutorial_VerticalSlice", menuName = "Pequeño Explorador/Tutorial Definition")]
    public sealed class TutorialDefinitionAsset : ScriptableObject
    {
        [SerializeField] private string _tutorialId = "tutorial.vertical-slice";
        [SerializeField, Min(1)] private int _contentVersion = 1;
        [SerializeField] private TutorialStepAsset[] _steps = Array.Empty<TutorialStepAsset>();
        [SerializeField] private string _placeholderId = "PH_TUTORIAL_VERTICAL_SLICE";
        [SerializeField] private string _releaseState = "ReleaseBlockedPendingNarration";

        public string TutorialId => _tutorialId;
        public int ContentVersion => _contentVersion;
        public string PlaceholderId => _placeholderId;
        public string ReleaseState => _releaseState;
        public TutorialDefinition ToRuntime() => new TutorialDefinition(_tutorialId, _contentVersion,
            (_steps ?? Array.Empty<TutorialStepAsset>()).Select(value => value == null
                ? throw new InvalidOperationException("Tutorial step asset is null.") : value.ToRuntime()));
    }
}
