using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ResourceMonitor
{
    /**
    * Component that will allow an event to be triggered when clicked and the cursor over the gameobject.
    */
    public class ClickHotzone : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public event Action OnPointerClickedEvent;
        public event Action OnPointerEnteredEvent;
        public event Action OnPointerExitedEvent;

        public void OnPointerClick(PointerEventData eventData) => OnPointerClickedEvent.Invoke();
        public void OnPointerEnter(PointerEventData eventData) => OnPointerEnteredEvent.Invoke();
        public void OnPointerExit(PointerEventData eventData) => OnPointerExitedEvent.Invoke();
    }
}
