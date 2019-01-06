using System.Collections.Generic;
using SMLHelper.V2.Crafting;
using UnityEngine;

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
        public override string IconFileName => "ResourceMonitorLarge.png";

        public ResourceMonitorScreenLarge() : base(CLASS_ID, NICE_NAME, DESCRIPTION)
        {
        }

        public override GameObject GetGameObject()
        {
            GameObject screen = base.GetGameObject();
            screen.transform.localScale = SCALE;
            return screen;
        }

        protected override TechData GetBlueprintRecipe()
        {
            return new TechData()
            {
                craftAmount = 1,
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient(TechType.Glass, 2),
                    new Ingredient(TechType.ComputerChip, 2),
                    new Ingredient(TechType.AdvancedWiringKit, 2)
                }
            };
        }
    }
}
