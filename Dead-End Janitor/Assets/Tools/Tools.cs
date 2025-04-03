using UnityEngine;
using System.Collections.Generic;

public class Tools : MonoBehaviour
{
  private static Tools main = null;
  private Dictionary <Tool, GameObject> tools;
  private Dictionary <Tool, Dirty> dirtTypes;
  [SerializeField] GameObject mop;
  [SerializeField] GameObject vacuum;
  [SerializeField] GameObject spray;
  [SerializeField] GameObject sponge;
  [SerializeField] GameObject featherDuster;
  [SerializeField] GameObject slippySoap;

  void Awake(){
    Debug.Log("main.tools");
    if(main == null){
      main = this;
      SetUp();
    }
    else Destroy(this);
  }

  public static GameObject Get(Tool t){
    if(main == null) Debug.LogError("Tool values do not yet exist!");
    else if(main.tools == null) Debug.Log("Tool values have not yet been initialized!");
    else if(!main.tools.ContainsKey(t)) Debug.LogError("Tool does not exist!");
    return(main.tools[t]);
  }
  public static Dirty GetDirtType(Tool t){
    if(main == null) Debug.LogError("Tool values do not yet exist!");
    else if(main.dirtTypes == null) Debug.Log("Tool Dirt Type values have not yet been initialized!");
    else if(!main.dirtTypes.ContainsKey(t)) Debug.LogError("Tool does not exist!");
    return(main.dirtTypes[t]);
  }
  private void SetUp(){
    tools = new Dictionary<Tool, GameObject>();
    dirtTypes = new Dictionary<Tool, Dirty>();
    CheckAndAdd(Tool.mop, mop);
    CheckAndAdd(Tool.vacuum, vacuum);
    CheckAndAdd(Tool.spray, spray);
    CheckAndAdd(Tool.sponge, sponge);
    CheckAndAdd(Tool.featherDuster, featherDuster);
    CheckAndAdd(Tool.slippySoap, slippySoap);
  }
  private void CheckAndAdd(Tool t, GameObject toolObject){
    CleanerItem cleaner = toolObject.GetComponent<CleanerItem>();
    if(cleaner == null) Debug.LogError("Tool " + toolObject + " (" + t + ")" + " did not have a cleaner item attached to it, did not add!");
    else{
      tools.Add(t, toolObject);
      dirtTypes.Add(t, cleaner.GetDirtType());
    }
  }
}
