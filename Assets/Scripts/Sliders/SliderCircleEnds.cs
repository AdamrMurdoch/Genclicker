using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class SliderCircleEnds : MonoBehaviour
{
    [SerializeField] private Image radialFillImage;
    [SerializeField] private RectTransform startCircle;
    [SerializeField] private RectTransform endCircle;
    
    [SerializeField] private float scanMultiplier = 100f;
    [SerializeField] private bool invertDirection = true;
    [SerializeField] private bool hideStartWhenEmpty = true;
    [SerializeField] private bool hideEndWhenFull = true;
    [SerializeField] private bool autoCalculateRadius = true;
    [SerializeField] private float radiusPercentage = 0.9f;
    
    void Update()
    {
        if (radialFillImage == null)
        {
            return;
        }
            
        UpdateCirclePositions();
    }
    
    void UpdateCirclePositions()
    {
        float currentValue = radialFillImage.fillAmount;
        
        float radius = scanMultiplier;
        if (autoCalculateRadius)
        {
            RectTransform fillRect = radialFillImage.GetComponent<RectTransform>();
            float imageSize = Mathf.Min(fillRect.rect.width, fillRect.rect.height);
            radius = (imageSize * 0.508f) * radiusPercentage;
        }
        
        float startAngle = 1.5f * Mathf.PI / 1f;
        
        if (startCircle != null)
        {
            startCircle.localPosition = new Vector2(Mathf.Cos(startAngle), Mathf.Sin(startAngle)) * radius * (invertDirection ? -1 : 1);
            
            if (hideStartWhenEmpty)
            {
                startCircle.gameObject.SetActive(currentValue > 0.001f);
            }
        }
        
        if (endCircle != null)
        {
            float angle = startAngle - (currentValue * 2f * Mathf.PI);
            
            endCircle.localPosition = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius * (invertDirection ? -1 : 1);
            
            if (hideEndWhenFull)
            {
                endCircle.gameObject.SetActive(currentValue < 0.999f);
            }
        }
    }
    
#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (radialFillImage == null)
        {
            return;
        }
            
        RectTransform fillRectTransform = radialFillImage.GetComponent<RectTransform>();
        Vector3 worldCenter = fillRectTransform.TransformPoint(Vector2.zero);
        
        float visualRadius = scanMultiplier;
        if (autoCalculateRadius)
        {
            float imageSize = Mathf.Min(fillRectTransform.rect.width, fillRectTransform.rect.height);
            visualRadius = (imageSize * 0.5f) * radiusPercentage;
        }
        
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas != null)
        {
            visualRadius *= canvas.transform.localScale.x;
        }
        
        Gizmos.color = Color.yellow;
        
        int segments = 64;
        for (int i = 0; i < segments; i++)
        {
            float angle1 = (float)i / segments * 2f * Mathf.PI;
            float angle2 = (float)(i + 1) / segments * 2f * Mathf.PI;
            
            float direction = invertDirection ? -1 : 1;
            Vector3 pos1 = worldCenter + new Vector3(Mathf.Cos(angle1), Mathf.Sin(angle1), 0) * visualRadius * direction;
            Vector3 pos2 = worldCenter + new Vector3(Mathf.Cos(angle2), Mathf.Sin(angle2), 0) * visualRadius * direction;
            
            Gizmos.DrawLine(pos1, pos2);
        }
        
        if (radialFillImage.fillAmount > 0)
        {
            Gizmos.color = Color.green;
            float startAngle = 3f * Mathf.PI / 2f;
            float endAngle = startAngle - (radialFillImage.fillAmount * 2f * Mathf.PI);
            
            int arcSegments = Mathf.Max(2, (int)(radialFillImage.fillAmount * segments));
            for (int i = 0; i < arcSegments; i++)
            {
                float t1 = (float)i / arcSegments;
                float t2 = (float)(i + 1) / arcSegments;
                
                float angle1 = Mathf.Lerp(startAngle, endAngle, t1);
                float angle2 = Mathf.Lerp(startAngle, endAngle, t2);
                
                float direction = invertDirection ? -1 : 1;
                Vector3 pos1 = worldCenter + new Vector3(Mathf.Cos(angle1), Mathf.Sin(angle1), 0) * visualRadius * direction;
                Vector3 pos2 = worldCenter + new Vector3(Mathf.Cos(angle2), Mathf.Sin(angle2), 0) * visualRadius * direction;
                
                Gizmos.DrawLine(pos1, pos2);
            }
        }
    }
#endif
}
