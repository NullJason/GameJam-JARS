using UnityEngine;
using System.Collections.Generic;

public class Spawner : MonoBehaviour
{
  [SerializeField] int cooldown = 30;
  private int timer;
  Queue<GameObject> toBeSummoned;
  void Start(){
    toBeSummoned = new Queue<GameObject>();
    timer = cooldown;
    GameplayManager.main.AddSpawner(this);
  }
  public void Spawn(GameObject g, int howMany){
    for(int i = 0; i < howMany; i++) toBeSummoned.Enqueue(g);
  }
  public void Update(){
    if(toBeSummoned.Count > 0){
      if(timer == 0){
        Instantiate(toBeSummoned.Dequeue(), transform.position, transform.rotation);
        timer = cooldown;
      }
      timer--;
    }
  }
}
