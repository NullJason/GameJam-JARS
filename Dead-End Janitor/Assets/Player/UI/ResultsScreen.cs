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
  private string textToDisplay = null; //If this is not null, this text will be displayed instead of getting the value. 

  private string currentValue;
  // Start is called once before the first execution of Update after the MonoBehaviour is created
  void Start()
  {
    if(display == null) display = GetComponent<TMP_Text>();
  }

  // Update is called once per frame
  void Update()
  {
    if(textToDisplay != null) display.text = textToDisplay;
    else if(currentValue != GetNewValue()){
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
    else if(infoToDisplay == DisplayInfo.solidTool) currentValue = "" + GameplayManager.main.GetSolidTool();
    else if(infoToDisplay == DisplayInfo.liquidTool) currentValue = "" + GameplayManager.main.GetLiquidTool();
    return currentValue;
  }

  //Tells the display panel that it should display a particular string, rather than the intended values. 
  //keepFormatters determines whether or not the pretext and posttext will be kept. 
  public void Hijack(string s, bool keepFormatters = true){
    if(keepFormatters) s = pretext + s + posttext;
    textToDisplay = s;
  }


  //Tells the display panel that it should display the intended values, rather than the hijacked string. 
  //TODO: Make return bool?
  public void Unhijack(){
    textToDisplay = null;
  }

  enum DisplayInfo{
    health,
    zombies, //Number of zombies left in wave.
    visibleCleanables,
    totalCleanables,
    wave,
    score,
    wallet, 
    solidTool, 
    liquidTool
  }
}
