using UnityEngine;
using UnityEngine.UI;

//A button that physically moves the camera when pressed.

public class MoveButton : ButtonFunctionality
{
  [SerializeField] private Transform other;
  private protected override void Start()
  {
    base.Start();
  }

  private protected override void ButtonDo(){
    MenuCamera.main.Move(other);
  }
}
