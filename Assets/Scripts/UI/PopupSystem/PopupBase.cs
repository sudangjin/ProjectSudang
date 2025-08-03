using System;
using UnityEngine;

public abstract class PopupBase : MonoBehaviour
{
    public virtual bool PauseGame => true;
    public virtual bool UsePooling => true;

    private Action closeCallback;

    public void OnOpen(Action onClose)
    {
        closeCallback = onClose;
        gameObject.SetActive(true);
        Initialize();
    }

    public virtual void Close()
    {
        closeCallback?.Invoke();
        closeCallback = null;
    }

    protected virtual void Initialize() { }
}
