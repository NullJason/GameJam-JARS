using UnityEngine;
//Represents a weapon that can be used by an Entity.
public class Weapon : MonoBehaviour
{
  [SerializeField] private protected GameObject wielder;
  [SerializeField] private protected int cooldownReset = 10;
  [SerializeField] private protected GameObject projectile; //The GameObject of the attack that this Weapon summons.
  [SerializeField] private protected int level; //The weapon's level will change the damage of the Attack created slightly. TODO!
  [SerializeField] private bool droppable; //Whether an enemy carrying this weapon can drop it.
  private protected int cooldown;

  // Start is called once before the first execution of Update after the MonoBehaviour is created
  void Start()
  {
    cooldown = cooldownReset;
  }
  void Update(){
    if(cooldown > 0) cooldown--;
//Placeholder attack functionality. In the actual game, attacking should be run through the associated Entity calling TryAttack().
//    if(Input.GetButtonDown("Fire1")) for(int i = 0; i < 5; i++) SummonProjectile();
  }
  //Feel free to override! This is just a default implementation.
  public virtual bool TryAttack(Quaternion rotation){
    Debug.Log("=D");
    if(cooldown == 0){
      cooldown = cooldownReset;
      SummonProjectile(rotation);
      return true;
    }
    return false;
  }
  private protected void SummonProjectile(Quaternion rotation){
    GameObject projectile = Instantiate(this.projectile, transform.position, new Quaternion(0, 0, 0, 0));
    AttackMove attackMove = projectile.GetComponent<AttackMove>();
    AttackDamage attackDamage = projectile.GetComponent<AttackDamage>();
    if(attackMove != null) attackMove.SetDirection(rotation);
    else Debug.LogWarning("Cannot set direction of projectile \"" + projectile.name + "\" because it has no AttackMove script attached to it!");
    if(attackDamage != null) SetUpHittability(attackDamage);
    else Debug.LogWarning("Cannot set projectile \"" + projectile.name + "\" to hit the proper tags because it has no AttackDamage script attached to it!");
  }
  private protected void SummonProjectile(){
    SummonProjectile(transform.rotation);
  }
  private protected void SetUpHittability(AttackDamage damager){
    damager.SetUpHittables(gameObject.tag != "Player", gameObject.tag != "Enemy");
  }
  //TODO: Two weapons are compared based on their names.
  //private void Equals()
}
