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
        public TechType Type
        {
            set
            {
                TextLineOne = TechTypeExtensions.Get(Language.main, value);
            }
        }

        public int Amount
        {
            set
            {
                TextLineTwo = "x" + value;
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
        }
    }
}
