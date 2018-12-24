using Harmony;
using System.Reflection;
using UnityEngine;

namespace ResourceMonitor
{
    /**
     * Entry class.
     */
    public class EntryPoint
    {
        public const string AssetsFolderLocation = "ResourceMonitor/Assets";
        public const string AssetBundleLocation = "./QMods/" + AssetsFolderLocation + "/resources";
        public static GameObject ResourceMonitorDisplayUIPrefab { private set; get; }
        public static GameObject ResourceMonitorDisplayItemUIPrefab { private set; get; }

        /**
        * Entry method.
        */
        public static void Entry()
        {
            HarmonyInstance harmony = HarmonyInstance.Create("taylor.brett.ResourceMonitor.mod");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            ResourceMonitorScreen.Singleton.Patch();

            LoadAssets();
        }

        private static void LoadAssets()
        {
            AssetBundle ab = AssetBundle.LoadFromFile(AssetBundleLocation);
            ResourceMonitorDisplayUIPrefab = ab.LoadAsset("ResourceMonitorDisplayUI") as GameObject;
            ResourceMonitorDisplayItemUIPrefab = ab.LoadAsset("ResourceItem") as GameObject;
        }
    }
}
