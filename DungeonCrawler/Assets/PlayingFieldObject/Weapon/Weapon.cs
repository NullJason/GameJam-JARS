using UnityEngine;
//Represents a weapon that can be used by an Entity.
public class Weapon : PlayingFieldObject
{
  [SerializeField] private protected GameObject drop;
  [SerializeField] private protected GameObject wielder;
  [SerializeField] private protected int cooldownReset = 10;
  [SerializeField] private protected GameObject projectile; //The GameObject of the attack that this Weapon summons.
  [SerializeField] private protected int level; //The weapon's level will change the damage of the Attack created slightly. TODO!
  [SerializeField] private bool droppable = true; //Whether an enemy carrying this weapon can drop it.
  [SerializeField] private string name = "Placeholder Weapon";
  [SerializeField] private string description = "Hello!";
  [SerializeField] private protected int levelUpFramedrop = 4; //How many frames faster the weapon fires after leveling up once.
  private protected int cooldown;

  // Start is called once before the first execution of Update after the MonoBehaviour is created
  void Start()
  {
    cooldownReset -= level * levelUpFramedrop; //TODO: Remove Magic Numbers!
    cooldown = cooldownReset;
  }
  private protected override void GameFixedUpdate(){
    if(cooldown > 0) cooldown--;
  }
  //Feel free to override! This is just a default implementation.
  public virtual bool TryAttack(Quaternion rotation){
    if(cooldown == 0){
      cooldown = cooldownReset;
      SummonProjectile(rotation);
      return true;
    }
    return false;
  }
  private protected GameObject SummonProjectile(Quaternion rotation){
    GameObject projectile = Instantiate(this.projectile, transform.position, new Quaternion(0, 0, 0, 0));
    AttackMove attackMove = projectile.GetComponent<AttackMove>();
    AttackDamage attackDamage = projectile.GetComponent<AttackDamage>();
    if(attackMove != null) attackMove.SetDirection(rotation);
    else Debug.LogWarning("Cannot set direction of projectile \"" + projectile.name + "\" because it has no AttackMove script attached to it!");
    if(attackDamage != null) SetUpHittability(attackDamage);
    else Debug.LogWarning("Cannot set projectile \"" + projectile.name + "\" to hit the proper tags because it has no AttackDamage script attached to it!");
    return projectile;
  }
  private protected GameObject SummonProjectile(){
    return SummonProjectile(transform.rotation);
  }
  private protected void SetUpHittability(AttackDamage damager){
    damager.SetUpHittables(transform.parent.tag != "Player", transform.parent.tag != "Enemy");
  }
  //TODO: Two weapons are compared based on their names.
  public override bool Equals(object other){
    return Equals(other as Weapon);
  }
  public bool Equals(Weapon other){
    return other != null && other.name == this.name;
  }
  public override int GetHashCode(){
    return name.GetHashCode();
  }
  public int ResetTime(){
    return cooldownReset;
  }
  public string GetName(){
    return name;
  }
  public string GetNameAndLevel(){
    if(level != 0) return GetName() + "   (lv. " + level + ")";
    else return GetName();
  }
  public string GetInfo(){
    return description;
  }
  public int LevelUp(){
    level++;
    cooldownReset = cooldownReset - levelUpFramedrop;
    if(cooldownReset <= 0) cooldownReset = 1;
    return level;
  }
  public bool TryDrop(){
    if(droppable){
      transform.SetParent(Instantiate(drop, transform.position, new Quaternion(0, 0, 0, 0)).transform);
      cooldownReset += level * levelUpFramedrop; //Removes any levelup buff the weapon would have. TODO: Remove magic numbers!
      level = 0;
      return true;
    }
    return false;
  }
}
