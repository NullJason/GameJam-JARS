using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadNewSaveFileButton : LoadSaveFileButton
{
  private protected override void Start(){
    base.Start();
    button.interactable = true;
  }
  private protected override void ButtonDo(){
    GameplayManager.main.ResetSave();
    base.ButtonDo();
  }
}
