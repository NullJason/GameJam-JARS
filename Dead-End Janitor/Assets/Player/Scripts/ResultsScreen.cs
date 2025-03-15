using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResultsScreen : MonoBehaviour
{
  [SerializeField] TMP_Text display;
  [SerializeField] DisplayInfo infoToDisplay;
  [SerializeField] string pretext;
  [SerializeField] string posttext;
  private string currentValue;
  // Start is called once before the first execution of Update after the MonoBehaviour is created
  void Start()
  {

  }

  // Update is called once per frame
  void Update()
  {
    if(currentValue != GetNewValue()){
      display.text = pretext + currentValue + posttext;
    }
  }
  private protected string GetNewValue(){
    if(infoToDisplay == DisplayInfo.health) currentValue = "" + GameplayManager.main.GetPlayerHealth();
    else if(infoToDisplay == DisplayInfo.zombies) currentValue = "" + GameplayManager.main.GetZombiesLeftInWave();
    return currentValue;
  }
  enum DisplayInfo{
    health,
    zombies,
    visibleCleanables,
    totalCleanables,
    wave,
    score,
    wallet
  }
}
