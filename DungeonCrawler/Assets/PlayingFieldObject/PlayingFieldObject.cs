using UnityEngine;

public class PlayingFieldObject : MonoBehaviour
{
  private protected static bool unpaused = true;
  protected virtual void Update()
  {
    if(unpaused) GameUpdate();
  }
  protected virtual void FixedUpdate(){
    if(unpaused) GameFixedUpdate();
  }

  private protected virtual void GameUpdate(){}
  private protected virtual void GameFixedUpdate(){}
  //Returns true if was unpaused and is now paused.
  public static bool Pause(){
    bool result = unpaused;
    unpaused = false;
    return result;
  }
  //Returns true if was paused and is now unpaused.
  public static bool Unpause(){
    bool result = !unpaused;
    unpaused = true;
    return result;
  }
  public static bool Swap(){
    unpaused = !unpaused;
    return unpaused;
  }
}
