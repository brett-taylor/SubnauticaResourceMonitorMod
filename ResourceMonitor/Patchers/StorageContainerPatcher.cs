using System;
using System.Collections.Generic;
using HarmonyLib;
using ResourceMonitor.Components;

namespace ResourceMonitor.Patchers
{
    /**
     * Patches into StorageContainer::Awake method to alert our Resource monitor about the newly placed storage container
     */
    [HarmonyPatch(typeof(StorageContainer))]
    [HarmonyPatch("Awake")]
    public class StorageContainerAwakePatcher
    {
        private static readonly List<ResourceMonitorLogic> registeredResourceMonitors = new List<ResourceMonitorLogic>();

        [HarmonyPostfix]
        public static void Postfix(StorageContainer __instance)
        {
            if (__instance == null)
            {
                return;
            }

            foreach(ResourceMonitorLogic resourceMonitor in registeredResourceMonitors)
            {
                resourceMonitor.AlertNewStorageContainerPlaced(__instance);
            }
        }

        public static void RegisterForNewStorageContainerUpdates(ResourceMonitorLogic resourceMonitor)
        {
            registeredResourceMonitors.Add(resourceMonitor);
        }

        public static void ClearRegisteredResourceMonitors()
        {
            registeredResourceMonitors.Clear();
        }
    }
}
