using UnityEngine;

public class ItemSystem : ButtonSystem
{
  [SerializeField] Tool tool;
  private protected override void Awake(){
    foreach(ItemButton b in buttons) b.SetUpSystem(tool, this);
  }
}
