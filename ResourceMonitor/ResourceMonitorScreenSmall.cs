using System.Collections.Generic;
using SMLHelper.V2.Assets;
using SMLHelper.V2.Crafting;
using UnityEngine;

namespace ResourceMonitor
{
    /**
    * The resource monitor object that will be placed in the sea base (or cyclops or any internal area).
    */
    public class ResourceMonitorScreenSmall : Buildable
    {
        public static ResourceMonitorScreenSmall Singleton { get; } = new ResourceMonitorScreenSmall();

        public override string AssetsFolder => EntryPoint.AssetsFolderLocation;
        public override TechGroup GroupForPDA => TechGroup.InteriorModules;
        public override TechCategory CategoryForPDA => TechCategory.InteriorModule;
        public override string IconFileName => "ResourceMonitorSmall.png";
        public override TechType RequiredForUnlock => TechType.None;
        public override string HandOverText => "Test Test Test 5";

        protected ResourceMonitorScreenSmall() : base("ResourceMonitorBuildableSmall", "Resource Monitor Screen Small", "Track how many resources you have stored away in your sea base on one handy small screen.")
        {
        }

        public override GameObject GetGameObject()
        {
            GameObject go = Object.Instantiate(CraftData.GetPrefabForTechType(TechType.PictureFrame));
            go.name = "ResourceMonitor";
            go.transform.localScale = new Vector3(1f, 1f, 1f);
            Object.DestroyImmediate(go.GetComponentInChildren<PictureFrame>());
            Object.DestroyImmediate(go.GetComponentInChildren<GenericHandTarget>());
            ResourceMonitorLogic rml = go.AddComponent<ResourceMonitorLogic>();
            return go;
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
