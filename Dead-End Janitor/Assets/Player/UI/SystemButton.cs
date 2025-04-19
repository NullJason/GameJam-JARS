using UnityEngine;
using System.Collections.Generic;

//A SystemButton is a button that works as a part of one or more larger system, run by a ButtonSystem. A SystemButton can signal for the ButtonSystem to reset other SystemButton-s within the system.
//This may be useful to avoid having to repeatedly run some check over whether another button is pressed or if a certain button-related condition is met.

public abstract class SystemButton : ButtonFunctionality
{
  private protected List<ButtonSystem> systems;
  bool initialized = false;

  //How the button should initialize itself when the system signals for it to.
  public abstract void Reset();
  private protected override void Init(){
    base.Init();
    initialized = true;
  }
  private void OnEnable(){
    if(initialized){
      Reset();
    }
  }

  //Reset, but first set the proper system.
  public void SetUpSystem(ButtonSystem b){
    if(systems == null) systems = new List<ButtonSystem>();
    if(systems.Contains(b)) Debug.LogError("Error: Duplicate button system " + b + " added for button " + button + "! ");
    systems.Add(b);
    Reset();
  }

  //Signals for ButtonSystem in systems[system] to reset all appropriate buttons.
  //If i < 0, assumes all buttons in the system. Otherwise, resets button i in the ButtonSystem's internal array.
  public void Signal(int system = -1, int i = -1){
    if(systems == null) Debug.LogWarning("Systems has not yet been initialized! Skipping System refresh.");
    else if(system < 0){
      foreach(ButtonSystem b in systems) b.Refresh(i);
    }
    else if(system >= systems.Count) Debug.LogError("Could not refresh system " + system + " because it was out of range! ");
    else if(systems[system] != null) systems[system].Refresh(i);
    else Debug.LogWarning("Could not refresh button system because the system did not exist or was null!");
  }
}
