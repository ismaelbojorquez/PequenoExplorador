using System;
using UnityEditor;
using UnityEngine;

namespace PequenoExplorador.Editor
{
    public static class VerticalSliceContentAdoptionSetup
    {
        [MenuItem("Pequeño Explorador/Development/Content/Adopt Approved VS-D-A01")]
        public static void Apply()
        {
            try
            {
                LocalizationFoundationSetup.ApplyApprovedContentEntries();
                ContentFoundationSetup.ApplyAssetsAndBootstrap();
                InteractionFoundationSetup.ApplyAssetsAndScenes();
                ToucanFixtureSetup.ApplyAssetsAndScene(writeRenders: false);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                Debug.Log("PE_VS_D_A01_ADOPTION_OK discovery=discovery.jungle.keel-billed-toucan interaction=interaction.jungle.keel-billed-toucan facts=7 sources=6 alias=1");
                if (UnityEngine.Application.isBatchMode) EditorApplication.Exit(0);
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
                if (UnityEngine.Application.isBatchMode) EditorApplication.Exit(2);
                throw;
            }
        }
    }
}
