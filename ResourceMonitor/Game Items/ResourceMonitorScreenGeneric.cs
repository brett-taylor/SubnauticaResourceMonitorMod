using System.Collections.Generic;
using SMLHelper.V2.Assets;
using UnityEngine;

namespace ResourceMonitor.Game_Items
{
    /**
    * The generic resource monitor object that will be shared among all different types of the resource monitor.
    */
    public abstract class ResourceMonitorScreenGeneric : Buildable
    {
        public override string AssetsFolder => EntryPoint.QMODS_ASSETS_FOLDER_LOCATION;
        public override TechGroup GroupForPDA => TechGroup.InteriorModules;
        public override TechCategory CategoryForPDA => TechCategory.InteriorModule;

        protected ResourceMonitorScreenGeneric(string classId, string friendlyName, string description) : base(classId, friendlyName, description)
        {
        }

        public override GameObject GetGameObject()
        {
            var screen = Object.Instantiate(EntryPoint.RESOURCE_MONITOR_DISPLAY_MODEL);
            var screenModel = screen.transform.GetChild(0).gameObject;

            var shader = Shader.Find("MarmosetUBER");
            var renderers = screen.GetComponentsInChildren<Renderer>();
            foreach (var renderer in renderers)
            {
                renderer.material.shader = shader;
            }

            var skyApplier = screen.AddComponent<SkyApplier>();
            skyApplier.renderers = renderers;
            skyApplier.anchorSky = Skies.Auto;

            var constructable = screen.AddComponent<Constructable>();
            constructable.allowedOnWall = true;
            constructable.allowedInSub = true;
            constructable.allowedOnGround = false;
            constructable.allowedOutside = false;
            constructable.model = screenModel;
            constructable.techType = this.TechType;
            
            screen.AddComponent<ConstructableBounds>().bounds = new OrientedBounds(new Vector3(-0.1f, -0.1f, 0f), new Quaternion(0, 0, 0, 0), new Vector3(0.9f, 0.5f, 0f));
            screen.AddComponent<TechTag>().type = this.TechType;
            screen.AddComponent<PrefabIdentifier>().ClassId = ClassID;
            screen.AddComponent<VFXSurface>();
            screen.AddComponent<Components.ResourceMonitorLogic>();
            return screen;
        }

        protected abstract int GetNumberOfIngredientsRequired();
        
#if SUBNAUTICA
        
        protected override SMLHelper.V2.Crafting.TechData GetBlueprintRecipe()
        {
            return new SMLHelper.V2.Crafting.TechData()
            {
                craftAmount = 1,
                Ingredients = new List<SMLHelper.V2.Crafting.Ingredient>()
                {
                    new SMLHelper.V2.Crafting.Ingredient(TechType.Glass, GetNumberOfIngredientsRequired()),
                    new SMLHelper.V2.Crafting.Ingredient(TechType.ComputerChip, GetNumberOfIngredientsRequired()),
                    new SMLHelper.V2.Crafting.Ingredient(TechType.AdvancedWiringKit, GetNumberOfIngredientsRequired())
                }
            };
        }
        
#elif BELOWZERO

        protected override SMLHelper.V2.Crafting.RecipeData GetBlueprintRecipe()
        {
            return new SMLHelper.V2.Crafting.RecipeData()
            {
                craftAmount = 1,
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient(TechType.Glass, GetNumberOfIngredientsRequired()),
                    new Ingredient(TechType.ComputerChip, GetNumberOfIngredientsRequired()),
                    new Ingredient(TechType.AdvancedWiringKit, GetNumberOfIngredientsRequired())
                }
            };
        }
        
#endif
    }
}
