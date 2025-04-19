using UnityEngine;
using UnityEngine.UI;

//A button that buys some item if the player has enough points.
//Deactivates if the player acquires the tool through this button, or if the player already has the tool when this button is created.

public class BuyButton : ItemButton
{
  public override void Reset(){
    base.Reset();
    CheckTurnOn();
  }
  [SerializeField] int cost;
  [SerializeField] bool save = true;
  private protected override void ButtonDo(){
    if(GameplayManager.main.TryUnlockTool(tool, cost)){
      TurnOffButton();
      Signal();
      if(save) GameplayManager.main.SaveGame();
    }
  }

  //Turns on or off the button, in this case based on whether the item hasn't already been purchased.
  private protected void CheckTurnOn(){
    if(GameplayManager.main.CheckToolUnlocked(tool)) TurnOffButton();
    else TurnOnButton();
}
  void TurnOffButton(){
    if(button == null) TryFindButton();
    button.interactable = false;
  }
  void TurnOnButton(){
    if(button == null) TryFindButton();
    button.interactable = true;
  }
}
