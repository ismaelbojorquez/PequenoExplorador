using NUnit.Framework;
using PequenoExplorador.Presentation.Bootstrap;
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

            view.SetDevelopmentDiagnosticsVisible(false);
            view.ShowRecoverableFailure();

            Assert.That(developmentObject.activeSelf, Is.False);
            Assert.That(statusObject.activeSelf, Is.True);
            Assert.That(view.CurrentStatus, Does.Contain("Retry"));
            Object.DestroyImmediate(root);
        }
    }
}
