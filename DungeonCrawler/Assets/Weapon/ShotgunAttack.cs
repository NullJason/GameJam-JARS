using UnityEngine;

public class ShotgunAttackMove : AttackMove
{
  private protected override void SetUp(){
    Wobble(40);
  }
  private protected override void Move(){
    GoForward(0.1f);
  }
}
