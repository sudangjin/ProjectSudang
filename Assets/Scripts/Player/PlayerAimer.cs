using UnityEngine;

public class PlayerAimer : MonoBehaviour
{
    [SerializeField] private Transform arrow;

    [SerializeField] private float rotateSpeed = 180f;

    private void Update()
    {
        float input = Input.GetAxisRaw("Horizontal");

        if (Mathf.Abs(input) > 0.01f)
        {
            float angle = -input * rotateSpeed * Time.deltaTime;
            arrow.Rotate(0, 0, angle);
        }
    }

    public Vector2 GetAimDirection()
    {
        return arrow.up.normalized;
    }

    public float GetAimAngle()
    {
        return arrow.eulerAngles.z;
    }
}
