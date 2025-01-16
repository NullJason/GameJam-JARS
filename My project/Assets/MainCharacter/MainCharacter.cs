using UnityEngine;

public class Hunter : MonoBehaviour
{
  //Detection works as follows: the hunter sends out a series of raycasts around it, forming a flattened cone, or arc. If it detects an object in the huntable layer, it will stop searching and start chasing. MinDetectionSize is the smallest size that the hunter can detect (and therefore indirectly, how close the rays must be together and how many there must be).
  [SerializeField] private protected LayerMask huntable;
  [SerializeField] private protected double minDetectionSize;
  [SerializeField] private protected double detectionDistance; //How far away an object must be to avoid detection.
  [SerializeField] private protected double detectionSpread; //How many degrees the detection cone should be.
  [SerializeField] private Quaternion[] raycastRotations; //Stores the directions of all the raycasts, relative to the player's rotation.
  [SerializeField] private int numberOfRays;
  void Start(){
    SetUpDetectionRays();
  }
  void Update(){
    SetUpDetectionRays();
    foreach(Quaternion q in raycastRotations){
      Debug.DrawRay(transform.position, q * transform.forward * detectionDistance, Color.red, 0, false);
    }
    Debug.DrawRay(transform.position, transform.forward, Color.green, 0, false);
    Debug.DrawRay(transform.position, Quaternion.Euler(0, detectionSpread, 0) * transform.forward, Color.blue, 0, false);
  }
  private void SetUpDetectionRays(){
    double arcOfDetection = 2 * Mathf.PI * detectionDistance * detectionSpread / 360; //The length of the arc of the detection cone.
    numberOfRays = (int)Mathf.Ceil(arcOfDetection / minDetectionSize);
    double spreadOfEachRay = detectionSpread / numberOfRays - 1;
    raycastRotations = new Quaternion[numberOfRays];
    Debug.Log("Arc of detection is " + arcOfDetection);
    for(int i = 0; i < numberOfRays; i++){
      raycastRotations[i] = Quaternion.Euler(0, spreadOfEachRay * (0.5 + i), 0);
    }
  }
}
