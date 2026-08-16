using System;
using System.IO;
using System.Linq;
using PequenoExplorador.Bootstrap;
using PequenoExplorador.Presentation.Explorer;
using Unity.AI.Navigation;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

namespace PequenoExplorador.Editor
{
    public static class ExplorerFoundationSetup
    {
        public const string Root = "Assets/_Game/Content/Explorer";
        public const string PrefabPath = Root + "/PH_Explorer.prefab";
        public const string BodyMaterialPath = Root + "/PH_Explorer_Body.mat";
        public const string AccentMaterialPath = Root + "/PH_Explorer_Accent.mat";
        public const string GroundMaterialPath = Root + "/PH_Jungle_Ground.mat";
        public const string DestinationMaterialPath = Root + "/PH_Destination_Valid.mat";
        public const string InvalidMaterialPath = Root + "/PH_Destination_Invalid.mat";
        public const string NavMeshDataPath = Root + "/PH_Jungle_NavMesh.asset";

        [MenuItem("Pequeño Explorador/Development/Explorer/Apply Foundation")]
        public static void Apply()
        {
            try
            {
                EnsureFolder(Root);
                Material body = LoadOrCreateMaterial(BodyMaterialPath, new Color(0.11f, 0.62f, 0.55f));
                Material accent = LoadOrCreateMaterial(AccentMaterialPath, new Color(1f, 0.72f, 0.18f));
                Material ground = LoadOrCreateMaterial(GroundMaterialPath, new Color(0.12f, 0.38f, 0.19f));
                Material destination = LoadOrCreateMaterial(DestinationMaterialPath, new Color(0.15f, 0.9f, 0.55f));
                Material invalid = LoadOrCreateMaterial(InvalidMaterialPath, new Color(1f, 0.42f, 0.2f));
                CreateExplorerPrefab(body, accent, destination, invalid);
                ConfigureJungle(ground, accent);
                ConfigureBootstrapCamera();
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                Debug.Log("PE_EXPLORER_SETUP_OK package=2.0.9 speed=2.4 acceleration=8 radius=0.35 rootMotion=false");
                if (UnityEngine.Application.isBatchMode) EditorApplication.Exit(0);
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
                if (UnityEngine.Application.isBatchMode) EditorApplication.Exit(2);
                throw;
            }
        }

        private static void CreateExplorerPrefab(
            Material body,
            Material accent,
            Material destination,
            Material invalid)
        {
            var root = new GameObject(ExplorerLocomotionRoot.PlaceholderRootName);
            try
            {
                NavMeshAgent agent = root.AddComponent<NavMeshAgent>();
                agent.speed = 2.4f;
                agent.acceleration = 8f;
                agent.angularSpeed = 420f;
                agent.radius = 0.35f;
                agent.height = 1.65f;
                agent.baseOffset = 0f;
                agent.stoppingDistance = 0.18f;
                agent.autoBraking = true;

                GameObject visual = new GameObject("PH_Visual");
                visual.transform.SetParent(root.transform, false);
                GameObject bodyObject = CreatePrimitive(
                    PrimitiveType.Capsule, "PH_Body", visual.transform,
                    new Vector3(0f, 0.83f, 0f), new Vector3(0.62f, 0.72f, 0.62f), body);
                DestroyCollider(bodyObject);
                GameObject head = CreatePrimitive(
                    PrimitiveType.Sphere, "PH_Head", visual.transform,
                    new Vector3(0f, 1.43f, 0f), Vector3.one * 0.48f, accent);
                DestroyCollider(head);
                GameObject backpack = CreatePrimitive(
                    PrimitiveType.Cube, "PH_Backpack", visual.transform,
                    new Vector3(0f, 0.9f, -0.34f), new Vector3(0.48f, 0.58f, 0.22f), accent);
                DestroyCollider(backpack);

                GameObject validMarker = CreatePrimitive(
                    PrimitiveType.Cylinder, "PH_Destination_Valid", root.transform,
                    Vector3.zero, new Vector3(0.5f, 0.025f, 0.5f), destination);
                DestroyCollider(validMarker);
                GameObject invalidMarker = CreatePrimitive(
                    PrimitiveType.Cylinder, "PH_Destination_Invalid", root.transform,
                    Vector3.zero, new Vector3(0.5f, 0.025f, 0.5f), invalid);
                DestroyCollider(invalidMarker);
                validMarker.SetActive(false);
                invalidMarker.SetActive(false);

                ExplorerLocomotionRoot locomotion = root.AddComponent<ExplorerLocomotionRoot>();
                var serialized = new SerializedObject(locomotion);
                serialized.FindProperty("_agent").objectReferenceValue = agent;
                serialized.FindProperty("_visual").objectReferenceValue = visual.transform;
                serialized.FindProperty("_destinationMarker").objectReferenceValue = validMarker;
                serialized.FindProperty("_invalidMarker").objectReferenceValue = invalidMarker;
                serialized.ApplyModifiedPropertiesWithoutUndo();
                PrefabUtility.SaveAsPrefabAsset(root, PrefabPath);
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(root);
            }
        }

