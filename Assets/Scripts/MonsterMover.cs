using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class MonsterMover : MonoBehaviour
{
    [Header("Move")]
    public float moveSpeed = 2f;

    [Header("Attack")]
    public float attackRange = 1.5f;

    private Transform target;

    public void Initialize(Transform targetTransform)
    {
        target = targetTransform;
    }

    private void Update()
    {
        if (target == null) return;

        float distance = Vector2.Distance(transform.position, target.position);
        if (distance > attackRange)
        {
            MoveTowardsTarget();
        }
    }

    private void MoveTowardsTarget()
    {
        Vector2 direction = (target.position - transform.position).normalized;
        transform.position += (Vector3)(direction * moveSpeed * Time.deltaTime);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
