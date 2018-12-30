using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ResourceMonitor.Components
{
    /**
    * Component that contains all the logic related to the resource monitor component.
    * When set up: We find the base we was placed in. Get all StorageContainer components in that base gameobject children.
    * We then subscribe to the events of items getting added and removed from that StorageContainer.
    * We care about when a new locker is added though as we need to subscribe to its events we use Harmony's Postfix on StorageContainer.Awake() for that.
    * We care about when a locker is removed though so we can unsubscribe from it. Either if its getting deleted or unloaded due to game exit. TO:DO implemenet this.
    */
    public class ResourceMonitorLogic : MonoBehaviour, IConstructable
    {
        public SortedDictionary<TechType, int> TrackedResources { private set; get; } = new SortedDictionary<TechType, int>();
        private ResourceMonitorDisplay rmd;
        private GameObject seaBase;
        private bool isEnabled = false;

        private IEnumerator Startup()
        {
            yield return new WaitForEndOfFrame();

            seaBase = gameObject?.transform?.parent?.gameObject;
            if (seaBase == null)
            {
                ErrorMessage.AddMessage("[ResourceMonitor] ERROR: Can not work out what base it was placed inside.");
                System.Console.WriteLine("[ResourceMonitor] ERROR: Can not work out what base it was placed inside.");
                yield break;
            }

            TrackExistingStorageContainers();
            bool result = Patchers.StorageContainerAwakePatcher.AddEventHandlerIfMissing(AlertedNewStorageContainerPlaced);
            TurnDisplayOn();
        }

        public void OnDestroy()
        {
            Patchers.StorageContainerAwakePatcher.RemoveEventHandler(AlertedNewStorageContainerPlaced);
            TrackedResources.Clear();
        }

        public void OnConstructedChanged(bool constructed)
        {
            if (constructed)
            {
                if (isEnabled == false)
                {
                    isEnabled = true;
                    StartCoroutine(Startup());
                }
                else
                {
                    TurnDisplayOn();
                }
            }
            else
            {
                if (isEnabled)
                {
                    TurnDisplayOff();
                }
            }
        }

        public bool CanDeconstruct(out string reason)
        {
            reason = null;
            return true;
        }

        private void TurnDisplayOn()
        {
            if (rmd != null)
            {
                TurnDisplayOff();
            }

            rmd = gameObject.AddComponent<ResourceMonitorDisplay>();
            rmd.Setup(this);
        }

        private void TurnDisplayOff()
        {
            if (rmd != null)
            {
                rmd.TurnDisplayOff();
                Destroy(rmd);
                rmd = null;
            }
        }

        private void TrackExistingStorageContainers()
        {
            StorageContainer[] containers = seaBase.GetComponentsInChildren<StorageContainer>();
            foreach (StorageContainer sc in containers)
            {
                TrackStorageContainer(sc);
            }
        }

        private void AlertedNewStorageContainerPlaced(StorageContainer sc)
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
            if (sc == null || sc.container == null)
            {
                return;
            }

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
