using Harmony;
using System.IO;
using System.Reflection;
using QModManager.API.ModLoading;
using UnityEngine;

namespace ResourceMonitor
{
    /**
     * Entry class.
     */
    [QModCore]
    public class EntryPoint
    {
        public static readonly string MOD_FOLDER_LOCATION = "./QMods/ResourceMonitor/";
        public static readonly string QMODS_ASSETS_FOLDER_LOCATION = "/ResourceMonitor/Assets/";
        public static readonly string ASSETS_FOLDER_LOCATION = MOD_FOLDER_LOCATION + "Assets/";
        public static readonly string ASSET_BUNDLE_LOCATION = ASSETS_FOLDER_LOCATION + "resources";
        public static readonly string SETTINGS_FILE_LOCATION = MOD_FOLDER_LOCATION + "DontTrackList.txt";
        public static GameObject RESOURCE_MONITOR_DISPLAY_UI_PREFAB { private set; get; }
        public static GameObject RESOURCE_MONITOR_DISPLAY_ITEM_UI_PREFAB { private set; get; }
        public static GameObject RESOURCE_MONITOR_DISPLAY_MODEL { private set; get; }

        /**
        * Entry method.
        */
        [QModPatch]
        public static void Entry()
        {
            HarmonyInstance harmony = HarmonyInstance.Create("taylor.brett.ResourceMonitor.mod");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            new Game_Items.ResourceMonitorScreenLarge().Patch();
            new Game_Items.ResourceMonitorScreenSmall().Patch();
            LoadAssets();
            LoadDontTrackList();
        }

        private static void LoadAssets()
        {
            AssetBundle ab = AssetBundle.LoadFromFile(ASSET_BUNDLE_LOCATION);
            RESOURCE_MONITOR_DISPLAY_UI_PREFAB = ab.LoadAsset("ResourceMonitorDisplayUI") as GameObject;
            RESOURCE_MONITOR_DISPLAY_ITEM_UI_PREFAB = ab.LoadAsset("ResourceItem") as GameObject;
            RESOURCE_MONITOR_DISPLAY_MODEL = ab.LoadAsset("ResourceMonitorModel") as GameObject;
        }

        private static void LoadDontTrackList()
        {
            if (File.Exists(SETTINGS_FILE_LOCATION))
            {
                System.Console.WriteLine("[ResourceMonitor] Found the dont track list at location: " + SETTINGS_FILE_LOCATION);
                using (StreamReader reader = new StreamReader(SETTINGS_FILE_LOCATION))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (string.IsNullOrEmpty(line) == false)
                        {
                            Components.ResourceMonitorLogic.DONT_TRACK_GAMEOBJECTS.Add(line.ToLower());
                        }
                    }
                }
                Components.ResourceMonitorLogic.DONT_TRACK_GAMEOBJECTS.Sort();
            }
            else
            {
                System.Console.WriteLine("[ResourceMonitor] Did not find the dont track list at location: " + SETTINGS_FILE_LOCATION);
            }
        }
    }
}
