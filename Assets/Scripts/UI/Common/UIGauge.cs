using UnityEngine;
using UnityEngine.UI;

public class UIGauge : MonoBehaviour
{
    [SerializeField] private Slider slider;

    public void Init(int maxValue)
    {
        slider.maxValue = maxValue;
        slider.value = maxValue;
    }

    public void UpdateValue(int current)
    {
        slider.value = current;
    }

    public void SetVisibility(bool isVisible)
    {
        gameObject.SetActive(isVisible);
    }

    public void FaceToCamera(Transform cameraTransform)
    {
        transform.forward = cameraTransform.forward;
    }
}
