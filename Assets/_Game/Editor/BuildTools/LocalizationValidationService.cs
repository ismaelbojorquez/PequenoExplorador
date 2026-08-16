using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using PequenoExplorador.Application.Localization;
using UnityEditor.Localization;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Pseudo;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;

namespace PequenoExplorador.Editor.BuildTools
{
    public static class LocalizationValidationService
    {
        private static readonly string[] ScenePaths =
        {
            ProjectFoundationSetup.BootstrapScenePath,
            SceneFlowFoundationSetup.CampScenePath,
            SceneFlowFoundationSetup.JungleScenePath
        };

        public static IReadOnlyList<string> Validate()
        {
            var violations = new List<string>();
            ValidateSettingsAndLocales(violations);
            ValidateStringTables(violations);
            ValidateAssetTables(violations);
            ValidateSceneText(violations);
            ValidateFont(violations);
            return violations;
        }

        private static void ValidateSettingsAndLocales(ICollection<string> violations)
        {
            LocalizationSettings settings = LocalizationEditorSettings.ActiveLocalizationSettings;
            if (settings == null)
            {
                violations.Add("LOC001 active LocalizationSettings is missing");
                return;
            }

            Locale[] realLocales = LocalizationEditorSettings.GetLocales().ToArray();
            PseudoLocale[] pseudoLocales = LocalizationEditorSettings.GetPseudoLocales().ToArray();
            if (realLocales.Count(locale => locale.Identifier.Code == LocaleCode.Spanish) != 1 ||
                realLocales.Count(locale => locale.Identifier.Code == LocaleCode.English) != 1 ||
                realLocales.Length != 2)
            {
                violations.Add("LOC002 exactly one es and one en runtime locale are required");
            }

            if (pseudoLocales.Length != 1 || pseudoLocales[0].Identifier.Code != LocaleCode.Spanish)
            {
                violations.Add("LOC003 exactly one pseudo locale sourced from es is required");
            }

            var selectors = settings.GetStartupLocaleSelectors();
            if (selectors.Count != 1 || selectors[0] is not SpecificLocaleSelector specific ||
                specific.LocaleId.Code != LocaleCode.Spanish)
            {
                violations.Add("LOC004 Spanish must be the sole explicit startup locale");
            }

            if (LocalizationSettings.ProjectLocale == null ||
                LocalizationSettings.ProjectLocale.Identifier.Code != LocaleCode.Spanish)
            {
                violations.Add("LOC005 project locale must be Spanish");
            }
        }

        private static void ValidateStringTables(ICollection<string> violations)
        {
            LocalizedKey[] keys = typeof(LocalizationKeys)
                .GetFields(BindingFlags.Public | BindingFlags.Static)
                .Where(field => field.FieldType == typeof(LocalizedKey))
                .Select(field => (LocalizedKey)field.GetValue(null))
                .ToArray();

            foreach (IGrouping<string, LocalizedKey> duplicate in keys
                         .GroupBy(key => key.ToString(), StringComparer.Ordinal)
                         .Where(group => group.Count() > 1))
            {
                violations.Add("LOC006 duplicate code key: " + duplicate.Key);
            }

            foreach (IGrouping<string, LocalizedKey> tableKeys in keys.GroupBy(key => key.Table, StringComparer.Ordinal))
            {
                StringTableCollection collection = LocalizationEditorSettings.GetStringTableCollection(tableKeys.Key);
                if (collection == null)
                {
                    violations.Add("LOC007 missing string table collection: " + tableKeys.Key);
                    continue;
                }

                ValidateSharedKeys(collection, tableKeys.ToArray(), violations);
                ValidateLocaleTable(collection, LocaleCode.Spanish, tableKeys.ToArray(), violations);
                ValidateLocaleTable(collection, LocaleCode.English, tableKeys.ToArray(), violations);
            }
        }

