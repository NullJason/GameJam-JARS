using UnityEngine;
using System.Collections.Generic;

public class Waypoint : MonoBehaviour
{
  [SerializeField] private protected List<Waypoint> drawPool;
  [SerializeField] private protected bool isDefault;
  [SerializeField] private protected int wait;
  private static List<Waypoint> defaultDrawPool;
  private protected void Start(){
    if(isDefault){
      if(defaultDrawPool == null) defaultDrawPool = new List<Waypoint>();
      defaultDrawPool.Add(this);
    }
  }
  public GameObject GetNext(){
    if(drawPool.Count == 0) return GetNew();
    return drawPool[Random.Range(0, drawPool.Count)].gameObject;
  }
  //Returns a random waypoint that was marked as isDefault.
  public static GameObject GetNew(){
    return defaultDrawPool[Random.Range(0, defaultDrawPool.Count)].gameObject;
  }
  public int GetTime(){
    return wait;
  }
}
