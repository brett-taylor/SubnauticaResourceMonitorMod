using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ResourceMonitor
{
    /**
    * Component that contains drawing to the frame in world space.
    * Alot of the drawing ui code is from https://github.com/RandyKnapp/
    */
    public class ResourceMonitorDisplay : MonoBehaviour, IPointerClickHandler, IEventSystemHandler, IPointerEnterHandler, IPointerExitHandler
    {
        private static float WIDTH = 260f;
        private static float HEIGHT = 140f;

        private ResourceMonitorLogic rml;
        private Canvas canvas;
        private Image background;

        public void Update()
        {
        }

        public void Setup(Transform parent, ResourceMonitorLogic rml)
        {
            this.rml = rml;
            CreateCanvas(parent);
            CreateBackground();
            CreateText("Resource Monitor Screen", y: -20, size: 14);
        }

        public void Destroy()
        {
            Destroy(canvas);
            Destroy(background);

            canvas = null;
            background = null;
        }

        private void CreateCanvas(Transform parent)
        {
            canvas = new GameObject("Canvas", new Type[] { typeof(RectTransform) }).AddComponent<Canvas>();
            Transform transform = canvas.transform;
            transform.SetParent(parent, false);
            canvas.sortingLayerID = 1;

            uGUI_GraphicRaycaster uGUI_GraphicRaycaster = canvas.gameObject.AddComponent<uGUI_GraphicRaycaster>();
            RectTransform rt = transform as RectTransform;
            RectTransformExtensions.SetParams(rt, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), null);
            RectTransformExtensions.SetSize(rt, 5f, 2f);
            transform.localPosition = new Vector3(0f, 0f, 0.015f);
            transform.localRotation = new Quaternion(0f, 1f, 0f, 0f);
            transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            canvas.scaleFactor = 0.01f;
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.referencePixelsPerUnit = 100f;
            CanvasScaler canvasScaler = canvas.gameObject.AddComponent<CanvasScaler>();
            canvasScaler.dynamicPixelsPerUnit = 20f;
        }

        private void CreateBackground()
        {
            background = new GameObject("Background", new Type[] { typeof(RectTransform) }).AddComponent<Image>();
            RectTransform rectTransform = background.rectTransform;
            RectTransformExtensions.SetParams(rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), canvas.transform);
            RectTransformExtensions.SetSize(rectTransform, WIDTH, HEIGHT);
            background.color = new Color(1f, 0f, 0f);
            background.transform.localScale = new Vector3(0.01f, 0.01f, 1f);
            background.type = Image.Type.Sliced;
        }

        private void CreateText(string initial, int y, int size)
        {
            Text text = new GameObject("Text", new Type[] { typeof(RectTransform) }).AddComponent<Text>();
            RectTransform rectTransform = text.rectTransform;
            RectTransformExtensions.SetParams(rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), background.transform);
            RectTransformExtensions.SetSize(rectTransform, WIDTH, HEIGHT);
            rectTransform.anchoredPosition = new Vector2(0f, y);
            text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            text.fontSize = size;
            text.color = Color.white;
            text.alignment = TextAnchor.MiddleCenter;
            text.text = initial;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            rml.OnPointerClick();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
        }

        public void OnPointerExit(PointerEventData eventData)
        {
        }
    }
}
