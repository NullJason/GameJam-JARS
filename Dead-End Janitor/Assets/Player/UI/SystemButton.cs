using UnityEngine;

//A SystemButton is a button that works as a part of a larger system, run by a ButtonSystem. A SystemButton can signal for the ButtonSystem to reset other SystemButton-s within the system.
//This may be useful to avoid having to repeatedly run some check over whether another button is pressed or if a certain button-related condition is met.

public abstract class SystemButton : ButtonFunctionality
{
  private protected ButtonSystem system;

  //How the button should initialize itself when the system signals for it to.
  public abstract void Reset();

  //Reset, but first set the proper system.
  public void SetUpSystem(ButtonSystem b){
    if(system != null) Debug.LogWarning("Setting button system of " + this + " from " + system + " to " + b + ". Was this intended? ");
    system = b;
    Reset();
  }

  //Signals for the ButtonSystem to reset all appropriate buttons.
  //If i < 0, assumes all buttons. Otherwise, resets button i in ButtonSystem's internal array.
  public void Signal(int i = -1){
    if(system != null) system.Refresh(i);
    else Debug.LogWarning("Could not refresh button system because the system did not exist or was null!");
  }
}
