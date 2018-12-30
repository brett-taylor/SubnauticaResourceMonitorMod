using SMLHelper.V2.Assets;
using UnityEngine;

namespace ResourceMonitor.Game_Items
{
    /**
    * The generic resource monitor object that will be shared among all different types of the resource monitor.
    */
    public abstract class ResourceMonitorScreenGeneric : Buildable
    {
        public override string AssetsFolder => EntryPoint.AssetsFolderLocation;
        public override TechGroup GroupForPDA => TechGroup.InteriorModules;
        public override TechCategory CategoryForPDA => TechCategory.InteriorModule;

        protected ResourceMonitorScreenGeneric(string classId, string friendlyName, string description) : base(classId, friendlyName, description)
        {
        }

        public override GameObject GetGameObject()
        {
            GameObject screen = Object.Instantiate(EntryPoint.ResourceMonitorDisplayModel);

            Shader shader = Shader.Find("MarmosetUBER");
            Renderer[] renderers = screen.GetComponentsInChildren<Renderer>();
            foreach (Renderer renderer in renderers)
            {
                renderer.material.shader = shader;
            }

            SkyApplier skyApplier = screen.AddComponent<SkyApplier>();
            skyApplier.renderers = renderers;
            skyApplier.anchorSky = Skies.Auto;

            Constructable constructable = screen.AddComponent<Constructable>();
            constructable.allowedOnWall = true;
            constructable.allowedInSub = true;
            constructable.allowedOnGround = false;
            constructable.allowedOutside = false;
            constructable.model = screen.transform.GetChild(0).gameObject;
            constructable.techType = this.TechType;

            BoxCollider boxCollider = screen.GetComponent<BoxCollider>();
            screen.AddComponent<ConstructableBounds>().bounds = new OrientedBounds(boxCollider.center, boxCollider.transform.rotation, new Vector3(boxCollider.size.x, boxCollider.size.y, 0f));
            screen.AddComponent<TechTag>().type = this.TechType;
            screen.AddComponent<Components.ResourceMonitorLogic>();
            screen.AddComponent<PrefabIdentifier>().ClassId = ClassID;
            screen.AddComponent<VFXSurface>();
            screen.AddComponent<LargeWorldEntity>().cellLevel = LargeWorldEntity.CellLevel.Near; // CHECK WHAT THIS DOES
            return screen;
        }
    }
}
