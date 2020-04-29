using System;
using Harmony;
using System.IO;
using System.Reflection;
using System.Text;
using OdinSerializer;
using QModManager.API.ModLoading;
using UnityEngine;
namespace ResourceMonitor
{
    [QModCore]
    public static class EntryPoint
    {
        public const string MOD_FOLDER_LOCATION = "./QMods/ResourceMonitor/";
        public const string QMODS_ASSETS_FOLDER_LOCATION = "/ResourceMonitor/Assets/";
        public const string ASSETS_FOLDER_LOCATION = MOD_FOLDER_LOCATION + "Assets/";
        public const string ASSET_BUNDLE_LOCATION = ASSETS_FOLDER_LOCATION + "resources";
        public const string SETTINGS_FILE_LOCATION = MOD_FOLDER_LOCATION + "Settings.json";
        public const string DONT_TRACK_LOCATION = MOD_FOLDER_LOCATION + "DontTrackList.txt";
        public static GameObject RESOURCE_MONITOR_DISPLAY_UI_PREFAB { get; private set; }
        public static GameObject RESOURCE_MONITOR_DISPLAY_ITEM_UI_PREFAB { get; private set; }
        public static GameObject RESOURCE_MONITOR_DISPLAY_MODEL { get; private set; }
        public static SettingsData SETTINGS { get; private set; }

        [QModPatch]
        public static void Entry()
        {
            var harmony = HarmonyInstance.Create("taylor.brett.ResourceMonitor.mod");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            new Game_Items.ResourceMonitorScreenLarge().Patch();
            new Game_Items.ResourceMonitorScreenSmall().Patch();
            LoadDontTrackList();
            LoadAssets();
            SETTINGS = File.Exists(SETTINGS_FILE_LOCATION) ?  LoadSettings() : CreateSettingsIfItDoesntExist();
        }

        private static void LoadAssets()
        {
            var ab = AssetBundle.LoadFromFile(ASSET_BUNDLE_LOCATION);
            RESOURCE_MONITOR_DISPLAY_UI_PREFAB = ab.LoadAsset("ResourceMonitorDisplayUI") as GameObject;
            RESOURCE_MONITOR_DISPLAY_ITEM_UI_PREFAB = ab.LoadAsset("ResourceItem") as GameObject;
            RESOURCE_MONITOR_DISPLAY_MODEL = ab.LoadAsset("ResourceMonitorModel") as GameObject;
        }
        
        private static void LoadDontTrackList()
        {
            if (File.Exists(DONT_TRACK_LOCATION))
            {
                Console.WriteLine("[ResourceMonitor] Found the dont track list at location: " + DONT_TRACK_LOCATION);
                using (var reader = new StreamReader(DONT_TRACK_LOCATION))
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
                Console.WriteLine("[ResourceMonitor] Did not find the dont track list at location: " + DONT_TRACK_LOCATION);
            }
        }
        
        private static SettingsData CreateSettingsIfItDoesntExist()
        {
            var data = new SettingsData();
            using (var stream = new StreamWriter(SETTINGS_FILE_LOCATION, false))
                stream.WriteLine(Encoding.UTF8.GetString(SerializationUtility.SerializeValue(data, DataFormat.JSON)));

            return data;
        }
        
        private static SettingsData LoadSettings()
        {
            using (var stream = new StreamReader(SETTINGS_FILE_LOCATION, Encoding.UTF8))
            {
                var json = stream.ReadToEnd();
                var bytes = Encoding.UTF8.GetBytes(json);
                return SerializationUtility.DeserializeValue<SettingsData>(bytes, DataFormat.JSON);
            }
        }
    }
}
