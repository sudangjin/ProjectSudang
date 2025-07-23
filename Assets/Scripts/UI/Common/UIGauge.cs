using UnityEngine;

[ExecuteAlways]
public class UIGauge : MonoBehaviour
{
    [SerializeField] private RectTransform fillArea;
    [SerializeField] private RectTransform gaugeRoot;

    public float paddingLeft = 0f;
    public float paddingRight = 0f;
    public float paddingTop = 0f;
    public float paddingBottom = 0f;

    [Header("Debug")]
    [Range(0f, 1f)][SerializeField] private float debugRatio = 1f;
    [SerializeField] private bool debugMode = false;

    private int maxValue = 100;

    public void Init(int maxValue)
    {
        this.maxValue = Mathf.Max(1, maxValue);
        SetRatio(1f);
    }

    public void UpdateValue(int current)
    {
        float ratio = Mathf.Clamp01((float)current / maxValue);
        SetRatio(ratio);
    }

    public void SetVisibility(bool isVisible)
    {
        gameObject.SetActive(isVisible);
    }

    public void FaceToCamera(Transform cameraTransform)
    {
        transform.forward = cameraTransform.forward;
    }

    private void SetRatio(float ratio)
    {
        if (fillArea == null || gaugeRoot == null) return;

        Rect rootRect = gaugeRoot.rect;
        if (rootRect.width <= 0f || rootRect.height <= 0f) return;

        float usableWidth = rootRect.width - paddingLeft - paddingRight;
        float anchorMaxX = (paddingLeft + usableWidth * ratio) / rootRect.width;
        float anchorMinX = paddingLeft / rootRect.width;

        float anchorMinY = paddingBottom / rootRect.height;
        float anchorMaxY = 1f - (paddingTop / rootRect.height);

        fillArea.anchorMin = new Vector2(anchorMinX, anchorMinY);
        fillArea.anchorMax = new Vector2(anchorMaxX, anchorMaxY);

        fillArea.offsetMin = Vector2.zero;
        fillArea.offsetMax = Vector2.zero;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (debugMode && !Application.isPlaying)
        {
            SetRatio(debugRatio);
        }
    }
#endif
}
