using UnityEngine;
using UnityEngine.SceneManagement;

//A debugging script for devs. If you want to be sent back to the Start scene when you have a scene open in the editor that isn't the start scene, put this scene
//TODO: Fix mouseLocking!

public class ThingThatSendsYouToTheStartSceneIfYouAreNotSupposedToBeHereYet : MonoBehaviour
{
  private protected void Start(){
    if(GameplayManager.main == null) SceneManager.LoadScene("StartMenu");
    Destroy(this);
  }
}
