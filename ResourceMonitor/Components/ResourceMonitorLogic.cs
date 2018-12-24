using System.Collections.Generic;
using UnityEngine;

namespace ResourceMonitor
{
    /**
    * Component that contains all the logic related to the resource monitor component.
    * When set up: We find the base we was placed in. Get all StorageContainer components in that base gameobject children.
    * We then subscribe to the events of items getting added and removed from that StorageContainer.
    * We don't care when a StorageContainer gets removed as for that to happen it has to be empty and so the remove item event will be called.
    * We do care about when a new locker is added though as we need to subscribe to its events. TO:DO implement this.
    */
    public class ResourceMonitorLogic : MonoBehaviour, IConstructable
    {
        private ResourceMonitorDisplay rmd;
        private GameObject seaBase = null;
        private Dictionary<TechType, uint> trackedResources = new Dictionary<TechType, uint>();

        public bool CanDeconstruct(out string reason)
        {
            reason = null;
            return true;
        }

        public void OnConstructedChanged(bool constructed)
        {
            if (constructed && rmd == null)
                TurnOn();
            else
                TurnOff();
        }

        private void TurnOn()
        {
            rmd = gameObject.AddComponent<ResourceMonitorDisplay>();
            rmd.Setup(gameObject.transform, this);

            TrackLockers();
        }

        private void TurnOff()
        {
            rmd?.Destroy();
            Destroy(rmd);
            rmd = null;
        }

        private void TrackLockers()
        {
            seaBase = gameObject?.transform?.parent?.gameObject;
            if (seaBase == null)
            {
                ErrorMessage.AddMessage("ResourceMonitorScreen: Can not work out what base it was placed inside.");
                TurnOff();
                return;
            }

            foreach (StorageContainer sc in seaBase.GetComponentsInChildren<StorageContainer>())
            {
                sc.container.onAddItem += ItemAddedToTrackedLocker;
                sc.container.onRemoveItem += ItemRemovedFromTrackedLocker;
            }
        }

        public void OnPointerClick()
        {
        }

        private void ItemAddedToTrackedLocker(InventoryItem item)
        {
            TechType itemTechType = item.item.GetTechType();
            if (trackedResources.ContainsKey(item.item.GetTechType()))
            {
                trackedResources[itemTechType] = trackedResources[itemTechType] + 1;
            }
            else
            {
                trackedResources.Add(itemTechType, 1);
            }

            rmd?.ItemModified(item, trackedResources[itemTechType]);
        }

        private void ItemRemovedFromTrackedLocker(InventoryItem item)
        {
            TechType itemTechType = item.item.GetTechType();
            if (trackedResources.ContainsKey(itemTechType))
            {
                trackedResources[itemTechType] = trackedResources[itemTechType] - 1;
                if (trackedResources[itemTechType] <= 0)
                {
                    trackedResources.Remove(itemTechType);
                }

                rmd?.ItemModified(item, trackedResources[itemTechType]);
            }
        }
    }
}
