using UnityEngine;

public class ItemSystem : ButtonSystem
{
  [SerializeField] Tool tool;
  private protected override void Awake(){
    if(tool != Tools.Empty()){
      foreach(ItemButton b in buttons){
        b.TrySetTool(tool);
        b.SetUpSystem(this);
      }
      Debug.Log("Set Up Properly!");
    }
    else foreach(ItemButton b in buttons) b.SetUpSystem(this);
  }
}
