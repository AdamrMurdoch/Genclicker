using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class SceneTransitionManager : MonoBehaviour
{
    [Header("Transition Settings")]
    public RectTransform panel;
    public float slideDuration = 0.25f;
    public float slideDistance = 2000f;

    [Header("Fade Settings")]
    public float fadeDuration = 1f;

    private bool isTransitioning = false;
    private Canvas transitionCanvas;
    public static System.Action OnSceneReady;

    private void Start()
    {
        slideDuration = 0.45f;
        if (panel == null)
        {
            panel = GetComponent<RectTransform>();
            if (panel == null)
            {
                Debug.LogWarning("Skipping the slide animation as no panel was found for the transition.");
            }
        }

        if (panel != null)
        {
            transitionCanvas = panel.GetComponentInParent<Canvas>();
        }
    }

    public void OnSceneChanged(string sceneName)
    {
        if (!isTransitioning)
        {
            StartCoroutine(Transition(sceneName));
        }
    }

    public void OnSceneChangedFade(string sceneName, float transitionLength)
    {
        if (!isTransitioning)
        {
            if (sceneName == "Home")
            {
                panel.GetComponent<Image>().color = new Color32(18, 17, 18, 0);
            }
            fadeDuration = transitionLength;
            StartCoroutine(Transition(sceneName, true));
        }
    }

    public void OnSceneChangedFadeSpecial(string sceneName, float transitionLength)
    {
        if (!isTransitioning)
        {
            fadeDuration = transitionLength;
            StartCoroutine(Transition(sceneName, true));
        }
    }

    private IEnumerator Transition(string sceneName)
    {
        return Transition(sceneName, false);
    }

    private IEnumerator Transition(string sceneName, bool useFade)
    {
        isTransitioning = true;

        var currentEventSystems = FindObjectsByType<UnityEngine.EventSystems.EventSystem>(FindObjectsSortMode.None);
        foreach (var es in currentEventSystems)
        {
            es.gameObject.SetActive(false);
        }

        var currentListeners = FindObjectsByType<AudioListener>(FindObjectsSortMode.None);
        foreach (var al in currentListeners)
        {
            al.enabled = false;
        }

        Scene oldScene = gameObject.scene;

        int originalSortOrder = 0;
        string originalSortingLayer = "";
        if (transitionCanvas != null)
        {
            originalSortOrder = transitionCanvas.sortingOrder;
            originalSortingLayer = transitionCanvas.sortingLayerName;

            transitionCanvas.sortingLayerName = "UI";
            transitionCanvas.sortingOrder = 9999;
            transitionCanvas.overrideSorting = true;
        }

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        asyncLoad.allowSceneActivation = false;

        while (asyncLoad.progress < 0.9f)
        {
            yield return null;
        }

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
            canvas.sortingOrder = -1000;
        }

        SceneManager.SetActiveScene(newScene);

        yield return Resources.UnloadUnusedAssets();
        yield return new WaitForEndOfFrame();

        var newEventSystems = FindObjectsByType<UnityEngine.EventSystems.EventSystem>(FindObjectsSortMode.None);
        if (newEventSystems.Length > 0)
        {
            foreach (var es in newEventSystems)
            {
                es.gameObject.SetActive(true);
            }
        }

        var newListeners = FindObjectsByType<AudioListener>(FindObjectsSortMode.None);
        if (newListeners.Length > 0)
        {
            newListeners[0].enabled = true;
            for (int i = 1; i < newListeners.Length; i++)
            {
                newListeners[i].enabled = false;
            }
        }

        OnSceneReady?.Invoke();

        foreach (var backup in canvasBackup)
        {
            backup.canvas.sortingOrder = backup.order;
            backup.canvas.sortingLayerName = backup.layer;
            backup.canvas.overrideSorting = backup.overrideSorting;
        }

        if (useFade)
        {
            yield return StartCoroutine(FadeOutPanel());
        }
        else
        {
            yield return StartCoroutine(SlideOutPanel());
        }

        if (transitionCanvas != null)
        {
            transitionCanvas.sortingOrder = originalSortOrder;
            transitionCanvas.sortingLayerName = originalSortingLayer;
            transitionCanvas.overrideSorting = false;
        }

        AsyncOperation unloadOp = SceneManager.UnloadSceneAsync(oldScene);
        while (unloadOp != null && !unloadOp.isDone)
        {
            yield return null;
        }

        isTransitioning = false;
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
        if (panel == null)
        {
            yield break;
        }

        CanvasGroup cg = EnsureCanvasGroup();
        if (cg == null)
        {
            yield break;
        }

        Vector2 canvasSize = panel.GetComponentInParent<Canvas>().GetComponent<RectTransform>().rect.size;
        Image panelImage = panel.GetComponent<Image>();
        panelImage.color = new Color32(18, 17, 18, 0);
        Vector2 slideOffset;
        //string direction = SingletonGlobal.Instance?.SceneTransitionDirection ?? "next";
        string direction = "next";

        float adjustedDuration = slideDuration;

        switch (direction.ToLower())
        {
            case "back":
                {
                    slideOffset = new Vector2(canvasSize.x, 0f);
                    break;
                }
            case "next":
                {
                    slideOffset = new Vector2(-canvasSize.x, 0f);
                    break;
                }
            case "up":
                {
                    slideOffset = new Vector2(0f, canvasSize.y);
                    adjustedDuration = slideDuration * 0.90f;
                    break;
                }
            case "down":
                {
                    slideOffset = new Vector2(0f, -canvasSize.y);
                    adjustedDuration = slideDuration * 0.90f;
                    break;
                }
            default:
                {
                    slideOffset = new Vector2(-canvasSize.x, 0f);
                    break;
                }
        }

        Vector3 startPos = panel.localPosition;
        Vector3 endPos = startPos + (Vector3)slideOffset;

        float totalDuration = adjustedDuration;

        float fadeDurationForSlide = 0.21f;

        cg.alpha = 1f;
        cg.blocksRaycasts = true;
        cg.interactable = true;

        float elapsed = 0f;
        while (elapsed < totalDuration)
        {
            elapsed += Time.deltaTime;
            float tTotal = Mathf.Clamp01(elapsed / totalDuration);

            float tPos = adjustedDuration > 0f ? Mathf.Clamp01(elapsed / adjustedDuration) : 1f;
            tPos = EaseInOutQuad(tPos);
            panel.localPosition = Vector3.Lerp(startPos, endPos, tPos);

            float tFade = fadeDurationForSlide > 0f ? Mathf.Clamp01(elapsed / fadeDurationForSlide) : 1f;
            tFade = EaseInOutQuad(tFade);
            cg.alpha = Mathf.Lerp(1f, 0f, tFade);

            yield return null;
        }

        panel.localPosition = endPos;
        panelImage.color = new Color32(18, 17, 18, 255);
        cg.alpha = 0f;
        cg.blocksRaycasts = false;
        cg.interactable = false;
    }

    private CanvasGroup EnsureCanvasGroup()
    {
        if (panel == null)
        {
            return null;
        }
        CanvasGroup cg = panel.GetComponent<CanvasGroup>();
        if (cg == null)
        {
            cg = panel.gameObject.AddComponent<CanvasGroup>();
        }
        return cg;
    }

    private IEnumerator FadeOutPanel()
    {
        if (panel == null)
        {
            yield break;
        }

        CanvasGroup cg = EnsureCanvasGroup();
        if (cg == null)
        {
            yield break;
        }

        cg.alpha = 1f;
        cg.blocksRaycasts = true;
        cg.interactable = true;
        float adjustedDuration = fadeDuration;
        float elapsed = 0f;
        while (elapsed < adjustedDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / adjustedDuration);
            t = EaseInOutQuad(t);
            cg.alpha = Mathf.Lerp(1f, 0f, t);

            yield return null;
        }

        cg.alpha = 0f;
        panel.GetComponent<Image>().color = new Color32(18, 17, 18, 255);
        cg.blocksRaycasts = false;
        cg.interactable = false;
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