using UnityEngine;

public class BlindChasingEnemy : Enemy
{
  [SerializeField] private float speed;
  [SerializeField] private int reload = -1; //Leave as neagative to use the Weapon's reload time instead!
  [SerializeField] private float maxChaseDistance;
  private protected override EnemyStateMachine GetStateMachine(){
    EnemyState charge = new EnemyState(delegate(){ transform.Translate(TowardsPlayer() * speed); Debug.Log("Trying to charge!"); });
    EnemyState attack = new EnemyState(delegate(){ Attack(TowardsPlayer()); Debug.Log("Trying to attack!"); });
    EnemyState idle = new EnemyState(delegate(){ Debug.Log("Doing naught!"); });
    EnemyStateTransition startAttacking = new EnemyStateTransition(delegate(){
        int i = reload;
        if(i < 0) i = currentWeapon.ResetTime();
        return TimeOver(i);
    }, attack);
    EnemyStateTransition startIdling = new EnemyStateTransition(delegate()
    {
        return !CloseToPlayer(maxChaseDistance);
    }, idle);
    EnemyStateTransition startCharging = new EnemyStateTransition(delegate()
    {
        return CloseToPlayer(maxChaseDistance);
    }, charge);
    charge.AddTransition(startAttacking);
    attack.AddTransition(startIdling);
    attack.AddTransition(startCharging);
    idle.AddTransition(startCharging);
    return new EnemyStateMachine(idle);
  }

}
