using Harmony;
using System.Reflection;

namespace ResourceMonitor
{
    /**
     * Entry class.
     */
    public class EntryPoint
    {
        /**
        * Entry method.
        */
        public static void Entry()
        {
            HarmonyInstance harmony = HarmonyInstance.Create("taylor.brett.ResourceMonitor.mod");
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            ResourceMonitorScreen.Singleton.Patch();
        }
    }
}
