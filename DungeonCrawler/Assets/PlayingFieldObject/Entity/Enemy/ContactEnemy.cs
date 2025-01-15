using UnityEngine;

public class ContactEnemy : Enemy
{
  [SerializeField] private protected float speed;
  [SerializeField] private protected float range;
  private protected override EnemyStateMachine GetStateMachine(){
    EnemyState idle = new EnemyState(delegate(){ });
    EnemyState charge = new EnemyState(delegate(){ transform.Translate(TowardsPlayer() * speed); Attack(transform.forward); });
    EnemyStateTransition startCharging = new EnemyStateTransition(delegate(){ return CloseToPlayer(range); }, charge);
    EnemyStateTransition outtaRange = new EnemyStateTransition(delegate(){ return !CloseToPlayer(range); }, idle);
    idle.AddTransition(startCharging);
    charge.AddTransition(outtaRange);
    return new EnemyStateMachine(idle);
  }
}
