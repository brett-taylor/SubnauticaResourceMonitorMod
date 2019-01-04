using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace ResourceMonitor.Components
{
    /**
     * Component that will be added onto the item button.
     */
    public class ItemButton : OnScreenButton, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        private TechType type = TechType.None;
        private int amount = 0;

        public TechType Type
        {
            set
            {
                TextLineOne = "Take " + TechTypeExtensions.Get(Language.main, value);
                type = value;
            }

            get
            {
                return type;
            }
        }

        public int Amount
        {
            set
            {
                TextLineTwo = "x" + value;
                amount = value;
            }

            get
            {
                return amount;
            }
        }

        public override void OnDisable()
        {
            base.OnDisable();
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            base.OnPointerClick(eventData);
            if (IsHovered && ResourceMonitorDisplay != null && ResourceMonitorDisplay.ResourceMonitorLogic != null && type != TechType.None)
            {
                ResourceMonitorDisplay.ResourceMonitorLogic.AttemptToTakeItem(type);
            }
        }
    }
}
