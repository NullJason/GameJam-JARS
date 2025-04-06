using UnityEngine;

public abstract class ItemButton : ButtonFunctionality
{
  [SerializeField] private protected Tool tool;
  public void Init(){
    Init(tool);
  }
  public void Init(Tool tool){
    this.tool = tool;
    Start();
  }
}
