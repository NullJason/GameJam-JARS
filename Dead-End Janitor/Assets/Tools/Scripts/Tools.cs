using UnityEngine;
using System.Collections.Generic;

public class Tools : MonoBehaviour
{
  private static Tools main = null;
  private Dictionary <Tool, GameObject> tools;
  private Dictionary <Tool, Dirty> dirtTypes;
  [SerializeField] GameObject mop;
  [SerializeField] GameObject mopII;
  [SerializeField] GameObject mopIII;
  [SerializeField] GameObject vacuum;
  [SerializeField] GameObject vacuumII;
  [SerializeField] GameObject vacuumIII;
  [SerializeField] GameObject spray;
  [SerializeField] GameObject sponge;
  [SerializeField] GameObject featherDuster;
  [SerializeField] GameObject slippySoap;
  [SerializeField] GameObject soapGun;
  [SerializeField] GameObject verySlippySoap;
  [SerializeField] GameObject soapBomb;
  [SerializeField] GameObject pressureWasher;
  [SerializeField] GameObject incinerator;
  [SerializeField] GameObject vaporizer;
  [SerializeField] GameObject purifierOrb;
  [SerializeField] GameObject decayOrb;
  [SerializeField] GameObject spinMop;
  [SerializeField] GameObject electricfier;
  [SerializeField] GameObject broom;
  [SerializeField] GameObject bubbleSigil;
  [SerializeField] GameObject soapBooster;
  [SerializeField] GameObject lintRoller;
  [SerializeField] GameObject especiallyLargeBubbleNuke;


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
    Dirty result;
    if(!main.dirtTypes.TryGetValue(t, out result)){
      Debug.LogWarning("Unknown trouble getting tool! Returning none. ");
      return(Dirty.none);
    }
    return(result);
  }
  private void SetUp(){
    tools = new Dictionary<Tool, GameObject>();
    dirtTypes = new Dictionary<Tool, Dirty>();
    // if variables are serialized here and name matches the name in enum, could do the following:
    // Reflect variables.
    //     foreach (Tool tool in Enum.GetValues(typeof(Tool)))
    // {
    //     string fieldName = tool.ToString();
    //     FieldInfo field = GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
    //     if (field != null && field.GetValue(this) is GameObject go && go != null)
    //     {
    //         CheckAndAdd(tool, go);
    //     }
    // }
    // or ig use scriptable objects
    CheckAndAdd(Tool.mop, mop);
    CheckAndAdd(Tool.mopII, mopII);
    CheckAndAdd(Tool.mopIII, mopIII);
    CheckAndAdd(Tool.vacuum, vacuum);
    CheckAndAdd(Tool.vacuumII, vacuumII);
    CheckAndAdd(Tool.vacuumIII, vacuumIII);
    CheckAndAdd(Tool.spray, spray);
    CheckAndAdd(Tool.sponge, sponge);
    CheckAndAdd(Tool.featherDuster, featherDuster);
    CheckAndAdd(Tool.slippySoap, slippySoap);
    CheckAndAdd(Tool.soapGun, soapGun);
    CheckAndAdd(Tool.verySlippySoap, verySlippySoap);
    CheckAndAdd(Tool.soapBomb, soapBomb);
    CheckAndAdd(Tool.pressureWasher, pressureWasher);
    CheckAndAdd(Tool.incinerator, incinerator);
    CheckAndAdd(Tool.vaporizer, vaporizer);
    CheckAndAdd(Tool.purifierOrb, purifierOrb);
    CheckAndAdd(Tool.decayOrb, decayOrb);
    CheckAndAdd(Tool.spinMop, spinMop);
    CheckAndAdd(Tool.electricfier, electricfier);
    CheckAndAdd(Tool.broom, broom);
    CheckAndAdd(Tool.bubbleSigil, bubbleSigil);
    CheckAndAdd(Tool.soapBooster, soapBooster);
    CheckAndAdd(Tool.lintRoller, lintRoller);
    CheckAndAdd(Tool.especiallyLargeBubbleNuke, especiallyLargeBubbleNuke);
  }
  private void CheckAndAdd(Tool t, GameObject toolObject){
    CleanerItem cleaner = toolObject.GetComponent<CleanerItem>();
    if(cleaner == null) {
      PlayerTool pt = toolObject.GetComponent<PlayerTool>();
      if(pt == null) Debug.LogError("Tool " + toolObject + " (" + t + ")" + " did not have a cleaner item attached to it, did not add!");
      else{
        Debug.LogError("Note that this won't add all the dirtTypes a playertool may have.");
        tools.Add(t, toolObject);
        dirtTypes.Add(t, pt.GetPrimaryCleanType());
      }
    }
    else{
      tools.Add(t, toolObject);
      if(t == Tool.featherDuster) Debug.LogWarning("Duster: " + cleaner.GetDirtType());
      dirtTypes.Add(t, cleaner.GetDirtType());
    }
  }
  public static Tool Empty(){
    return Tool.error;
  }
}
