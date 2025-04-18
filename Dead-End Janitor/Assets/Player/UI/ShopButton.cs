using UnityEngine;

public abstract class ItemButton : SystemButton
{
  [SerializeField] private protected Tool tool;

  public override void Reset(){

  }

  //Initialize the proper tool and system and reset.
  public void SetUpSystem(Tool tool, ButtonSystem system){
    this.tool = tool;
    SetUpSystem(system);
  }
}
