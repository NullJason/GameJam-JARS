using UnityEngine;

public abstract class ItemButton : SystemButton
{
  [SerializeField] private protected Tool tool;

  //Initialize with the proper tool. 
  public void Init(Tool tool){
    this.tool = tool;
    Init();
  }
}
