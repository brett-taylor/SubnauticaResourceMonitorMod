using UnityEngine;
using UnityEngine.EventSystems;

namespace ResourceMonitor.Components
{
    /**
     * Component that buttons on the resource monitor will inherit from. Handles working on whether something is hovered via IsHovered as well as interaction text.
     */
    public abstract class OnScreenButton : MonoBehaviour
    {
        public ResourceMonitorDisplay ResourceMonitorDisplay { get; set; }
        protected bool IsHovered { get; set; }
        protected string HoverText { get; set; }
        private bool isHoveredOutOfRange;

        public virtual void OnDisable()
        {
            IsHovered = false;
            isHoveredOutOfRange = false;
        }

        public virtual void Update()
        {
            var inInteractionRange = InInteractionRange();

            if (IsHovered && inInteractionRange)
            {
#if SUBNAUTICA
                HandReticle.main.SetInteractTextRaw(HoverText, "");
#elif BELOWZERO
                HandReticle.main.SetTextRaw(HandReticle.TextType.Hand, HoverText);
#endif
            }

            if (IsHovered && inInteractionRange == false)
            {
                IsHovered = false;
            }

            if (IsHovered == false && isHoveredOutOfRange && inInteractionRange)
            {
                IsHovered = true;
            }
        }

        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            if (InInteractionRange())
            {
                IsHovered = true;
            }

            isHoveredOutOfRange = true;
            ResourceMonitorDisplay.ResetIdleTimer();
        }

        public virtual void OnPointerExit(PointerEventData eventData)
        {
            IsHovered = false;
            isHoveredOutOfRange = false;
            ResourceMonitorDisplay.ResetIdleTimer();
        }

        public virtual void OnPointerClick(PointerEventData eventData)
        {
            ResourceMonitorDisplay.ResetIdleTimer();
        }

        protected bool InInteractionRange()
        {
            return Mathf.Abs(Vector3.Distance(gameObject.transform.position, Player.main.transform.position)) <= EntryPoint.SETTINGS.MaxInteractionDistance;
        }
    }
}
