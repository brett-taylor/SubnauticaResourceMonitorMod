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
    * Handles such things as the paginator, drawing all the items that are on the "current page"
    * Handles the idle screen saver.
    * Handles the welcome animations.
    */
    public class ResourceMonitorDisplay : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public static List<Color> PossibleIdleColors = new List<Color>()
        {
            new Color(0.07f, 0.38f, 0.70f), // BLUE
            new Color(0.86f, 0.22f, 0.22f), // RED
            new Color(0.22f, 0.86f, 0.22f) // GREEN
        };

        private static readonly float WELCOME_ANIMATION_TIME = 8.5f;
        private static readonly float MAIN_SCREEN_ANIMATION_TIME = 1.2f;
        private static readonly int ITEMS_PER_PAGE = 12;

        public ResourceMonitorLogic ResourceMonitorLogic { get; private set; }
        private Dictionary<TechType, GameObject> trackedResourcesDisplayElements;
        public int currentPage = 1;
        public int maxPage = 1;
        private float idlePeriodLength = Plugin.IdleTime.Value;
        private float timeSinceLastInteraction = 0f;
        private bool isIdle = false;
        private float nextColorTransitionCurrentTime;
        private float transitionIdleTime;
        private Color currentColor = PossibleIdleColors[0];
        private Color nextColor = PossibleIdleColors[1];
        private bool isHovered = false;
        private bool isHoveredOutOfRange = false;

        public GameObject CanvasGameObject { get; private set; }
        private Animator animator;
        private GameObject blackCover;
        private GameObject welcomeScreen;
        private GameObject mainScreen;
        private GameObject mainScreensCover;
        private GameObject mainScreenItemGrid;
        private GameObject previousPageGameObject;
        private GameObject nextPageGameObject;
        private GameObject pageCounterGameObject;
        private Text pageCounterText;
        private GameObject idleScreen;
        private Image idleScreenTitleBackgroundImage;

        public void Setup(ResourceMonitorLogic rml)
        {
            if (rml.IsBeingDeleted == true) return;

            ResourceMonitorLogic = rml;
            trackedResourcesDisplayElements = new Dictionary<TechType, GameObject>();

            if (FindAllComponents() == false)
            {
                TurnDisplayOff();
                return;
            }

            CalculateNewColourTransitionTime();
            CalculateNewIdleTime();
            currentPage = 1;
            UpdatePaginator();

            StartCoroutine(FinalSetup());
        }

        private IEnumerator FinalSetup()
        {
            animator.enabled = true;
            welcomeScreen.SetActive(false);
            blackCover.SetActive(false);
            mainScreen.SetActive(false);
            animator.Play("Reset");

            yield return new WaitForEndOfFrame();
            if (ResourceMonitorLogic.IsBeingDeleted == true) yield break;

            animator.Play("Welcome");

            if (ResourceMonitorLogic.IsBeingDeleted == true) yield break;
            yield return new WaitForSeconds(WELCOME_ANIMATION_TIME);
            if (ResourceMonitorLogic.IsBeingDeleted == true) yield break;

            animator.Play("ShowMainScreen");
            DrawPage(1);

            if (ResourceMonitorLogic.IsBeingDeleted == true) yield break;
            yield return new WaitForSeconds(MAIN_SCREEN_ANIMATION_TIME);
            if (ResourceMonitorLogic.IsBeingDeleted == true) yield break;

            welcomeScreen.SetActive(false);
            blackCover.SetActive(false);
            mainScreen.SetActive(true);
            animator.enabled = false;
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
            maxPage = Mathf.CeilToInt((ResourceMonitorLogic.TrackedResources.Count - 1) / ITEMS_PER_PAGE) + 1;
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

            var startingPosition = (currentPage - 1) * ITEMS_PER_PAGE;
            var endingPosition = startingPosition + ITEMS_PER_PAGE;
            if (endingPosition > ResourceMonitorLogic.TrackedResources.Count)
            {
                endingPosition = ResourceMonitorLogic.TrackedResources.Count;
            }

            ClearPage();
            for (var i = startingPosition; i < endingPosition; i++)
            {
                var kvp = ResourceMonitorLogic.TrackedResources.ElementAt(i);
                CreateAndAddItemDisplay(kvp.Key, kvp.Value.Amount);
            }

            UpdatePaginator();
        }

        private void UpdatePaginator()
        {
            CalculateNewMaxPages();
            pageCounterText.text = $"Page {currentPage} Of {maxPage}";
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
            var itemDisplay = Instantiate(Plugin.RESOURCE_MONITOR_DISPLAY_ITEM_UI_PREFAB);
            itemDisplay.transform.SetParent(mainScreenItemGrid.transform, false);
            itemDisplay.GetComponentInChildren<Text>().text = "x" + amount;

            var itemButton = itemDisplay.AddComponent<ItemButton>();
            itemButton.Type = type;
            itemButton.Amount = amount;
            itemButton.ResourceMonitorDisplay = this;

            var icon = itemDisplay.transform.Find("ItemHolder").gameObject.AddComponent<uGUI_Icon>();
            icon.sprite = SpriteManager.Get(type);

            trackedResourcesDisplayElements.Add(type, itemDisplay);
        }

        public void Update()
        {
            if (isIdle == false && timeSinceLastInteraction < idlePeriodLength)
            {
                timeSinceLastInteraction += Time.deltaTime;
            }

            if (Plugin.EnableIdle.Value && isIdle == false && timeSinceLastInteraction >= idlePeriodLength)
            {
                EnterIdleScreen();
            }

            if (isHovered == false && isHoveredOutOfRange == true && InIdleInteractionRange() == true)
            {
                isHovered = true;
                ExitIdleScreen();
            }

            if (isHovered == true)
            {
                ResetIdleTimer();
            }

            if (isIdle == true)
            {
                if (nextColorTransitionCurrentTime >= transitionIdleTime)
                {
                    nextColorTransitionCurrentTime = 0f;
                    for (int i = 0; i < PossibleIdleColors.Count; i++)
                    {
                        if (PossibleIdleColors[i] == nextColor)
                        {
                            i++;
                            currentColor = nextColor;
                            if (i >= PossibleIdleColors.Count)
                            {
                                i = 0;
                            }
                            nextColor = PossibleIdleColors[i];
                            CalculateNewColourTransitionTime();
                        }
                    }
                }

                nextColorTransitionCurrentTime += Time.deltaTime;
                idleScreenTitleBackgroundImage.color = Color.Lerp(currentColor, nextColor, nextColorTransitionCurrentTime / transitionIdleTime);
            }
        }

        private bool InIdleInteractionRange()
        {
            return Mathf.Abs(Vector3.Distance(gameObject.transform.position, Player.main.transform.position)) <= Plugin.MaxInteractionIdlePageDistance.Value;
        }
        
        public void OnPointerClick(PointerEventData eventData)
        {
            if (isIdle && InIdleInteractionRange())
            {
                ExitIdleScreen();
            }
            
            if (isIdle == false)
            {
                ResetIdleTimer();
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            isHoveredOutOfRange = true;
            if (InIdleInteractionRange())
            {
                isHovered = true;
            }

            if (isIdle && InIdleInteractionRange())
            {
                ExitIdleScreen();
            }

            if (isIdle == false)
            {
                ResetIdleTimer();
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            isHoveredOutOfRange = false;
            isHovered = false;
            if (isIdle && InIdleInteractionRange())
            {
                ExitIdleScreen();
            }

            if (isIdle == false)
            {
                ResetIdleTimer();
            }
        }

        private void EnterIdleScreen()
        {
            isIdle = true;
            mainScreen.SetActive(false);
            idleScreen.SetActive(true);
        }

        private void ExitIdleScreen()
        {
            isIdle = false;
            ResetIdleTimer();
            CalculateNewIdleTime();
            mainScreen.SetActive(true);
            idleScreen.SetActive(false);
        }
        
        private void CalculateNewIdleTime()
        {
            idlePeriodLength = Plugin.IdleTime.Value + Random.Range(Plugin.IdleTimeRandomnessLowBound.Value, Plugin.IdleTimeRandomnessHighBound.Value);
        }

        public void ResetIdleTimer()
        {
            timeSinceLastInteraction = 0f;
        }

        private void CalculateNewColourTransitionTime()
        {
            transitionIdleTime = Plugin.IdleScreenColorTransitionTime.Value + Random.Range(Plugin.IdleScreenColorTransitionRandomnessLowBound.Value, Plugin.IdleScreenColorTransitionRandomnessHighBound.Value);
        }

        public void OnApplicationQuit()
        {
            StopAllCoroutines();
        }

        private bool FindAllComponents()
        {
            CanvasGameObject = gameObject.GetComponentInChildren<Canvas>()?.gameObject;
            if (CanvasGameObject == null)
            {
                System.Console.WriteLine("[ResourceMonitor] Canvas not found.");
                return false;
            }

            animator = CanvasGameObject.GetComponent<Animator>();
            if (animator == null)
            {
                System.Console.WriteLine("[ResourceMonitor] Animator not found.");
                return false;
            }

            blackCover = CanvasGameObject.FindChild("BlackCover")?.gameObject;
            if (blackCover == null)
            {
                System.Console.WriteLine("[ResourceMonitor] BlackCover not found.");
                return false;
            }

            var screenHolder = CanvasGameObject.transform.Find("Screens")?.gameObject;
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

            var actualMainScreen = mainScreen.FindChild("ActualScreen")?.gameObject;
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

            var paginator = actualMainScreen.FindChild("Paginator")?.gameObject;
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
            
            var pb = previousPageGameObject.AddComponent<PaginatorButton>();
            pb.ResourceMonitorDisplay = this;
            pb.AmountToChangePageBy = -1;

            nextPageGameObject = paginator.FindChild("NextPage")?.gameObject;
            if (nextPageGameObject == null)
            {
                System.Console.WriteLine("[ResourceMonitor] Screen: Next Page GameObject not found.");
                return false;
            }
            var pb2 = nextPageGameObject.AddComponent<PaginatorButton>();
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

            idleScreen = screenHolder.FindChild("IdleScreen")?.gameObject;
            if (idleScreen == null)
            {
                System.Console.WriteLine("[ResourceMonitor] Screen: IdleScreen not found.");
                return false;
            }

            var idleScreenTitleBackground = idleScreen.FindChild("AlterraTitleBackground")?.gameObject;
            if (idleScreenTitleBackground == null)
            {
                System.Console.WriteLine("[ResourceMonitor] Screen: IdleScreen Background not found.");
                return false;
            }

            idleScreenTitleBackgroundImage = idleScreenTitleBackground.GetComponent<Image>();
            if (idleScreenTitleBackground == null)
            {
                System.Console.WriteLine("[ResourceMonitor] Screen: IdleScreen Background Image not found.");
                return false;
            }

            return true;
        }
    }
}