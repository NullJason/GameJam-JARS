using UnityEngine;
using UnityEngine.UI;
using TMPro;

//This class represents a display panel, used for displaying a particular player- or game-statistic. Currently supported statistics are listed in the DisplayInfo enum.
public class ResultsPanel : MonoBehaviour
{
  [SerializeField] TMP_Text display;
  [SerializeField] DisplayInfo infoToDisplay;
  [SerializeField] string pretext;
  [SerializeField] string posttext;
  private string currentValue;
  // Start is called once before the first execution of Update after the MonoBehaviour is created
  void Start()
  {
    if(display == null) display = GetComponent<TMP_Text>();
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
    else if(infoToDisplay == DisplayInfo.visibleCleanables) currentValue = "" + GameplayManager.main.GetVisibleCleanables();
    else if(infoToDisplay == DisplayInfo.totalCleanables) currentValue = "" + GameplayManager.main.GetCleanables();
    else if(infoToDisplay == DisplayInfo.wave) currentValue = "" + GameplayManager.main.GetWave();
    else if(infoToDisplay == DisplayInfo.score) currentValue = "" + GameplayManager.main.GetMatchPoints();
    else if(infoToDisplay == DisplayInfo.wallet) currentValue = "" + GameplayManager.main.GetPoints();
    return currentValue;
  }
  enum DisplayInfo{
    health,
    zombies, //Number of zombies left in wave.
    visibleCleanables,
    totalCleanables,
    wave,
    score,
    wallet
  }
}
