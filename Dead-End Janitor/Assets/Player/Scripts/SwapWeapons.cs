using UnityEngine;

public class SwapWeapons : MonoBehaviour
{
  [SerializeField] Transform mop;
  [SerializeField] Transform vacuum;
  [SerializeField] Transform hand;
  Transform current;
  Transform previous;
  [SerializeField] PlayerMovement player;
  // Start is called once before the first execution of Update after the MonoBehaviour is created
  void Start()
  {
    if(mop == null) mop = transform.Find("MopTool");
    if(vacuum == null) vacuum = transform.Find("Vacuum");
    if(hand == null) hand = transform.Find("Empty"); //Note: With my current configuration of the player, this will not work.
    if(mop == null) Debug.LogWarning("Could not find mop, and no default value was provided!");
    if(vacuum == null) Debug.LogWarning("Could not find vacuum, and no default value was provided!");
    if(hand == null) Debug.LogWarning("Could not find hand, and no default value was provided!");
    current = mop;
    previous = vacuum;

    if(player == null) player = gameObject.GetComponent<PlayerMovement>();
    if(player == null) Debug.LogWarning("Could not find a characterController!");
  }

  void Update(){
    if(Input.GetButtonDown("Fire2")) Next();
    if(Input.GetButtonDown("Fire3")) Sprint();
    if(Input.GetButtonDown("Fire1") && current.Equals(hand)) TakeUpPreviousTool();
  }

  void Next(){
    if(current == mop){
      previous = current;
      Set(vacuum);
    }
    else if(current == vacuum){
      previous = current;
      Set(mop);
    }
    else if(current == hand){
      TakeUpPreviousTool();
      Next();
    }
  }

  void Set(Transform t){
    current.gameObject.SetActive(false);
    current = t;
    current.gameObject.SetActive(true);
  }

  void Sprint(){
    if(current == hand){
      TakeUpPreviousTool();
    }
    else{
      previous = current;
      Set(hand);
      player.moveSpeed = 5;
    }
  }

  void TakeUpPreviousTool(){
    Set(previous);
    player.moveSpeed = 3;
  }
}
