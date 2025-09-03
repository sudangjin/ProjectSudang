using TMPro;
using UnityEngine;
using DG.Tweening;

public class DamageText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textMesh;

    public void Show(int damage, Color color, Vector3 position)
    {
        position.y = position.y - 0.25f;

        transform.position = position;
        textMesh.text = damage.ToString();
        textMesh.alpha = 1f;
        textMesh.color = color;

        transform.localScale = Vector3.one * 0.5f;

        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOMoveY(position.y + 0.25f, 0.5f));
        seq.Join(transform.DOScale(1f, 0.3f));
        seq.Join(textMesh.DOFade(0f, 0.5f).SetDelay(0.3f));
        seq.OnComplete(() => {
            ObjectPooler.Instance.Release(PrefabPreLoader.Instance.GetPrefab(PrefabType.DAMAGE_TEXT), gameObject, SceneHierarchy.Instance.damageTextParent);
        });
    }
}