        private static void ConfigureJungle(Material groundMaterial, Material obstacleMaterial)
        {
            Scene scene = EditorSceneManager.OpenScene(SceneFlowFoundationSetup.JungleScenePath, OpenSceneMode.Single);
            RemoveKnownRoot(scene, ExplorerLocomotionRoot.PlaceholderRootName);
            RemoveKnownRoot(scene, "PH_JUNGLE_GEOMETRY");
            RemoveKnownRoot(scene, "PH_NAVIGATION_JUNGLE");
            RemoveKnownRoot(scene, "PH_JUNGLE_LIGHT");
            if (AssetDatabase.LoadAssetAtPath<NavMeshData>(NavMeshDataPath) != null)
                AssetDatabase.DeleteAsset(NavMeshDataPath);

            Canvas worldCanvas = scene.GetRootGameObjects()
                .SelectMany(item => item.GetComponentsInChildren<Canvas>(true)).FirstOrDefault();
            if (worldCanvas != null) worldCanvas.enabled = false;

            var geometry = new GameObject("PH_JUNGLE_GEOMETRY");
            GameObject ground = CreatePrimitive(
                PrimitiveType.Cube, "PH_JUNGLE_GROUND", geometry.transform,
                new Vector3(0f, -0.1f, 0f), new Vector3(16f, 0.2f, 14f), groundMaterial);
            ground.AddComponent<WalkableSurfaceMarker>();
            CreateBoundary("North", geometry.transform, new Vector3(0f, 0.6f, 7.2f), new Vector3(17f, 1.2f, 0.4f), obstacleMaterial);
            CreateBoundary("South", geometry.transform, new Vector3(0f, 0.6f, -7.2f), new Vector3(17f, 1.2f, 0.4f), obstacleMaterial);
            CreateBoundary("East", geometry.transform, new Vector3(8.2f, 0.6f, 0f), new Vector3(0.4f, 1.2f, 15f), obstacleMaterial);
            CreateBoundary("West", geometry.transform, new Vector3(-8.2f, 0.6f, 0f), new Vector3(0.4f, 1.2f, 15f), obstacleMaterial);
            CreateBoundary("Tree_A", geometry.transform, new Vector3(-2.5f, 0.75f, 1.5f), new Vector3(0.8f, 1.5f, 0.8f), obstacleMaterial);
            CreateBoundary("Tree_B", geometry.transform, new Vector3(3.2f, 0.75f, -0.8f), new Vector3(0.9f, 1.5f, 0.9f), obstacleMaterial);

            var navigationRoot = new GameObject("PH_NAVIGATION_JUNGLE");
            NavMeshSurface surface = navigationRoot.AddComponent<NavMeshSurface>();
            surface.collectObjects = CollectObjects.All;
            surface.useGeometry = NavMeshCollectGeometry.PhysicsColliders;
            surface.layerMask = Physics.AllLayers;
            surface.BuildNavMesh();
            if (surface.navMeshData == null)
                throw new InvalidOperationException("Jungle NavMeshSurface did not create NavMeshData.");
            AssetDatabase.CreateAsset(surface.navMeshData, NavMeshDataPath);

            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);
            if (prefab == null) throw new InvalidOperationException("PH_ explorer prefab was not created.");
            GameObject explorer = (GameObject)PrefabUtility.InstantiatePrefab(prefab, scene);
            explorer.name = ExplorerLocomotionRoot.PlaceholderRootName;
            explorer.transform.position = new Vector3(0f, 0f, -2f);

            var lightRoot = new GameObject("PH_JUNGLE_LIGHT");
            Light light = lightRoot.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1.1f;
            lightRoot.transform.rotation = Quaternion.Euler(50f, -30f, 0f);

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
        }

        private static void ConfigureBootstrapCamera()
        {
            Scene scene = EditorSceneManager.OpenScene(ProjectFoundationSetup.BootstrapScenePath, OpenSceneMode.Single);
            DiagnosticBootstrap bootstrap = scene.GetRootGameObjects()
                .SelectMany(item => item.GetComponentsInChildren<DiagnosticBootstrap>(true)).Single();
            Camera camera = scene.GetRootGameObjects()
                .SelectMany(item => item.GetComponentsInChildren<Camera>(true)).Single();
            bootstrap.ConfigureExplorerCameraForEditorAndTests(camera);
            EditorUtility.SetDirty(bootstrap);
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
        }

        private static GameObject CreatePrimitive(
            PrimitiveType type,
            string name,
            Transform parent,
            Vector3 position,
            Vector3 scale,
            Material material)
        {
            GameObject value = GameObject.CreatePrimitive(type);
            value.name = name;
            if (parent != null) value.transform.SetParent(parent, false);
            value.transform.localPosition = position;
            value.transform.localScale = scale;
            value.GetComponent<Renderer>().sharedMaterial = material;
            return value;
        }

        private static void CreateBoundary(string id, Transform parent, Vector3 position, Vector3 scale, Material material) =>
            CreatePrimitive(PrimitiveType.Cube, "PH_JUNGLE_OBSTACLE_" + id, parent, position, scale, material);

        private static void DestroyCollider(GameObject value)
        {
            Collider collider = value.GetComponent<Collider>();
            if (collider != null) UnityEngine.Object.DestroyImmediate(collider);
        }

        private static Material LoadOrCreateMaterial(string path, Color color)
        {
            Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (material == null)
            {
                Shader shader = Shader.Find("Universal Render Pipeline/Lit");
                if (shader == null) throw new InvalidOperationException("URP Lit shader is unavailable.");
                material = new Material(shader) { name = Path.GetFileNameWithoutExtension(path) };
                AssetDatabase.CreateAsset(material, path);
            }
            material.color = color;
            EditorUtility.SetDirty(material);
            return material;
        }

        private static void RemoveKnownRoot(Scene scene, string name)
        {
            GameObject value = scene.GetRootGameObjects().FirstOrDefault(item => item.name == name);
            if (value != null) UnityEngine.Object.DestroyImmediate(value);
        }

        private static void EnsureFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path)) return;
            string parent = Path.GetDirectoryName(path)?.Replace('\\', '/');
            if (!string.IsNullOrEmpty(parent)) EnsureFolder(parent);
            AssetDatabase.CreateFolder(parent, Path.GetFileName(path));
        }
    }
}
