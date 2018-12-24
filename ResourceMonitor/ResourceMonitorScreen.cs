using System.Collections.Generic;
using SMLHelper.V2.Assets;
using SMLHelper.V2.Crafting;
using UnityEngine;

namespace ResourceMonitor
{
    /**
    * The resource monitor object that will be placed in the sea base (or cyclops or any internal area).
    */
    public class ResourceMonitorScreen : Buildable
    {
        public static ResourceMonitorScreen Singleton { get; } = new ResourceMonitorScreen();

        public override string AssetsFolder => EntryPoint.AssetsFolderLocation;
        public override TechGroup GroupForPDA => TechGroup.InteriorModules;
        public override TechCategory CategoryForPDA => TechCategory.InteriorModule;
        public override string IconFileName => "ResourceMonitor.png";
        public override TechType RequiredForUnlock => TechType.None;

        protected ResourceMonitorScreen() : base("ResourceMonitorBuildable", "Resource Monitor Screen", "Track how many resources you have in your sea base on one handy screen.")
        {
        }

        public override GameObject GetGameObject()
        {
            GameObject go = Object.Instantiate(Resources.Load<GameObject>("Submarine/Build/PictureFrame"));
            go.name = "ResourceMonitor";
            go.transform.localScale = new Vector3(2f, 2f, 1f);
            Object.DestroyImmediate(go.GetComponentInChildren<PictureFrame>());
            ResourceMonitorLogic rml = go.AddComponent<ResourceMonitorLogic>();
            return go;
        }

        protected override TechData GetBlueprintRecipe()
        {
            List<Ingredient> ingredients = new List<Ingredient>();
            ingredients.Add(new Ingredient(TechType.Titanium, 1));

            TechData techData = new TechData();
            techData.craftAmount = 1;
            techData.Ingredients = ingredients;
            return techData;
        }
    }
}
