using UnityEngine;
using UnityEngine.UI;

//A button that disables one or more gameObjects and opens exactly one other. Good for a button that closes one section of the menu and opens another.

public class BackButton : ButtonFunctionality
{
  [SerializeField] GameObject[] close;
  [SerializeField] GameObject open;
  private protected override void Init(){
    base.Init();
    if(close == null) {
      close = new GameObject[1];
      close[0] = gameObject;
    }
    if(close == null) Debug.Log("No object specified!");
  }
  private protected override void ButtonDo(){
    foreach(GameObject g in close) g.SetActive(false);
    open.SetActive(true);
  }
}
