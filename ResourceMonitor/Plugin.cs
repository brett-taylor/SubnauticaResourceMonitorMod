using System;
using HarmonyLib;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using BepInEx;
using BepInEx.Configuration;
using UnityEngine;
using Newtonsoft.Json;

namespace ResourceMonitor
{
    [BepInPlugin(GUID, MODNAME, VERSION)]
    internal class Plugin : BaseUnityPlugin
    {
        public const string MOD_FOLDER_LOCATION = "./BepInEx/plugins/ResourceMonitor/";
        public const string ASSETS_FOLDER_LOCATION = "./BepInEx/plugins/ResourceMonitor/Assets/";
        public const string ASSET_BUNDLE_LOCATION = ASSETS_FOLDER_LOCATION + "resources";
#if BZ
        public const string SETTINGS_FILE_LOCATION = MOD_FOLDER_LOCATION + "Settings.json";
        public const string DONT_TRACK_LOCATION = MOD_FOLDER_LOCATION + "DontTrackList.txt";
#endif
        public static GameObject RESOURCE_MONITOR_DISPLAY_UI_PREFAB { get; private set; }
        public static GameObject RESOURCE_MONITOR_DISPLAY_ITEM_UI_PREFAB { get; private set; }
        public static GameObject RESOURCE_MONITOR_DISPLAY_MODEL { get; private set; }

#region[Declarations]
        private const string
            MODNAME = "ResourceMonitor",
            AUTHOR = "taylor",
            GUID = "taylor.brett.ResourceMonitor.mod",
            VERSION = "2.0.0.0";
#endregion

        public void Awake()
        {
            Console.WriteLine("ResourceMonitor - Started patching v" + Assembly.GetExecutingAssembly().GetName().Version.ToString(3));
            var harmony = new Harmony(GUID);
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            new Game_Items.ResourceMonitorScreenLarge().Patch();
            new Game_Items.ResourceMonitorScreenSmall().Patch();
#if BZ
            LoadDontTrackList();
#endif
            LoadAssets();
            LoadSettings();
            Console.WriteLine("ResourceMonitor - Finished patching");
        }
        private static void LoadAssets()
        {
            var ab = AssetBundle.LoadFromFile(ASSET_BUNDLE_LOCATION);
            RESOURCE_MONITOR_DISPLAY_UI_PREFAB = ab.LoadAsset("ResourceMonitorDisplayUI") as GameObject;
            RESOURCE_MONITOR_DISPLAY_ITEM_UI_PREFAB = ab.LoadAsset("ResourceItem") as GameObject;
            RESOURCE_MONITOR_DISPLAY_MODEL = ab.LoadAsset("ResourceMonitorModel") as GameObject;
        }
#if BZ
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
#endif
        public static ConfigEntry<bool> AllowSelectingItemsFromMonitor;
        public static ConfigEntry<Color> PaginatorStartingColor;
        public static ConfigEntry<Color> PaginatorHoverColor;
        public static ConfigEntry<float> MaxInteractionDistance;
        public static ConfigEntry<float> MaxInteractionIdlePageDistance;
        public static ConfigEntry<bool> EnableIdle;
        public static ConfigEntry<float> IdleTime;
        public static ConfigEntry<float> IdleTimeRandomnessLowBound;
        public static ConfigEntry<float> IdleTimeRandomnessHighBound;
        public static ConfigEntry<float> IdleScreenColorTransitionTime;
        public static ConfigEntry<float> IdleScreenColorTransitionRandomnessHighBound;
        public static ConfigEntry<float> IdleScreenColorTransitionRandomnessLowBound;
        //public static ConfigEntry<List<Color>> PossibleIdleColors;
        public static ConfigEntry<Color> ItemButtonBackgroundColor;
        public static ConfigEntry<Color> ItemButtonHoverColor;
        //public static List<Color> ColorList = new List<Color>() { new Color(0.07f, 0.38f, 0.70f), new Color(0.86f, 0.22f, 0.22f), new Color(0.22f, 0.86f, 0.22f) };

        private void LoadSettings()
        {
            AllowSelectingItemsFromMonitor = Config.Bind(new ConfigDefinition("General", "Allow Selecting Items From Monitor"), true);
            PaginatorStartingColor = Config.Bind(new ConfigDefinition("General", "Paginator Starting Color"), Color.white, new ConfigDescription("Color for the page selection."));
            PaginatorHoverColor = Config.Bind(new ConfigDefinition("General", "Paginator Hover Color"), new Color(0.07f, 0.38f, 0.7f, 1f), new ConfigDescription("Color for page selection hover"));
            MaxInteractionDistance = Config.Bind(new ConfigDefinition("General", "Max Interaction Distance"), 2.5f);
            MaxInteractionIdlePageDistance = Config.Bind(new ConfigDefinition("General", "Max Interaction Idle Page Distance"), 5f, new ConfigDescription("Applies for idle mode"));
            EnableIdle = Config.Bind(new ConfigDefinition("General", "Idle mode"), true, new ConfigDescription("If disabled, resource monitor may require manual overriding to display accurate numbers"));
            IdleTime = Config.Bind(new ConfigDefinition("General", "Idle time"), 20f);
            IdleTimeRandomnessLowBound = Config.Bind(new ConfigDefinition("General", "Low bounduary for idle time randomness"), 1f);
            IdleTimeRandomnessHighBound = Config.Bind(new ConfigDefinition("General", "High bounduary for idle time randomness"), 10f);
            IdleScreenColorTransitionTime = Config.Bind(new ConfigDefinition("General", "Idle screen color transition time"), 2f);
            IdleScreenColorTransitionRandomnessHighBound = Config.Bind(new ConfigDefinition("General", "High bounduary for color transition randomness"), 2f);
            IdleScreenColorTransitionRandomnessLowBound = Config.Bind(new ConfigDefinition("General", "Low bounduary for color transition randomness"), 0f);
            //PossibleIdleColors = Config.Bind(new ConfigDefinition("General", "Possible Idle Colors"),new AcceptableValueList<Color> = ({ Color.blue, Color.red, Color.green}));
            ItemButtonBackgroundColor = Config.Bind(new ConfigDefinition("General", "Background colors for item buttons"), new Color(0.07843138f, 0.3843137f, 0.7058824f));
            ItemButtonHoverColor = Config.Bind(new ConfigDefinition("General", "Background colors for item buttons - hover"), new Color(0.07843137f, 0.1459579f, 0.7058824f));
        }
    }
}
