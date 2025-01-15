using UnityEngine;

//An example/testing enemy.

public class ShittyEnemy : Enemy
{
  private protected override EnemyStateMachine GetStateMachine(){
    EnemyState pooping = new EnemyState(delegate(){ Debug.Log("Pooping!"); }); //First, set up a few states that the Enemy can be in.
    EnemyState barfing = new EnemyState(delegate(){ Debug.Log("Barfing!"); });
    EnemyState eating = new EnemyState(delegate(){ Debug.Log("Eating!"); });
    EnemyStateTransition toBarfing = new EnemyStateTransition(delegate(){ return CloseToPlayer(2); }, barfing); //Then, make some transitions between them...
    EnemyStateTransition eatingToPooping = new EnemyStateTransition(delegate(){ return Input.GetKeyDown(KeyCode.Space); }, pooping);
    EnemyStateTransition barfingToEating = new EnemyStateTransition(delegate(){ return Input.GetButtonDown("Fire1"); }, eating);
    pooping.AddTransition(toBarfing); //Finally, add the transitions to the proper states...
    eating.AddTransition(toBarfing);
    eating.AddTransition(eatingToPooping);
    barfing.AddTransition(barfingToEating);
    return new EnemyStateMachine(barfing); //...And return a State Machine based off whatever you want the first stage to be.
  }
}
