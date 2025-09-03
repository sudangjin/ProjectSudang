using UnityEngine;

public class SceneHierarchy : MonoBehaviour
{
    public static SceneHierarchy Instance { get; private set; }

    [Header("Stage Parents")]
    public Transform monstersParent;
    public Transform projectilesParent;
    public Transform expParent;
    public Transform damageTextParent;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
}
