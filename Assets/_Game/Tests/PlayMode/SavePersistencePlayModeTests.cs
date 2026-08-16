using System;
using System.Collections;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using PequenoExplorador.Application.Save;
using PequenoExplorador.Infrastructure.Save;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace PequenoExplorador.Tests.PlayMode
{
    public sealed class SavePersistencePlayModeTests
    {
        [UnityTest]
        public IEnumerator ProgressSurvivesSceneReloadAndServiceRecreation()
        {
            string parent = Path.Combine(Path.GetTempPath(), "PequenoExploradorPlayModeSaveTests");
            string directory = Path.Combine(parent, Guid.NewGuid().ToString("N"));
            try
            {
                using (var store = new LocalFileStore(directory))
                {
                    LocalSaveService writer = CreateService(store);
                    Task initialize = writer.InitializeAsync(CancellationToken.None);
                    yield return WaitForTask(initialize);
                    Task<SaveOperationResult> save = writer.SaveAsync(
                        writer.Current.WithStars(11),
                        CancellationToken.None);
                    yield return WaitForTask(save);
                    Assert.That(save.Result.IsSuccess, Is.True);
                }

                SceneManager.LoadScene("Bootstrap", LoadSceneMode.Single);
                yield return null;

                using (var store = new LocalFileStore(directory))
                {
                    LocalSaveService reader = CreateService(store);
                    Task initialize = reader.InitializeAsync(CancellationToken.None);
                    yield return WaitForTask(initialize);
                    Assert.That(reader.Current.Stars, Is.EqualTo(11));
                    Assert.That(reader.LastLoadResult.Status, Is.EqualTo(SaveLoadStatus.Loaded));
                }
            }
            finally
            {
                string fullParent = Path.GetFullPath(parent) + Path.DirectorySeparatorChar;
                string fullDirectory = Path.GetFullPath(directory);
                if (fullDirectory.StartsWith(fullParent, StringComparison.Ordinal) && Directory.Exists(fullDirectory))
                {
                    Directory.Delete(fullDirectory, true);
                }
            }
        }

        private static LocalSaveService CreateService(LocalFileStore store)
        {
            return new LocalSaveService(
                store,
                "0.1.0-test",
                new SilentLogger(),
                new ISaveMigration[] { new LegacyV0ToV1Migration() });
        }

        private static IEnumerator WaitForTask(Task task)
        {
            float deadline = UnityEngine.Time.realtimeSinceStartup + 10f;
            while (!task.IsCompleted && UnityEngine.Time.realtimeSinceStartup < deadline)
            {
                yield return null;
            }

            Assert.That(task.IsCompleted, Is.True, "Save operation timed out.");
            if (task.IsFaulted)
            {
                Assert.Fail(task.Exception?.ToString());
            }
        }

        private sealed class SilentLogger : PequenoExplorador.Application.Logging.IAppLogger
        {
            public void Write(PequenoExplorador.Application.Logging.AppLogEntry entry)
            {
            }
        }
    }
}
