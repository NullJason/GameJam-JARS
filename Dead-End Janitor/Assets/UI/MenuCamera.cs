using UnityEngine;

public class MenuCamera : MonoBehaviour
{
  //TODO: Make less jumpy!
  public static MenuCamera main;
  [SerializeField] private int howLong = 300;
  int timer = 0;
  Vector3 delta;
  Transform target;
  private void Start(){
    if(main == null) main = this;
    target = transform.parent;
  }
  public void Move(Transform t){
    if(target != null)target.gameObject.SetActive(false);
    transform.SetParent(null);
    target = t;
    Vector3 otherPosition = t.position;
    delta = (otherPosition - transform.position) / howLong;
    timer = howLong;
  }
  void Update(){
    if(timer > 0){
      transform.Translate(delta);
      timer--;
      if(timer == 0){
        target.gameObject.SetActive(true);
        transform.SetParent(target);
      }
    }
  }
}
