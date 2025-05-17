using UnityEngine;
using System;

public abstract class Weapon : MonoBehaviour, IComparable
{
  //TODO: Test.
  //See also WeaponSystem (TODO: Implement!).
  //A Weapon represents a particular means by which a Hunter may deal damage to its target. 
  //Which Weapon is chosen is determined through the mediation of a WeaponSystem. 
  //A Weapon has a particular cooldown. 
  //However, instead of constantly updating this cooldown, the cooldown is decreased at specific intervals by the mediating WeaponSystem. 
  //Specifically, the WeaponSystem subtracts the cooldown whenever a weapon enters cooldown, or whenever the Hunter or another class tells it that time has passed. 

  [SerializeField] int priority; //Used for choosing which Weapon is more 'desirable' for the Hunter's WeaponSystem to select. 
  [SerializeField] private protected int cooldownReset = 60; //How long it takes for the weapon to be usable again after using to attack. 
  int cooldown = 0; 
  [SerializeField] private protected int cooldownDecrease = 20; //How much the cooldown of other weapons is decreased after this weapon is used to attack. It is recommended that this is roughly the amount of time in frames that the weapon is in use. 
  Humanoid target;
  WeaponSystem system;
  [SerializeField] RangeCategory category;

  //Decrease the cooldown timer by a set amount. 
  public void DecreaseCooldown(int decrease){
    cooldown -= decrease;
    if(cooldown < 0) cooldown = 0;
    Debug.Log("Cleaning time remaining for weapon " + gameObject + ": " + cooldown);
  }
  public bool CanAttack(){
    return cooldown == 0 && !IsAttacking();
  }

  //Returns true if the given weapon is currently performing an attack. 
  //Optional operation. 
  public abstract bool IsAttacking();

  //Called by the weapon system every frame until Done() is called. 
  public abstract void OnUpdate();

  //Tell the weapon system it no longer needs to update this weapon. 
  private protected void Done(){
    system.End(this);
  }

  //Tells the weapon system that it can find another weapon to start attacking with. 
  public virtual void DoneAttacking(){
    cooldown = cooldownReset;
    system.ResetAttacking(this, cooldownDecrease);
  }

  public bool TryStartAttack(Humanoid target){
    if(CanAttack()){
      this.target = target;
      system.Run(this);
      StartAttack();
      return true;
    }
    return false;
  }
  
  public RangeCategory GetCategory(){
    return category;
  }

  //Compares the priority of this weapon to a value.
  //  positive numbers indicate the priority is higher than the value. 
  //  negative numbers indicate the priority is lower than the value. 
  //  zero indicates the priority is equal to the value. 
  public int ComparePriority(int i){
    return priority - i;
  }

  //Initializes the weapon system of this weapon. 
  public void SetSystem(WeaponSystem system){
    if(this.system == system) Debug.LogWarning("Setting Weapon System of " + this + " from " + this.system + " to " + system + ". (Was this intended?");
    if(system == null) Debug.LogError("Setting Weapon System of " + this + " from " + this.system + " to null. (Was this intended?");
    this.system = system;
  }

  //The default comparison of a Weapon is directly based on that weapon's priority. 
  public int CompareTo(object obj){
    if(obj == null) Debug.LogWarning("Could not compare Weapon " + this + " to null!");
    Weapon other = obj as Weapon;
    if(other == null) Debug.LogWarning("Could not compare Weapon " + this + " to " + obj + " because it was not a Weapon!");
    return other.ComparePriority(this.priority);
  }

  private protected abstract void StartAttack();
}

public enum RangeCategory{
  infinite = 10, 
  far = 8, 
  mid = 6, 
  near = 4, 
  melee = 2
}
