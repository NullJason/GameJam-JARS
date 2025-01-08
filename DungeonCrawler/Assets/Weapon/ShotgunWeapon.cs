using UnityEngine;

public class ShotgunWeapon : Weapon
{
  [SerializeField] private protected int howMany = 5;
  public override bool TryAttack(Quaternion rotation){
    if(cooldown == 0){
      cooldown = cooldownReset;
      for(int i = 0; i < howMany; i++){
        SummonProjectile(rotation);
      }
      return true;
    }
    return false;
  }
}
