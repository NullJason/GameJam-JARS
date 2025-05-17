using UnityEngine;

public class ItemSystem : ButtonSystem
{
  [SerializeField] Tool tool;
  [SerializeField] DisplayToolName display;
  private protected override void Awake(){
    if(tool != Tools.Empty()){
      foreach(ItemButton b in buttons){
        b.TrySetTool(tool);
        b.SetUpSystem(this);
      }
      Debug.Log("Set Up Properly!");
    }
    else foreach(ItemButton b in buttons) b.SetUpSystem(this);
    if(display != null) display.SetUp(tool);
  }
  public Tool GetTool(){
    return tool;
  }

  void OnDisable(){
    Debug.Log("Bad!");
  }

  public void Display(){
    if(display != null){
      display.SetUp(tool);
      display.Display();
    }
    else Debug.LogWarning("Could not display tool because no display was provided! ");
  }
	

}
