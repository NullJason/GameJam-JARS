using UnityEngine;
using UnityEngine.SceneManagement;

//A button that starts a new game with a new save file.

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
    GameplayManager.main.SetUpWave();
  }
}
