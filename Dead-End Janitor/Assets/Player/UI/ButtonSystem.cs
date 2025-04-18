using UnityEngine;

public class ButtonSystem : MonoBehaviour
{
  [SerializeField] private protected SystemButton[] buttons;

  private protected virtual void Awake(){
    foreach(SystemButton b in buttons) b.SetUpSystem(this);
  }

  //Signals that this ButtonSystem should re-initialize buttons[i].
  //If i == -1, will re-initialize all buttons.
  public void Refresh(int i = -1){
    Debug.Log("Refreshing!");
    if(i < 0){
      foreach(SystemButton b in buttons) Reset(b);
    }
    else if(i < buttons.Length) Reset(buttons[i]);
    else Debug.LogError(i + " was out of range in " + buttons + "! ");
    foreach(SystemButton b in buttons) Debug.Log("Buttons!!");
  }

  private protected virtual void Reset(SystemButton button){
    button.Reset();
  }
}
