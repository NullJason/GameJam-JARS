using UnityEngine;

public class Hunter : MonoBehaviour
{
  //Detection works as follows: the hunter sends out a series of raycasts around it, forming a flattened cone, or arc. If it detects an object in the huntable layer, it will stop searching and start chasing. MinDetectionSize is the smallest size that the hunter can detect (and therefore indirectly, how close the rays must be together and how many there must be).
  [SerializeField] private protected LayerMask visible;
  [SerializeField] private protected float minDetectionSize;
  [SerializeField] private protected float detectionDistance; //How far away an object must be to avoid detection.
  [SerializeField] private protected float detectionSpread; //How many degrees the detection cone should be.
  private Quaternion[] raycastRotations; //Stores the directions of all the raycasts, relative to the player's rotation.
  private int numberOfRays;
  private GameObject target;

  void Start(){
    visible = LayerMask.GetMask("Visible");
    SetUpDetectionRays();
  }
  void FixedUpdate(){
    DisplayRays();
    target = null;
    CheckRays(true);
    Debug.Log(target);
  }
  private void SetUpDetectionRays(){
    float arcOfDetection = 2 * Mathf.PI * detectionDistance * detectionSpread / 360; //The length of the arc of the detection cone.
    numberOfRays = (int)Mathf.Ceil(arcOfDetection / minDetectionSize);
    float spreadOfEachRay = detectionSpread / numberOfRays;
    raycastRotations = new Quaternion[numberOfRays];
    for(int i = 0; i < numberOfRays; i++){
      raycastRotations[i] = Quaternion.Euler(0, spreadOfEachRay * (i + 0.5f) - detectionSpread / 2, 0);
    }
  }
  private void DisplayRays(){
    foreach(Quaternion q in raycastRotations){
      Debug.DrawRay(transform.position, q * transform.forward * detectionDistance, Color.red, 0, false);
    }
    Debug.DrawRay(transform.position, transform.forward, Color.green, 0, false);
    Debug.DrawRay(transform.position, Quaternion.Euler(0, -detectionSpread / 2, 0) * transform.forward, Color.blue, 0, false);
    Debug.DrawRay(transform.position, Quaternion.Euler(0, detectionSpread / 2, 0) * transform.forward, Color.blue, 0, false);
  }

  //Checks the raycasts. Returns whether the target has been set properly, regardless of whether it was set before the calling or not.
  //TODO!
  private bool CheckRays(bool displayHits){
    float distance = detectionDistance;
    for(int i = 0; i < numberOfRays; i++){
      float rayDistance = CheckRay(i, distance, displayHits);
      if(rayDistance != -1) distance = rayDistance;
    }
    return target != null;
  }
  //Checks a particular ray. Returns the distance to the first hit if it is not a wall. If it detects a wall or doesn't detect anything, returns -1. Updates the target to the object that was hit if not returning -1.
  private float CheckRay(int whichRay, float closerThan, /*TODO!*/bool display = false){
    RaycastHit hit;
    if(Physics.Raycast(transform.position, raycastRotations[whichRay] * transform.forward, out hit, closerThan, visible)){
      Debug.DrawRay(transform.position, raycastRotations[whichRay] * transform.forward * hit.distance, Color.yellow, 0, false);
      if(hit.collider.gameObject.tag != "Wall"){
        target = hit.collider.gameObject;
        return hit.distance;
      }
    }
    Debug.DrawRay(transform.position, raycastRotations[whichRay] * transform.forward * closerThan, Color.yellow, 0, false);
    return -1;
  }
}
