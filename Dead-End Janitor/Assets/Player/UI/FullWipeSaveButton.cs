using UnityEngine;
using UnityEngine.UI;

//A button that resets all save file values to the default.
//Meant mostly for debug purposes.

public class WipeButton : ButtonFunctionality
{
  private protected override void ButtonDo(){
    GameplayManager.main.FullResetSave(); 
  }
}
