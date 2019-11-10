using Harmony;

namespace ResourceMonitor.Patchers
{
    [HarmonyPatch(typeof(IngameMenu))]
    [HarmonyPatch("QuitGameAsync")]
    public class QuitGamePatcher
    {
        [HarmonyPostfix]
        public static void Postfix()
        {
            StorageContainerAwakePatcher.ClearRegisteredResourceMonitors();
        }
    }
}
