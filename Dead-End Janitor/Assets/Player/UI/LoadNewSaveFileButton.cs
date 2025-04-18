using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadNewSaveFileButton : LoadSaveFileButton
{
  private protected override void Init(){
    base.Init();
    button.interactable = true;
  }
  private protected override void ButtonDo(){
    GameplayManager.main.ResetSave();
    base.ButtonDo();
  }
}
