using UnityEngine;

public abstract class ItemButton : SystemButton
{
  [SerializeField] private protected Tool tool = Tool.error;

  public override void Reset(){

  }

  public void TrySetTool(Tool t){
    if(tool == Tools.Empty()) tool = t;
    else Debug.LogError("Could not set tool because tool was already set to " + tool + "! ");
  }
}
