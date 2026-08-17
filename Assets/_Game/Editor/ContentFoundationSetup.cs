using System;
using System.Collections.Generic;
using System.Linq;
using PequenoExplorador.Application.Content;
using PequenoExplorador.Bootstrap;
using PequenoExplorador.Content.Data;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PequenoExplorador.Editor
{
    public static class ContentFoundationSetup
    {
        public const string Root = "Assets/_Game/Content/Data";
        public const string CatalogPath = Root + "/ContentCatalog.asset";
        public const string DiscoveryPath = Root + "/Definitions/VS_Discovery_KeelBilledToucan.asset";
        public const string DiscoveryId = "discovery.jungle.keel-billed-toucan";
        public const string RetiredDiscoveryId = "discovery.jungle.placeholder";
        private const string CategoryPath = Root + "/Definitions/VS_Category_Animals.asset";
        private const string TagPath = Root + "/Definitions/VS_Tag_Jungle.asset";
        private const string DefinitionsRoot = Root + "/Definitions";
        private static readonly string[] RetiredDefinitionPaths =
        {
            DefinitionsRoot + "/PH_Category_Nature.asset",
            DefinitionsRoot + "/PH_Tag_Jungle.asset",
            DefinitionsRoot + "/PH_Source_PendingReview.asset",
            DefinitionsRoot + "/PH_Fact_PendingReview.asset",
            DefinitionsRoot + "/PH_Discovery_Jungle.asset"
        };

        private static readonly SourceSpec[] SourceSpecs =
        {
            new SourceSpec("source.conabio.ramphastos-sulfuratus-2025", "CONABIO", "Base técnica con bibliografía AOS 2025", "Tucán pico canoa (Ramphastos sulfuratus)", "https://enciclovida.mx/especies/36504.pdf"),
            new SourceSpec("source.itis.ramphastos-sulfuratus", "Integrated Taxonomic Information System", "IOC World Bird List v10.2", "ITIS report TSN 685778", "https://www.itis.gov/servlet/SingleRpt/SingleRpt?search_topic=TSN&search_value=685778"),
            new SourceSpec("source.cornell.bow-keel-billed-toucan-v1", "Cornell Lab of Ornithology", "Revee Jones y Carole S. Griffiths; editor T. S. Schulenberg", "Keel-billed Toucan (Ramphastos sulfuratus), Birds of the World v1.0", "https://doi.org/10.2173/bow.kebtou1.01"),
            new SourceSpec("source.cornell.ebird-keel-billed-toucan", "Cornell Lab of Ornithology", "Identificación powered by Merlin", "Keel-billed Toucan — Ramphastos sulfuratus", "https://ebird.org/species/kebtou1/MX-ROO"),
            new SourceSpec("source.condor.remsen-hyde-chapman-1993", "The Condor / University of South Florida Scholar Commons", "J. V. Remsen Jr., Mary Ann Hyde y Angela Chapman", "The Diets of Neotropical Trogons, Motmots, Barbets and Toucans", "https://digitalcommons.usf.edu/condor/vol95/iss1/18/"),
            new SourceSpec("source.umich.adw-ramphastos-sulfuratus-2001", "University of Michigan Animal Diversity Web", "Megan Carney; editora Terry Root", "Ramphastos sulfuratus", "https://animaldiversity.org/accounts/Ramphastos_sulfuratus/")
        };

        private static readonly FactSpec[] FactSpecs =
        {
            new FactSpec("identity", "La especie candidata es Ramphastos sulfuratus, familia Ramphastidae.", 0, 1, 2),
            new FactSpec("common-name", "En México el nombre oficial usado es Tucán pico canoa; Keel-billed Toucan es el nombre EN aprobado.", 0, 1, 2),
            new FactSpec("range", "Ocurre desde el sur de México por Centroamérica hasta el norte de Colombia y extremo noroeste de Venezuela.", 0, 2),
            new FactSpec("habitat", "Habita bosques tropicales perennifolios de tierras bajas y bosques secundarios.", 2),
            new FactSpec("diet", "Su dieta es mayormente fruta; artrópodos y pequeños vertebrados son complementos, no la base.", 2, 4),
            new FactSpec("bill", "Su pico diagnóstico combina verde, naranja, rojo y azul.", 2, 3),
            new FactSpec("voice", "La vocalización se describe como un croar lejano que se repite regularmente.", 3)
        };

        [MenuItem("Pequeño Explorador/Development/Content/Apply Data Foundation")]
        public static void Apply()
        {
            try
            {
                ApplyAssetsAndBootstrap();
                Debug.Log("PE_CONTENT_FOUNDATION_SETUP_OK discoveries=1 state=Approved aliases=1 facts=7 sources=6 release=true");
                if (UnityEngine.Application.isBatchMode) EditorApplication.Exit(0);
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
                if (UnityEngine.Application.isBatchMode) EditorApplication.Exit(2);
                throw;
            }
        }

        public static void ApplyAssetsAndBootstrap()
        {
            EnsureFolder(DefinitionsRoot);
            CategoryDefinitionAsset category = LoadOrCreate<CategoryDefinitionAsset>(CategoryPath);
            TagDefinitionAsset tag = LoadOrCreate<TagDefinitionAsset>(TagPath);
            ConfigureApproved(category, "category.discovery.animals", "Ismael Bojórquez — Product/Education");
            var categorySerialized = new SerializedObject(category);
            categorySerialized.FindProperty("_displayNameTable").stringValue = "Content";
            categorySerialized.FindProperty("_displayNameKey").stringValue = "content.category.discovery.animals";
            categorySerialized.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(category);
            ConfigureApproved(tag, "tag.world.jungle", "Ismael Bojórquez — Product/Education");

            ContentSourceRecordAsset[] sources = SourceSpecs.Select(EnsureSource).ToArray();
            EducationalFactDefinitionAsset[] facts = FactSpecs.Select(spec => EnsureFact(spec, sources)).ToArray();
            DiscoveryDefinitionAsset discovery = LoadOrCreate<DiscoveryDefinitionAsset>(DiscoveryPath);
            ConfigureApproved(discovery, DiscoveryId, "Ismael Bojórquez — Product/Education");
            ConfigureDiscovery(discovery, category, tag, facts);
            ContentCatalogAsset catalog = LoadOrCreate<ContentCatalogAsset>(CatalogPath);
            ConfigureCatalog(catalog, category, tag, sources, facts, discovery);
            foreach (string retiredPath in RetiredDefinitionPaths) AssetDatabase.DeleteAsset(retiredPath);

            Scene scene = EditorSceneManager.OpenScene(ProjectFoundationSetup.BootstrapScenePath, OpenSceneMode.Single);
            DiagnosticBootstrap bootstrap = UnityEngine.Object.FindFirstObjectByType<DiagnosticBootstrap>();
            if (bootstrap == null) throw new InvalidOperationException("Bootstrap scene has no DiagnosticBootstrap.");
            bootstrap.ConfigureContentForEditorAndTests(catalog);
            EditorUtility.SetDirty(bootstrap);
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
        }

        private static ContentSourceRecordAsset EnsureSource(SourceSpec spec)
        {
            string path = DefinitionsRoot + "/VS_Source_" + SlugToAssetName(spec.Id) + ".asset";
            ContentSourceRecordAsset source = LoadOrCreate<ContentSourceRecordAsset>(path);
            ConfigureApproved(source, spec.Id, "Ismael Bojórquez — Investigador");
            var serialized = new SerializedObject(source);
            serialized.FindProperty("_institution").stringValue = spec.Institution;
            serialized.FindProperty("_author").stringValue = spec.Author;
            serialized.FindProperty("_title").stringValue = spec.Title;
            serialized.FindProperty("_reference").stringValue = spec.Reference;
            serialized.FindProperty("_consultedOn").stringValue = "2026-08-16";
            serialized.FindProperty("_reviewer").stringValue = "Ismael Bojórquez — Investigador; H-009-IB-2026-08-16";
            serialized.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(source);
            return source;
        }

        private static EducationalFactDefinitionAsset EnsureFact(FactSpec spec, IReadOnlyList<ContentSourceRecordAsset> sources)
        {
            string id = "fact.jungle.keel-billed-toucan." + spec.Suffix;
            string path = DefinitionsRoot + "/VS_Fact_KeelBilledToucan_" + SlugToAssetName(spec.Suffix) + ".asset";
            EducationalFactDefinitionAsset fact = LoadOrCreate<EducationalFactDefinitionAsset>(path);
            ConfigureApproved(fact, id, "Ismael Bojórquez — Product/Education; factual H-009");
            var serialized = new SerializedObject(fact);
            serialized.FindProperty("_childCopyTable").stringValue = "Content";
            serialized.FindProperty("_childCopyKey").stringValue = "content.fact.keel-billed-toucan." + spec.Suffix;
            serialized.FindProperty("_claimForReview").stringValue = spec.Claim;
            SetObjectArray(serialized.FindProperty("_sources"), spec.SourceIndexes.Select(index => sources[index]).ToArray());
            serialized.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(fact);
            return fact;
        }

        private static void ConfigureApproved(ContentDefinitionAsset asset, string id, string owner)
        {
            asset.ConfigureIdentityForEditorAndTests(id);
            var serialized = new SerializedObject(asset);
            SerializedProperty editorial = serialized.FindProperty("_editorial");
            editorial.FindPropertyRelative("_state").enumValueIndex = (int)EditorialState.Approved;
            editorial.FindPropertyRelative("_isPlaceholder").boolValue = false;
            editorial.FindPropertyRelative("_owner").stringValue = owner;
            editorial.FindPropertyRelative("_developmentWatermark").stringValue = string.Empty;
            serialized.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(asset);
        }

        private static void ConfigureDiscovery(DiscoveryDefinitionAsset discovery, CategoryDefinitionAsset category, TagDefinitionAsset tag, EducationalFactDefinitionAsset[] facts)
        {
            var serialized = new SerializedObject(discovery);
            serialized.FindProperty("_worldId").stringValue = "world.jungle";
            serialized.FindProperty("_category").objectReferenceValue = category;
            SetObjectArray(serialized.FindProperty("_tags"), tag);
            SetObjectArray(serialized.FindProperty("_facts"), facts);
            serialized.FindProperty("_displayNameTable").stringValue = "Content";
            serialized.FindProperty("_displayNameKey").stringValue = "content.discovery.keel-billed-toucan.name";
            serialized.FindProperty("_nameAudioCueId").stringValue = "audio.feedback.confirm";
            serialized.FindProperty("_visualAssetId").stringValue = ToucanFixtureSetup.VisualId;
            serialized.FindProperty("_visualAsset").objectReferenceValue = AssetDatabase.LoadAssetAtPath<GameObject>(ToucanFixtureSetup.PrefabPath);
            serialized.FindProperty("_albumHabitatFact").objectReferenceValue = FindFact(facts, "habitat");
            serialized.FindProperty("_albumDietFact").objectReferenceValue = FindFact(facts, "diet");
            serialized.FindProperty("_albumSizeFact").objectReferenceValue = null;
            serialized.FindProperty("_albumCuriosityFact").objectReferenceValue = FindFact(facts, "bill");
            serialized.FindProperty("_albumSoundFact").objectReferenceValue = FindFact(facts, "voice");
            serialized.FindProperty("_albumHasPlayableAudio").boolValue = false;
            serialized.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(discovery);
        }

        private static EducationalFactDefinitionAsset FindFact(
            IEnumerable<EducationalFactDefinitionAsset> facts,
            string suffix)
        {
            return facts.Single(fact => fact.RawId.EndsWith("." + suffix, StringComparison.Ordinal));
        }

        private static void ConfigureCatalog(ContentCatalogAsset catalog, CategoryDefinitionAsset category, TagDefinitionAsset tag, ContentSourceRecordAsset[] sources, EducationalFactDefinitionAsset[] facts, DiscoveryDefinitionAsset discovery)
        {
            var serialized = new SerializedObject(catalog);
            serialized.FindProperty("_id").stringValue = "catalog.jungle.vertical-slice";
            SetObjectArray(serialized.FindProperty("_categories"), category);
            SetObjectArray(serialized.FindProperty("_tags"), tag);
            SetObjectArray(serialized.FindProperty("_sources"), sources);
            SetObjectArray(serialized.FindProperty("_facts"), facts);
            SetObjectArray(serialized.FindProperty("_discoveries"), discovery);
            SerializedProperty aliases = serialized.FindProperty("_discoveryAliases");
            aliases.arraySize = 1;
            SerializedProperty alias = aliases.GetArrayElementAtIndex(0);
            alias.FindPropertyRelative("_previousId").stringValue = RetiredDiscoveryId;
            alias.FindPropertyRelative("_current").objectReferenceValue = discovery;
            serialized.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(catalog);
        }

        private static void SetObjectArray(SerializedProperty property, params UnityEngine.Object[] values)
        {
            property.arraySize = values.Length;
            for (int index = 0; index < values.Length; index++) property.GetArrayElementAtIndex(index).objectReferenceValue = values[index];
        }

        private static T LoadOrCreate<T>(string path) where T : ScriptableObject
        {
            T asset = AssetDatabase.LoadAssetAtPath<T>(path);
            if (asset != null) return asset;
            asset = ScriptableObject.CreateInstance<T>();
            asset.name = System.IO.Path.GetFileNameWithoutExtension(path);
            AssetDatabase.CreateAsset(asset, path);
            return asset;
        }

        private static string SlugToAssetName(string value) => string.Join("_", value.Split('.', '-').Select(part => part.Length == 0 ? part : char.ToUpperInvariant(part[0]) + part.Substring(1)));

        private static void EnsureFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path)) return;
            string parent = System.IO.Path.GetDirectoryName(path)?.Replace('\\', '/');
            if (!string.IsNullOrEmpty(parent)) EnsureFolder(parent);
            AssetDatabase.CreateFolder(parent, System.IO.Path.GetFileName(path));
        }

        private readonly struct SourceSpec
        {
            public SourceSpec(string id, string institution, string author, string title, string reference)
            {
                Id = id; Institution = institution; Author = author; Title = title; Reference = reference;
            }
            public string Id { get; }
            public string Institution { get; }
            public string Author { get; }
            public string Title { get; }
            public string Reference { get; }
        }

        private readonly struct FactSpec
        {
            public FactSpec(string suffix, string claim, params int[] sourceIndexes)
            {
                Suffix = suffix; Claim = claim; SourceIndexes = sourceIndexes;
            }
            public string Suffix { get; }
            public string Claim { get; }
            public int[] SourceIndexes { get; }
        }
    }
}
