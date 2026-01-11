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
    
    [Header("Scroll Detection")]
    [SerializeField] private float scrollVelocityThreshold = 30f;
    [SerializeField] private bool debugScrollDetection = false;
    
    private Vector3 originalScale;
    private Coroutine currentAnimation;
    private Button buttonComponent;
    private ScrollRect parentScrollRect;
    private float pressStartTime;
    private float actualPressDuration;
    private bool isPressed = false;
    private bool isPotentialScroll = false;
    
    void Awake()
    {
        // Get the button component
        buttonComponent = GetComponent<Button>();
        
        if (buttonComponent == null)
        {
            Debug.LogError("TapSizing: No Button component found on " + gameObject.name);
        }
    }
    
    void Start()
    {
        originalScale = transform.localScale;
        parentScrollRect = GetComponentInParent<ScrollRect>();
        
        if (parentScrollRect == null)
        {
            GameObject scrollObject = GameObject.Find("Scroll");
            if (scrollObject != null)
            {
                parentScrollRect = scrollObject.GetComponent<ScrollRect>();
            }
        }
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        if (!buttonComponent.interactable) return;
        
        if (IsScrolling())
        {
            isPotentialScroll = true;
            return;
        }
        
        isPotentialScroll = false;
        isPressed = true;
        pressStartTime = Time.time;
        
        if (currentAnimation != null)
        {
            StopCoroutine(currentAnimation);
            currentAnimation = null;
        }
        
        currentAnimation = StartCoroutine(AnimateScale(originalScale * pressedScale, pressAnimationDuration, pressAnimationCurve));
    }
    
    public void OnPointerUp(PointerEventData eventData)
    {
        if (!isPressed || isPotentialScroll) 
        {
            isPotentialScroll = false;
            return;
        }
        
        if (IsScrolling())
        {
            if (currentAnimation != null)
            {
                StopCoroutine(currentAnimation);
            }
            currentAnimation = StartCoroutine(AnimateScale(originalScale, pressAnimationDuration, pressAnimationCurve));
            isPressed = false;
            isPotentialScroll = false;
            return;
        }
        
        isPressed = false;
        actualPressDuration = Time.time - pressStartTime;
        
        if (currentAnimation != null)
        {
            StopCoroutine(currentAnimation);
        }
        
        currentAnimation = StartCoroutine(AnimateReleaseWithDuration(actualPressDuration));
    }
    
    private bool IsScrolling()
    {
        if (parentScrollRect == null)
        {
            return false;
        }
        
        Vector2 velocity = parentScrollRect.velocity;
        float velocityMagnitude = velocity.magnitude;
        
        return velocityMagnitude > scrollVelocityThreshold;
    }
    
    private IEnumerator AnimateScale(Vector3 targetScale, float duration, AnimationCurve curve)
    {
        Vector3 startScale = transform.localScale;
        float elapsedTime = 0f;
        
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            float curveValue = curve.Evaluate(t);
            transform.localScale = Vector3.Lerp(startScale, targetScale, curveValue);
            
            yield return null;
        }
        
        transform.localScale = targetScale;
    }
    
    private IEnumerator AnimateReleaseWithDuration(float pressDuration)
    {
        if (enableBounce)
        {
            yield return StartCoroutine(AnimateBounceRelease(pressDuration));
        }
        else
        {
            yield return StartCoroutine(AnimateScale(originalScale, releaseAnimationDuration, releaseAnimationCurve));
        }
    }
    
    private IEnumerator AnimateBounceRelease(float pressDuration)
    {
        Vector3 startScale = transform.localScale;
        
        float normalizedPressDuration = Mathf.Clamp01(pressDuration / maxPressDuration);
        float forceMultiplier = 1f + (normalizedPressDuration * forceBounceMultiplier);
        
        Vector3 overshootScale = originalScale * (1f + (bounceOvershoot - 1f) * forceMultiplier);
        Vector3 undershootScale = originalScale * (1f - (1f - bounceUndershoot) * forceMultiplier);
        
        float dynamicMultiplier = 1f + (normalizedPressDuration * dynamicTiming);
        
        float totalDuration = releaseAnimationDuration * dynamicMultiplier;
        float phase1Duration = bounceDuration * 0.4f;
        float phase2Duration = bounceDuration * 0.6f;
        float phase3Duration = settleDuration * dynamicMultiplier;
        
        float elapsedTime = 0f;
        
        while (elapsedTime < phase1Duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / phase1Duration;
            float curveValue = 1f - Mathf.Pow(1f - t, 2f);
            
            transform.localScale = Vector3.Lerp(startScale, overshootScale, curveValue);
            yield return null;
        }
        
        elapsedTime = 0f;
        startScale = transform.localScale;
        
        while (elapsedTime < phase2Duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / phase2Duration;
            float curveValue = Mathf.Sin(t * Mathf.PI * 0.5f);
            
            transform.localScale = Vector3.Lerp(overshootScale, undershootScale, curveValue);
            yield return null;
        }
        
        elapsedTime = 0f;
        startScale = transform.localScale;
        
        while (elapsedTime < phase3Duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / phase3Duration;
            
            float curveValue = 1f - Mathf.Exp(-5f * t) * (1f - t);
            
            float oscillation = Mathf.Sin(t * Mathf.PI * 3f) * (1f - t) * 0.02f * forceMultiplier;
            
            transform.localScale = Vector3.Lerp(undershootScale, originalScale, curveValue + oscillation);
            yield return null;
        }
        
        transform.localScale = originalScale;
    }
    
    public void SimulatePress(float simulatedDuration = 0.1f)
    {
        StartCoroutine(AnimateSimulatedPressAndRelease(simulatedDuration));
    }
    
    private IEnumerator AnimateSimulatedPressAndRelease(float simulatedDuration)
    {
        if (currentAnimation != null)
        {
            StopCoroutine(currentAnimation);
            currentAnimation = null;
        }
        
        yield return StartCoroutine(AnimateScale(originalScale * pressedScale, pressAnimationDuration, pressAnimationCurve));
        yield return new WaitForSeconds(simulatedDuration);
        yield return StartCoroutine(AnimateReleaseWithDuration(simulatedDuration));
    }
    
    public void ResetButton()
    {
        if (currentAnimation != null)
        {
            StopCoroutine(currentAnimation);
            currentAnimation = null;
        }
        
        StopAllCoroutines();
        
        transform.localScale = originalScale;
        isPressed = false;
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        if (isPressed && !isPotentialScroll)
        {
            if (IsScrolling())
            {
                if (currentAnimation != null)
                {
                    StopCoroutine(currentAnimation);
                }
                currentAnimation = StartCoroutine(AnimateScale(originalScale, pressAnimationDuration, pressAnimationCurve));
                isPressed = false;
            }
        }
    }
    
    void OnDisable()
    {
        ResetButton();
    }
    
    public void SetScrollRect(ScrollRect scrollRect)
    {
        parentScrollRect = scrollRect;
        if (debugScrollDetection)
        {
            Debug.Log($"TapSizing: ScrollRect manually set for button {gameObject.name}");
        }
    }
}