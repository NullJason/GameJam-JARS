using UnityEngine;

public class Skeleking : BossEnemy
{
  [SerializeField] private protected GameObject minion;
  private GameObject minion1;
  private GameObject minion2;
  private GameObject minion3;
  private GameObject minion4;
  private GameObject minion5;
  private protected const int SUMMON_RELOAD = 350;
  private protected override EnemyStateMachine GetStateMachine(){
    EnemyState idle = new EnemyState(delegate(){Attack(TowardsPlayer());});
    EnemyState summon = new EnemyState(delegate(){
        if(minion1 == null) minion1 = Instantiate(minion, transform.position, transform.rotation, transform.parent);
        else if(minion2 == null) minion2 = Instantiate(minion, transform.position, transform.rotation, transform.parent);
        else if(minion3 == null) minion3 = Instantiate(minion, transform.position, transform.rotation, transform.parent);
        else if(minion4 == null) minion4 = Instantiate(minion, transform.position, transform.rotation, transform.parent);
        else if(minion5 == null) minion5 = Instantiate(minion, transform.position, transform.rotation, transform.parent);
    });
    idle.AddTransition(new EnemyStateTransition(delegate(){return TimeOver(SUMMON_RELOAD);}, summon));
    summon.AddTransition(new EnemyStateTransition(delegate(){return true;}, idle));
    return new EnemyStateMachine(summon);
  }
}
