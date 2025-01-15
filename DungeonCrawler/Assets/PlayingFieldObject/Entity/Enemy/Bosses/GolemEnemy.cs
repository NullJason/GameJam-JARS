using UnityEngine;

public class GolemEnemy : BossEnemy
{
  [SerializeField] private Weapon swingWeapon;
  [SerializeField] private Weapon stompWeapon;
  [SerializeField] private protected float speed = 0.02f;
  [SerializeField] private protected float rotationSpeed = 0.2f;
  [SerializeField] private protected float rotationAccuracy = 5;
  private protected const float MAX_STOMP_DISTANCE = 3f;
  private protected const int SWING_ATTACK_RELOAD = 90;
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
    EnemyState swing = new EnemyState(delegate(){swingWeapon.TryAttack(transform.rotation);});
    EnemyState waitToStomp = new EnemyState(delegate(){
        transform.LookAt(PlayerMovement.mainPlayer.transform);
        transform.Rotate(0, -90, 0); //TODO: Remove Magic Numbers!
        if(PlayerMovement.mainPlayer.transform.position.x < transform.position.x) transform.Rotate(180, 0, 0);
    });
    EnemyState stomp = new EnemyState(delegate(){stompWeapon.TryAttack(transform.rotation);});
    move.AddTransition(new EnemyStateTransition(delegate(){return TimeOver(SWING_ATTACK_RELOAD);}, swing));
    move.AddTransition(new EnemyStateTransition(delegate(){return Vector3.SqrMagnitude((transform.position - PlayerMovement.mainPlayer.transform.position)) < MAX_STOMP_DISTANCE *  MAX_STOMP_DISTANCE;}, waitToStomp));
    waitToStomp.AddTransition(new EnemyStateTransition(delegate(){return TimeOver(60);}, stomp));
    move.AddTransition(new EnemyStateTransition(delegate(){return Mathf.Abs(transform.position.x) > 12 || Mathf.Abs(transform.position.y) > 6 && TimeOver(30);}, waitToStomp)); //TODO: Remove magic numbers! These represent the approximate bounds of the playing area. Find a better way to do this!
    swing.AddTransition(new EnemyStateTransition(delegate(){return true;}, move));
    stomp.AddTransition(new EnemyStateTransition(delegate(){return true;}, move));
    return new EnemyStateMachine(move);
  }
}
