using UnityEngine;

public class GolemEnemy : Enemy
{
  private Weapon swingWeapon;
  private Weapon stompWeapon;
  [SerializeField] private protected float speed = 0.02f;
  [SerializeField] private protected float rotationSpeed = 0.2f;
  [SerializeField] private protected float rotationAccuracy = 5;
  private protected override EnemyStateMachine GetStateMachine(){
    EnemyState move = new EnemyState(delegate(){
      Vector2 currentDirection = transform.rotation * new Vector2(1, 0);
      float rotation = Vector2.SignedAngle(currentDirection, TowardsPlayer());
      Debug.Log(rotation);
      if(rotation > rotationAccuracy || rotation < -rotationAccuracy){
        rotation = Mathf.Sign(rotation);
        transform.Rotate(0, 0, rotation * rotationSpeed);
      }
      transform.Translate(new Vector2(speed, 0));
    });
    move.AddTransition(new EnemyStateTransition(delegate(){return true;}, move));
    return new EnemyStateMachine(move);
  }
}
