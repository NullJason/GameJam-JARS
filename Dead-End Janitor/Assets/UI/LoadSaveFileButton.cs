using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSaveFileButton : ButtonFunctionality
{
  private protected override void Start(){
    base.Start();
    if(!GameplayManager.main.SaveExists()){
      button.interactable = false;
    }
  }
  private protected override void ButtonDo(){
    GameplayManager.main.LoadGame();
    SceneManager.LoadScene("Demo Scene");
    GameplayManager.main.SetGameActive(true);
    GameplayManager.main.SetUpWave();
  }
}
