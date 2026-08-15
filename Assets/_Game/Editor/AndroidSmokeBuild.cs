using PequenoExplorador.Editor.BuildTools;

namespace PequenoExplorador.Editor
{
    public static class AndroidSmokeBuild
    {
        public static void Build()
        {
            BuildToolsCli.BuildAndroidDevelopment();
        }
    }
}
