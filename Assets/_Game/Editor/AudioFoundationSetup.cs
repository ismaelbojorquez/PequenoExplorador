using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using PequenoExplorador.Application.Audio;
using PequenoExplorador.Application.Localization;
using PequenoExplorador.Bootstrap;
using PequenoExplorador.Content.Audio;
using PequenoExplorador.Presentation.Audio;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace PequenoExplorador.Editor
{
    public static class AudioFoundationSetup
    {
        public const string MixerPath = "Assets/_Game/Audio/Mixers/PE_Main.mixer";
        public const string ClipRoot = "Assets/_Game/Audio/Placeholders";
        public const string CueRoot = "Assets/_Game/Content/Audio/Cues";
        public const string CatalogPath = "Assets/_Game/Content/Audio/AudioCueCatalog.asset";
        public const string AudioLabel = "audio-local";
        public const string PlaceholderLabel = "audio-placeholder";

        private static readonly CueSeed[] CueSeeds =
        {
            new CueSeed("audio.music.camp", AudioCueCategory.Music, AudioBus.Music, AudioPriority.Low, true, 0f, 0.24f, null, "PH_MUSIC_CAMP", "PH_Music_Camp", 196f, 196f, 2f),
            new CueSeed("audio.ambience.camp", AudioCueCategory.Ambience, AudioBus.Ambience, AudioPriority.Low, true, 0f, 0.20f, null, "PH_AMBIENCE_CAMP", "PH_Ambience_Camp", 131f, 131f, 2f),
            new CueSeed("audio.feedback.confirm", AudioCueCategory.Feedback, AudioBus.Effects, AudioPriority.Normal, false, 0.08f, 0.28f, null, "PH_FEEDBACK_CONFIRM", "PH_Feedback_Confirm", 523f, 523f, 0.18f),
            new CueSeed("audio.feedback.retry", AudioCueCategory.Feedback, AudioBus.Effects, AudioPriority.Normal, false, 0.08f, 0.24f, null, "PH_FEEDBACK_RETRY", "PH_Feedback_Retry", 392f, 392f, 0.18f),
            new CueSeed("audio.voice.instruction.explore", AudioCueCategory.VoiceInstruction, AudioBus.Voice, AudioPriority.High, false, 0f, 0.30f, "content.audio.instruction.explore", "PH_VOICE_INSTRUCTION_EXPLORE", "PH_Voice_Instruction_Explore", 294f, 330f, 0.55f),
            new CueSeed("audio.voice.name.jungle", AudioCueCategory.VoiceName, AudioBus.Voice, AudioPriority.Normal, false, 0f, 0.30f, "content.audio.name.jungle", "PH_VOICE_NAME_JUNGLE", "PH_Voice_Name_Jungle", 349f, 370f, 0.38f),
            new CueSeed("audio.voice.narration.welcome", AudioCueCategory.Narration, AudioBus.Voice, AudioPriority.Critical, false, 0f, 0.30f, "content.audio.narration.welcome", "PH_VOICE_NARRATION_WELCOME", "PH_Voice_Narration_Welcome", 262f, 277f, 0.60f)
        };

        [MenuItem("Pequeño Explorador/Development/Audio/Apply Foundation")]
        public static void Apply()
        {
            try
            {
                EnsureFolder("Assets/_Game/Audio");
                EnsureFolder("Assets/_Game/Audio/Mixers");
                EnsureFolder(ClipRoot);
                EnsureFolder(CueRoot);
                AudioMixer mixer = EnsureMixer();
                var groups = RequiredGroups(mixer);
                var cues = new List<AudioCueDefinition>();
                foreach (CueSeed seed in CueSeeds)
                {
                    cues.Add(EnsureCue(seed));
                }

                AudioCueCatalogAsset catalog = EnsureCatalog(mixer, groups, cues);
                ConfigureAddressables(cues);
                ConfigureBootstrapScene(catalog);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                Debug.Log("PE_AUDIO_SETUP_OK buses=5 cues=7 clips=10 placeholders=10 sampleRate=48000 remote=false");
                if (UnityEngine.Application.isBatchMode) EditorApplication.Exit(0);
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
                if (UnityEngine.Application.isBatchMode) EditorApplication.Exit(2);
                throw;
            }
        }

        private static AudioMixer EnsureMixer()
        {
            AudioMixer mixer = AssetDatabase.LoadAssetAtPath<AudioMixer>(MixerPath);
            if (mixer == null)
            {
                Type controllerType = AppDomain.CurrentDomain.GetAssemblies()
                    .Select(assembly => assembly.GetType("UnityEditor.Audio.AudioMixerController", false))
                    .FirstOrDefault(type => type != null);
                if (controllerType == null) throw new InvalidOperationException("Unity AudioMixer editor API is unavailable.");
                MethodInfo create = controllerType.GetMethod("CreateMixerControllerAtPath", BindingFlags.Public | BindingFlags.Static);
                mixer = create?.Invoke(null, new object[] { MixerPath }) as AudioMixer;
                if (mixer == null) throw new InvalidOperationException("Could not create AudioMixer at " + MixerPath);
            }

            Type type = mixer.GetType();
            PropertyInfo masterProperty = type.GetProperty("masterGroup", BindingFlags.Public | BindingFlags.Instance);
            object master = masterProperty?.GetValue(mixer);
            MethodInfo createGroup = type.GetMethod("CreateNewGroup", BindingFlags.Public | BindingFlags.Instance);
            MethodInfo addChild = type.GetMethod("AddChildToParent", BindingFlags.Public | BindingFlags.Instance);
            if (master == null || createGroup == null || addChild == null)
            {
                throw new InvalidOperationException("Pinned Unity AudioMixer group API changed; setup stopped safely.");
            }

            foreach (string name in new[] { "Music", "Ambience", "Effects", "Voice" })
            {
                if (mixer.FindMatchingGroups(name).Any(group => group.name == name)) continue;
                object child = createGroup.Invoke(mixer, new object[] { name, false });
                addChild.Invoke(mixer, new[] { child, master });
            }

            EditorUtility.SetDirty(mixer);
            return mixer;
        }

        private static Dictionary<string, AudioMixerGroup> RequiredGroups(AudioMixer mixer)
        {
            return new[] { "Master", "Music", "Ambience", "Effects", "Voice" }
                .ToDictionary(name => name, name => mixer.FindMatchingGroups(name).Single(group => group.name == name));
        }

        private static AudioCueDefinition EnsureCue(CueSeed seed)
        {
            string esPath = ClipRoot + "/" + seed.FileStem + "_es.wav";
            string enPath = seed.Category == AudioCueCategory.Music || seed.Category == AudioCueCategory.Ambience || seed.Category == AudioCueCategory.Feedback
                ? esPath
                : ClipRoot + "/" + seed.FileStem + "_en.wav";
            WriteToneWav(esPath, seed.SpanishFrequency, seed.Duration, seed.Loop);
            if (enPath != esPath) WriteToneWav(enPath, seed.EnglishFrequency, seed.Duration, seed.Loop);
            AssetDatabase.ImportAsset(esPath, ImportAssetOptions.ForceUpdate);
            if (enPath != esPath) AssetDatabase.ImportAsset(enPath, ImportAssetOptions.ForceUpdate);
            ConfigureImporter(esPath);
            if (enPath != esPath) ConfigureImporter(enPath);

            string assetPath = CueRoot + "/" + seed.FileStem + ".asset";
            AudioCueDefinition cue = AssetDatabase.LoadAssetAtPath<AudioCueDefinition>(assetPath);
            if (cue == null)
            {
                cue = ScriptableObject.CreateInstance<AudioCueDefinition>();
                AssetDatabase.CreateAsset(cue, assetPath);
            }

            var serialized = new SerializedObject(cue);
            Set(serialized, "_cueId", seed.Id);
            serialized.FindProperty("_category").intValue = (int)seed.Category;
            serialized.FindProperty("_bus").intValue = (int)seed.Bus;
            serialized.FindProperty("_priority").intValue = (int)seed.Priority;
            serialized.FindProperty("_cooldownSeconds").floatValue = seed.Cooldown;
            serialized.FindProperty("_gain").floatValue = seed.Gain;
            serialized.FindProperty("_loop").boolValue = seed.Loop;
            Set(serialized, "_subtitleTable", seed.SubtitleKey == null ? string.Empty : LocalizationKeys.ContentTable);
            Set(serialized, "_subtitleKey", seed.SubtitleKey ?? string.Empty);
            serialized.FindProperty("_spanishClip").objectReferenceValue = AssetDatabase.LoadAssetAtPath<AudioClip>(esPath);
            serialized.FindProperty("_englishClip").objectReferenceValue = AssetDatabase.LoadAssetAtPath<AudioClip>(enPath);
            Set(serialized, "_spanishAddress", Address(seed.Id, "es", enPath == esPath));
            Set(serialized, "_englishAddress", Address(seed.Id, "en", enPath == esPath));
            serialized.FindProperty("_placeholder").boolValue = true;
            Set(serialized, "_placeholderId", seed.PlaceholderId);
            Set(serialized, "_releaseState", "ReleaseBlocked");
            serialized.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(cue);
            return cue;
        }

        private static AudioCueCatalogAsset EnsureCatalog(
            AudioMixer mixer,
            IReadOnlyDictionary<string, AudioMixerGroup> groups,
            IReadOnlyList<AudioCueDefinition> cues)
        {
            AudioCueCatalogAsset catalog = AssetDatabase.LoadAssetAtPath<AudioCueCatalogAsset>(CatalogPath);
            if (catalog == null)
            {
                catalog = ScriptableObject.CreateInstance<AudioCueCatalogAsset>();
                AssetDatabase.CreateAsset(catalog, CatalogPath);
            }

            var serialized = new SerializedObject(catalog);
            serialized.FindProperty("_mixer").objectReferenceValue = mixer;
            serialized.FindProperty("_master").objectReferenceValue = groups["Master"];
            serialized.FindProperty("_music").objectReferenceValue = groups["Music"];
            serialized.FindProperty("_ambience").objectReferenceValue = groups["Ambience"];
            serialized.FindProperty("_effects").objectReferenceValue = groups["Effects"];
            serialized.FindProperty("_voice").objectReferenceValue = groups["Voice"];
            SerializedProperty cueArray = serialized.FindProperty("_cues");
            cueArray.arraySize = cues.Count;
            for (int index = 0; index < cues.Count; index++) cueArray.GetArrayElementAtIndex(index).objectReferenceValue = cues[index];
            serialized.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(catalog);
            return catalog;
        }

        private static void ConfigureAddressables(IEnumerable<AudioCueDefinition> cues)
        {
            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings ??
                throw new InvalidOperationException("Addressables settings must exist before audio setup.");
            settings.AddLabel(AudioLabel);
            settings.AddLabel(PlaceholderLabel);
            var seen = new HashSet<string>(StringComparer.Ordinal);
            foreach (AudioCueDefinition cue in cues)
            {
                AddClip(cue.SpanishClip, cue.SpanishAddress);
                AddClip(cue.EnglishClip, cue.EnglishAddress);
            }

            void AddClip(AudioClip clip, string address)
            {
                string path = AssetDatabase.GetAssetPath(clip);
                if (!seen.Add(path)) return;
                AddressableAssetEntry entry = settings.CreateOrMoveEntry(AssetDatabase.AssetPathToGUID(path), settings.FindGroup(SceneFlowFoundationSetup.SharedGroupName));
                entry.address = address;
                entry.SetLabel(AudioLabel, true, true, false);
                entry.SetLabel(PlaceholderLabel, true, true, false);
            }

            EditorUtility.SetDirty(settings);
        }

        private static void ConfigureBootstrapScene(AudioCueCatalogAsset catalog)
        {
            Scene scene = EditorSceneManager.OpenScene(ProjectFoundationSetup.BootstrapScenePath, OpenSceneMode.Single);
            DiagnosticBootstrap bootstrap = UnityEngine.Object.FindFirstObjectByType<DiagnosticBootstrap>();
            GameObject old = GameObject.Find("PH_UI_AUDIO_DIAGNOSTIC");
            if (old != null) UnityEngine.Object.DestroyImmediate(old);

            var canvasObject = new GameObject("PH_UI_AUDIO_DIAGNOSTIC", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            Canvas canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 120;
            canvasObject.GetComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasObject.GetComponent<CanvasScaler>().referenceResolution = new Vector2(1920f, 1080f);
            AudioDiagnosticView view = canvasObject.AddComponent<AudioDiagnosticView>();
            GameObject panel = new GameObject("Development Audio Panel", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            panel.transform.SetParent(canvasObject.transform, false);
            panel.GetComponent<Image>().color = new Color(0.03f, 0.07f, 0.09f, 0.92f);
            SetRect((RectTransform)panel.transform, new Vector2(0.01f, 0.02f), new Vector2(0.31f, 0.40f));
            Text subtitle = CreateText(canvasObject.transform, "Audio Subtitle", 34);
            SetRect(subtitle.rectTransform, new Vector2(0.20f, 0.78f), new Vector2(0.80f, 0.90f));
            Button play = CreateButton(panel.transform, "Play Instruction", "INSTRUCCIÓN");
            Button replay = CreateButton(panel.transform, "Replay Instruction", "REPETIR");
            Button feedback = CreateButton(panel.transform, "Play Feedback", "FEEDBACK");
            SetRect((RectTransform)play.transform, new Vector2(0.02f, 0.78f), new Vector2(0.32f, 0.98f));
            SetRect((RectTransform)replay.transform, new Vector2(0.35f, 0.78f), new Vector2(0.65f, 0.98f));
            SetRect((RectTransform)feedback.transform, new Vector2(0.68f, 0.78f), new Vector2(0.98f, 0.98f));
            Toggle subtitles = DefaultControls.CreateToggle(new DefaultControls.Resources()).GetComponent<Toggle>();
            subtitles.name = "Subtitles";
            subtitles.transform.SetParent(panel.transform, false);
            SetRect((RectTransform)subtitles.transform, new Vector2(0.05f, 0.64f), new Vector2(0.95f, 0.76f));
            Slider[] sliders = { CreateSlider(panel.transform, "Master", .52f), CreateSlider(panel.transform, "Music", .41f), CreateSlider(panel.transform, "Ambience", .30f), CreateSlider(panel.transform, "Effects", .19f), CreateSlider(panel.transform, "Voice", .08f) };
            foreach (Text text in panel.GetComponentsInChildren<Text>(true)) text.text = string.Empty;

            var viewSerialized = new SerializedObject(view);
            viewSerialized.FindProperty("_developmentPanel").objectReferenceValue = panel;
            viewSerialized.FindProperty("_subtitleText").objectReferenceValue = subtitle;
            viewSerialized.FindProperty("_playInstructionButton").objectReferenceValue = play;
            viewSerialized.FindProperty("_replayButton").objectReferenceValue = replay;
            viewSerialized.FindProperty("_feedbackButton").objectReferenceValue = feedback;
            viewSerialized.FindProperty("_subtitlesToggle").objectReferenceValue = subtitles;
            string[] fields = { "_masterSlider", "_musicSlider", "_ambienceSlider", "_effectsSlider", "_voiceSlider" };
            for (int index = 0; index < fields.Length; index++) viewSerialized.FindProperty(fields[index]).objectReferenceValue = sliders[index];
            viewSerialized.ApplyModifiedPropertiesWithoutUndo();

            var bootstrapSerialized = new SerializedObject(bootstrap);
            bootstrapSerialized.FindProperty("_audioView").objectReferenceValue = view;
            bootstrapSerialized.FindProperty("_audioCatalog").objectReferenceValue = catalog;
            bootstrapSerialized.ApplyModifiedPropertiesWithoutUndo();
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
        }

        private static Slider CreateSlider(Transform parent, string name, float anchorY)
        {
            Slider slider = DefaultControls.CreateSlider(new DefaultControls.Resources()).GetComponent<Slider>();
            slider.name = name;
            slider.transform.SetParent(parent, false);
            slider.minValue = 0f;
            slider.maxValue = 1f;
            SetRect((RectTransform)slider.transform, new Vector2(0.12f, anchorY), new Vector2(0.95f, anchorY + .08f));
            return slider;
        }

        private static Button CreateButton(Transform parent, string name, string label)
        {
            Button button = DefaultControls.CreateButton(new DefaultControls.Resources()).GetComponent<Button>();
            button.name = name;
            button.transform.SetParent(parent, false);
            Text text = button.GetComponentInChildren<Text>();
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.text = label;
            text.resizeTextForBestFit = true;
            return button;
        }

        private static Text CreateText(Transform parent, string name, int size)
        {
            var gameObject = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
            gameObject.transform.SetParent(parent, false);
            Text text = gameObject.GetComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = size;
            text.alignment = TextAnchor.MiddleCenter;
            text.color = Color.white;
            text.text = string.Empty;
            return text;
        }

        private static void SetRect(RectTransform rect, Vector2 min, Vector2 max)
        {
            rect.anchorMin = min;
            rect.anchorMax = max;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        private static void ConfigureImporter(string path)
        {
            var importer = AssetImporter.GetAtPath(path) as AudioImporter;
            if (importer == null) throw new InvalidOperationException("Audio importer unavailable: " + path);
            importer.forceToMono = true;
            importer.loadInBackground = false;
            importer.defaultSampleSettings = new AudioImporterSampleSettings
            {
                loadType = AudioClipLoadType.DecompressOnLoad,
                compressionFormat = AudioCompressionFormat.PCM,
                sampleRateSetting = AudioSampleRateSetting.PreserveSampleRate,
                preloadAudioData = true,
                quality = 1f
            };
            importer.SaveAndReimport();
        }

        private static void WriteToneWav(string assetPath, float frequency, float duration, bool loop)
        {
            const int sampleRate = 48000;
            int sampleCount = Mathf.RoundToInt(duration * sampleRate);
            string fullPath = Path.GetFullPath(assetPath);
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
            using var stream = File.Create(fullPath);
            using var writer = new BinaryWriter(stream);
            writer.Write(new[] { 'R', 'I', 'F', 'F' });
            writer.Write(36 + sampleCount * 2);
            writer.Write(new[] { 'W', 'A', 'V', 'E', 'f', 'm', 't', ' ' });
            writer.Write(16);
            writer.Write((short)1);
            writer.Write((short)1);
            writer.Write(sampleRate);
            writer.Write(sampleRate * 2);
            writer.Write((short)2);
            writer.Write((short)16);
            writer.Write(new[] { 'd', 'a', 't', 'a' });
            writer.Write(sampleCount * 2);
            for (int index = 0; index < sampleCount; index++)
            {
                double phase = 2d * Math.PI * frequency * index / sampleRate;
                double envelope = loop ? 1d : Math.Sin(Math.PI * index / Math.Max(1, sampleCount - 1));
                double modulation = loop ? 0.75d + 0.25d * Math.Sin(2d * Math.PI * index / sampleCount) : 1d;
                writer.Write((short)(Math.Sin(phase) * envelope * modulation * 1500d));
            }
        }

        private static string Address(string id, string locale, bool shared) =>
            "audio/" + id.Substring("audio.".Length).Replace('.', '/') + (shared ? string.Empty : "/" + locale);

        private static void Set(SerializedObject serialized, string name, string value) => serialized.FindProperty(name).stringValue = value;

        private static void EnsureFolder(string path)
        {
            string[] segments = path.Split('/');
            string current = segments[0];
            for (int index = 1; index < segments.Length; index++)
            {
                string next = current + "/" + segments[index];
                if (!AssetDatabase.IsValidFolder(next)) AssetDatabase.CreateFolder(current, segments[index]);
                current = next;
            }
        }

        private sealed class CueSeed
        {
            public CueSeed(string id, AudioCueCategory category, AudioBus bus, AudioPriority priority, bool loop, float cooldown, float gain, string subtitleKey, string placeholderId, string fileStem, float spanishFrequency, float englishFrequency, float duration)
            {
                Id = id; Category = category; Bus = bus; Priority = priority; Loop = loop; Cooldown = cooldown; Gain = gain;
                SubtitleKey = subtitleKey; PlaceholderId = placeholderId; FileStem = fileStem; SpanishFrequency = spanishFrequency;
                EnglishFrequency = englishFrequency; Duration = duration;
            }

            public string Id { get; }
            public AudioCueCategory Category { get; }
            public AudioBus Bus { get; }
            public AudioPriority Priority { get; }
            public bool Loop { get; }
            public float Cooldown { get; }
            public float Gain { get; }
            public string SubtitleKey { get; }
            public string PlaceholderId { get; }
            public string FileStem { get; }
            public float SpanishFrequency { get; }
            public float EnglishFrequency { get; }
            public float Duration { get; }
        }
    }
}
