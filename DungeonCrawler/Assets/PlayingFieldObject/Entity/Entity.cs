using UnityEngine;
//An Entity represents a thing that has weapons, stats, and movement, i.e. an enemy or the player.
public abstract class Entity : PlayingFieldObject
{
  private const int INITIAL_HEALTH = 100;
  [SerializeField] private int health = INITIAL_HEALTH;
  private int invincibilityFrames = 0;
  [SerializeField] private protected Weapon currentWeapon;
  abstract private protected void Behave();//Controls movement and firing.
  private protected override void GameFixedUpdate(){
    Behave();
    if(invincibilityFrames > 0) invincibilityFrames--;
  }
  //Manually sets the enemy or player invincibility frame number to the specified number of frames, regardless of current invincibility status. Prints various warning info for debugging purposes.
  public void GrantManualInvincibility(int numOfFrames){
    GrantInvincibility(numOfFrames, true);
  }
  private void GrantInvincibility(int numOfFrames, bool info = false){
    if(info){
      Debug.LogWarning("Granting invincibility to " + gameObject.name + " for " + numOfFrames + "frames...");
      if(numOfFrames < 0) Debug.LogWarning("  A negative number of invincibility frames have been assigned! No invincibility will be granted.");
      if(invincibilityFrames > 0) Debug.LogWarning("  Invincibility frames have been assigned, but the GameObject is already invincible! Resetting invincibility to the assigned amount...");
    }
    invincibilityFrames = numOfFrames;
  }
  public void AddManualHealth(int amount){
    AddHealth(amount, true);
  }
  //Manually changes the enemy or player health by the specified number, regardless of current invincibility status and allowing <0 and >STARTING_HEALTH. Prints various warning info for debugging purposes.
  private void AddHealth(int amount, bool info = false){
    if(info){
      Debug.LogWarning("Adding " + amount + " health to " + gameObject.name + " for a total of " + (amount + health) + "...");
      if(health + amount < 0) Debug.LogWarning("  The new total health is negative! This will automatically kill the player or enemy!");
      if(health + amount > INITIAL_HEALTH) Debug.LogWarning("  The new total health is greater than the starting health! Was this intended?");
      if(IsInvincible()) Debug.LogWarning("  Health is being changed during invincibility! Was this intended?");
    }
    health += amount;
    if(health <= 0 && health - amount > 0){ //If it becomes dead after this hit, but not if it would already be dead before this hit! This is to prevent Die() from being called multiple times per hit.
      if(info) Debug.LogWarning("  Killing player or enemy...");
      Die();
    }
  }
  public int GetHealth(){
    return health;
  }
  //Does damage and triggers invincibility frames, regardless of whether the object is still invincible or not.
  public void ForceHit(int damage, int invincibility){
    AddHealth(-damage, false);
    GrantInvincibility(invincibility, false);
  }
  //Does damage and triggers invincibility frames, assuming the Entity can be damaged (i.e. the Entity is not invincible).
  public bool TryHit(int damage, int invincibility){
    if(!IsInvincible()){
      ForceHit(damage, invincibility);
      return true;
    }
    return false;
  }
  public bool IsInvincible(){
    return invincibilityFrames > 0;
  }

  private protected virtual void Die(){
    DropWeapons();
    Destroy(gameObject);
  }
  private protected virtual void DropWeapons(){
    currentWeapon.TryDrop();
  }
  private protected void Attack(Vector3 direction){
    float angle = (Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
    currentWeapon.TryAttack(Quaternion.Euler(new Vector3(0, 0, angle)));
  }
  public void SetWeapon(Weapon weapon){
    currentWeapon = weapon;
  }
  //Like SetWeapon, but only works if it doesn't already have a weapon. Returns true if it wasn't holding a weapon and now it is.
  public bool InitializeWeapon(Weapon weapon){
    if(currentWeapon != null) return false;
    if(weapon == null){
      Debug.LogWarning("Weapon was set from null to null. Was this intended?");
      SetWeapon(weapon);
      return false;
    }
    SetWeapon(weapon);
    return true;
  }
}
