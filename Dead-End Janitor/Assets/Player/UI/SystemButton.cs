using UnityEngine;

public abstract class SystemButton : ButtonFunctionality
{
  ButtonSystem system;

  //How the button should initialize itself when the system signals for it to.
  public void Init(){
    Start();
  }

  //Initialize, but first set the proper system.
  public void Init(ButtonSystem b){
    if(system != null) Debug.LogWarning("Setting button system of " + this + " from " + system + " to " + b + ". Was this intended? ");
    system = b;
    Init();
  }

  //Signals for the ButtonSystem to reset all appropriate buttons.
  //If i < 0, assumes all buttons. Otherwise, resets button i in ButtonSystem's internal array.
  public void Signal(int i = -1){
    if(system != null) system.Refresh(i);
    else Debug.LogWarning("Could not refresh button system because the system did not exist or was null!");
  }
}
