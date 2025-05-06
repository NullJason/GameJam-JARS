using UnityEngine;
using System.Collections.Generic;

public class WeaponSystem : MonoBehaviour
{
  //TODO!!!

  private protected HashSet<Weapon> running; //A set that contains all weapons that are currently running on this system. 
  private protected List<Weapon> superRanged;
  private protected List<Weapon> ranged;
  private protected List<Weapon> mid;
  private protected List<Weapon> near;
  private protected List<Weapon> all;//The category with the lowest minimum range. It contains all weapons, since a weapon that can hit a farther target can automatically hit a nearer target. 
  private protected Weapon attacking; //The weapon that is currently attacking. 
  private protected Humanoid target; //The target that the attacking weapon is attacking. 


  //Let the WeaponSystem know it should run continuous updates on a particular weapon. 
  //Usually called by a Weapon when it starts attacking. 
  public void Run(Weapon weapon){
    running.Add(weapon);
  }

  private void Update(){
    foreach(Weapon weapon in running) weapon.OnUpdate();
    if(attacking == null && target != null) StartNewAttack();
  }

  //Ends the weapon that is currently attacking. 
  //If weapon does not match attacking, throws an error. 
  public void ResetAttacking(Weapon weapon, int timeElapsed){
    if(attacking != weapon) Debug.LogError("Weapon " + weapon + " did not match the currently attacking weapon " + attacking + "! Attacking has not been stopped. ");
    else{
      attacking = null;
      ElapseCooldownTime(timeElapsed);
    }
  }

  //Lets the WeaponSystem know that it should no longer run continuous updates on a particular weapon. 
  //Usually called by a Weapon that has completed everything related to attacking. 
  public void End(Weapon weapon){
    running.Remove(weapon);
  }

  //Decreases the cooldown of all weapons by time. 
  //Meant to be called when a weapon finishes attacking for a certain amount of time, or when the associated Hunter wants a certain amount of time to pass. 
  //Automatically called when a weapon finishes attacking by calling ResetAttack(). 
  public void ElapseCooldownTime(int time){
    foreach(Weapon weapon in all) weapon.DecreaseCooldown(time);
  }

  //Adds a weapon to the WeaponSystem's internal collections. 
  private protected void AddWeapon(Weapon weapon){
    all.Add(weapon);
    if(weapon.GetCategory() >= RangeCategory.near) near.Add(weapon);
    if(weapon.GetCategory() >= RangeCategory.mid) mid.Add(weapon);
    if(weapon.GetCategory() >= RangeCategory.far) ranged.Add(weapon);
    if(weapon.GetCategory() >= RangeCategory.infinite) superRanged.Add(weapon);
    superRanged.Sort();
    ranged.Sort();
    mid.Sort();
    near.Sort();
    all.Sort();
  }

  private protected Weapon GetBestWeaponByCategory(RangeCategory category){
    if(category == RangeCategory.melee) return GetBest(all);
    if(category == RangeCategory.near) return GetBest(near);
    if(category == RangeCategory.mid) return GetBest(mid);
    if(category == RangeCategory.far) return GetBest(ranged);
    if(category == RangeCategory.infinite) return GetBest(superRanged);
    Debug.LogError("Unexpected range category " + category + ". Returning best longest-ranged. ");
    return GetBest(superRanged);
  }

  //Takes a sorted list of weapons, and returns the first one that can attack. 
  //Takes advantage of the fact that, by default, Weapons are sorted based on priority. 
  //By doing this, we can find the weapon with the highest priority that can attack. 
  private protected Weapon GetBest(List<Weapon> weapons){
    foreach(Weapon weapon in weapons){
      if(weapon.CanAttack()) return weapon;
    }
    Debug.LogError("No valid weapon! Returning null.");
    return null;
  }
  

//  public void Attack(Humanoid target){
//    if(target != null && !attacking){
//      RangeCategory targetCategory = GetCategory(target.gameObject.transform);
//      if(!GetBestWeaponByCategory(targetCategory).TryStartAttack(target)) Debug.LogWarning("Weapon could not attack!");
//    }
//  }

  //Finds the range category that an enemy fits under, based on its distance to the hunter. 
  //TODO!!
  private protected RangeCategory GetCategory(Transform t){
    return RangeCategory.melee;
  }

  //Sets attacking to the next available highest priority weapon, and causes it to run. 
  //Throws an error if attacking was not null. 
  //Throws an error if target is null. 
  //Throws an error if the weapon somehow failed to start attacking. 
  private protected void StartNewAttack(){
    if(attacking != null) Debug.LogError("Could not start attacking with new weapon, as attacking weapon was " + attacking + " and not null!");
    if(target == null) Debug.LogError("Could not start attacking, as target was null!");
    RangeCategory targetCategory = GetCategory(target.gameObject.transform);
    attacking = GetBestWeaponByCategory(targetCategory);
    bool b = attacking.TryStartAttack(target);
    if(!b) Debug.LogError("Attacking weapon " + attacking + " failed to start attacking!");
  }
}
