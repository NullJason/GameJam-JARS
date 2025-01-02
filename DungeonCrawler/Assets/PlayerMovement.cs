using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
     public float moveSpeed = 5f; // move speed

    private Vector2 movement;

    void Update()
    {
        // simple movement
        movement.x = Input.GetAxisRaw("Horizontal"); // A/D or Left/Right
        movement.y = Input.GetAxisRaw("Vertical");   // W/S or Up/Down

        if (Input.GetMouseButtonDown(0)) // Left mouse button
        {
            Attack();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Dash();
        }
    }

    void FixedUpdate()
    {
        Move();
    }

    void Move()
    {
        Vector3 newPosition = transform.position + new Vector3(movement.x, movement.y, 0f) * moveSpeed * Time.fixedDeltaTime;
        transform.position = newPosition;
    }

    void Attack()
    {
        // Placeholder for attack functionality
        Debug.Log("Attack triggered!");
    }

    void Dash()
    {
        // Placeholder for dash functionality
        Debug.Log("Dash triggered!");
    }
}
