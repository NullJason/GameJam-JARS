using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSaveFileButton : ButtonFunctionality
{
  [SerializeField] private protected string sceneName = "Demo Scene";
  private protected override void Start(){
    base.Start();
    if(!GameplayManager.main.SaveExists()){
      button.interactable = false;
    }
  }
  private protected override void ButtonDo(){
    GameplayManager.main.LoadGame();
    SceneManager.LoadScene(sceneName);
    GameplayManager.main.SetGameActive(true);
    GameplayManager.main.SetUpWave();
  }
}