        private static void ValidateSharedKeys(
            StringTableCollection collection,
            IReadOnlyCollection<LocalizedKey> expected,
            ICollection<string> violations)
        {
            string prefix = collection.TableCollectionName.ToLowerInvariant() + ".";
            string[] actual = collection.SharedData.Entries.Select(entry => entry.Key).ToArray();
            if (actual.Distinct(StringComparer.Ordinal).Count() != actual.Length)
            {
                violations.Add("LOC008 duplicate shared key in " + collection.TableCollectionName);
            }

            foreach (string key in actual.Where(key => !key.StartsWith(prefix, StringComparison.Ordinal)))
            {
                violations.Add("LOC009 key is not namespaced for its table: " + collection.TableCollectionName + ":" + key);
            }

            foreach (LocalizedKey key in expected.Where(key => !actual.Contains(key.Entry, StringComparer.Ordinal)))
            {
                violations.Add("LOC010 missing shared key: " + key);
            }
        }

        private static void ValidateLocaleTable(
            StringTableCollection collection,
            string localeCode,
            IReadOnlyCollection<LocalizedKey> expected,
            ICollection<string> violations)
        {
            StringTable table = collection.GetTable(new LocaleIdentifier(localeCode)) as StringTable;
            if (table == null)
            {
                violations.Add("LOC011 missing locale table: " + collection.TableCollectionName + "_" + localeCode);
                return;
            }

            foreach (LocalizedKey key in expected)
            {
                StringTableEntry entry = table.GetEntry(key.Entry);
                if (entry == null || string.IsNullOrWhiteSpace(entry.Value))
                {
                    violations.Add("LOC012 empty translation: " + localeCode + ":" + key);
                }
            }
        }

        private static void ValidateAssetTables(ICollection<string> violations)
        {
            ValidateAssetTable(
                LocalizationKeys.VoiceAssetTable,
                new[] { "content.world.camp.name", "content.world.jungle.name" },
                violations);
            ValidateAssetTable(
                LocalizationKeys.IllustrationAssetTable,
                new[] { "content.world.camp.background", "content.world.jungle.background" },
                violations);
        }

        private static void ValidateAssetTable(
            string tableName,
            IEnumerable<string> expectedKeys,
            ICollection<string> violations)
        {
            AssetTableCollection collection = LocalizationEditorSettings.GetAssetTableCollection(tableName);
            if (collection == null)
            {
                violations.Add("LOC013 missing asset table collection: " + tableName);
                return;
            }

            string[] actual = collection.SharedData.Entries.Select(entry => entry.Key).ToArray();
            foreach (string expected in expectedKeys.Where(key => !actual.Contains(key, StringComparer.Ordinal)))
            {
                violations.Add("LOC014 missing conceptual asset key: " + tableName + ":" + expected);
            }

            foreach (string localeCode in new[] { LocaleCode.Spanish, LocaleCode.English })
            {
                if (collection.GetTable(new LocaleIdentifier(localeCode)) == null)
                {
                    violations.Add("LOC015 missing asset locale table: " + tableName + "_" + localeCode);
                }
            }
        }

        private static void ValidateSceneText(ICollection<string> violations)
        {
            foreach (string path in ScenePaths)
            {
                if (!File.Exists(path))
                {
                    violations.Add("LOC016 missing runtime scene: " + path);
                    continue;
                }

                foreach (string line in File.ReadLines(path))
                {
                    string trimmed = line.Trim();
                    if (trimmed.StartsWith("m_Text:", StringComparison.Ordinal) &&
                        !string.IsNullOrWhiteSpace(trimmed.Substring("m_Text:".Length)))
                    {
                        violations.Add("LOC017 visible scene text must be populated from tables: " + path);
                        break;
                    }
                }
            }
        }

        private static void ValidateFont(ICollection<string> violations)
        {
            Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            if (font == null)
            {
                violations.Add("LOC018 LegacyRuntime font is unavailable");
                return;
            }

            const string required = "áéíóúüñÁÉÍÓÚÜÑ¿¡…·";
            string missing = new string(required.Where(character => !font.HasCharacter(character)).ToArray());
            if (missing.Length > 0)
            {
                violations.Add("LOC019 runtime font lacks required ES/pseudo glyphs: " + missing);
            }
        }
    }
}
