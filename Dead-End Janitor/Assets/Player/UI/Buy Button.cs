using UnityEngine;
using UnityEngine.UI;

//A button that buys some item if the player has enough points.
//Deactivates if the player acquires the tool through this button, or if the player already has the tool when this button is created.

public class BuyButton : ItemButton
{
  public override void Reset(){
    base.Reset();
    if(GameplayManager.main.CheckToolUnlocked(tool)) TurnOffButton();
  }
  [SerializeField] int cost;
  [SerializeField] bool save = true;
  private protected override void ButtonDo(){
    Debug.Log("=3" + transform.parent.gameObject.name);
    if(GameplayManager.main.TryUnlockTool(tool, cost)){
      TurnOffButton();
      Signal();
      if(save) GameplayManager.main.SaveGame();
      transform.Translate(500, 0, 0);
    }
  }
  void TurnOffButton(){
    button.interactable = false;
  }
  void Warning(){
    Debug.Log("=3");
  }
}
