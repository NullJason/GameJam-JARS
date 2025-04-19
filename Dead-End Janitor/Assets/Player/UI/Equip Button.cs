using UnityEngine;
using UnityEngine.UI;

//A button that equips an item when clicked. When part of a button system, it will behave as following when signalled:
//  It will automatically disable the button if the tool is already equipped, or if the tool is not yet owned.
//  It will automatically enable the button if the tool is owned and unequipped.

public class EquipButton : ItemButton
{
  private protected override void Init(){
    base.Init();
    Reset();
  }
  public override void Reset(){
    dirtType = Tools.GetDirtType(tool);
    CheckTurnOn();
  }
  private Dirty dirtType;
  [SerializeField] bool save = true;
  private protected override void ButtonDo(){
    GameplayManager.main.SetTool(tool, dirtType);
    if(save) GameplayManager.main.SaveGame();
    Signal();
  }

  //Removed, due to being slightly expensive.
/*  private protected void Update(){
    if(GameplayManager.main.GetTool(dirtType) == tool){
      TurnOffButton();
    }
    else{
      TurnOnButton();
    }
    if(GameplayManager.main.CheckToolUnlocked(tool)) button.gameObject.TurnOffButton();
  }*/

  //Turns on or off the button, in this case based on whether the item has been purchased and isn't already being held.
  private protected void CheckTurnOn(){
    if(!GameplayManager.main.CheckToolUnlocked(tool) || GameplayManager.main.GetTool(dirtType) == tool) TurnOffButton();
    else TurnOnButton();
}

  void TurnOffButton(){
    if(button == null){
      Debug.Log("Button not initialized, attempting to find!");
      TryFindButton();
    }
    button.interactable = false;
  }
  void TurnOnButton(){
    if(button == null){
      Debug.Log("Button not initialized, attempting to find!");
      TryFindButton();
    }
    button.interactable = true;
  }
}
