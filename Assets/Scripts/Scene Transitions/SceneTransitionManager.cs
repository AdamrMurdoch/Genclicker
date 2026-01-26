using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class SceneTransitionManager : MonoBehaviour
{
    [SerializeField] private RectTransform Panel;
    [SerializeField] private float SlideDuration = 0.45f;
    [SerializeField] private float FadeDuration = 1f;

    private bool isTransitioning = false;
    private Canvas TransitionCanvas;
    public static event System.Action OnSceneReady;
    [SerializeField] private Color PanelFadeColourBefore = new Color32(18, 17, 18, 0);
    [SerializeField] private Color PanelFadeColourAfter = new Color32(18, 17, 18, 255);
    private int SortingLayerAboveNumber = 9999;
    private int SortingLayerBelowNumber = -1000;
    private string SortingLayerTagName = "UI";
    [SerializeField] private float FadeDurationForSlide = 0.21f;
    private string SlideDirection = "Next";

    private CanvasGroup TargetCanvasGroup;
    private Vector2 canvasSize;
    private Image PanelImage;
    private Vector2 slideOffset;
    private float adjustedDuration;
    private RectTransform TransitionCanvasRect;

    private void Start()
    {
        if (Panel == null)
        {
            Panel = GetComponent<RectTransform>();
        }

        if (Panel == null)
        {
            Debug.LogWarning("Skipping the slide animation as no Panel was found... Great job!");
            return;
        }

        PanelImage = Panel.GetComponent<Image>();
        TargetCanvasGroup = Panel.GetComponent<CanvasGroup>();
        if (TargetCanvasGroup == null)
        {
            TargetCanvasGroup = Panel.gameObject.AddComponent<CanvasGroup>();
        }

        TransitionCanvas = Panel.GetComponentInParent<Canvas>();
        TransitionCanvasRect = TransitionCanvas != null ? TransitionCanvas.GetComponent<RectTransform>() : null;
    }

    public void OnSceneChanged(string sceneName)
    {
        if (!SingletonGlobal.Instance.IsSceneTransitioning)
        {
            StartCoroutine(Transition(sceneName, false));
        }
    }

    public void OnSceneChangedFade(string sceneName, float transitionLength)
    {
        if (!SingletonGlobal.Instance.IsSceneTransitioning)
        {
            if (sceneName == "Home")
            {
                PanelImage.color = PanelFadeColourBefore;
            }
            FadeDuration = transitionLength;
            StartCoroutine(Transition(sceneName, true));
        }
    }

    public void OnSceneChangedFadeSpecial(string sceneName, float transitionLength)
    {
        if (!SingletonGlobal.Instance.IsSceneTransitioning)
        {
            FadeDuration = transitionLength;
            StartCoroutine(Transition(sceneName, true));
        }
    }

    private EventSystem FindEventSystemInScene(Scene scene)
    {
        foreach (var root in scene.GetRootGameObjects())
        {
            var eventSystem = root.GetComponentInChildren<EventSystem>(true);
            if (eventSystem != null)
            {
                return eventSystem;
            }
        }
        return null;
    }


    private void KeepOnlyEventSystemFromScene(Scene newScene)
    {
        var newSceneEventSystem = FindEventSystemInScene(newScene);
        var allListeners = FindObjectsByType<EventSystem>(FindObjectsSortMode.None);

        if (newSceneEventSystem == null)
        {
            if (allListeners.Length == 0)
            {
                return;
            }
            var keep = allListeners[0];
            foreach (var eventSystem in allListeners)
            {
                if(newSceneEventSystem != null && eventSystem == newSceneEventSystem)
                {
                    eventSystem.enabled = true;
                }
                else
                {
                   eventSystem.enabled = false; 
                }
            }
            return;
        }

        if (newSceneEventSystem == null && allListeners.Length > 0)
        {
            allListeners[0].enabled = true;   
        }
    }

    private AudioListener FindAudioListenerInScene(Scene scene)
    {
        foreach (var root in scene.GetRootGameObjects())
        {
            var listener = root.GetComponentInChildren<AudioListener>(true);
            if (listener != null)
            {
                return listener;
            }
        }
        return null;
    }

    private void KeepOnlyAudioListenerFromScene(Scene newScene)
    {
        var newSceneListener = FindAudioListenerInScene(newScene);
        var allListeners = FindObjectsByType<AudioListener>(FindObjectsSortMode.None);

        if (newSceneListener == null)
        {
            if (allListeners.Length == 0)
            {
                return;
            }

            var keep = allListeners[0];
            foreach (var audioListener in allListeners)
            {
                bool value = audioListener == keep;
                audioListener.enabled = value;
            }    
            return;
        }

        foreach (var audioListener in allListeners)
        {
            audioListener.enabled = audioListener == newSceneListener;
        }
    }

    private void SetAllEventSystemsEnabled(bool enabled)
    {
        var systems = FindObjectsByType<EventSystem>(FindObjectsSortMode.None);
        foreach (var eventSystem in systems)
        {
            eventSystem.enabled = enabled;
        }
    }

    private void DisableAllAudioListeners()
    {
        var allListeners = FindObjectsByType<AudioListener>(FindObjectsSortMode.None);
        foreach (var audioListener in allListeners)
        {
            audioListener.enabled = false;
        }
    }


    private IEnumerator Transition(string sceneName, bool useFade)
    {
        SetAllEventSystemsEnabled(false);
        SingletonGlobal.Instance.IsSceneTransitioning = true;
        Scene oldScene = SceneManager.GetActiveScene();

        int originalSortOrder = 0;
        string originalSortingLayer = "";
        if (TransitionCanvas != null)
        {
            originalSortOrder = TransitionCanvas.sortingOrder;
            originalSortingLayer = TransitionCanvas.sortingLayerName;

            TransitionCanvas.sortingLayerName = SortingLayerTagName;
            TransitionCanvas.sortingOrder = SortingLayerAboveNumber;
            TransitionCanvas.overrideSorting = true;
        }

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        asyncLoad.allowSceneActivation = false;

        while (asyncLoad.progress < 0.9f)
        {
            yield return null;
        }

        DisableAllAudioListeners();
        asyncLoad.allowSceneActivation = true;

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        Scene newScene = SceneManager.GetSceneByName(sceneName);
        Canvas[] newSceneCanvases = GetCanvasesFromScene(newScene);
        List<(Canvas canvas, int order, string layer, bool overrideSorting)> canvasBackup = new List<(Canvas, int, string, bool)>();

        foreach (Canvas canvas in newSceneCanvases)
        {
            canvasBackup.Add((canvas, canvas.sortingOrder, canvas.sortingLayerName, canvas.overrideSorting));
            canvas.overrideSorting = true;
            canvas.sortingLayerName = "Default";
            canvas.sortingOrder = SortingLayerBelowNumber;
        }

        SceneManager.SetActiveScene(newScene);

        var newEventSystem = FindEventSystemInScene(newScene);
        if (newEventSystem != null)
        {
            newEventSystem.gameObject.SetActive(true);
            newEventSystem.enabled = true;
        }
        KeepOnlyEventSystemFromScene(newScene);

        yield return null;
        KeepOnlyAudioListenerFromScene(newScene);

        yield return new WaitForEndOfFrame();

        OnSceneReady?.Invoke();

        foreach (var backup in canvasBackup)
        {
            backup.canvas.sortingOrder = backup.order;
            backup.canvas.sortingLayerName = backup.layer;
            backup.canvas.overrideSorting = backup.overrideSorting;
        }

        if (useFade == true)
        {
            yield return StartCoroutine(FadeOutPanel());
        }
        else
        {
            yield return StartCoroutine(SlideOutPanel());
        }

        if (TransitionCanvas != null)
        {
            TransitionCanvas.sortingOrder = originalSortOrder;
            TransitionCanvas.sortingLayerName = originalSortingLayer;
            TransitionCanvas.overrideSorting = false;
        }

        KeepOnlyEventSystemFromScene(SceneManager.GetActiveScene());

        SingletonGlobal.Instance.IsSceneTransitioning = false;

        AsyncOperation unloadOp = SceneManager.UnloadSceneAsync(oldScene);
        while (unloadOp != null && !unloadOp.isDone)
        {
            yield return null;
        }
    }


    private Canvas[] GetCanvasesFromScene(Scene scene)
    {
        List<Canvas> canvases = new List<Canvas>();
        GameObject[] rootObjects = scene.GetRootGameObjects();

        foreach (GameObject root in rootObjects)
        {
            Canvas[] foundCanvases = root.GetComponentsInChildren<Canvas>(true);
            canvases.AddRange(foundCanvases);
        }

        return canvases.ToArray();
    }

    private IEnumerator SlideOutPanel()
    {
        if (Panel == null || TargetCanvasGroup == null || TransitionCanvasRect == null)
        {
            Debug.Log("SlideOutPanel has found that either Panel, TargetCanvasGroup, or TransitionCanvasRect is missing.");
            yield break;
        }
        SetupSlidePanel();
        SetupSlideDirection();

        Vector2 startPos = Panel.anchoredPosition;
        Vector2 endPos = startPos + slideOffset;

        float totalDuration = adjustedDuration;

        TargetCanvasGroup.alpha = 1f;
        TargetCanvasGroup.blocksRaycasts = true;
        TargetCanvasGroup.interactable = true;

        float elapsed = 0f;
        while (elapsed < totalDuration)
        {
            elapsed += Time.deltaTime;

            float temporaryPosition = adjustedDuration > 0f ? Mathf.Clamp01(elapsed / adjustedDuration) : 1f;
            temporaryPosition = EaseInOutQuad(temporaryPosition);
            Panel.anchoredPosition = Vector2.Lerp(startPos, endPos, temporaryPosition);

            float fadeTime = FadeDurationForSlide > 0f ? Mathf.Clamp01(elapsed / FadeDurationForSlide) : 1f;
            fadeTime = EaseInOutQuad(fadeTime);
            TargetCanvasGroup.alpha = Mathf.Lerp(1f, 0f, fadeTime);

            yield return null;
        }

        Panel.anchoredPosition = endPos;
        PanelImage.color = PanelFadeColourBefore;
        TargetCanvasGroup.alpha = 0f;
        TargetCanvasGroup.blocksRaycasts = false;
        TargetCanvasGroup.interactable = false;
    }

    private void SetupSlidePanel()
    {
        Debug.Log("SetupSlidePanel started");

        if (Panel == null)
        {
            Debug.Log("panel not found");
            return;
        }

        if (TargetCanvasGroup == null)
        {
            Debug.Log("targetCanvasGroup not found");
            return;
        }

        canvasSize = TransitionCanvasRect.rect.size;
        PanelImage.color = PanelFadeColourBefore;
    }

    private void SetupSlideDirection()
    {
        switch (SlideDirection.ToLower())
        {
            case "back":
            {
                slideOffset = new Vector2(canvasSize.x, 0f);
                adjustedDuration = SlideDuration;
                break;
            }
            case "next":
            {
                slideOffset = new Vector2(-canvasSize.x, 0f);
                adjustedDuration = SlideDuration;
                break;
            }
            case "up":
            {
                slideOffset = new Vector2(0f, canvasSize.y);
                adjustedDuration = SlideDuration * 0.90f;
                break;
            }
            case "down":
            {
                slideOffset = new Vector2(0f, -canvasSize.y);
                adjustedDuration = SlideDuration * 0.90f;
                break;
            }
            default:
            {
                slideOffset = new Vector2(-canvasSize.x, 0f);
                adjustedDuration = SlideDuration;
                break;
            }
        }
    }

    private IEnumerator FadeOutPanel()
    {
        if (Panel == null)
        {
            yield break;
        }

        if (TargetCanvasGroup == null)
        {
            yield break;
        }

        TargetCanvasGroup.alpha = 1f;
        TargetCanvasGroup.blocksRaycasts = true;
        TargetCanvasGroup.interactable = true;
        adjustedDuration = FadeDuration;
        float elapsed = 0f;
        while (elapsed < adjustedDuration)
        {
            elapsed += Time.deltaTime;
            float time = Mathf.Clamp01(elapsed / adjustedDuration);
            time = EaseInOutQuad(time);
            TargetCanvasGroup.alpha = Mathf.Lerp(1f, 0f, time);

            yield return null;
        }

        TargetCanvasGroup.alpha = 0f;
        PanelImage.color = PanelFadeColourAfter;
        TargetCanvasGroup.blocksRaycasts = false;
        TargetCanvasGroup.interactable = false;
    }

    private float EaseInOutQuad(float t)
    {
        if (t < 0.5f)
        {
            return 2f * t * t;
        }
        else
        {
            return 1f - Mathf.Pow(-2f * t + 2f, 2f) / 2f;
        }
    }
}