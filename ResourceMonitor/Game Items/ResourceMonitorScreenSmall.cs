using System.Collections.Generic;
using SMLHelper.V2.Crafting;
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
            GameObject screen = base.GetGameObject();
            return screen;
        }

        protected override TechData GetBlueprintRecipe()
        {
            List<Ingredient> ingredients = new List<Ingredient>();
            ingredients.Add(new Ingredient(TechType.Glass, 1));
            ingredients.Add(new Ingredient(TechType.ComputerChip, 1));
            ingredients.Add(new Ingredient(TechType.AdvancedWiringKit, 1));

            TechData techData = new TechData();
            techData.craftAmount = 1;
            techData.Ingredients = ingredients;
            return techData;
        }
    }
}
