using UnityEngine;
using System.Collections.Generic;

//An Enemy is any Entity that uses an EnemyStateMachine.

public abstract class Enemy : Entity
{
  private protected virtual void Start(){
    machine = GetStateMachine();
  }
  private protected EnemyStateMachine machine;
  override private protected void Behave(){
    machine.Run();
  }
  private protected abstract EnemyStateMachine GetStateMachine();
  private protected bool CloseToPlayer(double d){
    return (PlayerMovement.mainPlayer.transform.position - transform.position).sqrMagnitude < (d * d);
  }
  private protected Vector2 TowardsPlayer(){
    Vector2 player = new Vector2(PlayerMovement.mainPlayer.transform.position.x, PlayerMovement.mainPlayer.transform.position.y);
    Vector2 here = new Vector2(transform.position.x, transform.position.y);
    Vector2 result = player - here;
    result.Normalize();
    return result;
  }
  private protected bool TimeOver(int i){
    return machine.Time() > i;
  }
  private protected bool TimeOn(int i){
    return machine.Time() == i;
  }
}


class EnemyStateMachine{
  int timer = 0;
  public EnemyStateMachine(EnemyState current){
    this.current = current;
  }
  private protected EnemyState current;
  public void Run(){
    timer++;
    current.DoAction();
    EnemyState nextMaybe = current.GetNext();
    if(nextMaybe != null) {
      current = nextMaybe;
      timer = 0;
    }
  }
  //How long it's been since the current State began.
  public int Time(){
    return timer;
  }
}


class EnemyState{
  private Action action;
  public EnemyState(Action action){
    this.action = action;
  }
  HashSet<EnemyStateTransition> transitions;
  public delegate void Action();
  public void DoAction(){
    action();
  }
  public void AddTransition(EnemyStateTransition transition){
    if(transitions == null) transitions = new HashSet<EnemyStateTransition>();
    transitions.Add(transition);
  }
  public EnemyState GetNext(){
    foreach(EnemyStateTransition transition in transitions){
      EnemyState nextMaybe = transition.Check();
      if(nextMaybe != null) return nextMaybe;
    }
    return null;
  }
}


 class EnemyStateTransition{
  private EnemyState to;
  private Conditions conditions;
  public EnemyStateTransition(Conditions conditions, EnemyState to){
    this.conditions = conditions;
    this.to = to;
  }
  //Returns the 'to' EnemyState if the transition conditions have been met, null otherwise.
  public delegate bool Conditions();

  public EnemyState Check(){
    if(conditions()) {
      return to;
    }
    else return null;
  }
}
