using System;
using System.Collections.Generic;
using System.Linq;
using PequenoExplorador.Application.Localization;
using PequenoExplorador.Bootstrap;
using PequenoExplorador.Presentation.Bootstrap;
using PequenoExplorador.Presentation.SceneFlow;
using UnityEditor;
using UnityEditor.Localization;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Pseudo;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace PequenoExplorador.Editor
{
    public static class LocalizationFoundationSetup
    {
        public const string RootPath = "Assets/_Game/Content/Localization";
        public const string SettingsPath = RootPath + "/LocalizationSettings.asset";
        public const string SpanishLocalePath = RootPath + "/Locales/Spanish_es.asset";
        public const string EnglishLocalePath = RootPath + "/Locales/English_en.asset";
        public const string PseudoLocalePath = RootPath + "/Locales/Pseudo_es.asset";
        public const string StringTablesPath = RootPath + "/StringTables";
        public const string AssetTablesPath = RootPath + "/AssetTables";

        [MenuItem("Pequeño Explorador/Development/Localization/Apply Foundation")]
        public static void Apply()
        {
            EnsureFolders();
            LocalizationSettings settings = EnsureSettings();
            Locale spanish = EnsureLocale(SpanishLocalePath, LocaleCode.Spanish, "Español");
            Locale english = EnsureLocale(EnglishLocalePath, LocaleCode.English, "English");
            PseudoLocale pseudo = EnsurePseudoLocale();

            AddLocaleIfNeeded(spanish);
            AddLocaleIfNeeded(english);
            AddLocaleIfNeeded(pseudo);
            ConfigureSettings(settings, spanish);

            var realLocales = new List<Locale> { spanish, english };
            UpsertStringCollection(LocalizationKeys.SharedTable, realLocales, SharedEntries);
            UpsertStringCollection(LocalizationKeys.UiTable, realLocales, UiEntries);
            UpsertStringCollection(LocalizationKeys.ContentTable, realLocales, ContentEntries);
            UpsertAssetCollection(
                LocalizationKeys.VoiceAssetTable,
                realLocales,
                "content.world.camp.name",
                "content.world.jungle.name",
                "audio.voice.instruction.explore",
                "audio.voice.name.jungle",
                "audio.voice.narration.welcome");
            UpsertAssetCollection(
                LocalizationKeys.IllustrationAssetTable,
                realLocales,
                "content.world.camp.background",
                "content.world.jungle.background");

            ConfigureBootstrapScene();
            ClearWorldPlaceholderText(SceneFlowFoundationSetup.CampScenePath);
            ClearWorldPlaceholderText(SceneFlowFoundationSetup.JungleScenePath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("PE_LOCALIZATION_SETUP_OK package=1.5.12 locales=3 stringTables=3 assetTables=2");
        }

        public static void ApplyCli()
        {
            try
            {
                Apply();
                EditorApplication.Exit(0);
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
                EditorApplication.Exit(1);
            }
        }

        private static readonly Entry[] SharedEntries =
        {
            new Entry("shared.product.name", "Pequeño Explorador: Aprende Jugando", "Little Explorer: Learning Through Play"),
            new Entry("shared.build.version", "Versión {0}", "Version {0}", isSmart: true),
            new Entry("shared.fallback.safe", "Algo no salió como esperábamos.", "Something did not go as expected."),
            new Entry("shared.progress.stars", "{0:plural:Una estrella|{} estrellas}", "{0:plural:One star|{} stars}", isSmart: true)
        };

        private static readonly Entry[] UiEntries =
        {
            new Entry("ui.diagnostic.notice", "DIAGNÓSTICO TEMPORAL · SIN GAMEPLAY", "TEMPORARY DIAGNOSTIC · NO GAMEPLAY"),
            new Entry("ui.status.initializing", "Preparando la aventura…", "Preparing the adventure…"),
            new Entry("ui.status.ready", "Listo", "Ready"),
            new Entry("ui.status.progress_recovered", "Listo · Recuperamos tu progreso", "Ready · Your progress is safe"),
            new Entry("ui.status.newer_protected", "Listo · Tu progreso nuevo está protegido", "Ready · Your newer progress is protected"),
            new Entry("ui.status.failure", "No pudimos iniciar · Puedes intentar otra vez", "We could not start · You can try again"),
            new Entry("ui.status.stopped", "Pausado", "Paused"),
            new Entry("ui.transition.error", "No pudimos cambiar de lugar · Puedes intentar otra vez", "We could not change places · You can try again"),
            new Entry("ui.transition.preparing", "Preparando {0}…", "Preparing {0}…", isSmart: true),
            new Entry("ui.action.enter_jungle", "Ir a la selva", "Go to the jungle"),
            new Entry("ui.action.return_camp", "Volver al campamento", "Return to camp"),
            new Entry("ui.action.retry", "Intentar otra vez", "Try again"),
            new Entry("ui.action.simulate_failure", "Simular fallo", "Simulate failure"),
            new Entry("ui.locale.spanish", "Español", "Spanish"),
            new Entry("ui.locale.english", "Inglés", "English"),
            new Entry("ui.locale.pseudo", "Pseudo", "Pseudo"),
            new Entry("ui.pause.title", "Pausa tranquila", "A quiet pause"),
            new Entry("ui.action.resume", "Continuar", "Continue")
        };

        private static readonly Entry[] ContentEntries =
        {
            new Entry("content.world.boot.name", "Inicio", "Start"),
            new Entry("content.world.camp.name", "Campamento", "Camp"),
            new Entry("content.world.jungle.name", "Expedición Selva", "Jungle Expedition"),
            new Entry("content.world.camp.placeholder", "Campamento · PLACEHOLDER", "Camp · PLACEHOLDER"),
            new Entry("content.world.jungle.placeholder", "Expedición Selva · PLACEHOLDER", "Jungle Expedition · PLACEHOLDER"),
            new Entry("content.discovery.placeholder.name", "Descubrimiento de prueba", "Test discovery"),
            new Entry("content.audio.instruction.explore", "Mira a tu alrededor. ¿Qué descubrimos?", "Look around. What can we discover?"),
            new Entry("content.audio.name.jungle", "Selva", "Jungle"),
            new Entry("content.audio.narration.welcome", "Vamos a explorar con calma.", "Let’s explore at our own pace.")
        };

        private static void EnsureFolders()
        {
            EnsureFolder(RootPath);
            EnsureFolder(RootPath + "/Locales");
            EnsureFolder(StringTablesPath);
            EnsureFolder(AssetTablesPath);
        }

        private static LocalizationSettings EnsureSettings()
        {
            LocalizationSettings settings = AssetDatabase.LoadAssetAtPath<LocalizationSettings>(SettingsPath);
            if (settings == null)
            {
                settings = ScriptableObject.CreateInstance<LocalizationSettings>();
                settings.name = "LocalizationSettings";
                AssetDatabase.CreateAsset(settings, SettingsPath);
            }

            LocalizationEditorSettings.ActiveLocalizationSettings = settings;
            return settings;
        }

        private static Locale EnsureLocale(string path, string code, string localeName)
        {
            Locale locale = AssetDatabase.LoadAssetAtPath<Locale>(path);
            if (locale == null)
            {
                locale = Locale.CreateLocale(code);
                AssetDatabase.CreateAsset(locale, path);
            }

            locale.Identifier = new LocaleIdentifier(code);
            locale.LocaleName = localeName;
            EditorUtility.SetDirty(locale);
            return locale;
        }

        private static PseudoLocale EnsurePseudoLocale()
        {
            PseudoLocale locale = AssetDatabase.LoadAssetAtPath<PseudoLocale>(PseudoLocalePath);
            if (locale == null)
            {
                locale = PseudoLocale.CreatePseudoLocale();
                AssetDatabase.CreateAsset(locale, PseudoLocalePath);
            }

            locale.Identifier = new LocaleIdentifier(LocaleCode.Spanish);
            locale.LocaleName = "Pseudo ES (Development)";
            EditorUtility.SetDirty(locale);
            return locale;
        }

        private static void AddLocaleIfNeeded(Locale locale)
        {
            bool exists = LocalizationEditorSettings.GetLocales().Any(
                candidate => candidate == locale);
            bool pseudoExists = locale is PseudoLocale && LocalizationEditorSettings.GetPseudoLocales().Any(
                candidate => candidate == locale);
            if (!exists && !pseudoExists)
            {
                LocalizationEditorSettings.AddLocale(locale);
            }
        }

        private static void ConfigureSettings(LocalizationSettings settings, Locale spanish)
        {
            settings.GetStartupLocaleSelectors().Clear();
            settings.GetStartupLocaleSelectors().Add(new SpecificLocaleSelector
            {
                LocaleId = new LocaleIdentifier(LocaleCode.Spanish)
            });
            settings.GetStringDatabase().UseFallback = true;
            settings.GetAssetDatabase().UseFallback = true;
            LocalizationSettings.ProjectLocale = spanish;
            EditorUtility.SetDirty(settings);
        }

        private static void UpsertStringCollection(
            string tableName,
            IList<Locale> locales,
            IEnumerable<Entry> entries)
        {
            StringTableCollection collection = LocalizationEditorSettings.GetStringTableCollection(tableName) ??
                LocalizationEditorSettings.CreateStringTableCollection(tableName, StringTablesPath, locales);

            foreach (Entry entry in entries)
            {
                if (!collection.SharedData.Entries.Any(existing => existing.Key == entry.Key))
                {
                    collection.SharedData.AddKey(entry.Key);
                }
                UpsertString(collection, locales[0], entry.Key, entry.Spanish, entry.IsSmart);
                UpsertString(collection, locales[1], entry.Key, entry.English, entry.IsSmart);
            }

            foreach (LocalizationTable table in collection.StringTables)
            {
                LocalizationEditorSettings.SetPreloadTableFlag(table, true);
                EditorUtility.SetDirty(table);
            }

            EditorUtility.SetDirty(collection.SharedData);
            EditorUtility.SetDirty(collection);
        }

        private static void UpsertString(
            StringTableCollection collection,
            Locale locale,
            string key,
            string value,
            bool isSmart)
        {
            StringTable table = collection.GetTable(locale.Identifier) as StringTable;
            if (table == null)
            {
                throw new InvalidOperationException("Missing generated table " + collection.TableCollectionName + "_" + locale.Identifier.Code);
            }

            StringTableEntry tableEntry = table.AddEntry(key, value);
            tableEntry.IsSmart = isSmart;
            EditorUtility.SetDirty(table);
        }

        private static void UpsertAssetCollection(
            string tableName,
            IList<Locale> locales,
            params string[] conceptualKeys)
        {
            AssetTableCollection collection = LocalizationEditorSettings.GetAssetTableCollection(tableName) ??
                LocalizationEditorSettings.CreateAssetTableCollection(tableName, AssetTablesPath, locales);
            foreach (string key in conceptualKeys)
            {
                if (!collection.SharedData.Entries.Any(existing => existing.Key == key))
                {
                    collection.SharedData.AddKey(key);
                }
            }

            foreach (LocalizationTable table in collection.AssetTables)
            {
                EditorUtility.SetDirty(table);
            }

            EditorUtility.SetDirty(collection.SharedData);
            EditorUtility.SetDirty(collection);
        }

        private static void ConfigureBootstrapScene()
        {
            Scene scene = EditorSceneManager.OpenScene(ProjectFoundationSetup.BootstrapScenePath, OpenSceneMode.Single);
            DiagnosticBootstrap bootstrap = FindInScene<DiagnosticBootstrap>(scene, DiagnosticBootstrap.PlaceholderObjectName);
            BootstrapStatusView statusView = FindInScene<BootstrapStatusView>(scene, DiagnosticBootstrap.PlaceholderObjectName);
            SceneTransitionView sceneView = FindInScene<SceneTransitionView>(scene, "PH_UI_SCENE_FLOW");
            Text notice = FindInScene<Text>(scene, "Temporary Notice");
            Text product = FindInScene<Text>(scene, "Product Name");
            Text version = FindInScene<Text>(scene, "Development Version");
            Text transitionStatus = FindInScene<Text>(scene, "Transition Status");

            GameObject sceneRoot = sceneView.gameObject;
            Text currentLocation = FindOptionalInScene<Text>(scene, "Current Location") ??
                CreateText(sceneRoot.transform, "Current Location", 34, TextAnchor.MiddleCenter);
            SetRect(currentLocation.rectTransform, new Vector2(0.25f, 0.87f), new Vector2(0.75f, 0.96f));

            GameObject controls = FindInScene<Transform>(scene, "Development Controls").gameObject;
            Button spanish = FindOptionalInScene<Button>(scene, "Locale Spanish") ??
                CreateButton(controls.transform, "Locale Spanish", new Vector2(0.04f, 0.03f), new Vector2(0.30f, 0.16f));
            Button english = FindOptionalInScene<Button>(scene, "Locale English") ??
                CreateButton(controls.transform, "Locale English", new Vector2(0.37f, 0.03f), new Vector2(0.63f, 0.16f));
            Button pseudo = FindOptionalInScene<Button>(scene, "Locale Pseudo") ??
                CreateButton(controls.transform, "Locale Pseudo", new Vector2(0.70f, 0.03f), new Vector2(0.96f, 0.16f));
            SetRect((RectTransform)spanish.transform, new Vector2(0.04f, 0.03f), new Vector2(0.30f, 0.25f));
            SetRect((RectTransform)english.transform, new Vector2(0.37f, 0.03f), new Vector2(0.63f, 0.25f));
            SetRect((RectTransform)pseudo.transform, new Vector2(0.70f, 0.03f), new Vector2(0.96f, 0.25f));

            var statusSerialized = new SerializedObject(statusView);
            statusSerialized.FindProperty("_diagnosticNoticeText").objectReferenceValue = notice;
            statusSerialized.ApplyModifiedPropertiesWithoutUndo();

            var sceneSerialized = new SerializedObject(sceneView);
            sceneSerialized.FindProperty("_currentLocationText").objectReferenceValue = currentLocation;
            sceneSerialized.FindProperty("_localeSpanishButton").objectReferenceValue = spanish;
            sceneSerialized.FindProperty("_localeEnglishButton").objectReferenceValue = english;
            sceneSerialized.FindProperty("_localePseudoButton").objectReferenceValue = pseudo;
            sceneSerialized.ApplyModifiedPropertiesWithoutUndo();

            foreach (Text text in new[] { notice, product, version, transitionStatus, currentLocation })
            {
                text.text = string.Empty;
            }

            foreach (Button button in scene.GetRootGameObjects().SelectMany(root => root.GetComponentsInChildren<Button>(true)))
            {
                Text label = button.GetComponentInChildren<Text>(true);
                if (label != null)
                {
                    label.text = string.Empty;
                }
            }

            EditorUtility.SetDirty(bootstrap);
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
        }

        private static void ClearWorldPlaceholderText(string scenePath)
        {
            Scene scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
            Text label = FindInScene<Text>(scene, "World Label");
            label.text = string.Empty;
            label.gameObject.SetActive(false);
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
        }

        private static T FindInScene<T>(Scene scene, string objectName) where T : Component
        {
            T result = FindOptionalInScene<T>(scene, objectName);
            if (result == null)
            {
                throw new InvalidOperationException("Missing scene object/component: " + objectName + " / " + typeof(T).Name);
            }

            return result;
        }

        private static T FindOptionalInScene<T>(Scene scene, string objectName) where T : Component
        {
            return scene.GetRootGameObjects()
                .SelectMany(root => root.GetComponentsInChildren<T>(true))
                .FirstOrDefault(component => component.gameObject.name == objectName);
        }

        private static Text CreateText(Transform parent, string name, int fontSize, TextAnchor alignment)
        {
            var gameObject = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
            gameObject.transform.SetParent(parent, false);
            Text text = gameObject.GetComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = fontSize;
            text.resizeTextForBestFit = true;
            text.resizeTextMinSize = 14;
            text.resizeTextMaxSize = fontSize;
            text.alignment = alignment;
            text.color = Color.white;
            return text;
        }

        private static Button CreateButton(
            Transform parent,
            string name,
            Vector2 anchorMin,
            Vector2 anchorMax)
        {
            GameObject gameObject = DefaultControls.CreateButton(new DefaultControls.Resources());
            gameObject.name = name;
            gameObject.transform.SetParent(parent, false);
            Button button = gameObject.GetComponent<Button>();
            Text label = button.GetComponentInChildren<Text>();
            label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            label.fontSize = 22;
            label.resizeTextForBestFit = true;
            label.text = string.Empty;
            SetRect(gameObject.GetComponent<RectTransform>(), anchorMin, anchorMax);
            return button;
        }

        private static void SetRect(RectTransform rect, Vector2 anchorMin, Vector2 anchorMax)
        {
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        private static void EnsureFolder(string path)
        {
            string[] segments = path.Split('/');
            string current = segments[0];
            for (int index = 1; index < segments.Length; index++)
            {
                string next = current + "/" + segments[index];
                if (!AssetDatabase.IsValidFolder(next))
                {
                    AssetDatabase.CreateFolder(current, segments[index]);
                }

                current = next;
            }
        }

        private readonly struct Entry
        {
            public Entry(string key, string spanish, string english, bool isSmart = false)
            {
                Key = key;
                Spanish = spanish;
                English = english;
                IsSmart = isSmart;
            }

            public string Key { get; }
            public string Spanish { get; }
            public string English { get; }
            public bool IsSmart { get; }
        }
    }
}
