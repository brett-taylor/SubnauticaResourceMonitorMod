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
        protected string TextLineOne { get; set; }
        protected string TextLineTwo { get; set; }

        public virtual void OnDisable()
        {
            IsHovered = false;
        }

        public virtual void Update()
        {
            if (IsHovered && InInteractionRange())
            {
                HandReticle.main.SetInteractTextRaw(TextLineOne, TextLineTwo);
            }
        }

        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            if (InInteractionRange())
            {
                IsHovered = true;
            }

            ResourceMonitorDisplay.ResetIdleTimer();
        }

        public virtual void OnPointerExit(PointerEventData eventData)
        {
            IsHovered = false;
            ResourceMonitorDisplay.ResetIdleTimer();
        }

        public virtual void OnPointerClick(PointerEventData eventData)
        {
            ResourceMonitorDisplay.ResetIdleTimer();
        }

        protected bool InInteractionRange()
        {
            return Mathf.Abs(Vector3.Distance(gameObject.transform.position, Player.main.transform.position)) <= ResourceMonitorDisplay.MAX_INTERACTION_DISTANCE;
        }
    }
}
