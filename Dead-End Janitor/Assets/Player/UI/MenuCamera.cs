using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MenuCamera : MonoBehaviour
{
  //TODO: Make less jumpy!
  public static MenuCamera main;
  [SerializeField] private float howLong = 3f;
  float elapsedTime;
  Vector3 delta;
  Transform target;
  Vector3 targetPos;
  Vector3 StartPos;

  // In an attempt to fix the fact that the camera doesn't move to the correct spot sometimes. (works)
  [SerializeField] List<Transform> Menus = new List<Transform>();
  List<Vector3> StaticMenuPositions = new List<Vector3>();
  private void Start(){
    if(main == null) main = this;
    target = Menus[0]; //transform.parent;
    foreach(Transform menu in Menus){
      StaticMenuPositions.Add(menu.position);
    }    
    targetPos = StaticMenuPositions[0];
    elapsedTime = howLong;
  }
  public void Move(int MenuNumber){
    if(target != null)target.gameObject.SetActive(false);
    elapsedTime = 0f;
    target = Menus[MenuNumber];
    targetPos = StaticMenuPositions[MenuNumber];
    Vector3 otherPosition = targetPos;
    delta = (otherPosition - transform.position) / howLong;
    StartPos = transform.position;
    // print(targetPos.ToString());
    // print(delta);
  }
  void Update(){
    if(elapsedTime < howLong){
      elapsedTime += Time.deltaTime;
      float t = elapsedTime / howLong;

      transform.position = Vector3.Lerp(StartPos, targetPos, t);
    }
    else{
        target.gameObject.SetActive(true);
        transform.position = targetPos;
      }
  }
}
