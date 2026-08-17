using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using PequenoExplorador.Application.Content;
using PequenoExplorador.Content.Visuals;
using PequenoExplorador.Editor.BuildTools;
using PequenoExplorador.Presentation.Interaction;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Profiling;

namespace PequenoExplorador.Editor
{
    public static class ToucanFixtureSetup
    {
        public const string GeneratorVersion = "1.0.0";
        public const string GeneratedDate = "2026-08-16";
        public const string VisualId = "visual.discovery.jungle.keel-billed-toucan";
        public const string FutureDiscoveryId = "discovery.jungle.keel-billed-toucan";
        public const string FutureInteractionId = "interaction.jungle.keel-billed-toucan";
        public const string Root = "Assets/_Game/Content/Discoveries/Jungle/KeelBilledToucan";
        public const string MaterialsRoot = Root + "/Materials";
        public const string PrefabPath = Root + "/VS_ToucanPicoCanoa.prefab";
        public const string ProvenancePath = Root + "/VS_ToucanPicoCanoa.provenance.json";

        private static readonly MaterialSpec[] MaterialSpecs =
        {
            new MaterialSpec("Toucan_Dark", new Color(0.035f, 0.055f, 0.07f)),
            new MaterialSpec("Toucan_Yellow", new Color(1f, 0.73f, 0.10f)),
            new MaterialSpec("Toucan_BillGreen", new Color(0.29f, 0.73f, 0.34f)),
            new MaterialSpec("Toucan_BillOrange", new Color(1f, 0.43f, 0.08f)),
            new MaterialSpec("Toucan_BillRed", new Color(0.88f, 0.10f, 0.12f)),
            new MaterialSpec("Toucan_BillBlue", new Color(0.08f, 0.48f, 0.83f)),
            new MaterialSpec("Toucan_EyeWhite", new Color(0.96f, 0.96f, 0.88f))
        };

        [MenuItem("Pequeño Explorador/Development/Content/Build Toucan Review Fixture")]
        public static void Apply()
        {
            try
            {
                ApplyAssetsAndScene(true);
                Debug.Log("PE_TOUCAN_FIXTURE_OK visual=Sourced placeholder=false materials=7 externalMedia=0");
                if (UnityEngine.Application.isBatchMode) EditorApplication.Exit(0);
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
                if (UnityEngine.Application.isBatchMode) EditorApplication.Exit(2);
                throw;
            }
        }

        public static ToucanFixtureMetrics ApplyAssetsAndScene(bool writeRenders)
        {
            EnsureFolder(MaterialsRoot);
            Dictionary<string, Material> materials = MaterialSpecs.ToDictionary(
                item => item.Name,
                item => EnsureMaterial(item));
            BuildPrefab(materials);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            IntegrateExistingJungle();
            ToucanFixtureMetrics metrics = MeasurePrefab();
            WriteProvenance(metrics);
            WriteMetrics(metrics);
            if (writeRenders) RenderReviewImages();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            return metrics;
        }

        private static void IntegrateExistingJungle()
        {
            Scene scene = SceneManager.GetSceneByPath(SceneFlowFoundationSetup.JungleScenePath);
            bool opened = !scene.IsValid() || !scene.isLoaded;
            if (opened) scene = EditorSceneManager.OpenScene(SceneFlowFoundationSetup.JungleScenePath, OpenSceneMode.Additive);
            try
            {
                WorldInteractableView animal = scene.GetRootGameObjects()
                    .SelectMany(item => item.GetComponentsInChildren<WorldInteractableView>(true))
                    .Single(item => item.RawInteractionId == "interaction.fixture.animal");
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);
                ToucanReviewFixtureMetadata metadata = animal.GetComponentInChildren<ToucanReviewFixtureMetadata>(true);
                if (metadata == null || PrefabUtility.GetCorrespondingObjectFromSource(metadata.gameObject) != prefab)
                {
                    Transform legacy = animal.transform.Find("PH_FIXTURE_ANIMAL_VISUAL");
                    if (legacy != null) UnityEngine.Object.DestroyImmediate(legacy.gameObject);
                    if (metadata != null) UnityEngine.Object.DestroyImmediate(metadata.gameObject);
                    GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, animal.transform);
                    instance.transform.localPosition = Vector3.zero;
                    instance.transform.localRotation = Quaternion.identity;
                    instance.transform.localScale = Vector3.one;
                    metadata = instance.GetComponent<ToucanReviewFixtureMetadata>();
                }

