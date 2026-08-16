using NUnit.Framework;
using PequenoExplorador.Presentation.Bootstrap;
using PequenoExplorador.Application.Save;
using PequenoExplorador.Application.Localization;
using PequenoExplorador.Tests.EditMode.Fixtures;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace PequenoExplorador.Tests.EditMode
{
    public sealed class BootstrapStatusViewTests
    {
        [Test]
        public void FailureIsVisibleAndDevelopmentObjectsCanBeRemovedSafely()
        {
            var root = new GameObject("StatusRoot");
            var statusObject = new GameObject("Status", typeof(RectTransform), typeof(CanvasRenderer));
            var developmentObject = new GameObject("DevelopmentOnly");
            statusObject.transform.SetParent(root.transform);
            developmentObject.transform.SetParent(root.transform);
            Text statusText = statusObject.AddComponent<Text>();
            BootstrapStatusView view = root.AddComponent<BootstrapStatusView>();
            var serializedView = new SerializedObject(view);
            serializedView.FindProperty("_statusText").objectReferenceValue = statusText;
            SerializedProperty developmentObjects = serializedView.FindProperty("_developmentOnlyObjects");
            developmentObjects.arraySize = 1;
            developmentObjects.GetArrayElementAtIndex(0).objectReferenceValue = developmentObject;
            serializedView.ApplyModifiedPropertiesWithoutUndo();
            view.BindLocalization(CreateLocalization());

            view.SetDevelopmentDiagnosticsVisible(false);
            view.ShowRecoverableFailure();

            Assert.That(developmentObject.activeSelf, Is.False);
            Assert.That(statusObject.activeSelf, Is.True);
            Assert.That(view.CurrentStatus, Does.Contain("Retry"));
            Object.DestroyImmediate(root);
        }

        [Test]
        public void RecoveredProgressUsesNonAlarmistVisibleCopy()
        {
            var root = new GameObject("StatusRoot");
            var statusObject = new GameObject("Status", typeof(RectTransform), typeof(CanvasRenderer));
            statusObject.transform.SetParent(root.transform);
            Text statusText = statusObject.AddComponent<Text>();
            BootstrapStatusView view = root.AddComponent<BootstrapStatusView>();
            var serializedView = new SerializedObject(view);
            serializedView.FindProperty("_statusText").objectReferenceValue = statusText;
            serializedView.ApplyModifiedPropertiesWithoutUndo();
            view.BindLocalization(CreateLocalization());

            view.ShowReady(SaveUserNotice.ProgressRecovered);

            Assert.That(view.CurrentStatus, Does.Contain("restored safely"));
            Assert.That(view.CurrentStatus, Does.Not.Contain("corrupt"));
            Assert.That(view.CurrentStatus, Does.Not.Contain("lost"));
            Object.DestroyImmediate(root);
        }

        private static FakeLocalizationService CreateLocalization()
        {
            return new FakeLocalizationService(new Dictionary<string, string>
            {
                [LocalizationKeys.StatusFailure.ToString()] = "Initialization failed · Retry available",
                [LocalizationKeys.StatusRecovered.ToString()] = "Ready · Progress restored safely"
            });
        }
    }
}
