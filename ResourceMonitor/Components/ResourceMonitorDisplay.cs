using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ResourceMonitor.Components
{
    /**
    * Component that contains the controller to the view. Majority of the view is set up already on the prefab inside of the unity editor.
    */
    public class ResourceMonitorDisplay : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public static readonly float MAX_INTERACTION_DISTANCE = 2.5f;
        private static readonly float WELCOME_ANIMATION_TIME = 8.5f;
        private static readonly float MAIN_SCREEN_ANIMATION_TIME = 3f;
        private static readonly bool QUICK_SHOW = false;
        private static readonly int ITEMS_PER_PAGE = 12;

        private ResourceMonitorLogic rml;
        private Dictionary<TechType, GameObject> trackedResourcesDisplayElements;
        private int currentPage;
        private int maxPage;

        private Animator animator;
        private GameObject canvasGameObject;
        private GameObject blackCover;
        private GameObject welcomeScreen;
        private GameObject mainScreen;
        private GameObject mainScreensCover;
        private GameObject mainScreenItemGrid;
        private GameObject previousPageGameObject;
        private GameObject nextPageGameObject;
        private GameObject pageCounterGameObject;
        private Text pageCounterText;

        public void Setup(ResourceMonitorLogic rml)
        {
            this.rml = rml;
            trackedResourcesDisplayElements = new Dictionary<TechType, GameObject>();

            if (FindAllComponents() == false)
            {
                TurnDisplayOff();
                return;
            }

            currentPage = 1;
            UpdatePaginator();

            if (QUICK_SHOW == false)
            {
                StartCoroutine(FinalSetup());
            }
            else
            {
                welcomeScreen.SetActive(false);
                blackCover.SetActive(false);
                mainScreen.SetActive(true);
                mainScreensCover.SetActive(false);
                DrawPage(1);
            }
        }

        private IEnumerator FinalSetup()
        {
            welcomeScreen.SetActive(false);
            blackCover.SetActive(false);
            mainScreen.SetActive(false);
            animator.Play("Reset");
            yield return new WaitForEndOfFrame();
            animator.Play("Welcome");
            yield return new WaitForSeconds(WELCOME_ANIMATION_TIME);
            animator.Play("ShowMainScreen");
            yield return new WaitForSeconds(MAIN_SCREEN_ANIMATION_TIME);
            welcomeScreen.SetActive(false);
            blackCover.SetActive(false);
            mainScreen.SetActive(true);
            DrawPage(1);
        }

        public void TurnDisplayOff()
        {
            StopCoroutine(FinalSetup());
            blackCover.SetActive(true);
            trackedResourcesDisplayElements.Clear();
            trackedResourcesDisplayElements = null;
        }

        public void ItemModified(TechType type, int newAmount)
        {
            if (newAmount > 0 && trackedResourcesDisplayElements.ContainsKey(type))
            {
                trackedResourcesDisplayElements[type].GetComponentInChildren<Text>().text = "x" + newAmount;
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

        public void ChangePageBy(int amount)
        {
            DrawPage(currentPage + amount);
        }

        private void DrawPage(int page)
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

        private void UpdatePaginator()
        {
            CalculateNewMaxPages();
            pageCounterText.text = string.Format("Page {0} Of {1}", currentPage, maxPage);
            previousPageGameObject.SetActive(currentPage != 1);
            nextPageGameObject.SetActive(currentPage != maxPage);
        }

        private void ClearPage()
        {
            for (int i = 0; i < mainScreenItemGrid.transform.childCount; i++)
            {
                Destroy(mainScreenItemGrid.transform.GetChild(i).gameObject);
            }

            if (trackedResourcesDisplayElements != null)
            {
                trackedResourcesDisplayElements.Clear();
            }
        }

        private void CreateAndAddItemDisplay(TechType type, int amount)
        {
            GameObject itemDisplay = Instantiate(EntryPoint.ResourceMonitorDisplayItemUIPrefab);
            itemDisplay.transform.SetParent(mainScreenItemGrid.transform, false);
            itemDisplay.GetComponentInChildren<Text>().text = "x" + amount;

            ItemButton itemButton = itemDisplay.AddComponent<ItemButton>();
            itemButton.Type = type;
            itemButton.Amount = amount;

            uGUI_Icon icon = itemDisplay.transform.Find("ItemHolder").gameObject.AddComponent<uGUI_Icon>();
            icon.sprite = SpriteManager.Get(type);

            trackedResourcesDisplayElements.Add(type, itemDisplay);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
        }

        public void OnPointerExit(PointerEventData eventData)
        {
        }

        private bool FindAllComponents()
        {
            canvasGameObject = gameObject.GetComponentInChildren<Canvas>()?.gameObject;
            if (canvasGameObject == null)
            {
                System.Console.WriteLine("[ResourceMonitor] Canvas not found.");
                return false;
            }

            animator = canvasGameObject.GetComponent<Animator>();
            if (animator == null)
            {
                System.Console.WriteLine("[ResourceMonitor] Animator not found.");
                return false;
            }

            blackCover = canvasGameObject.FindChild("BlackCover")?.gameObject;
            if (blackCover == null)
            {
                System.Console.WriteLine("[ResourceMonitor] BlackCover not found.");
                return false;
            }

            GameObject screenHolder = canvasGameObject.transform.Find("Screens")?.gameObject;
            if (screenHolder == null)
            {
                System.Console.WriteLine("[ResourceMonitor] Screen Holder Gameobject not found.");
                return false;
            }

            welcomeScreen = screenHolder.FindChild("WelcomeScreen")?.gameObject;
            if (welcomeScreen == null)
            {
                System.Console.WriteLine("[ResourceMonitor] Screen: WelcomeScreen not found.");
                return false;
            }

            mainScreen = screenHolder.FindChild("MainScreen")?.gameObject;
            if (mainScreen == null)
            {
                System.Console.WriteLine("[ResourceMonitor] Screen: MainScreen not found.");
                return false;
            }

            mainScreensCover = mainScreen.FindChild("BlackCover")?.gameObject;
            if (mainScreensCover == null)
            {
                System.Console.WriteLine("[ResourceMonitor] Screen: MainScreen Cover not found.");
                return false;
            }

            GameObject actualMainScreen = mainScreen.FindChild("ActualScreen")?.gameObject;
            if (actualMainScreen == null)
            {
                System.Console.WriteLine("[ResourceMonitor] Screen: Actual Main Screen not found.");
                return false;
            }

            mainScreenItemGrid = actualMainScreen.FindChild("MainGrid")?.gameObject;
            if (mainScreenItemGrid == null)
            {
                System.Console.WriteLine("[ResourceMonitor] Screen: Main Screen Item Grid not found.");
                return false;
            }

            GameObject paginator = actualMainScreen.FindChild("Paginator")?.gameObject;
            if (paginator == null)
            {
                System.Console.WriteLine("[ResourceMonitor] Screen: Paginator not found.");
                return false;
            }

            previousPageGameObject = paginator.FindChild("PreviousPage")?.gameObject;
            if (previousPageGameObject == null)
            {
                System.Console.WriteLine("[ResourceMonitor] Screen: Previous Page GameObject not found.");
                return false;
            }
            PaginatorButton pb = previousPageGameObject.AddComponent<PaginatorButton>();
            pb.ResourceMonitorDisplay = this;
            pb.AmountToChangePageBy = -1;

            nextPageGameObject = paginator.FindChild("NextPage")?.gameObject;
            if (nextPageGameObject == null)
            {
                System.Console.WriteLine("[ResourceMonitor] Screen: Next Page GameObject not found.");
                return false;
            }
            PaginatorButton pb2 = nextPageGameObject.AddComponent<PaginatorButton>();
            pb2.ResourceMonitorDisplay = this;
            pb2.AmountToChangePageBy = 1;

            pageCounterGameObject = paginator.FindChild("PageCounter")?.gameObject;
            if (pageCounterGameObject == null)
            {
                System.Console.WriteLine("[ResourceMonitor] Screen: Page Counter GameObject not found.");
                return false;
            }

            pageCounterText = pageCounterGameObject.GetComponent<Text>();
            if (pageCounterText == null)
            {
                System.Console.WriteLine("[ResourceMonitor] Screen: Page Counter Text not found.");
                return false;
            }

            return true;
        }
    }
}