                Transform oldPoint = animal.transform.Find("PH_INTERACTION_POINT");
                if (oldPoint != null) UnityEngine.Object.DestroyImmediate(oldPoint.gameObject);
                var serialized = new SerializedObject(animal);
                serialized.FindProperty("_interactionPoint").objectReferenceValue = metadata.InteractionPoint;
                SerializedProperty colliders = serialized.FindProperty("_targetColliders");
                colliders.arraySize = 1;
                colliders.GetArrayElementAtIndex(0).objectReferenceValue = metadata.TouchCollider;
                serialized.ApplyModifiedPropertiesWithoutUndo();
                EditorUtility.SetDirty(animal);
                EditorSceneManager.MarkSceneDirty(scene);
                EditorSceneManager.SaveScene(scene);
            }
            finally
            {
                if (opened) EditorSceneManager.CloseScene(scene, true);
            }
        }

        public static ToucanFixtureMetrics MeasurePrefab()
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);
            if (prefab == null) throw new InvalidOperationException("Toucan prefab is missing: " + PrefabPath);
            Renderer[] renderers = prefab.GetComponentsInChildren<Renderer>(true);
            MeshFilter[] filters = prefab.GetComponentsInChildren<MeshFilter>(true);
            int vertices = filters.Where(item => item.sharedMesh != null).Sum(item => item.sharedMesh.vertexCount);
            int triangles = filters.Where(item => item.sharedMesh != null)
                .Sum(item => (int)(item.sharedMesh.GetIndexCount(0) / 3));
            Material[] materials = renderers.SelectMany(item => item.sharedMaterials).Where(item => item != null)
                .Distinct().ToArray();
            Bounds bounds = CombinedLocalBounds(prefab.transform, renderers);
            long memory = filters.Where(item => item.sharedMesh != null).Select(item => item.sharedMesh).Distinct()
                              .Sum(item => Profiler.GetRuntimeMemorySizeLong(item)) +
                          materials.Sum(item => Profiler.GetRuntimeMemorySizeLong(item));
            return new ToucanFixtureMetrics(
                filters.Length,
                vertices,
                triangles,
                materials.Length,
                renderers.Length,
                bounds,
                memory);
        }

        private static void BuildPrefab(IReadOnlyDictionary<string, Material> materials)
        {
            var root = new GameObject("VS_ToucanPicoCanoa");
            try
            {
                var visualRoot = new GameObject("VisualRoot");
                visualRoot.transform.SetParent(root.transform, false);

                AddPart(visualRoot.transform, "Body", PrimitiveType.Sphere,
                    new Vector3(0f, 1.18f, 0f), new Vector3(1.15f, 1.48f, 0.94f), Vector3.zero,
                    materials["Toucan_Dark"]);
                AddPart(visualRoot.transform, "Head", PrimitiveType.Sphere,
                    new Vector3(0.31f, 2.08f, 0f), new Vector3(0.83f, 0.86f, 0.78f), Vector3.zero,
                    materials["Toucan_Dark"]);
                AddPart(visualRoot.transform, "Throat", PrimitiveType.Sphere,
                    new Vector3(0.48f, 1.85f, 0f), new Vector3(0.74f, 0.88f, 0.82f), Vector3.zero,
                    materials["Toucan_Yellow"]);
                AddPart(visualRoot.transform, "WingNear", PrimitiveType.Sphere,
                    new Vector3(-0.18f, 1.23f, -0.46f), new Vector3(0.86f, 1.07f, 0.18f),
                    new Vector3(0f, 0f, -14f), materials["Toucan_Dark"]);
                AddPart(visualRoot.transform, "WingFar", PrimitiveType.Sphere,
                    new Vector3(-0.18f, 1.23f, 0.46f), new Vector3(0.86f, 1.07f, 0.18f),
                    new Vector3(0f, 0f, 14f), materials["Toucan_Dark"]);
                AddPart(visualRoot.transform, "Tail", PrimitiveType.Cube,
                    new Vector3(-0.72f, 0.66f, 0f), new Vector3(0.30f, 1.05f, 0.54f),
                    new Vector3(0f, 0f, 21f), materials["Toucan_Dark"]);

                AddPart(visualRoot.transform, "BillGreen", PrimitiveType.Cube,
                    new Vector3(1.11f, 2.22f, 0f), new Vector3(1.48f, 0.48f, 0.57f),
                    new Vector3(0f, 0f, 4f), materials["Toucan_BillGreen"]);
                AddPart(visualRoot.transform, "BillOrange", PrimitiveType.Cube,
                    new Vector3(1.12f, 2.09f, 0f), new Vector3(1.42f, 0.18f, 0.59f),
                    new Vector3(0f, 0f, 1f), materials["Toucan_BillOrange"]);
                AddPart(visualRoot.transform, "BillRedTip", PrimitiveType.Cube,
                    new Vector3(1.82f, 2.18f, 0f), new Vector3(0.18f, 0.43f, 0.60f),
                    new Vector3(0f, 0f, 4f), materials["Toucan_BillRed"]);
                AddPart(visualRoot.transform, "BillBlueMark", PrimitiveType.Cube,
                    new Vector3(0.65f, 2.27f, -0.30f), new Vector3(0.32f, 0.19f, 0.025f),
                    new Vector3(0f, 0f, 4f), materials["Toucan_BillBlue"]);

                AddEye(visualRoot.transform, "EyeNear", -0.39f, materials);
                AddEye(visualRoot.transform, "EyeFar", 0.39f, materials);
                AddPart(visualRoot.transform, "FootNear", PrimitiveType.Cylinder,
                    new Vector3(0.13f, 0.35f, -0.27f), new Vector3(0.10f, 0.26f, 0.10f),
                    new Vector3(0f, 0f, 8f), materials["Toucan_BillOrange"]);
                AddPart(visualRoot.transform, "FootFar", PrimitiveType.Cylinder,
                    new Vector3(0.13f, 0.35f, 0.27f), new Vector3(0.10f, 0.26f, 0.10f),
                    new Vector3(0f, 0f, -8f), materials["Toucan_BillOrange"]);

                Renderer[] renderers = root.GetComponentsInChildren<Renderer>(true);
                Bounds bounds = CombinedLocalBounds(root.transform, renderers);
                BoxCollider collider = root.AddComponent<BoxCollider>();
                collider.isTrigger = true;
                collider.center = bounds.center;
                collider.size = bounds.size + new Vector3(0.30f, 0.20f, 0.30f);

                var point = new GameObject("VS_InteractionPoint");
                point.transform.SetParent(root.transform, false);
                point.transform.localPosition = new Vector3(2.35f, 0f, 0f);
                var photoAnchor = new GameObject("VS_PhotoAnchor");
                photoAnchor.transform.SetParent(root.transform, false);
                photoAnchor.transform.localPosition = bounds.center;

                ToucanReviewFixtureMetadata metadata = root.AddComponent<ToucanReviewFixtureMetadata>();
                var serialized = new SerializedObject(metadata);
                serialized.FindProperty("_visualId").stringValue = VisualId;
                serialized.FindProperty("_futureDiscoveryId").stringValue = FutureDiscoveryId;
                serialized.FindProperty("_futureInteractionId").stringValue = FutureInteractionId;
                serialized.FindProperty("_author").stringValue = "Ismael Bojórquez";
                serialized.FindProperty("_sourceType").stringValue = "Creación propia mediante primitives y tooling Unity";
                serialized.FindProperty("_licenseDeclaration").stringValue =
                    "Propia — pendiente de confirmación asset-specific";
                serialized.FindProperty("_generatorVersion").stringValue = GeneratorVersion;
                serialized.FindProperty("_generatedDate").stringValue = GeneratedDate;
                serialized.FindProperty("_editorialState").enumValueIndex = (int)EditorialState.Sourced;
                serialized.FindProperty("_isPlaceholder").boolValue = false;
                serialized.FindProperty("_factualReviewState").stringValue = "PENDING_SPECIALIST_SIGNOFF";
                serialized.FindProperty("_candidatePhotoBounds").boundsValue = bounds;
                serialized.FindProperty("_visualRoot").objectReferenceValue = visualRoot.transform;
                serialized.FindProperty("_interactionPoint").objectReferenceValue = point.transform;
                serialized.FindProperty("_touchCollider").objectReferenceValue = collider;
                serialized.ApplyModifiedPropertiesWithoutUndo();

                PrefabUtility.SaveAsPrefabAsset(root, PrefabPath);
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(root);
            }
        }

        private static void AddEye(
            Transform parent,
            string name,
            float z,
            IReadOnlyDictionary<string, Material> materials)
        {
            AddPart(parent, name + "Patch", PrimitiveType.Sphere,
                new Vector3(0.57f, 2.28f, z), new Vector3(0.34f, 0.36f, 0.08f), Vector3.zero,
                materials["Toucan_EyeWhite"]);
            AddPart(parent, name + "Pupil", PrimitiveType.Sphere,
                new Vector3(0.67f, 2.30f, z + (z < 0f ? -0.045f : 0.045f)),
                new Vector3(0.12f, 0.13f, 0.055f), Vector3.zero, materials["Toucan_Dark"]);
        }

        private static void AddPart(
            Transform parent,
            string name,
            PrimitiveType primitive,
            Vector3 position,
            Vector3 scale,
            Vector3 rotation,
            Material material)
        {
            GameObject part = GameObject.CreatePrimitive(primitive);
            part.name = name;
            part.transform.SetParent(parent, false);
            part.transform.localPosition = position;
            part.transform.localScale = scale;
            part.transform.localEulerAngles = rotation;
            UnityEngine.Object.DestroyImmediate(part.GetComponent<Collider>());
            Renderer renderer = part.GetComponent<Renderer>();
            renderer.sharedMaterial = material;
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
        }

        private static Material EnsureMaterial(MaterialSpec spec)
        {
            string path = MaterialsRoot + "/" + spec.Name + ".mat";
            Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (material == null)
            {
                Shader shader = Shader.Find("Universal Render Pipeline/Lit");
                if (shader == null) throw new InvalidOperationException("URP Lit shader is unavailable.");
                material = new Material(shader) { name = spec.Name, enableInstancing = true };
                AssetDatabase.CreateAsset(material, path);
            }
            material.color = spec.Color;
            material.enableInstancing = true;
            EditorUtility.SetDirty(material);
            return material;
        }

        private static Bounds CombinedLocalBounds(Transform root, IReadOnlyList<Renderer> renderers)
        {
            if (renderers.Count == 0) return new Bounds(Vector3.zero, Vector3.zero);
            Bounds bounds = new Bounds(root.InverseTransformPoint(renderers[0].bounds.center), Vector3.zero);
            foreach (Renderer renderer in renderers)
            {
                Vector3 min = root.InverseTransformPoint(renderer.bounds.min);
                Vector3 max = root.InverseTransformPoint(renderer.bounds.max);
                bounds.Encapsulate(min);
                bounds.Encapsulate(max);
            }
            return bounds;
        }

        private static void WriteProvenance(ToucanFixtureMetrics metrics)
        {
            var ledger = new ToucanProvenance
            {
                assetId = VisualId,
                author = "Ismael Bojórquez",
                sourceType = "Creación propia mediante primitives y tooling Unity",
                generator = "PequenoExplorador.Editor.ToucanFixtureSetup",
                generatorVersion = GeneratorVersion,
                generatedDate = GeneratedDate,
                sourceComponents = "Unity built-in Sphere/Cube/Cylinder meshes; project-authored URP materials",
                runtimePrefab = PrefabPath,
                runtimeMaterials = MaterialSpecs.Select(item => MaterialsRoot + "/" + item.Name + ".mat").ToArray(),
                licenseDeclaration = "Propia — pendiente de confirmación asset-specific",
                externalMedia = false,
                editorialState = EditorialState.Sourced.ToString(),
                factualReview = "PENDING_SPECIALIST_SIGNOFF",
                legalNotice = "Registro técnico de procedencia; no es un dictamen legal definitivo.",
                files = RelevantAssetPaths().Select(path => new ProvenanceFile
                {
                    path = path,
                    sha256 = File.Exists(path) ? Sha256(path) : "NOT_AVAILABLE"
                }).ToArray(),
                metrics = metrics.ToRecord()
            };
            File.WriteAllText(ProvenancePath, JsonUtility.ToJson(ledger, true) + "\n", new UTF8Encoding(false));
            AssetDatabase.ImportAsset(ProvenancePath, ImportAssetOptions.ForceUpdate);
        }

        private static void WriteMetrics(ToucanFixtureMetrics metrics)
        {
            string directory = BuildArtifactPaths.RequireInsideArtifacts(
                Path.Combine(BuildArtifactPaths.ArtifactsRoot, "reports"));
            Directory.CreateDirectory(directory);
            File.WriteAllText(Path.Combine(directory, "toucan-fixture.json"),
                JsonUtility.ToJson(metrics.ToRecord(), true) + "\n", new UTF8Encoding(false));
            string markdown = string.Format(CultureInfo.InvariantCulture,
                "# Toucan fixture metrics\n\n- Mesh instances: {0}\n- Vertices: {1}\n- Triangles: {2}\n- Shared materials: {3}\n- Renderers: {4}\n- Bounds center: {5}\n- Bounds size: {6}\n- Approx. Unity runtime mesh/material memory: {7} bytes\n",
                metrics.Meshes, metrics.Vertices, metrics.Triangles, metrics.Materials, metrics.Renderers,
                metrics.Bounds.center, metrics.Bounds.size, metrics.ApproximateRuntimeBytes);
            File.WriteAllText(Path.Combine(directory, "toucan-fixture.md"), markdown, new UTF8Encoding(false));
        }

        private static IEnumerable<string> RelevantAssetPaths()
        {
            yield return PrefabPath;
            foreach (MaterialSpec item in MaterialSpecs) yield return MaterialsRoot + "/" + item.Name + ".mat";
            yield return "Assets/_Game/Editor/ToucanFixtureSetup.cs";
        }

        private static string Sha256(string path)
        {
            using (SHA256 algorithm = SHA256.Create())
            using (FileStream stream = File.OpenRead(path))
                return BitConverter.ToString(algorithm.ComputeHash(stream)).Replace("-", string.Empty).ToLowerInvariant();
        }

        private static void RenderReviewImages()
        {
            string directory = BuildArtifactPaths.RequireInsideArtifacts(
                Path.Combine(BuildArtifactPaths.ArtifactsRoot, "review", "toucan"));
            Directory.CreateDirectory(directory);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);
            var stage = new GameObject("ToucanReviewStage");
            GameObject instance = null;
            try
            {
                instance = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
                if (instance == null) throw new InvalidOperationException("Could not instantiate toucan review prefab.");
                instance.transform.SetParent(stage.transform, false);
                AddReviewLight(stage.transform, "Key", new Vector3(35f, -35f, 0f), 1.2f);
                AddReviewLight(stage.transform, "Fill", new Vector3(20f, 145f, 0f), 0.65f);
                Render(instance, Path.Combine(directory, "front-16x9.png"), 1280, 720,
                    new Vector3(3.3f, 2.25f, -5.5f), new Color(0.70f, 0.88f, 0.88f));
                Render(instance, Path.Combine(directory, "side-16x9.png"), 1280, 720,
                    new Vector3(0.2f, 2.0f, -6.2f), new Color(0.70f, 0.88f, 0.88f));
                Render(instance, Path.Combine(directory, "three-quarter-16x9.png"), 1280, 720,
                    new Vector3(4.4f, 2.6f, 4.4f), new Color(0.70f, 0.88f, 0.88f));
                RenderSilhouette(instance, Path.Combine(directory, "silhouette-light-16x9.png"), 1280, 720,
                    new Vector3(0.2f, 2.0f, -6.2f), Color.black, new Color(0.93f, 0.91f, 0.82f));
                RenderSilhouette(instance, Path.Combine(directory, "silhouette-dark-20x9.png"), 1600, 720,
                    new Vector3(4.4f, 2.6f, -4.4f), Color.white, new Color(0.025f, 0.04f, 0.07f));
            }
            finally
            {
                if (instance != null) UnityEngine.Object.DestroyImmediate(instance);
                UnityEngine.Object.DestroyImmediate(stage);
            }
            RenderJungleReview(Path.Combine(directory, "jungle-20x9.png"));
        }

        private static void RenderJungleReview(string path)
        {
            Scene scene = SceneManager.GetSceneByPath(SceneFlowFoundationSetup.JungleScenePath);
            bool opened = !scene.IsValid() || !scene.isLoaded;
            if (opened) scene = EditorSceneManager.OpenScene(SceneFlowFoundationSetup.JungleScenePath, OpenSceneMode.Additive);
            GameObject lighting = null;
            try
            {
                ToucanReviewFixtureMetadata metadata = scene.GetRootGameObjects()
                    .SelectMany(item => item.GetComponentsInChildren<ToucanReviewFixtureMetadata>(true))
                    .Single();
                lighting = new GameObject("ToucanJungleReviewLighting") { hideFlags = HideFlags.HideAndDontSave };
                AddReviewLight(lighting.transform, "ReviewKey", new Vector3(35f, -35f, 0f), 0.8f);
                Vector3 cameraPosition = metadata.transform.position + new Vector3(4.8f, 2.4f, -4.8f);
                Render(metadata.gameObject, path, 1600, 720, cameraPosition, new Color(0.10f, 0.29f, 0.18f));
            }
            finally
            {
                if (lighting != null) UnityEngine.Object.DestroyImmediate(lighting);
                if (opened) EditorSceneManager.CloseScene(scene, true);
            }
        }

        private static void RenderSilhouette(
            GameObject subject,
            string path,
            int width,
            int height,
            Vector3 cameraPosition,
            Color silhouette,
            Color background)
        {
            Renderer[] renderers = subject.GetComponentsInChildren<Renderer>(true);
            Material[][] originals = renderers.Select(item => item.sharedMaterials).ToArray();
            Shader shader = Shader.Find("Universal Render Pipeline/Unlit");
            var material = new Material(shader) { color = silhouette };
            try
            {
                foreach (Renderer renderer in renderers) renderer.sharedMaterial = material;
                Render(subject, path, width, height, cameraPosition, background);
            }
            finally
            {
                for (int index = 0; index < renderers.Length; index++) renderers[index].sharedMaterials = originals[index];
                UnityEngine.Object.DestroyImmediate(material);
            }
        }

        private static void Render(
            GameObject subject,
            string path,
            int width,
            int height,
            Vector3 cameraPosition,
            Color background)
        {
            var cameraObject = new GameObject("ToucanReviewCamera");
            var target = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32);
            var texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
            RenderTexture previous = RenderTexture.active;
            try
            {
                Camera camera = cameraObject.AddComponent<Camera>();
                camera.transform.position = cameraPosition;
                camera.transform.LookAt(subject.transform.position + new Vector3(0.45f, 1.45f, 0f));
                camera.clearFlags = CameraClearFlags.SolidColor;
                camera.backgroundColor = background;
                camera.fieldOfView = 31f;
                camera.nearClipPlane = 0.1f;
                camera.farClipPlane = 30f;
                camera.targetTexture = target;
                // Warm up the URP material variants before reading the deterministic review frame.
                camera.Render();
                camera.Render();
                RenderTexture.active = target;
                texture.ReadPixels(new Rect(0f, 0f, width, height), 0, 0);
                texture.Apply(false, false);
                File.WriteAllBytes(path, texture.EncodeToPNG());
            }
            finally
            {
                RenderTexture.active = previous;
                target.Release();
                UnityEngine.Object.DestroyImmediate(target);
                UnityEngine.Object.DestroyImmediate(texture);
                UnityEngine.Object.DestroyImmediate(cameraObject);
            }
        }

        private static void AddReviewLight(Transform parent, string name, Vector3 euler, float intensity)
        {
            var lightObject = new GameObject(name);
            lightObject.transform.SetParent(parent, false);
            lightObject.transform.eulerAngles = euler;
            Light light = lightObject.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = intensity;
        }

        private static void EnsureFolder(string path)
        {
            string[] parts = path.Split('/');
            string current = parts[0];
            for (int index = 1; index < parts.Length; index++)
            {
                string next = current + "/" + parts[index];
                if (!AssetDatabase.IsValidFolder(next)) AssetDatabase.CreateFolder(current, parts[index]);
                current = next;
            }
        }

        private readonly struct MaterialSpec
        {
            public MaterialSpec(string name, Color color)
            {
                Name = name;
                Color = color;
            }

            public string Name { get; }
            public Color Color { get; }
        }

        [Serializable]
        private sealed class ToucanProvenance
        {
            public string assetId;
            public string author;
            public string sourceType;
            public string generator;
            public string generatorVersion;
            public string generatedDate;
            public string sourceComponents;
            public string runtimePrefab;
            public string[] runtimeMaterials;
            public string licenseDeclaration;
            public bool externalMedia;
            public string editorialState;
            public string factualReview;
            public string legalNotice;
            public ProvenanceFile[] files;
            public ToucanMetricRecord metrics;
        }

        [Serializable]
        private sealed class ProvenanceFile
        {
            public string path;
            public string sha256;
        }
    }

    public readonly struct ToucanFixtureMetrics
    {
        public ToucanFixtureMetrics(
            int meshes,
            int vertices,
            int triangles,
            int materials,
            int renderers,
            Bounds bounds,
            long approximateRuntimeBytes)
        {
            Meshes = meshes;
            Vertices = vertices;
            Triangles = triangles;
            Materials = materials;
            Renderers = renderers;
            Bounds = bounds;
            ApproximateRuntimeBytes = approximateRuntimeBytes;
        }

        public int Meshes { get; }
        public int Vertices { get; }
        public int Triangles { get; }
        public int Materials { get; }
        public int Renderers { get; }
        public Bounds Bounds { get; }
        public long ApproximateRuntimeBytes { get; }

        public ToucanMetricRecord ToRecord() => new ToucanMetricRecord
        {
            meshes = Meshes,
            vertices = Vertices,
            triangles = Triangles,
            materials = Materials,
            renderers = Renderers,
            boundsCenter = Bounds.center,
            boundsSize = Bounds.size,
            approximateRuntimeBytes = ApproximateRuntimeBytes,
            memoryScope = "Unity Profiler estimate for distinct shared meshes/materials in Editor; not device peak memory"
        };
    }

    [Serializable]
    public sealed class ToucanMetricRecord
    {
        public int meshes;
        public int vertices;
        public int triangles;
        public int materials;
        public int renderers;
        public Vector3 boundsCenter;
        public Vector3 boundsSize;
        public long approximateRuntimeBytes;
        public string memoryScope;
    }
}
