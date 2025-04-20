using UnityEngine;

//A buy button that doesn't work unless certain prerequisite tools have been acquired. 

public class ProgressiveUnlockBuyButton : BuyButton
{
  [SerializeField] Tool[] prereqs;
  [SerializeField] int howMany = -1; //How many tools from prereqs are required for the button to activate. A negative number means all tools are required.  

  private protected override void CheckTurnOn()
  {

    if(GameplayManager.main.CheckToolUnlocked(tool) || !AcquiredEnough()) TurnOffButton();
    else TurnOnButton();
  }

  private protected int HowManyAcquired(){
    int count = 0;
    foreach(Tool t in prereqs){
      if(GameplayManager.main.CheckToolUnlocked(t)) count++;
    }
    return count;
  }

  public bool AcquiredEnough(){
    int limit;
    if(howMany < 0) limit = prereqs.Length;
    else limit = howMany;
    return HowManyAcquired() >= limit;
  }
}
