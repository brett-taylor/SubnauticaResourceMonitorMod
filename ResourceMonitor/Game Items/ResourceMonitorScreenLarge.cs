using SMLHelper.V2.Utility;
using System.Reflection;
using UnityEngine;
using static Atlas;
using Valve.VR;
using System.IO;

namespace ResourceMonitor.Game_Items
{
    /**
    * The resource monitor (large version) object that will be placed in the sea base (or cyclops or any internal area).
    */
    public class ResourceMonitorScreenLarge : ResourceMonitorScreenGeneric
    {
        public static readonly string CLASS_ID = "ResourceMonitorBuildableLarge";
        public static readonly string NICE_NAME = "Resource Monitor Screen Large";
        public static readonly string DESCRIPTION = "Track how many resources you have stored away in your sea base on one handy large screen.";
        public static readonly Vector3 SCALE = new Vector3(2.3f, 2.3f, 1f);

        public ResourceMonitorScreenLarge() : base(CLASS_ID, NICE_NAME, DESCRIPTION)
        {
        }

        public override GameObject GetGameObject()
        {
            var screen = base.GetGameObject();
            screen.transform.localScale = SCALE;
            return screen;
        }

        protected override int GetNumberOfIngredientsRequired() => 2;

        protected override Atlas.Sprite GetItemSprite()
        {
            return ImageUtils.LoadSpriteFromFile(Path.Combine(Plugin.ASSETS_FOLDER_LOCATION, "ResourceMonitorLarge.png"));
        }
    }
}
