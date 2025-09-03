using System.Collections.Generic;
using UnityEngine;

public class PopupManager : MonoBehaviour
{
    public static PopupManager Instance { get; private set; }

    [SerializeField] private Transform popupRoot;
    [SerializeField] private NoticeLabel noticeLebel;

    private Stack<PopupBase> activePopupStack = new();
    private Queue<string> popupQueue = new();

    private Dictionary<string, GameObject> popupPrefabs = new();
    private Dictionary<string, Queue<GameObject>> popupPools = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public T Open<T>() where T : PopupBase
    {
        return (T)Open(typeof(T).Name);
    }


    public PopupBase Open(string popupName)
    {
        if (IsHigherPriorityPopupOpen(popupName))
        {
            popupQueue.Enqueue(popupName);
            return null;
        }

        GameObject instance = GetPopupInstance(popupName, out PopupBase popup);
        if (popup == null)
        {
            Debug.LogError($"Popup {popupName} 에 PopupBase가 없습니다.");
            return null;
        }

        activePopupStack.Push(popup);

        if (popup.PauseGame)
            Time.timeScale = 0;

        popup.OnOpen(() => ClosePopup(popupName, popup));

        return popup;
    }

    public void ShowLabel(string text)
    {
        noticeLebel.ShowLabel(text, Color.white);
    }

    public void ShowLabel(string text, Color color)
    {
        if (noticeLebel == null)
        {
            Debug.LogError("NoticeLabel 컴포넌트가 할당되지 않았습니다.");
            return;
        }

        noticeLebel.ShowLabel(text, color);
    }

    private void ClosePopup(string popupName, PopupBase popup)
    {
        if (activePopupStack.Count == 0 || activePopupStack.Peek() != popup)
            return;

        activePopupStack.Pop();

        if (popup.PauseGame && !HasBlockingPopup())
            Time.timeScale = 1;

        if (popup.UsePooling)
        {
            popup.gameObject.SetActive(false);
            if (!popupPools.ContainsKey(popupName))
                popupPools[popupName] = new Queue<GameObject>();

            popupPools[popupName].Enqueue(popup.gameObject);
        }
        else
        {
            Destroy(popup.gameObject);
        }

        TryDequeuePopup();
    }

    private GameObject GetPopupInstance(string popupName, out PopupBase popup)
    {
        GameObject prefab = LoadPopupPrefab(popupName);
        if (prefab == null)
        {
            popup = null;
            return null;
        }

        if (!popupPools.ContainsKey(popupName))
            popupPools[popupName] = new Queue<GameObject>();

        GameObject instance;
        if (popupPools[popupName].Count > 0)
        {
            instance = popupPools[popupName].Dequeue();
            instance.SetActive(true);
        }
        else
        {
            instance = Instantiate(prefab, popupRoot != null ? popupRoot : transform);
            instance.name = popupName;
        }

        popup = instance.GetComponent<PopupBase>();
        return instance;
    }

    private GameObject LoadPopupPrefab(string popupName)
    {
        if (!popupPrefabs.ContainsKey(popupName))
        {
            GameObject loaded = Resources.Load<GameObject>($"Popups/{popupName}");
            if (loaded == null)
            {
                Debug.LogError($"Popup Prefab {popupName} 을 Resources/Popups 경로에서 찾을 수 없습니다.");
                return null;
            }

            popupPrefabs[popupName] = loaded;
        }

        return popupPrefabs[popupName];
    }

    private void TryDequeuePopup()
    {
        if (popupQueue.Count == 0) return;

        string next = popupQueue.Dequeue();
        Open(next);
    }

    private bool HasBlockingPopup()
    {
        foreach (var popup in activePopupStack)
        {
            if (popup.PauseGame) return true;
        }
        return false;
    }

    private bool IsHigherPriorityPopupOpen(string popupName)
    {
        return false;
    }
}
