using UnityEngine;

public class ButtonSystem : MonoBehaviour
{
  [SerializeField] SystemButton[] buttons;

  //Signals that this ButtonSystem should re-initialize buttons[i].
  //If i == -1, will re-initialize all buttons.
  public void Refresh(int i = -1){
    if(i < 0){
      foreach(SystemButton b in buttons) b.Init();
    }
    else if(i < buttons.Length) buttons[i].Init();
    else Debug.LogError(i + " was out of range in " + buttons + "! ");
  }
}
