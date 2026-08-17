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

        public static void ApplyApprovedContentEntries()
        {
            LocalizationSettings settings = AssetDatabase.LoadAssetAtPath<LocalizationSettings>(SettingsPath);
            Locale spanish = AssetDatabase.LoadAssetAtPath<Locale>(SpanishLocalePath);
            Locale english = AssetDatabase.LoadAssetAtPath<Locale>(EnglishLocalePath);
            if (settings == null || spanish == null || english == null)
                throw new InvalidOperationException("Localization foundation must exist before adopting approved content.");
            LocalizationEditorSettings.ActiveLocalizationSettings = settings;
            UpsertStringCollection(LocalizationKeys.ContentTable, new[] { spanish, english }, ContentEntries);
            AssetDatabase.SaveAssets();
        }

        public static void ApplyPhotographyEntries()
        {
            LocalizationSettings settings = AssetDatabase.LoadAssetAtPath<LocalizationSettings>(SettingsPath);
            Locale spanish = AssetDatabase.LoadAssetAtPath<Locale>(SpanishLocalePath);
            Locale english = AssetDatabase.LoadAssetAtPath<Locale>(EnglishLocalePath);
            if (settings == null || spanish == null || english == null)
                throw new InvalidOperationException("Localization foundation must exist before photography setup.");
            LocalizationEditorSettings.ActiveLocalizationSettings = settings;
            UpsertStringCollection(LocalizationKeys.UiTable, new[] { spanish, english }, UiEntries);
            AssetDatabase.SaveAssets();
        }

        public static void ApplyAlbumEntries()
        {
            LocalizationSettings settings = AssetDatabase.LoadAssetAtPath<LocalizationSettings>(SettingsPath);
            Locale spanish = AssetDatabase.LoadAssetAtPath<Locale>(SpanishLocalePath);
            Locale english = AssetDatabase.LoadAssetAtPath<Locale>(EnglishLocalePath);
            if (settings == null || spanish == null || english == null)
                throw new InvalidOperationException("Localization foundation must exist before album setup.");
            LocalizationEditorSettings.ActiveLocalizationSettings = settings;
            UpsertStringCollection(LocalizationKeys.UiTable, new[] { spanish, english }, UiEntries);
            UpsertStringCollection(LocalizationKeys.ContentTable, new[] { spanish, english }, ContentEntries);
            AssetDatabase.SaveAssets();
        }

        public static void ApplyMissionEntries()
        {
            LocalizationSettings settings = AssetDatabase.LoadAssetAtPath<LocalizationSettings>(SettingsPath);
            Locale spanish = AssetDatabase.LoadAssetAtPath<Locale>(SpanishLocalePath);
            Locale english = AssetDatabase.LoadAssetAtPath<Locale>(EnglishLocalePath);
            if (settings == null || spanish == null || english == null)
                throw new InvalidOperationException("Localization foundation must exist before mission setup.");
            LocalizationEditorSettings.ActiveLocalizationSettings = settings;
            UpsertStringCollection(LocalizationKeys.UiTable, new[] { spanish, english }, UiEntries);
            AssetDatabase.SaveAssets();
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
            new Entry("ui.action.resume", "Continuar", "Continue"),
            new Entry("ui.world.unavailable", "Este lugar todavía no está disponible.", "This place is not available yet."),
            new Entry("ui.world.missing", "Este lugar ya no está disponible. Tu progreso sigue seguro.", "This place is no longer available. Your progress is still safe."),
            new Entry("ui.interaction.approaching", "Vamos a acercarnos.", "Let’s move a little closer."),
            new Entry("ui.interaction.action", "Mirar de cerca", "Look closer"),
            new Entry("ui.interaction.cancel", "Ahora no", "Not now"),
            new Entry("ui.interaction.unavailable", "Todavía no podemos mirar esto. Probemos con otra cosa.", "We can’t look at this yet. Let’s try something else."),
            new Entry("ui.interaction.completed", "¡Lo vimos de cerca!", "We took a closer look!"),
            new Entry("ui.interaction.wait", "Esperemos un momento para volver a mirar.", "Let’s wait a moment before looking again."),
            new Entry("ui.discovery.new", "¡Nuevo descubrimiento!", "New discovery!"),
            new Entry("ui.discovery.repeated", "¡Lo observaste otra vez!", "You spotted it again!"),
            new Entry("ui.discovery.debug_count", "{0} · Observaciones: {1}", "{0} · Observations: {1}", isSmart: true),
            new Entry("ui.economy.virtual_notice", "Estrellas virtuales · se ganan jugando y no se compran", "Virtual stars · earned by playing and never purchased"),
            new Entry("ui.economy.debug_grant", "DEBUG +1 estrella", "DEBUG +1 star"),
            new Entry("ui.economy.insufficient", "Todavía faltan algunas estrellas. Podemos seguir explorando.", "A few more stars are needed. We can keep exploring."),
            new Entry("ui.mission.photograph_toucan.title", "La foto del tucán", "The toucan photo"),
            new Entry("ui.mission.photograph_toucan.summary", "Busca al tucán y toma una foto.", "Find the toucan and take a photo."),
            new Entry("ui.mission.photograph_toucan.objective", "Fotografía al tucán pico canoa", "Photograph the Keel-billed Toucan"),
            new Entry("ui.mission.photograph_toucan.completion", "¡Misión completa! El tucán quedó en tu álbum.", "Mission complete! The toucan is now in your album."),
            new Entry("ui.mission.activate", "Comenzar misión", "Start mission"),
            new Entry("ui.mission.progress", "{0}: {1} de {2}", "{0}: {1} of {2}", isSmart: true),
            new Entry("ui.mission.prerequisites", "Primero terminemos otra misión.", "Let’s finish another mission first."),
            new Entry("ui.photography.open", "Vamos a tomar una foto.", "Let’s take a photo."),
            new Entry("ui.photography.move_closer", "Acércate un poquito.", "Move a little closer."),
            new Entry("ui.photography.center", "Pon al tucán dentro del marco.", "Place the toucan inside the frame."),
            new Entry("ui.photography.ready", "¡Listo para la foto!", "Ready for the photo!"),
            new Entry("ui.photography.capture", "Foto", "Photo"),
            new Entry("ui.photography.exit", "Salir", "Exit"),
            new Entry("ui.photography.positive_hint", "Casi. Probemos desde otro lugar.", "Almost. Let’s try from another spot."),
            new Entry("ui.photography.captured_new", "¡Nuevo descubrimiento!", "New discovery!"),
            new Entry("ui.photography.captured_repeated", "¡Qué buena observación!", "Great observation!"),
            new Entry("ui.photography.storage_fallback", "¡Descubrimiento guardado! Usaremos su ilustración.", "Discovery saved! We’ll use its illustration."),
            new Entry("ui.album.open", "Álbum", "Album"),
            new Entry("ui.album.title", "Álbum de la Selva", "Jungle Album"),
            new Entry("ui.album.back", "Volver", "Back"),
            new Entry("ui.album.locked_name", "Por descubrir", "Waiting to be discovered"),
            new Entry("ui.album.locked_hint", "Explora la selva para encontrarlo.", "Explore the jungle to find it."),
            new Entry("ui.album.discovered", "Descubierto", "Discovered"),
            new Entry("ui.album.category_progress", "{0}: {1} de {2}", "{0}: {1} of {2}", isSmart: true),
            new Entry("ui.album.world_progress", "Encontrados: {0} de {1}", "Found: {0} of {1}", isSmart: true),
            new Entry("ui.album.loading", "Preparando el álbum…", "Preparing the album…"),
            new Entry("ui.album.empty", "Todavía no hay entradas aquí.", "There are no entries here yet."),
            new Entry("ui.album.error", "No pudimos abrir esta página. Puedes volver e intentar otra vez.", "We could not open this page. You can go back and try again."),
            new Entry("ui.album.page", "Página {0} de {1}", "Page {0} of {1}", isSmart: true),
            new Entry("ui.album.previous", "Anterior", "Previous"),
            new Entry("ui.album.next", "Siguiente", "Next"),
            new Entry("ui.album.field.habitat", "Dónde vive", "Where it lives"),
            new Entry("ui.album.field.diet", "Qué come", "What it eats"),
            new Entry("ui.album.field.size", "Tamaño", "Size"),
            new Entry("ui.album.field.curiosity", "Algo curioso", "Something curious"),
            new Entry("ui.album.field.sound", "Cómo suena", "How it sounds"),
            new Entry("ui.album.fact_pending", "Este dato está por confirmar.", "This fact is still being checked."),
            new Entry("ui.album.replay", "Escuchar otra vez", "Listen again"),
            new Entry("ui.album.audio_pending", "Audio por preparar", "Audio is being prepared"),
            new Entry("ui.album.photo_loading", "Buscando tu mejor foto…", "Finding your best photo…"),
            new Entry("ui.album.canonical_fallback", "Usamos su imagen del álbum.", "Using its album picture."),
            new Entry("ui.album.best_photo", "Tu mejor foto", "Your best photo")
        };

        private static readonly Entry[] ContentEntries =
        {
            new Entry("content.world.boot.name", "Inicio", "Start"),
            new Entry("content.world.camp.name", "Campamento", "Camp"),
            new Entry("content.world.jungle.name", "Expedición Selva", "Jungle Expedition"),
            new Entry("content.world.camp.placeholder", "Campamento · PLACEHOLDER", "Camp · PLACEHOLDER"),
            new Entry("content.world.jungle.placeholder", "Expedición Selva · PLACEHOLDER", "Jungle Expedition · PLACEHOLDER"),
            new Entry("content.discovery.placeholder.name", "Descubrimiento de prueba", "Test discovery"),
            new Entry("content.discovery.keel-billed-toucan.name", "Tucán pico canoa", "Keel-billed Toucan"),
            new Entry("content.category.discovery.animals", "Animales", "Animals"),
            new Entry("content.fact.keel-billed-toucan.identity", "Este tucán es un Ramphastos sulfuratus.", "This toucan is Ramphastos sulfuratus."),
            new Entry("content.fact.keel-billed-toucan.common-name", "Tucán pico canoa.", "Keel-billed Toucan."),
            new Entry("content.fact.keel-billed-toucan.range", "Vive desde el sur de México, a través de Centroamérica, hasta una parte del norte de Sudamérica.", "It lives from southern Mexico through Central America to part of northern South America."),
            new Entry("content.fact.keel-billed-toucan.habitat", "Vive entre los árboles de selvas cálidas y bosques que están creciendo de nuevo.", "It lives among the trees of warm forests, including forests that are growing back."),
            new Entry("content.fact.keel-billed-toucan.diet", "Come sobre todo frutas.", "It mostly eats fruit."),
            new Entry("content.fact.keel-billed-toucan.bill", "Su pico grande tiene varios colores: verde, naranja, rojo y azul.", "Its large bill has several colors: green, orange, red, and blue."),
            new Entry("content.fact.keel-billed-toucan.voice", "Su llamado se parece a un croar que se repite.", "Its call sounds like a repeated croak."),
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
