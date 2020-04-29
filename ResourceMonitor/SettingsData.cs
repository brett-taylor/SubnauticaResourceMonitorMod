using System.Collections.Generic;
using UnityEngine;

namespace ResourceMonitor
{
    [System.Serializable]
    public class SettingsData
    {
        public bool AllowSelectingItemsFromMonitor = true;
        
        public Color PaginatorStartingColor = Color.white;
        
        public Color PaginatorHoverColor = new Color(0.07f, 0.38f, 0.7f, 1f);
        
        public float MaxInteractionDistance = 2.5f;
        
        public float MaxInteractionIdlePageDistance = 5f;

        public bool EnableIdle = true;
        
        public float IdleTime = 20f;
        
        public float IdleTimeRandomnessLowBound = 1f;
        
        public float IdleTimeRandomnessHighBound = 10f;
        
        public float IdleScreenColorTransitionTime = 2f;
        
        public float IdleScreenColorTransitionRandomnessHighBound = 2f;
        
        public float IdleScreenColorTransitionRandomnessLowBound = 0f;

        public List<Color> PossibleIdleColors = new List<Color>()
        {
            new Color(0.07f, 0.38f, 0.70f), // BLUE
            new Color(0.86f, 0.22f, 0.22f), // RED
            new Color(0.22f, 0.86f, 0.22f) // GREEN
        };
        
        public Color ItemButtonBackgroundColor = new Color(0.07843138f, 0.3843137f, 0.7058824f);
        
        public Color ItemButtonHoverColor = new Color(0.07843137f, 0.1459579f, 0.7058824f);
    }
}