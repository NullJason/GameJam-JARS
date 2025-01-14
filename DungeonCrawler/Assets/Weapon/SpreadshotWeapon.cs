using UnityEngine;

public class SpreadshotWeapon : Weapon
{
  [SerializeField] private protected float spread = 5;
  [SerializeField] private protected int howMany = 5;

  public override bool TryAttack(Quaternion rotation){
    if(cooldown == 0){
      cooldown = cooldownReset;
      for(int i = 0; i < howMany; i++){
        GameObject projectile = SummonProjectile(rotation);
        projectile.transform.Rotate(0, 0, (-spread * (howMany - 1)) / 2);
        projectile.transform.Rotate(0, 0, i * spread);
      }
      return true;
    }
    return false;
  }
}
