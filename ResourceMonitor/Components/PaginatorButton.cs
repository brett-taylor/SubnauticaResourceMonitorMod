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
        private static readonly Color HOVER_COLOR = new Color(0.07f, 0.38f, 0.7f, 1f);
        private static Color STARTING_COLOR = Color.white;

        public int AmountToChangePageBy { get; set; } = 1;
        private Text text;

        public void Start()
        {
            text = GetComponent<Text>();
            STARTING_COLOR = text.color;

            TextLineOne = text.text;
            TextLineTwo = null;
        }

        public void OnEnable()
        {
            if (text != null)
            {
                text.color = STARTING_COLOR;
            }
        }

        public override void OnDisable()
        {
            if (text != null)
            {
                text.color = STARTING_COLOR;
            }
            base.OnDisable();
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);
            if (IsHovered)
            {
                text.color = HOVER_COLOR;
            }
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);
            text.color = STARTING_COLOR;
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
