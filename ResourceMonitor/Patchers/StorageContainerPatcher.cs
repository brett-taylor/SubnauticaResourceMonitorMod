using System;
using Harmony;

namespace ResourceMonitor.Patchers
{
    /**
     * Patches into StorageContainer::Awake method to alert our Resource monitor about the newly placed storage container
     */
    [HarmonyPatch(typeof(StorageContainer))]
    [HarmonyPatch("Awake")]
    public class StorageContainerAwakePatcher
    {
        private static Action<StorageContainer> onStorageContainerAdded;

        [HarmonyPostfix]
        public static void Postfix(StorageContainer __instance)
        {
            if (onStorageContainerAdded != null)
            {
                onStorageContainerAdded.Invoke(__instance);
            }
        }

        public static bool AddEventHandlerIfMissing(Action<StorageContainer> newHandler)
        {
            if (onStorageContainerAdded == null)
            {
                onStorageContainerAdded += newHandler;
                return true;
            }
            else
            {
                foreach (Action<StorageContainer> action in onStorageContainerAdded.GetInvocationList())
                {
                    if (action == newHandler)
                    {
                        return false;
                    }
                }

                onStorageContainerAdded += newHandler;
                return true;
            }
        }

        public static void RemoveEventHandler(Action<StorageContainer> handler)
        {
            onStorageContainerAdded -= handler;
        }
    }
}
