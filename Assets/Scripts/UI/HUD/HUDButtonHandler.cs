using UnityEngine;

public class HUDButtonHandler : MonoBehaviour
{
    public void OnClickPause() => PopupManager.Instance.Open<Popup_Pause>();
}