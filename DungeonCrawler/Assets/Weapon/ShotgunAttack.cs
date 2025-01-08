using UnityEngine;
public class ShotgunAttackMove : AttackMove
{
  [SerializeField] private protected float wobbleRange = 40;
  [SerializeField] private protected float speed = 0.1f;
  private protected override void SetUp(){
    Wobble(wobbleRange);
  }
  private protected override void Move(){
    GoForward(speed);
  }
}
