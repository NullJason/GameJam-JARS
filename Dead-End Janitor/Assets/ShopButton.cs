using UnityEngine;

public abstract class ItemButton : SystemButton
{
  [SerializeField] private protected Tool tool;
  public void Init(Tool tool){
    this.tool = tool;
    Init();
  }
}
