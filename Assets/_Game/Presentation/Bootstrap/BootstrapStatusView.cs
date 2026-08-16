using System;
using PequenoExplorador.Application.Save;
using UnityEngine;
using UnityEngine.UI;

namespace PequenoExplorador.Presentation.Bootstrap
{
    [DisallowMultipleComponent]
    public sealed class BootstrapStatusView : MonoBehaviour
    {
        [SerializeField] private Text _statusText;
        [SerializeField] private GameObject[] _developmentOnlyObjects = Array.Empty<GameObject>();

        public string CurrentStatus => _statusText == null ? string.Empty : _statusText.text;

        public void SetDevelopmentDiagnosticsVisible(bool visible)
        {
            foreach (GameObject target in _developmentOnlyObjects)
            {
                if (target != null)
                {
                    target.SetActive(visible);
                }
            }
        }

        public void ShowInitializing()
        {
            SetStatus("Initializing");
        }

        public void ShowReady(SaveUserNotice saveNotice = SaveUserNotice.None)
        {
            switch (saveNotice)
            {
                case SaveUserNotice.ProgressRecovered:
                    SetStatus("Ready · Progress restored safely");
                    break;
                case SaveUserNotice.NewerSaveVersionDetected:
                    SetStatus("Ready · Newer progress protected");
                    break;
                default:
                    SetStatus("Ready");
                    break;
            }
        }

        public void ShowRecoverableFailure()
        {
            SetStatus("Initialization failed · Retry available");
        }

        public void ShowShutdown()
        {
            SetStatus("Stopped");
        }

        private void SetStatus(string value)
        {
            if (_statusText == null)
            {
                Debug.LogError("PE_BOOTSTRAP_VIEW_MISSING statusText");
                return;
            }

            _statusText.text = value;
        }
    }
}
