using UnityEngine;

public class ItemSystem : ButtonSystem
{
  [SerializeField] Tool tool;
  private protected override void Awake(){
    foreach(ItemButton b in buttons) b.Init(tool);
  }

    // Update is called once per frame
    void Update()
    {

    }
}
