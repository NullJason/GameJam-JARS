using UnityEngine;

public class SlimeCloner : ContactEnemy
{
  [SerializeField] private GameObject toBeSummonedOnDeath;
  [SerializeField] private int howMany = 1;
  [SerializeField] private float spread = 0.7f;
  private protected override void Die(){
    for(int i = 0; i < howMany; i++){
      Instantiate(toBeSummonedOnDeath, RandomPosition(spread), transform.rotation, transform.parent);
    }
    base.Die();
  }
  private protected Vector3 RandomPosition(float range){
    return new Vector3(transform.position.x + Random.Range(-range, range), transform.position.y + Random.Range(-range, range), transform.position.z);
  }
}
