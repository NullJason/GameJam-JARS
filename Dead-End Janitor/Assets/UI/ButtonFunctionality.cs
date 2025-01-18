using UnityEngine;
using UnityEngine.UI;

public abstract class ButtonFunctionality : MonoBehaviour
{
  [SerializeField] private Button button;
  private protected virtual void Start()
  {
    if(button == null) button = GetComponent<Button>();
    if(button == null) Debug.LogWarning("There wasn't a button attached to " + gameObject.name + " and no button was provided!");
    button.onClick.AddListener(ButtonDo);
  }

  private protected abstract void ButtonDo();
}
