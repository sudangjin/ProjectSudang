using UnityEngine;

public class MonsterAnimEvent : MonoBehaviour
{
    public void OnDeathAnimationEnd()
    {
        Destroy(gameObject);
    }
}
