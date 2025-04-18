using UnityEngine;

public class GiveFreeMoneyButton : ButtonFunctionality
{
  [SerializeField] int howMuch = 500;
  private protected override void ButtonDo()
  {
    GameplayManager.main.AddPoints(howMuch);
  }
}
