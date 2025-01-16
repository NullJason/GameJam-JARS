using UnityEngine;

public class CowardEnemy : Enemy
{
  [SerializeField] private float speed;
  [SerializeField] private int reloadTime = -1; //Leave as neagative to use the Weapon's reload time instead!
  [SerializeField] private float minFleeDistance;
  private protected override EnemyStateMachine GetStateMachine(){
    EnemyState flee = new EnemyState(delegate(){ transform.Translate(TowardsPlayer() * -speed); Debug.Log("Trying to flee!"); });
    EnemyState attack = new EnemyState(delegate(){ Attack(TowardsPlayer()); Debug.Log("Trying to attack!"); });
    EnemyState reload = new EnemyState(delegate(){ Debug.Log("Doing naught!"); });
    EnemyStateTransition startAttacking = new EnemyStateTransition(delegate(){
        int i = reloadTime;
        if(i < 0) i = currentWeapon.ResetTime();
        return TimeOver(i);
    }, attack);
    EnemyStateTransition startReloading = new EnemyStateTransition(delegate()
    {
        return !CloseToPlayer(minFleeDistance);
    }, reload);
    EnemyStateTransition startFleeing = new EnemyStateTransition(delegate()
    {
        return CloseToPlayer(minFleeDistance);
    }, flee);
    reload.AddTransition(startAttacking);
    reload.AddTransition(startFleeing);
    attack.AddTransition(startFleeing);
    attack.AddTransition(startReloading);
    flee.AddTransition(startReloading);
    return new EnemyStateMachine(reload);
  }

}
