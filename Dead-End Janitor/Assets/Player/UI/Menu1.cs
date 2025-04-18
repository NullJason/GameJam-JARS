using UnityEngine;
using UnityEngine.UI;

//A button that physically moves the camera when pressed.

public class MoveButton : ButtonFunctionality
{
  [SerializeField] private int MenuToMoveTo;
  private protected override void Init()
  {
    base.Init();
  }

  private protected override void ButtonDo(){
    MenuCamera.main.Move(MenuToMoveTo);
  }
}
