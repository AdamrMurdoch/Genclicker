using System.Collections;
using UnityEngine;

public class PopupAnimation : MonoBehaviour
{
    public IEnumerator AnimatePopup(CanvasGroup blurCanvasGroup, CanvasGroup popupCanvasGroup, float blurStartAlpha, float blurEndAlpha, float popupStartAlpha, float popupEndAlpha)
    {
        float blurDuration = 0.27f;
        float popupDuration = 0.25f;
        float scaleDuration = 0.45f;

        RectTransform popupRect = popupCanvasGroup.GetComponent<RectTransform>();

        StartCoroutine(AnimateCanvasGroup(blurCanvasGroup, blurStartAlpha, blurEndAlpha, blurDuration, true));
        StartCoroutine(AnimateCanvasGroup(popupCanvasGroup, popupStartAlpha, popupEndAlpha, popupDuration, true));

        if (popupRect != null && popupEndAlpha > 0)
        {
            StartCoroutine(AnimatePopupBounce(popupRect));
        }

        yield return new WaitForSeconds(Mathf.Max(blurDuration, popupDuration, scaleDuration));
    }

    public IEnumerator AnimateCanvasGroup(CanvasGroup canvasGroup, float startAlpha, float endAlpha, float duration, bool enableRaycasts)
    {
        float elapsed = 0f;
        canvasGroup.blocksRaycasts = enableRaycasts;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float easedT = EaseInOutCubic(t);
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, easedT);
            yield return null;
        }

        canvasGroup.alpha = endAlpha;

        if (endAlpha == 0)
        {
            canvasGroup.blocksRaycasts = false;
        }
    }

    private IEnumerator AnimatePopupBounce(RectTransform rectTransform)
    {
        float startScale = 0.3f;
        float overshootScale = 0.36f;
        float undershootScale = 0.32f;
        float targetScale = 0.35f;

        float growDuration = 0.15f;
        float bounceDuration = 0.22f;
        float phase1Duration = bounceDuration * 0.4f;
        float phase2Duration = bounceDuration * 0.6f;
        float settleDuration = 0.50f;

        Vector3 startScaleVec = Vector3.one * startScale;
        Vector3 overshootScaleVec = Vector3.one * overshootScale;
        Vector3 undershootScaleVec = Vector3.one * undershootScale;
        Vector3 targetScaleVec = Vector3.one * targetScale;

        float elapsedTime = 0f;

        while (elapsedTime < growDuration)
        {
            elapsedTime += Time.deltaTime;
            float time = elapsedTime / growDuration;

            float curveValue = 1f - Mathf.Pow(1f - time, 3f);

            rectTransform.localScale = Vector3.Lerp(startScaleVec, targetScaleVec * 0.95f, curveValue);
            yield return null;
        }

        elapsedTime = 0f;
        Vector3 currentScale = rectTransform.localScale;

        while (elapsedTime < phase1Duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / phase1Duration;

            float curveValue = 1f - Mathf.Pow(1f - t, 2f);

            rectTransform.localScale = Vector3.Lerp(currentScale, overshootScaleVec, curveValue);
            yield return null;
        }

        elapsedTime = 0f;

        while (elapsedTime < phase2Duration)
        {
            elapsedTime += Time.deltaTime;
            float time = elapsedTime / phase2Duration;
            float curveValue = Mathf.Sin(time * Mathf.PI * 0.5f);

            rectTransform.localScale = Vector3.Lerp(overshootScaleVec, undershootScaleVec, curveValue);
            yield return null;
        }

        elapsedTime = 0f;

        while (elapsedTime < settleDuration)
        {
            elapsedTime += Time.deltaTime;
            float time = elapsedTime / settleDuration;
            float curveValue = 1f - Mathf.Exp(-5f * time) * (1f - time);

            float oscillation = Mathf.Sin(time * Mathf.PI * 3f) * (1f - time) * 0.02f;

            rectTransform.localScale = Vector3.Lerp(undershootScaleVec, targetScaleVec, curveValue + oscillation);
            yield return null;
        }
        rectTransform.localScale = targetScaleVec;
    }

    private float EaseInOutCubic(float t)
    {
        return t < 0.5f ? 4 * t * t * t : 1 - Mathf.Pow(-2 * t + 2, 3) / 2;
    }
}