using UnityEngine;
using UnityEngine.SceneManagement;

//A button that starts a new game with a preexisting save file.

public class LoadSaveFileButton : ButtonFunctionality
{
  [SerializeField] private protected string sceneName = "Demo Scene"; //The name of the scene that the match should start in.
  private protected override void Init(){
    base.Init();
    if(!GameplayManager.main.SaveExists()){
      button.interactable = false;
    }
  }
  private protected override void ButtonDo(){
    GameplayManager.main.SetUpNewRound(sceneName);
  }
}
