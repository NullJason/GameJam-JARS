using UnityEngine;

public class StupidEnemy : Enemy
{
  [SerializeField] private float speed;
  [SerializeField] private float range;
  [SerializeField] private int reload = -1; //Leave as neagative to use the Weapon's reload time instead!
  [SerializeField] private float maxChaseDistance;
  private protected override EnemyStateMachine GetStateMachine(){
    EnemyState charge = new EnemyState(delegate(){ transform.Translate(TowardsPlayer() * speed); Debug.Log("Trying to charge!"); });
    EnemyState attack = new EnemyState(delegate(){ Attack(TowardsPlayer()); Debug.Log("Trying to attack!"); });
    EnemyState idle = new EnemyState(delegate(){ Debug.Log("Doing naught!"); });
    EnemyStateTransition startAttacking = new EnemyStateTransition(delegate(){ return CloseToPlayer(range); }, attack);
    EnemyStateTransition startRestingAfterAttack = new EnemyStateTransition(delegate(){ return true; }, idle);
    EnemyStateTransition startChargingAgainAfterResting = new EnemyStateTransition(delegate()
    {
        int i = reload;
        if(i < 0) i = currentWeapon.ResetTime();
        return (TimeOver(i) && CloseToPlayer(maxChaseDistance) && !CloseToPlayer(range));
    }, charge);
    EnemyStateTransition startAttackingAgainAfterResting = new EnemyStateTransition(delegate()
    {
        int i = reload;
        if(i < 0) i = currentWeapon.ResetTime();
        return (TimeOver(i) && CloseToPlayer(range));
    }, attack);
    charge.AddTransition(startAttacking);
    attack.AddTransition(startRestingAfterAttack);
    idle.AddTransition(startChargingAgainAfterResting);
    idle.AddTransition(startAttackingAgainAfterResting);
    return new EnemyStateMachine(idle);
  }

}
