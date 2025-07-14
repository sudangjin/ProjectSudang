using UnityEngine;

public class BulletView : MonoBehaviour
{
    private BulletController controller;

    public void Init(BulletController controller)
    {
        this.controller = controller;
        RotateToDirection(controller.Direction);
    }

    public void Move(Vector2 direction, float speed)
    {
        transform.position += (Vector3)(direction * speed * Time.deltaTime);
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }

    private void RotateToDirection(Vector2 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - 90f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out MonsterController monster))
        {
            controller.OnHit(monster);
        }
    }
}
