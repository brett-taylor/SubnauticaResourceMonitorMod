using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace ResourceMonitor
{
    /**
    * Component that contains drawing to the frame in world space.
    * Create canvas code is from https://github.com/RandyKnapp/
    */
    public class ResourceMonitorDisplay : MonoBehaviour
    {
        private const int ITEMS_PER_PAGE = 12;
        private const float MAX_INTERACTION_DISTANCE = 2f;

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
        private Text prevPageText;
        private Text nextPageText;
        private ClickHotzone activeClickHotzone = null;

        public void Setup(Transform parent, ResourceMonitorLogic rml)
        {
            this.rml = rml;
            trackedResourcesDisplayElements = new Dictionary<TechType, GameObject>();

            CreateCanvas(parent);
            ui = Instantiate(EntryPoint.ResourceMonitorDisplayUIPrefab);
            ui.transform.SetParent(canvas.transform, false);
            ui.GetComponent<RectTransform>().localScale = new Vector3(0.01f, 0.01f, 1f);
            itemGrid = ui.transform.Find("ItemGridBackground").gameObject;

            FindRequiredUIElements();
            InitialiseClickHotzones();
            FinalSetup();
        }

        private void FindRequiredUIElements()
        {
            prevPage = ui.transform.Find("PreviousPage").gameObject;
            nextPage = ui.transform.Find("NextPage").gameObject;
            prevPageText = prevPage.GetComponent<Text>();
            nextPageText = nextPage.GetComponent<Text>();
            pageCounter = ui.transform.Find("PageCounter").gameObject;
        }

        private void InitialiseClickHotzones()
        {
            ClickHotzone prevPageClickZone = prevPage.AddComponent<ClickHotzone>();
            prevPageClickZone.HoveredMessage = "Previous Page";

            ClickHotzone nextPageClickZone = nextPage.AddComponent<ClickHotzone>();
            nextPageClickZone.HoveredMessage = "Next Page";

            prevPageClickZone.OnPointerEnteredEvent += () => PaginatorButtonMouseEntered(prevPageText, prevPageClickZone);
            nextPageClickZone.OnPointerEnteredEvent += () => PaginatorButtonMouseEntered(nextPageText, nextPageClickZone);
            prevPageClickZone.OnPointerExitedEvent += () => PaginatorButtonMouseExited(prevPageText);
            nextPageClickZone.OnPointerExitedEvent += () => PaginatorButtonMouseExited(nextPageText);
            prevPageClickZone.OnPointerClickedEvent += () => PaginatorButtonMouseClicked(prevPageText, currentPage - 1, prevPageClickZone);
            nextPageClickZone.OnPointerClickedEvent += () => PaginatorButtonMouseClicked(nextPageText, currentPage + 1, nextPageClickZone);
        }

        private void FinalSetup()
        {
            currentPage = 1;
            CalculateNewMaxPages();
            UpdatePaginator();
            DrawPage(1);
        }

        public void Destroy()
        {
            activeClickHotzone = null;
            trackedResourcesDisplayElements.Clear();
            trackedResourcesDisplayElements = null;
            Destroy(canvas);
            canvas = null;
        }

        public void ItemModified(TechType type, int newQuantity)
        {
            if (newQuantity > 0 && trackedResourcesDisplayElements.ContainsKey(type))
            {
                trackedResourcesDisplayElements[type].GetComponentInChildren<Text>().text = "x" + newQuantity;
                trackedResourcesDisplayElements[type].GetComponentInChildren<ClickHotzone>().HoveredSubMessage = "x" + newQuantity;
                return;
            }

            DrawPage(currentPage);
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

            uGUI_Icon icon = itemDisplay.transform.Find("ItemHolder").gameObject.AddComponent<uGUI_Icon>();
            icon.sprite = SpriteManager.Get(type);

            ClickHotzone clickHotzone = itemDisplay.AddComponent<ClickHotzone>();
            clickHotzone.HoveredMessage = TechTypeExtensions.Get(Language.main, type);
            clickHotzone.HoveredSubMessage = "x" + amount;
            clickHotzone.OnPointerEnteredEvent += () => ItemMouseEntered(clickHotzone);
            clickHotzone.OnPointerExitedEvent += () => ItemMouseExited(clickHotzone);

            trackedResourcesDisplayElements.Add(type, itemDisplay);
        }

        private void UpdatePaginator()
        {
            CalculateNewMaxPages();
            pageCounter.GetComponent<Text>().text = string.Format("Page {0} Of {1}", currentPage, maxPage);
            prevPage.SetActive(currentPage != 1);
            nextPage.SetActive(currentPage != maxPage);

            if (prevPage.activeSelf == false)
            {
                prevPageText.color = Color.white;
            }

            if (nextPage.activeSelf == false)
            {
                nextPageText.color = Color.white;
            }
        }

        public void Update()
        {
            if (activeClickHotzone != null)
            {
                HandReticle.main.SetInteractTextRaw(activeClickHotzone.HoveredMessage, activeClickHotzone.HoveredSubMessage);
                if (InInteractionRange() == false)
                {
                    activeClickHotzone = null;
                }
            }
        }

        private void PaginatorButtonMouseEntered(Text text, ClickHotzone clickHotzone)
        {
            if (InInteractionRange() == false)
            {
                return;
            }

            text.color = Color.red;
            activeClickHotzone = clickHotzone;
        }

        private void PaginatorButtonMouseExited(Text text)
        {
            text.color = Color.white;
            activeClickHotzone = null;
        }

        private void PaginatorButtonMouseClicked(Text text, int page, ClickHotzone clickHotzone)
        {
            if (InInteractionRange() == false)
            {
                return;
            }

            DrawPage(page);
            if (clickHotzone == activeClickHotzone)
            {
                if (activeClickHotzone.gameObject == null || activeClickHotzone.gameObject.activeSelf == false)
                {
                    text.color = Color.white;
                    activeClickHotzone = null;
                }
            }
        }

        private void ItemMouseEntered(ClickHotzone clickHotzone)
        {
            if (InInteractionRange() == false)
            {
                return;
            }

            activeClickHotzone = clickHotzone;
        }

        private void ItemMouseExited(ClickHotzone clickHotzone)
        {
            activeClickHotzone = null;
        }

        private Boolean InInteractionRange()
        {
            return Mathf.Abs(Vector3.Distance(canvas.transform.position, Player.main.transform.position)) <= MAX_INTERACTION_DISTANCE;
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