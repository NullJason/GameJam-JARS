using UnityEngine;
using UnityEngine.UI;

public abstract class ButtonFunctionality : MonoBehaviour
{
  [SerializeField] private protected Button button;

  void Start(){
    Init();
  }

  //How this button should be initialized.
  private protected virtual void Init()
  {
    TryFindButton();
    button.onClick.AddListener(ButtonDo);
  }

  //Tries to find and initialize value for button. Does not set up functionality.
  private protected void TryFindButton(){
    if(button == null) button = GetComponent<Button>();
    if(button == null) Debug.LogWarning("There wasn't a button attached to " + gameObject.name + " and no button was provided!");
  }

  private protected abstract void ButtonDo();
}
