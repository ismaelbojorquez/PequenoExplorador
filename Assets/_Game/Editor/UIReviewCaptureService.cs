using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace PequenoExplorador.Editor
{
    public static class UIReviewCaptureService
    {
        private static readonly (string Name, int Width, int Height)[] Viewports =
        {
            ("tablet-4x3", 1024, 768),
            ("phone-16x9", 1280, 720),
            ("phone-20x9", 1600, 720),
            ("tablet-16x10", 1280, 800)
        };

        private static readonly Surface[] Surfaces =
        {
            new Surface("boot", "Diagnostic Canvas"),
            new Surface("loading", "PH_UI_SCENE_FLOW", "Transition Panel"),
            new Surface("camp", "PH_UI_CAMP_HUB", null, "PH_Upgrade Preview"),
            new Surface("camera", "PH_UI_PHOTOGRAPHY", "PH_PHOTOGRAPHY_PANEL", "PH_DISCOVERY_CARD"),
            new Surface("discovery-card", "PH_UI_PHOTOGRAPHY", "PH_PHOTOGRAPHY_PANEL"),
            new Surface("album", "PH_UI_ALBUM", "PH_ALBUM_PANEL", "PH_ALBUM_DETAIL"),
            new Surface("album-detail", "PH_UI_ALBUM", "PH_ALBUM_DETAIL"),
            new Surface("activity", "PH_UI_LEARNING"),
            new Surface("mission", "PH_UI_MISSIONS"),
            new Surface("customization", "PH_UI_CUSTOMIZATION", "PH_Customization Panel")
        };

        [MenuItem("Pequeño Explorador/Development/UI/Capture Review Matrix")]
        public static void CaptureAll()
        {
            try
            {
                string phase = Environment.GetEnvironmentVariable("PE_UI_CAPTURE_PHASE") ?? "manual";
                string output = Path.Combine(Directory.GetCurrentDirectory(), "artifacts", "ui-review", phase);
                Directory.CreateDirectory(output);
                Scene scene = EditorSceneManager.OpenScene(ProjectFoundationSetup.BootstrapScenePath, OpenSceneMode.Single);
                GameObject[] roots = scene.GetRootGameObjects();
                Canvas[] canvases = roots.SelectMany(value => value.GetComponentsInChildren<Canvas>(true)).ToArray();
                var cameraObject = new GameObject("UI Review Camera", typeof(Camera));
                Camera camera = cameraObject.GetComponent<Camera>();
                camera.clearFlags = CameraClearFlags.SolidColor;
                camera.backgroundColor = new Color32(17, 48, 39, 255);
                camera.orthographic = true;
                camera.nearClipPlane = 0.1f;
                camera.farClipPlane = 100f;
                camera.transform.position = new Vector3(0f, 0f, -10f);

                foreach (Surface surface in Surfaces)
                {
                    ConfigureSurface(roots, canvases, surface);
                    foreach ((string name, int width, int height) in Viewports)
                    {
                        string path = Path.Combine(output, surface.Slug + "-" + name + ".png");
                        Render(camera, canvases, width, height, path);
                    }
                }

                UnityEngine.Object.DestroyImmediate(cameraObject);
                Debug.Log($"PE_UI_CAPTURE_OK phase={phase} surfaces={Surfaces.Length} viewports={Viewports.Length} output=artifacts/ui-review/{phase}");
                if (UnityEngine.Application.isBatchMode) EditorApplication.Exit(0);
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
                if (UnityEngine.Application.isBatchMode) EditorApplication.Exit(2);
                throw;
            }
        }

        private static void ConfigureSurface(GameObject[] roots, Canvas[] canvases, Surface surface)
        {
            foreach (Canvas value in canvases) value.enabled = false;
            GameObject root = roots.FirstOrDefault(value => value.name == surface.RootName) ??
                throw new InvalidOperationException("UI review root is missing: " + surface.RootName);
            root.SetActive(true);
            foreach (Canvas value in root.GetComponentsInChildren<Canvas>(true)) value.enabled = true;
            if (!string.IsNullOrEmpty(surface.ActiveChild)) Find(root.transform, surface.ActiveChild)?.gameObject.SetActive(true);
            if (!string.IsNullOrEmpty(surface.InactiveChild)) Find(root.transform, surface.InactiveChild)?.gameObject.SetActive(false);
            if (surface.Slug == "discovery-card") Find(root.transform, "PH_DISCOVERY_CARD")?.gameObject.SetActive(true);
            if (surface.Slug == "album") Find(root.transform, "PH_ALBUM_STATE_TEXT")?.gameObject.SetActive(false);
            foreach (Transform value in root.GetComponentsInChildren<Transform>(true))
            {
                if (value.name.StartsWith("PH_DEBUG", StringComparison.Ordinal)) value.gameObject.SetActive(false);
                if (surface.Slug.StartsWith("album", StringComparison.Ordinal) &&
                    value.name.StartsWith("PH_ALBUM_ENTRY_", StringComparison.Ordinal) &&
                    value.name != "PH_ALBUM_ENTRY_0" && value.parent != null && value.parent.name == "PH_ALBUM_GRID")
                    value.gameObject.SetActive(false);
            }
            PopulateReviewCopy(root);
        }

        private static void PopulateReviewCopy(GameObject root)
        {
            foreach (Text text in root.GetComponentsInChildren<Text>(true))
            {
                if (!string.IsNullOrWhiteSpace(text.text)) continue;
                text.text = ReviewCopy(text);
            }
        }

        private static string ReviewCopy(Text text)
        {
            string name = text.name;
            string owner = text.transform.parent == null ? string.Empty : text.transform.parent.name;
            if (name == "Station Title" && text.transform.parent != null) owner = text.transform.parent.name;
            string hierarchy = name;
            for (Transform current = text.transform.parent; current != null; current = current.parent) hierarchy += " " + current.name;
            string target = (hierarchy + " " + owner).ToLowerInvariant();
            if (target.Contains("station expedition")) return name == "Station Description" ? "Ir a la Selva" : "Expedición";
            if (target.Contains("station album")) return name == "Station Description" ? "Ver descubrimientos" : "Mi álbum";
            if (target.Contains("station customization")) return name == "Station Description" ? "Elegir mi estilo" : "Mi explorador";
            if (target.Contains("station parents")) return name == "Station Description" ? "Solo con una persona adulta" : "Familias";
            if (target.Contains("ph_album_category_0")) return "Todos";
            if (target.Contains("ph_album_category_1")) return "Animales";
            if (target.Contains("ph_album_category_2")) return "Plantas";
            if (target.Contains("ph_album_category_3")) return "Insectos";
            if (target.Contains("ph_album_back") || target.Contains("ph_album_detail_back") || target.Contains("close") || target.Contains("exit")) return "Volver";
            if (target.Contains("ph_album_previous")) return "Anterior";
            if (target.Contains("ph_album_next")) return "Siguiente";
            if (target.Contains("ph_album_replay") || target.Contains("replay")) return "Escuchar";
            if (target.Contains("ph_album_open")) return "Abrir álbum";
            if (target.Contains("ph_photography_shutter")) return "Foto";
            if (target.Contains("ph_photography_learn")) return "Conocer";
            if (target.Contains("ph_observation upgrade")) return "Mejorar rincón";
            if (target.Contains("confirm upgrade")) return "Confirmar";
            if (target.Contains("cancel upgrade")) return "Ahora no";
            if (target.Contains("mission activate")) return "Empezar";
            if (target.Contains("ph_ui_learning") && target.Contains("option 1")) return "Fruta";
            if (target.Contains("ph_ui_learning") && target.Contains("option 2")) return "Semillas";
            if (target.Contains("ph_ui_learning") && target.Contains("option 3")) return "Hojas";
            if (target.Contains("ph_ui_customization") && target.Contains("option 0")) return "Azul río";
            if (target.Contains("ph_ui_customization") && target.Contains("option 1")) return "Verde hoja";
            if (target.Contains("ph_ui_customization") && target.Contains("option 2")) return "Mango";
            if (target.Contains("ph_ui_customization") && target.Contains("option 3")) return "Cielo";
            if (target.Contains("hint")) return "Pista";
            if (target.Contains("unlock")) return "Desbloquear";
            if (target.Contains("equip")) return "Usar";
            if (target.Contains("slot 0")) return "Piel";
            if (target.Contains("slot 1")) return "Cabello";
            if (target.Contains("slot 2")) return "Camiseta";
            if (target.Contains("slot 3")) return "Pantalón";
            if (target.Contains("slot 4")) return "Zapatos";
            if (target.Contains("slot 5")) return "Sombrero";
            if (target.Contains("slot 6")) return "Mochila";
            if (target.Contains("slot 7")) return "Equipo";
            string normalized = name.ToLowerInvariant();
            if (normalized.Contains("product name")) return "Pequeño Explorador";
            if (normalized.Contains("development version")) return "Versión de desarrollo";
            if (normalized.Contains("temporary notice")) return "DIAGNÓSTICO TEMPORAL";
            if (normalized.Contains("status")) return "Preparando la expedición…";
            if (normalized.Contains("title")) return normalized.Contains("album") ? "Mi álbum de la Selva" : normalized.Contains("preview") ? "Rincón de exploración" : "¡Vamos a explorar!";
            if (normalized.Contains("progress")) return "1 de 1 descubierto";
            if (normalized.Contains("guidance")) return "Centra al tucán";
            if (normalized.Contains("card_text")) return "¡Nuevo descubrimiento! Tucán pico canoa";
            if (normalized.Contains("instruction")) return "¿Qué fruta elegirías?";
            if (normalized.Contains("feedback")) return "¡Mira los colores y prueba otra vez!";
            if (normalized.Contains("balance")) return "★ 6 Estrellas";
            if (normalized.Contains("selected name")) return "Camiseta azul río";
            if (normalized.Contains("selected state")) return "Disponible por 3 estrellas";
            if (normalized.Contains("description")) return "Un lugar acogedor para observar tus descubrimientos.";
            if (normalized.Contains("cost")) return "Costo: 3 estrellas";
            if (normalized.Contains("page")) return "1 / 1";
            if (normalized.Contains("name")) return "Tucán pico canoa";
            if (normalized.Contains("body")) return "Fotografía al tucán para completar la misión.";
            if (normalized.Contains("state")) return "Descubierto";
            if (normalized.Contains("label")) return "Continuar";
            return "Continuar";
        }

        private static Transform Find(Transform root, string name)
        {
            foreach (Transform value in root.GetComponentsInChildren<Transform>(true))
                if (value.name == name) return value;
            return null;
        }

        private static void Render(Camera camera, Canvas[] canvases, int width, int height, string path)
        {
            var target = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32);
            var texture = new Texture2D(width, height, TextureFormat.RGB24, false, false);
            RenderTexture previous = RenderTexture.active;
            try
            {
                camera.targetTexture = target;
                foreach (Canvas canvas in canvases.Where(value => value.enabled))
                {
                    canvas.renderMode = RenderMode.ScreenSpaceCamera;
                    canvas.worldCamera = camera;
                    canvas.planeDistance = 1f;
                }
                Canvas.ForceUpdateCanvases();
                camera.Render();
                RenderTexture.active = target;
                texture.ReadPixels(new Rect(0f, 0f, width, height), 0, 0);
                texture.Apply(false, false);
                File.WriteAllBytes(path, texture.EncodeToPNG());
            }
            finally
            {
                RenderTexture.active = previous;
                camera.targetTexture = null;
                UnityEngine.Object.DestroyImmediate(texture);
                UnityEngine.Object.DestroyImmediate(target);
            }
        }

        private readonly struct Surface
        {
            public Surface(string slug, string rootName, string activeChild = null, string inactiveChild = null)
            { Slug = slug; RootName = rootName; ActiveChild = activeChild; InactiveChild = inactiveChild; }
            public string Slug { get; }
            public string RootName { get; }
            public string ActiveChild { get; }
            public string InactiveChild { get; }
        }
    }
}
