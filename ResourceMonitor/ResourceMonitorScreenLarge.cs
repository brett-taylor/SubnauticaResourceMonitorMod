using System.Collections.Generic;
using SMLHelper.V2.Assets;
using SMLHelper.V2.Crafting;
using UnityEngine;

namespace ResourceMonitor
{
    /**
    * The resource monitor object that will be placed in the sea base (or cyclops or any internal area).
    */
    public class ResourceMonitorScreenLarge : Buildable
    {
        public static ResourceMonitorScreenLarge Singleton { get; } = new ResourceMonitorScreenLarge();

        public override string AssetsFolder => EntryPoint.AssetsFolderLocation;
        public override TechGroup GroupForPDA => TechGroup.InteriorModules;
        public override TechCategory CategoryForPDA => TechCategory.InteriorModule;
        public override string IconFileName => "ResourceMonitorLarge.png";

        protected ResourceMonitorScreenLarge() : base("ResourceMonitorBuildableLarge", "Resource Monitor Screen Large", "Track how many resources you have stored away in your sea base on one handy large screen.")
        {
        }

        public override GameObject GetGameObject()
        {
            GameObject go = Object.Instantiate(CraftData.GetPrefabForTechType(TechType.PictureFrame));
            go.name = "ResourceMonitor";
            go.transform.localScale = new Vector3(2f, 2f, 1f);
            Object.DestroyImmediate(go.GetComponentInChildren<PictureFrame>());
            Object.DestroyImmediate(go.GetComponentInChildren<GenericHandTarget>());
            ResourceMonitorLogic rml = go.AddComponent<ResourceMonitorLogic>();
            return go;
        }

        protected override TechData GetBlueprintRecipe()
        {
            List<Ingredient> ingredients = new List<Ingredient>();
            ingredients.Add(new Ingredient(TechType.Glass, 2));
            ingredients.Add(new Ingredient(TechType.ComputerChip, 2));
            ingredients.Add(new Ingredient(TechType.AdvancedWiringKit, 2));

            TechData techData = new TechData();
            techData.craftAmount = 1;
            techData.Ingredients = ingredients;
            return techData;
        }
    }
}
