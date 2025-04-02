using UnityEngine;
using System.Collections.Generic;

public class Tools : MonoBehaviour
{
  private static Tools main = null;
  [SerializeField] private Dictionary <Tool, GameObject> tools;
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
      SetUpTools();
    }
    else Destroy(this);
  }

  public static GameObject Get(Tool t){
    if(main == null) Debug.LogError("Tool values do not yet exist!");
    else if(main.tools == null) Debug.Log("Tool values have not yet been initialized!");
    else if(!main.tools.ContainsKey(t)) Debug.LogError("Tool does not exist!");
    return(main.tools[t]);
  }
  private void SetUpTools(){
    tools = new Dictionary<Tool, GameObject>();
    tools.Add(Tool.mop, mop);
    tools.Add(Tool.vacuum, vacuum);
    tools.Add(Tool.spray, spray);
    tools.Add(Tool.sponge, sponge);
    tools.Add(Tool.featherDuster, featherDuster);
    tools.Add(Tool.slippySoap, slippySoap);
  }
}
