using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeaponUiTab : MonoBehaviour
{
  [SerializeField] private protected Weapon weapon;
  void Start(){
    Button button = GetComponent<Button>();
    button.onClick.AddListener(ViewCurrentWeapon);
    TMP_Text TmpText = gameObject.GetComponent<TMP_Text>();
    TmpText.SetText(weapon.GetName());
    TmpText.color = Color.black;
    Debug.Log(gameObject.GetComponent<TMP_Text>());
  }
  public void SetWeapon(Weapon weapon){
    if(this.weapon != null) Debug.LogWarning("Error- Tried assigning weapon to a weapon ui tab, but there was already a weapon in that tab!");
    else this.weapon = weapon;
  }
  private protected void ViewCurrentWeapon(){
    WeaponUiManager.main.Display(weapon, this);
//    WeaponUiManager.main.HoldWeapon(weapon);
  }
}
