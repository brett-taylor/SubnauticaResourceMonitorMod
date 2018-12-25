using System;
using System.Collections.Generic;
using System.Linq;
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
        private const int ITEMS_PER_PAGE = 12;
        private ResourceMonitorLogic rml;
        private Dictionary<TechType, GameObject> trackedResourcesDisplayElements = new Dictionary<TechType, GameObject>();
        private int currentPage;
        private int maxPage;

        private Canvas canvas;
        private GameObject ui;
        private GameObject itemGrid;
        private GameObject prevPage;
        private GameObject nextPage;
        private GameObject pageCounter;

        public void Setup(Transform parent, ResourceMonitorLogic rml)
        {
            this.rml = rml;
            trackedResourcesDisplayElements = new Dictionary<TechType, GameObject>();

            CreateCanvas(parent);
            ui = Instantiate(EntryPoint.ResourceMonitorDisplayUIPrefab);
            ui.transform.SetParent(canvas.transform, false);
            ui.GetComponent<RectTransform>().localScale = new Vector3(0.01f, 0.01f, 1f);
            itemGrid = ui.transform.Find("ItemGridBackground").gameObject;

            prevPage = ui.transform.Find("PreviousPage").gameObject;
            nextPage = ui.transform.Find("NextPage").gameObject;
            pageCounter = ui.transform.Find("PageCounter").gameObject;

            Text prevPageText = prevPage.GetComponent<Text>();
            Text nextPageText = nextPage.GetComponent<Text>();
            ClickHotzone prevPageClickZone = prevPage.AddComponent<ClickHotzone>();
            ClickHotzone nextPageClickZone = nextPage.AddComponent<ClickHotzone>();
            prevPageClickZone.OnPointerEnteredEvent += () => PaginatorButtonMouseEntered(prevPageText);
            nextPageClickZone.OnPointerEnteredEvent += () => PaginatorButtonMouseEntered(nextPageText);
            prevPageClickZone.OnPointerExitedEvent += () => PaginatorButtonMouseExited(prevPageText);
            nextPageClickZone.OnPointerExitedEvent += () => PaginatorButtonMouseExited(nextPageText);
            prevPageClickZone.OnPointerClickedEvent += () => PaginatorButtonMouseClicked(prevPageText, currentPage - 1);
            nextPageClickZone.OnPointerClickedEvent += () => PaginatorButtonMouseClicked(nextPageText, currentPage + 1);

            FinalSetup();
        }

        // To be called always after the display is done creating all other objects it needs.
        private void FinalSetup()
        {
            currentPage = 1;
            CalculateNewMaxPages();
            UpdatePaginator();
            DrawPage(1);
        }

        public void Destroy()
        {
            trackedResourcesDisplayElements.Clear();
            trackedResourcesDisplayElements = null;
            Destroy(canvas);
            canvas = null;
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.O))
            {
                DrawPage(currentPage - 1);
            }
            else if (Input.GetKeyDown(KeyCode.P))
            {
                DrawPage(currentPage + 1);
            }
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

        public void ItemModified(TechType type, int newQuantity)
        {
            if (newQuantity > 0)
            {
                if (trackedResourcesDisplayElements.ContainsKey(type))
                {
                    trackedResourcesDisplayElements[type].GetComponentInChildren<Text>().text = "x" + newQuantity;
                }
                else
                {
                    // TO:DO If were meant to show it right now redraw that page.
                    // If we are not meant to show it right now at least execute UpdatePaginator();
                    DrawPage(currentPage);
                }
            }
            else
            {
                DrawPage(currentPage);
            }
        }

        private void CalculateNewMaxPages()
        {
            maxPage = Mathf.CeilToInt((rml.TrackedResources.Count - 1) / ITEMS_PER_PAGE) + 1;
            if (currentPage > maxPage)
            {
                currentPage = maxPage;
            }
        }

        public void DrawPage(int page)
        {
            currentPage = page;
            if (currentPage <= 0)
            {
                currentPage = 1;
            }
            else if (currentPage > maxPage)
            {
                currentPage = maxPage;
            }

            int startingPosition = (currentPage - 1) * ITEMS_PER_PAGE;
            int endingPosition = startingPosition + ITEMS_PER_PAGE;
            if (endingPosition > rml.TrackedResources.Count)
            {
                endingPosition = rml.TrackedResources.Count;
            }

            ClearPage();
            for (int i = startingPosition; i < endingPosition; i++)
            {
                KeyValuePair<TechType, int> kvp = rml.TrackedResources.ElementAt(i);
                CreateAndAddItemDisplay(kvp.Key, kvp.Value);
            }

            UpdatePaginator();
        }

        private void ClearPage()
        {
            foreach (GameObject go in trackedResourcesDisplayElements.Values)
            {
                Destroy(go);
            }

            trackedResourcesDisplayElements.Clear();
        }

        private void CreateAndAddItemDisplay(TechType type, int amount)
        {
            GameObject itemDisplay = Instantiate(EntryPoint.ResourceMonitorDisplayItemUIPrefab);
            itemDisplay.transform.SetParent(itemGrid.transform, false);
            itemDisplay.GetComponentInChildren<Text>().text = "x" + amount;

            var icon = itemDisplay.transform.Find("ItemHolder").gameObject.AddComponent<uGUI_Icon>();
            icon.sprite = SpriteManager.Get(type);

            trackedResourcesDisplayElements.Add(type, itemDisplay);
        }

        private void UpdatePaginator()
        {
            CalculateNewMaxPages();
            pageCounter.GetComponent<Text>().text = string.Format("Page {0} Of {1}", currentPage, maxPage);
            prevPage.SetActive(currentPage != 1);
            nextPage.SetActive(currentPage != maxPage);
        }

        private void PaginatorButtonMouseEntered(Text text)
        {
            text.color = Color.red;
            HandReticle.main.SetInteractText(text.text);
        }

        private void PaginatorButtonMouseExited(Text text)
        {
            text.color = Color.white;
            HandReticle.main.SetInteractText("");
        }

        private void PaginatorButtonMouseClicked(Text text, int page)
        {
            text.color = Color.white;
            DrawPage(page);
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