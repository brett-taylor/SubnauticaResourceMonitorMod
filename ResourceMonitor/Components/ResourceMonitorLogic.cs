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
        private Dictionary<TechType, int> trackedResources = new Dictionary<TechType, int>();

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
                foreach (var item in sc.container.GetItemTypes())
                {
                    AddItemsToTracker(item, sc.container.GetCount(item));
                }

                sc.container.onAddItem += (item) => AddItemsToTracker(item.item.GetTechType());
                sc.container.onRemoveItem += (item) => RemoveItemsFromTracker(item.item.GetTechType());
            }
        }

        private void AddItemsToTracker(TechType item, int amountToAdd = 1)
        {
            if (trackedResources.ContainsKey(item))
            {
                trackedResources[item] = trackedResources[item] + amountToAdd;
            }
            else
            {
                trackedResources.Add(item, amountToAdd);
            }

            rmd?.ItemModified(item, trackedResources[item]);
        }

        private void RemoveItemsFromTracker(TechType item, int amountToRemove = 1)
        {
            if (trackedResources.ContainsKey(item))
            {
                trackedResources[item] = trackedResources[item] - amountToRemove;
                rmd?.ItemModified(item, trackedResources[item]);

                if (trackedResources[item] <= 0)
                {
                    trackedResources.Remove(item);
                }
            }
        }

        public void OnPointerClick()
        {
        }
    }
}
