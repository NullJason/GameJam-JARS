using UnityEngine;
using UnityEngine.AI;
public class Hunter : MonoBehaviour
{
  //Detection works as follows: the hunter sends out a series of raycasts around it, forming a flattened cone, or arc. If it detects an object in the huntable layer, it will stop searching and start chasing. MinDetectionSize is the smallest size that the hunter can detect (and therefore indirectly, how close the rays must be together and how many there must be).
  [SerializeField] private protected LayerMask visible;
  [SerializeField] private protected float minDetectionSize;
  [SerializeField] private protected float detectionDistance; //How far away an object must be to avoid detection.
  [SerializeField] private protected float detectionSpread; //How many degrees the detection cone should be.
  [SerializeField] private protected GameObject player; //TODO: Find better way to get this!
  [SerializeField] private protected float attackDistance = 0.65f; //How close the target must be to start dealing damage.
  [SerializeField] private protected Vector3 SightOriginOffset = new Vector3(0,0,0);
  private Quaternion[] raycastRotations; //Stores the directions of all the raycasts, relative to the player's rotation.
  private int numberOfRays;
  private GameObject target;
  private NavMeshAgent navigate;
  private PriorityTier priority = 0;
  private int waypointTimer = -1;
  [SerializeField] private int damageTimerReset;
  private int damageTimer = 0;
  [SerializeField] private Transform damageCenter; //Where the chainsaw is, approximately. Mostly, it's so the hunter has to turn around to attack a zombie behind.
  [SerializeField] private Transform[] patrolPoints;

  void Start(){
    if(GameplayManager.hunter == null) GameplayManager.hunter = this.gameObject;
    SetUpDetectionRays();
    navigate = GetComponent<UnityEngine.AI.NavMeshAgent>();
  }
  void FixedUpdate(){
    DisplayRays();
    if(target == null) priority = 0;
    if(priority < PriorityTier.ContactZombie){
//      CheckContact(); //Instead of checking for zombies in contact here, it is now checked in OnTriggerStay() .
      if(priority < PriorityTier.Player){
        CheckRayPlayer();
        if(priority < PriorityTier.SeenZombie){
          CheckRays(true);
          if(priority < PriorityTier.Waypoint){
//            FindNextWaypoint();
          }
        }
      }
    }
    PursueTarget();
  }
  private void SetUpDetectionRays(){
    visible = LayerMask.GetMask("Visible");//TODO: Remove magic numbers!
    float arcOfDetection = 2 * Mathf.PI * detectionDistance * detectionSpread / 360; //The length of the arc of the detection cone.
    numberOfRays = (int)Mathf.Ceil(arcOfDetection / minDetectionSize);
    float spreadOfEachRay = detectionSpread / numberOfRays;
    raycastRotations = new Quaternion[numberOfRays];
    for(int i = 0; i < numberOfRays; i++){
      raycastRotations[i] = Quaternion.Euler(0, spreadOfEachRay * (i + 0.5f) - detectionSpread / 2, 0);
    }
  }
  //For debugging purposes. Shows field of view rays.
  private void DisplayRays(){
    foreach(Quaternion q in raycastRotations){
      Debug.DrawRay(transform.position, q * transform.forward * detectionDistance, Color.red, 0, false);
    }
    Debug.DrawRay(transform.position, transform.forward, Color.green, 0, false);
    Debug.DrawRay(transform.position, Quaternion.Euler(0, -detectionSpread / 2, 0) * transform.forward, Color.blue, 0, false);
    Debug.DrawRay(transform.position, Quaternion.Euler(0, detectionSpread / 2, 0) * transform.forward, Color.blue, 0, false);
  }

