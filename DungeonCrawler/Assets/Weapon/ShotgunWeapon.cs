using UnityEngine;

public class ShotgunWeapon : Weapon
{
  public override bool TryAttack(Quaternion rotation){
    if(cooldown == 0){
      cooldown = cooldownReset;
      SummonProjectile(rotation);
      SummonProjectile(rotation);
      SummonProjectile(rotation);
      SummonProjectile(rotation);
      SummonProjectile(rotation);
      return true;
    }
    return false;
  }
}
