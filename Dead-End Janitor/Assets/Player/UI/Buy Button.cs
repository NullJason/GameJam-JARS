using UnityEngine;
using UnityEngine.UI;

//A button that buys some item if the player has enough points.
//Deactivates if the player acquires the tool through this button, or if the player already has the tool when this button is created.

public class BuyButton : ItemButton
{
  private protected override void Start(){
    base.Start();
    if(GameplayManager.main.CheckToolUnlocked(tool)) TurnOffButton();
  }
  [SerializeField] int cost;
  [SerializeField] bool save = true;
  private protected override void ButtonDo(){
    if(GameplayManager.main.TryUnlockTool(tool, cost)){
      TurnOffButton();
      if(save) GameplayManager.main.SaveGame();
    }
  }
  void TurnOffButton(){
    button.interactable = false;
  }
}
