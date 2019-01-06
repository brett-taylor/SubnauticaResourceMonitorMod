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
            GameObject screen = Object.Instantiate(EntryPoint.RESOURCE_MONITOR_DISPLAY_MODEL);
            GameObject screenModel = screen.transform.GetChild(0).gameObject;

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
            constructable.model = screenModel;
            constructable.techType = this.TechType;

            BoxCollider boxCollider = screen.GetComponent<BoxCollider>();
            screen.AddComponent<ConstructableBounds>().bounds = new OrientedBounds(new Vector3(-0.1f, -0.1f, 0f), new Quaternion(0, 0, 0, 0), new Vector3(0.9f, 0.5f, 0f));
            screen.AddComponent<TechTag>().type = this.TechType;
            screen.AddComponent<PrefabIdentifier>().ClassId = ClassID;
            screen.AddComponent<VFXSurface>();
            screen.AddComponent<Components.ResourceMonitorLogic>();
            return screen;
        }
    }
}
