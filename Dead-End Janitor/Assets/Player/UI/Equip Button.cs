using UnityEngine;
using UnityEngine.UI;

//A button that equips an item when clicked. It will automatically disable the button if the tool is already equipped, and re-enable if the tool is un-equipped.
//Automatically disables the GameObject attached to the button if the tool is not yet unlocked, and only enables when the tool becomes unlocked. Note that this will not work if the button is attached to the same gameObject as this script!

public class EquipButton : ItemButton
{
  private protected override void Init(){
    base.Init();
    dirtType = Tools.GetDirtType(tool);
    if(!GameplayManager.main.CheckToolUnlocked(tool)) button.gameObject.SetActive(false);
  }
  private Dirty dirtType;
  [SerializeField] bool save = true;
  private protected override void ButtonDo(){
    GameplayManager.main.SetTool(tool, dirtType);
    if(save) GameplayManager.main.SaveGame();
  }
  private protected void Update(){
    if(GameplayManager.main.GetTool(dirtType) == tool){
      TurnOffButton();
    }
    else{
      TurnOnButton();
    }
    if(GameplayManager.main.CheckToolUnlocked(tool)) button.gameObject.SetActive(true);
  }
  void TurnOffButton(){
    button.interactable = false;
  }
  void TurnOnButton(){
    button.interactable = true;
  }
}
