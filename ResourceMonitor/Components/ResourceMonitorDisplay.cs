using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ResourceMonitor
{
    /**
    * Component that contains drawing to the frame in world space.
    * Create canvas code is from https://github.com/RandyKnapp/
    */
    public class ResourceMonitorDisplay : MonoBehaviour, IPointerClickHandler, IEventSystemHandler, IPointerEnterHandler, IPointerExitHandler
    {
        private const int ITEMS_PER_PAGE = 8;

        private ResourceMonitorLogic rml;
        private Canvas canvas;
        private GameObject ui;
        private GameObject itemGrid;
        private Dictionary<TechType, GameObject> itemDisplayElements = new Dictionary<TechType, GameObject>();

        public void Setup(Transform parent, ResourceMonitorLogic rml)
        {
            this.rml = rml;
            itemDisplayElements = new Dictionary<TechType, GameObject>();

            CreateCanvas(parent);
            ui = Instantiate(EntryPoint.ResourceMonitorDisplayUIPrefab);
            ui.transform.SetParent(canvas.transform, false);
            ui.GetComponent<RectTransform>().localScale = new Vector3(0.01f, 0.01f, 1f);
            itemGrid = ui.transform.Find("ItemGridBackground").gameObject;
        }

        public void Destroy()
        {
            itemDisplayElements.Clear();
            itemDisplayElements = null;
            Destroy(canvas);
            canvas = null;
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

        public void ItemModified(TechType item, int newQuantity)
        {
            if (newQuantity <= 0)
            {
                Destroy(itemDisplayElements[item]);
                itemDisplayElements.Remove(item);
            }
            else
            {
                if (itemDisplayElements.ContainsKey(item))
                {
                    itemDisplayElements[item].GetComponentInChildren<Text>().text = "x" + newQuantity;
                }
                else
                {
                    GameObject itemDisplay = Instantiate(EntryPoint.ResourceMonitorDisplayItemUIPrefab);
                    itemDisplay.transform.SetParent(itemGrid.transform, false);
                    itemDisplay.GetComponentInChildren<Text>().text = "x" + newQuantity;

                    var icon = itemDisplay.transform.Find("ItemHolder").gameObject.AddComponent<uGUI_Icon>();
                    icon.sprite = SpriteManager.Get(item);
                    icon.SetAllDirty();

                    itemDisplayElements.Add(item, itemDisplay);
                }
            }

            UpdatePaginator();
        }

        private void UpdatePaginator()
        {
        }

        private void CreateCanvas(Transform parent)
        {
            canvas = new GameObject("ReosurceMonitorDisplayCanvas", new Type[] { typeof(RectTransform) }).AddComponent<Canvas>();
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

            // If we dont add something to the canvas then our PointerEnter/Exit/Click events wont work. Idk why.
            Image background = new GameObject("ResourceMonitorDisplayBackground", new Type[] { typeof(RectTransform) }).AddComponent<Image>();
            RectTransform rectTransform = background.rectTransform;
            RectTransformExtensions.SetParams(rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), canvas.transform);
            RectTransformExtensions.SetSize(rectTransform, 0, 0);
            background.color = Color.black;
            background.transform.localScale = new Vector3(0.01f, 0.01f, 1f);
            background.type = Image.Type.Sliced;
        }
    }
}

/*
ErrorMessage.AddMessage("Called");
GameObject grid = ui.transform.Find("ItemGridBackground").gameObject;
ErrorMessage.AddMessage("Grid Found: " + (grid != null));
ErrorMessage.AddMessage("Grid Children Found: " + grid.transform.childCount);
foreach (Transform resourceMonitorItem in grid.transform)
{
    GameObject itemHolder = resourceMonitorItem.Find("ItemHolder").gameObject;
    ErrorMessage.AddMessage("Is ItemHolder valid:" + (itemHolder != null));

    Atlas.Sprite sprite = SpriteManager.Get(TechType.Beacon);
    var icon = itemHolder.AddComponent<uGUI_Icon>();
    ErrorMessage.AddMessage("Icon added: " + (icon != null));
    icon.sprite = sprite;
    icon.SetAllDirty();
}
ErrorMessage.AddMessage("Finished Called");
*/
