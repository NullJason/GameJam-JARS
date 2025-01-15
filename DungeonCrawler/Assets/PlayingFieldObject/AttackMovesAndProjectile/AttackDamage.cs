using UnityEngine;

//Attach this script to an attack or projectile (either player's attack or enemy's), and it will deal damage correctly!

public class AttackDamage : MonoBehaviour
{
  //TODO: Replace bool hitPlayer and bool hitEnemy with String tagToBeHit? This would allow an AttackDamage to hit GameObjects of any tag, but would also only allow it to hit that one particular tag.
  [SerializeField] private bool hitPlayer = false; //Represents whether this attack will damage the player. Should be set by the Weapon that summons this attack.
  [SerializeField] private bool hitEnemy = true; //Represents whether this attack will damage enemies. Should be set by the Weapon that summons this attack.
  [SerializeField] private int damage = 1; //TODO: Should this be a float?
  [SerializeField] private bool despawnImmediatelyOnContact = true; //Whether the attack will disappear after hitting something it damages.
  [SerializeField] private int invincibilityFrameCount = 5; //How many frames an entity will be immune to damage after being hit by this attack.

  void OnTriggerEnter2D(Collider2D col){
    if((col.gameObject.tag == "Player" && hitPlayer) || (col.gameObject.tag == "Enemy" && hitEnemy)){
      GameObject hit = col.gameObject;
      Entity data = hit.GetComponent<Entity>();
      OnHit(data);
      if(despawnImmediatelyOnContact) {
        Destroy(gameObject);
      }
    }
  }
  //What happens when colliding with a GameObject with a correct tag?
  private protected void OnHit(Entity hitData){
    if(hitData != null) hitData.TryHit(damage, invincibilityFrameCount);
    else Debug.LogWarning("Attempting to hit a GameObject without an Entity Script!");
  }
  public void SetUpHittables(bool hitPlayer, bool hitEnemy){
    this.hitPlayer = hitPlayer;
    this.hitEnemy = hitEnemy;
  }
}
