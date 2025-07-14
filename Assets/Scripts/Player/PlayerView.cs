using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class PlayerView : MonoBehaviour
{
    private SpriteRenderer sr;

    public void Init()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public void PlayHitEffect()
    {
        sr.color = Color.red;
        Invoke(nameof(RestoreColor), 0.1f);
    }

    private void RestoreColor()
    {
        sr.color = Color.white;
    }

    public void Die()
    {
        Debug.Log("Player died.");
    }
}