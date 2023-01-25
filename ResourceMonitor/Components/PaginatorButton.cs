using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace ResourceMonitor.Components
{
    /**
     * Component that will be added onto the paginator buttons. Gives the hover effect and clicking function.
     */
    public class PaginatorButton : OnScreenButton, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
    {
        public int AmountToChangePageBy = 1;
        private Text text;

        public void Start()
        {
            text = GetComponent<Text>();
            HoverText = text.text;
        }

        public void OnEnable()
        {
            if (text != null)
            {
                text.color = Plugin.PaginatorStartingColor.Value;
            }
        }

        public override void OnDisable()
        {
            if (text != null)
            {
                text.color = Plugin.PaginatorStartingColor.Value;
            }
            base.OnDisable();
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);
            if (IsHovered)
            {
                text.color = Plugin.PaginatorHoverColor.Value;
            }
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);
            text.color = Plugin.PaginatorStartingColor.Value;
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            base.OnPointerClick(eventData);
            if (IsHovered)
            {
                ResourceMonitorDisplay.ChangePageBy(AmountToChangePageBy);
            }
        }
    }
}
