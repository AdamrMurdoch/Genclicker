using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

[RequireComponent(typeof(Button))]
public class BounceAnimation : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("Animation Settings")]
    [SerializeField] private float pressedScale = 0.95f;
    [SerializeField] private float pressAnimationDuration = 0.08f;
    [SerializeField] private float releaseAnimationDuration = 0.3f;
    [SerializeField] private AnimationCurve pressAnimationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private AnimationCurve releaseAnimationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Bounce Settings")]
    [SerializeField] private bool enableBounce = true;
    [SerializeField] private float bounceOvershoot = 1.02f;
    [SerializeField] private float bounceUndershoot = 0.99f;
    [SerializeField] private float bounceDuration = 0.26f;
    [SerializeField] private float settleDuration = 0.19f;
    [SerializeField] private float dynamicTiming = 0.5f;

    [Header("Press Tracking")]
    [SerializeField] private float maxPressDuration = 1f;
    [SerializeField] private float forceBounceMultiplier = 1.5f;

    [Header("Haptics")]
    public int VibrationType;

    private Vector3 originalScale;
    private Coroutine currentAnimation;
    private Button buttonComponent;

    private float pressStartTime;
    private bool isPressed;

    private void Awake()
    {
        buttonComponent = GetComponent<Button>();
        originalScale = transform.localScale;
    }

    private void OnEnable()
    {
        ResetButton();
    }

    private void OnDisable()
    {
        ResetButton();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (buttonComponent == null || !buttonComponent.interactable)
        {
            return;
        }

        isPressed = true;
        pressStartTime = Time.time;

        StartScaleTo(originalScale * pressedScale, pressAnimationDuration, pressAnimationCurve);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (buttonComponent == null || !buttonComponent.interactable)
        {
            return;
        }

        float actualPressDuration = isPressed ? (Time.time - pressStartTime) : 0.1f;
        isPressed = false;

        StartRelease(actualPressDuration);
    }

    private void StartScaleTo(Vector3 target, float duration, AnimationCurve curve)
    {
        if (currentAnimation != null)
        {
            StopCoroutine(currentAnimation);
        }
        currentAnimation = StartCoroutine(AnimateScale(target, duration, curve));
    }

    private void StartRelease(float pressDuration)
    {
        if (currentAnimation != null)
        {
            StopCoroutine(currentAnimation);
        }
        currentAnimation = StartCoroutine(AnimateReleaseWithDuration(pressDuration));
    }

    private IEnumerator AnimateScale(Vector3 targetScale, float duration, AnimationCurve curve)
    {
        Vector3 startScale = transform.localScale;
        float t = 0f;

        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            float p = Mathf.Clamp01(t / duration);
            float cv = curve.Evaluate(p);
            transform.localScale = Vector3.Lerp(startScale, targetScale, cv);
            yield return null;
        }

        transform.localScale = targetScale;
    }

    private IEnumerator AnimateReleaseWithDuration(float pressDuration)
    {
        if (!enableBounce)
        {
            yield return AnimateScale(originalScale, releaseAnimationDuration, releaseAnimationCurve);
            yield break;
        }

        float normalized = Mathf.Clamp01(pressDuration / maxPressDuration);
        float force = 1f + (normalized * forceBounceMultiplier);
        float dyn = 1f + (normalized * dynamicTiming);

        Vector3 start = transform.localScale;
        Vector3 overshoot = originalScale * (1f + (bounceOvershoot - 1f) * force);
        Vector3 undershoot = originalScale * (1f - (1f - bounceUndershoot) * force);

        float p1 = bounceDuration * 0.4f;
        float p2 = bounceDuration * 0.6f;
        float p3 = settleDuration * dyn;

        float e = 0f;
        while (e < p1)
        {
            e += Time.unscaledDeltaTime;
            float x = Mathf.Clamp01(e / p1);
            float cv = 1f - Mathf.Pow(1f - x, 2f);
            transform.localScale = Vector3.Lerp(start, overshoot, cv);
            yield return null;
        }

        e = 0f;
        while (e < p2)
        {
            e += Time.unscaledDeltaTime;
            float x = Mathf.Clamp01(e / p2);
            float cv = Mathf.Sin(x * Mathf.PI * 0.5f);
            transform.localScale = Vector3.Lerp(overshoot, undershoot, cv);
            yield return null;
        }

        e = 0f;
        while (e < p3)
        {
            e += Time.unscaledDeltaTime;
            float x = Mathf.Clamp01(e / p3);
            float cv = 1f - Mathf.Exp(-5f * x) * (1f - x);
            float osc = Mathf.Sin(x * Mathf.PI * 3f) * (1f - x) * 0.02f * force;
            transform.localScale = Vector3.Lerp(undershoot, originalScale, cv + osc);
            yield return null;
        }

        transform.localScale = originalScale;
    }

    public void ResetButton()
    {
        if (currentAnimation != null)
        {
            StopCoroutine(currentAnimation);
            currentAnimation = null;
        }

        transform.localScale = originalScale;
        isPressed = false;
    }
}