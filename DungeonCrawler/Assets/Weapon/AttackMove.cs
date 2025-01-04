using UnityEngine;

public abstract class AttackMove : MonoBehaviour
{
  [SerializeField] private GameObject secondary; //The secondary projectile that should be summoned when SecondaryProjectile() is called.
  [SerializeField] private int despawnTimer;

  // Start is called once before the first execution of Update after the MonoBehaviour is created
  void Start()
  {
    SetUp();
  }
  void FixedUpdate(){
    RunDespawn();
    Move();
  }

  abstract private protected void SetUp(); //Called at Start().
  abstract private protected void Move(); //Called on FixedUpdate(). Controls the main behaviour of the projectile.

  private void RunDespawn(){
    despawnTimer--;
    if(despawnTimer <= 0) Destroy(gameObject);
  }
  //Turn in a random direction, within range. So, if range is 30, it can turn anywhere between -30 and 30 degrees from its current rotation.
  private protected void Wobble(float range){
    Rotate(Random.Range(-range, range));
  }
  private protected void Rotate(float rotation){
    transform.Rotate(0, 0, rotation);
  }
  public void SetDirection(Quaternion rotation){
    transform.rotation = rotation;
  }
  private protected void GoForward(float speed){
    transform.Translate(new Vector2(speed, 0));
  }
  //Summons a second projectile.
  private protected void SecondaryProjectile(){
    Instantiate(secondary);
  }
  private protected void SecondaryProjectile(GameObject secondary){
    Instantiate(secondary);
  }
}