  //Checks the raycasts. Returns whether the target has been set properly, regardless of whether it was set before the calling or not.
  private bool CheckRays(bool displayHits){
    float distance = detectionDistance;
    for(int i = 0; i < numberOfRays; i++){
      float rayDistance = CheckRay(i, distance, displayHits);
      if(rayDistance != -1) distance = rayDistance;
    }
    return target != null;
  }
  //Checks a particular ray. Returns the distance to the first hit if it is not a wall. If it detects a wall or doesn't detect anything, returns -1. Updates the target to the object that was hit if not returning -1.
  private float CheckRay(int whichRay, float closerThan, bool display = false){
    RaycastHit hit;
    if(Physics.Raycast(transform.position + SightOriginOffset, raycastRotations[whichRay] * transform.forward, out hit, closerThan, visible)){
      if(display) Debug.DrawRay(transform.position + SightOriginOffset, raycastRotations[whichRay] * transform.forward * hit.distance, Color.yellow, 0, false);
      if(hit.collider.gameObject.tag != "Wall"){
        AssignTarget(hit.collider.gameObject, PriorityTier.SeenZombie);
        return hit.distance;
      }
    }
    if(display) Debug.DrawRay(transform.position + SightOriginOffset, raycastRotations[whichRay] * transform.forward * closerThan, Color.yellow, 0, false);
    return -1;
  }
  //Returns true if close enough to attack the target.
  private bool PursueTarget(){
    if(target != null){
      if((damageCenter.transform.position - target.transform.position).sqrMagnitude < attackDistance * attackDistance){
        navigate.destination = transform.position;
        if(priority == PriorityTier.Waypoint){
          if(waypointTimer > 0) waypointTimer--;
          else if(waypointTimer == -1) waypointTimer = target.GetComponent<Waypoint>().GetTime();
          else if(waypointTimer == 0){
            AssignTarget(target.GetComponent<Waypoint>().GetNext(), PriorityTier.Waypoint);
            waypointTimer = -1;
          }
        }
        else TryDealDamage(target, 1);
        return true;
      }
      navigate.destination = target.transform.position;
    }
    else AssignTarget(Waypoint.GetNew(), PriorityTier.Waypoint);
    return false;
  }
  private void AssignTarget(GameObject target, PriorityTier priority){
    this.target = target;
    SetSpeed(priority);
    this.priority = priority;
  }
  private void SetSpeed(PriorityTier priority){
    if(priority == PriorityTier.ContactZombie) navigate.speed = 2.75f;
    else if(priority == PriorityTier.Player) navigate.speed = 2.5f;
    else if(priority == PriorityTier.SeenZombie) navigate.speed = 2f;
    else if(priority == PriorityTier.Waypoint) navigate.speed = 0.5f;
  }
  //Checks whether a raycast from the Hunter to the Player would be within the cone, and then checks whether such a raycast would actually hit the player.
  private bool CheckRayPlayer(){
    Vector2 directionToPlayer2D = new Vector2(player.transform.position.x - transform.position.x, player.transform.position.z - transform.position.z);
    Vector3 directionToPlayer = new Vector3(directionToPlayer2D.x, 0, directionToPlayer2D.y).normalized;
    Vector2 directionFacing2D = new Vector2(transform.forward.x, transform.forward.z);
    if(!(Vector2.Angle(directionToPlayer2D, directionFacing2D) < detectionSpread / 2) || (directionToPlayer2D.sqrMagnitude > detectionDistance * detectionDistance)) return false;
    RaycastHit hit;
    Physics.Raycast(transform.position, directionToPlayer, out hit, detectionDistance, visible);
    if(hit.collider == null) return false;
    if(hit.collider.gameObject.Equals(player)){
      AssignTarget(player, PriorityTier.Player);
      return true;
    }
    return false;
  }

  private protected void OnTriggerStay(Collider col){
    Debug.Log("=D");
    if(priority < PriorityTier.ContactZombie && col.gameObject.layer == LayerMask.NameToLayer("Visible") && col.GetComponent<Collider>().gameObject.tag != "Wall" && !col.gameObject.Equals(player)) {AssignTarget(col.GetComponent<Collider>().gameObject, PriorityTier.ContactZombie); Debug.Log("=D");}
  }

  private protected bool TryDealDamage(GameObject target, int damage = 1){
    if(damageTimer == 0){
      damageTimer = damageTimerReset;
      Humanoid h = target.GetComponent<Humanoid>();
      if(h == null)Debug.LogWarning("target \'" + target.name + "\' does not have a Humanoid component, and so cannot be damaged!");
      target.GetComponent<Humanoid>().AddHp(-damage);
      return true;
    }
    damageTimer-=1;
    return false;
  }

  public Player GetPlayer(){
    return player.GetComponent<Player>();
  }

  enum PriorityTier{
    ContactZombie = 4,
    Player = 3,
    SeenZombie = 2,
    Waypoint = 1
  }
}
