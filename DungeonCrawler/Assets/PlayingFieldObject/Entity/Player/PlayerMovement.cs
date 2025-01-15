using UnityEngine;

public class PlayerMovement : Entity
{
  public static PlayerMovement mainPlayer;
  private protected float moveSpeed = 5f; // move speed

  private Vector2 movement;

  void Start(){
    if(mainPlayer == null) mainPlayer = this;
  }
  private protected override void GameUpdate(){
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
  private protected override void Behave()
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
      Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
      Vector3 relativeMousePosition = mousePos - new Vector3(transform.position.x, transform.position.y, 0);
      Attack(relativeMousePosition);
  }

  void Dash()
  {
      // Placeholder for dash functionality
      Debug.Log("Dash triggered!");
  }

  private protected override void Die(){
    gameObject.SetActive(false);
    //TODO: Add Die Screen Transition Here!
    //Code for setting up the death screen should be here! --Stephen
  }
}
