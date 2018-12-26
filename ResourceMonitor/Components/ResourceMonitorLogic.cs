using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ResourceMonitor
{
    /**
    * Component that contains all the logic related to the resource monitor component.
    * When set up: We find the base we was placed in. Get all StorageContainer components in that base gameobject children.
    * We then subscribe to the events of items getting added and removed from that StorageContainer.
    * We don't care when a StorageContainer gets removed as for that to happen it has to be empty and so the remove item event will be called.
    * We do care about when a new locker is added though as we need to subscribe to its events we use Harmony's Postfox on StorageContainer.Awake() for that.
    */
    public class ResourceMonitorLogic : MonoBehaviour, IConstructable
    {
        public SortedDictionary<TechType, int> TrackedResources { private set; get; } = new SortedDictionary<TechType, int>();
        private ResourceMonitorDisplay rmd;
        private GameObject seaBase;

        public void OnConstructedChanged(bool constructed)
        {
            if (constructed && rmd == null)
                TurnOn();
            else
                TurnOff();
        }

        public bool CanDeconstruct(out string reason)
        {
            reason = null;
            return true;
        }

        private void TurnOn()
        {
            seaBase = gameObject?.transform?.parent?.gameObject;
            if (seaBase == null)
            {
                ErrorMessage.AddMessage("[ERROR] ResourceMonitorScreen: Can not work out what base it was placed inside.");
                System.Console.WriteLine("[ERROR] ResourceMonitorScreen: Can not work out what base it was placed inside.");
                return;
            }

            TrackExistingStorageContainers();
            rmd = gameObject.AddComponent<ResourceMonitorDisplay>();
            rmd.Setup(gameObject.transform, this);

            Patchers.BuilderPatcher.OnStorageContainedAdded += AlertedNewStorageContainerPlaced;
        }

        private void TurnOff()
        {
            rmd?.Destroy();
            Destroy(rmd);
            rmd = null;
        }

        private void TrackExistingStorageContainers()
        {
            StorageContainer[] containers = seaBase.GetComponentsInChildren<StorageContainer>();
            foreach (StorageContainer sc in containers)
            {
                TrackStorageContainer(sc);
            }
        }

        public void AlertedNewStorageContainerPlaced(StorageContainer sc)
        {
            StartCoroutine("TrackNewStorageContainerCoroutine", sc);
        }

        public IEnumerator TrackNewStorageContainerCoroutine(StorageContainer sc)
        {
            // We yield to the end of the frame as we need the parent/children tree to update.
            yield return new WaitForEndOfFrame();

            GameObject newSeaBase = sc?.gameObject?.transform?.parent?.gameObject;
            if (newSeaBase != null && newSeaBase == seaBase)
            {
                TrackStorageContainer(sc);
            }

            StopCoroutine("TrackNewStorageContainerCoroutine");
        }

        private void TrackStorageContainer(StorageContainer sc)
        {
            foreach (var item in sc.container.GetItemTypes())
            {
                AddItemsToTracker(item, sc.container.GetCount(item));
            }

            sc.container.onAddItem += (item) => AddItemsToTracker(item.item.GetTechType());
            sc.container.onRemoveItem += (item) => RemoveItemsFromTracker(item.item.GetTechType());
        }

        private void AddItemsToTracker(TechType item, int amountToAdd = 1)
        {
            if (TrackedResources.ContainsKey(item))
            {
                TrackedResources[item] = TrackedResources[item] + amountToAdd;
            }
            else
            {
                TrackedResources.Add(item, amountToAdd);
            }

            rmd?.ItemModified(item, TrackedResources[item]);
        }

        private void RemoveItemsFromTracker(TechType item, int amountToRemove = 1)
        {
            if (TrackedResources.ContainsKey(item))
            {
                int newQuantity = TrackedResources[item] - amountToRemove;
                TrackedResources[item] = newQuantity;
                if (newQuantity <= 0)
                {
                    TrackedResources.Remove(item);
                }

                rmd?.ItemModified(item, newQuantity);
            }
        }
    }
}
