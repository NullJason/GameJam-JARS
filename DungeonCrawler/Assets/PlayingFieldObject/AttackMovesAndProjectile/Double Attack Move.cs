using UnityEngine;

public class DoubleAttackMove : ShotgunAttackMove
{
  [SerializeField] private protected int despawnRange = 0;
  private protected override void Start(){
    base.Start();
    despawnTimer+=Random.Range(-despawnRange, despawnRange);
  }
  void OnDestroy(){
    Instantiate(secondary, transform.position, transform.rotation);
  }
}
