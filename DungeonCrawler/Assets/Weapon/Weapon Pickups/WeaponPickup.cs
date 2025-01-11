using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
  void OnTriggerEnter2D(Collider2D collider){
    Debug.Log("Hit!");
    if(collider.gameObject.tag == "Player"){
      Debug.Log("=D");
      WeaponUiManager.main.PickUpWeapon(transform.GetChild(0).GetComponent<Weapon>());
      Destroy(gameObject);
    }
  }
}
