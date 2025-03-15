using UnityEngine;
using UnityEngine.UI;

//A button that disables one gameObject and opens another. Good for a button that closes one section of the menu and opens another.

public class BackButton : ButtonFunctionality
{
  [SerializeField] GameObject close;
  [SerializeField] GameObject open;
  private protected override void Start(){
    base.Start();
    if(close == null) close = gameObject;
    if(close == null) Debug.Log("No object specified!");
  }
  private protected override void ButtonDo(){
    close.SetActive(false);
    open.SetActive(true);
  }
}
