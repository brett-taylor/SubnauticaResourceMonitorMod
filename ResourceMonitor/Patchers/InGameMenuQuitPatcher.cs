using System;
using Harmony;

namespace ResourceMonitor.Patchers
{
    /**
     * Patches into StorageContainer::Awake method to alert our Resource monitor about the newly placed storage container
     */
    [HarmonyPatch(typeof(IngameMenu))]
    [HarmonyPatch("QuitGame")]
    public class InGameMenuQuitPatcher
    {
        private static Action onQuitButtonPressed;

        [HarmonyPostfix]
        public static void Postfix()
        {
            if (onQuitButtonPressed != null)
            {
                onQuitButtonPressed.Invoke();
            }

            onQuitButtonPressed = null;
        }

        public static bool AddEventHandlerIfMissing(Action newHandler)
        {
            if (onQuitButtonPressed == null)
            {
                onQuitButtonPressed += newHandler;
                return true;
            }
            else
            {
                foreach (Action action in onQuitButtonPressed.GetInvocationList())
                {
                    if (action == newHandler)
                    {
                        return false;
                    }
                }

                onQuitButtonPressed += newHandler;
                return true;
            }
        }
    }
}
