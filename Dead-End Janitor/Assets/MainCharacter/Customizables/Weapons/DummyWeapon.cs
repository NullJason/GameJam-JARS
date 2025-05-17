using UnityEngine;

public class DummyWeapon : Weapon
{
  int timer;

  public override bool IsAttacking(){
    return false;
  }
  public override void OnUpdate(){
    Debug.Log("weaponie" + gameObject.name);
    timer++;
    if(timer == cooldownDecrease) DoneAttacking();
    if(timer == cooldownDecrease + 0) Done();
  }
  private protected override void StartAttack(){
    Debug.Log("weaponie" + gameObject.name + "start!");
    transform.localScale = new Vector3(1, 2, 1);
    timer = 0;
  }
  public override void DoneAttacking(){
    base.DoneAttacking();
    transform.localScale = new Vector3(1, 1, 1);
  }
}
