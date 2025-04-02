using UnityEngine;

public class SwapWeapons : MonoBehaviour
{
  [SerializeField] Transform liquidsTool;
  [SerializeField] Transform solidsTool;
  [SerializeField] Transform hand;
  [SerializeField] Transform LeftHand;
  [SerializeField] Transform playerCamera;
  [SerializeField] KeyCode EquipSecondaryKey = KeyCode.Alpha1;
  Transform current;
  Transform previous;
  [SerializeField] PlayerMovement player;
  // Start is called once before the first execution of Update after the MonoBehaviour is created
  void Start()
  {
    if(playerCamera == null) playerCamera = transform.Find("PlayerCamera");
    if(liquidsTool == null) liquidsTool = transform.Find("MopTool");
    if(solidsTool == null) solidsTool = transform.Find("Vacuum");
    if(hand == null) hand = playerCamera.Find("Hand"); //Note: With my current configuration of the player, this will not work.
    if(liquidsTool == null) Debug.LogWarning("Could not find mop (or any other liquid-tool) under player cam, and no default value was provided!");
    if(solidsTool == null) Debug.LogWarning("Could not find vacuum (or any other solid-tool) under player cam, and no default value was provided!");
    if(hand == null) Debug.LogWarning("Could not find hand, and no default value was provided!");
    current = liquidsTool;
    previous = solidsTool;

    if(player == null) player = gameObject.GetComponent<PlayerMovement>();
    if(player == null) Debug.LogWarning("Could not find a characterController!");
  }

  void Update(){
    if(Input.GetButtonDown("Fire2")) Next();
    if(Input.GetButtonDown("Fire3")) Sprint();
    if(Input.GetButtonDown("Fire1") && current.Equals(hand)) TakeUpPreviousTool();
    if(Input.GetKeyDown(EquipSecondaryKey)){
      LeftHand.gameObject.SetActive(!LeftHand.gameObject.activeSelf);
    }
  }

  //Switches the tool in your main hand.
  void Next(){
    if(current == liquidsTool){
      previous = current;
      Set(solidsTool);
    }
    else if(current == solidsTool){
      previous = current;
      Set(liquidsTool);
    }
    else if(current == hand){
      TakeUpPreviousTool();
      Next();
    }
  }

  //Disables the most recent tool or hand, and enables it as the new tool.
  void Set(Transform t){
    current.gameObject.SetActive(false);
    current = t;
    current.gameObject.SetActive(true);
  }

  //Toggles sprinting.
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

  //Picks up the last tool that you were holding.
  void TakeUpPreviousTool(){
    Set(previous);
    player.moveSpeed = 3;
  }

  //Changes the tool the player has equipped. This is distinct from the tool the player is holding.
  //Throws an exception if the tool is not valid.
  //TODO: Test!
  public void SetTool(Transform tool){
    if(!tool.GetComponent<CleanerItem>()) Debug.LogWarning("Transform " + tool + " had no CleanerItem component associated with it!");
    tool = Instantiate(tool, playerCamera);
    Dirty newToolDirtType = tool.GetComponent<CleanerItem>().GetDirtType();
    if(newToolDirtType == Dirty.liquid){
      Destroy(liquidsTool);
      liquidsTool = tool;
    }
    else if(newToolDirtType == Dirty.solid){
      Destroy(solidsTool);
      solidsTool = tool;
    }
    if(current.Equals(tool)) tool.gameObject.SetActive(true);
    else tool.gameObject.SetActive(false);
  }
}
