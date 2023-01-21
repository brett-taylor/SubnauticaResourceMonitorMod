using SMLHelper.V2.Utility;
using System.IO;
using UnityEngine;

namespace ResourceMonitor.Game_Items
{
    /**
    * The resource monitor (small version) object that will be placed in the sea base (or cyclops or any internal area).
    */
    public class ResourceMonitorScreenSmall : ResourceMonitorScreenGeneric
    {
        public static readonly string CLASS_ID = "ResourceMonitorBuildableSmall";
        public static readonly string NICE_NAME = "Resource Monitor Screen Small";
        public static readonly string DESCRIPTION = "Track how many resources you have stored away in your sea base on one handy small screen.";
        public override string IconFileName => "ResourceMonitorSmall.png";

        public ResourceMonitorScreenSmall() : base(CLASS_ID, NICE_NAME, DESCRIPTION)
        {
        }

        public override GameObject GetGameObject()
        {
            var screen = base.GetGameObject();
            return screen;
        }

        protected override int GetNumberOfIngredientsRequired() => 1;

        protected override Atlas.Sprite GetItemSprite()
        {
            return ImageUtils.LoadSpriteFromFile(Path.Combine(Plugin.ASSETS_FOLDER_LOCATION, "ResourceMonitorSmall.png"));
        }
    }
}
