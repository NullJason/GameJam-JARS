using UnityEngine;

public abstract class Humanoid : MonoBehaviour
{
    [SerializeField] private float MaxHp = 100;
    private float Hp = 100;
    Transform HumanoidTransform;
    Vector3 PreviousPosition;
    [SerializeField] private Rigidbody HumanoidBody;
    private bool Invulnerable = false;
    public float velocityThreshold = 100f; // Minimum speed to consider as "moving"

    private void Start() {
        SetHp(MaxHp);
        HumanoidTransform = transform;
        PreviousPosition = transform.position;
        if(HumanoidBody == null) HumanoidBody = transform.GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (transform.position != PreviousPosition)
        {
            OnMoving();
        }
        //TODO: Remove Distance calculations in favor of distance squared! This will save computation time, especially since this is being called every frame.
        if(Vector3.Distance(PreviousPosition,HumanoidTransform.position) < 0.1) OnMoved();
    }
    public float GetHp() {return Hp;}
    public float GetMaxHp() {return MaxHp;}
    public void SetHp(float num, bool damageSource = true){float previous = Hp; Hp=num; if(Hp>MaxHp) Hp = MaxHp; if(Hp<0) Hp=0; if(previous>Hp && damageSource) OnTakeDamage();}
    public void SetMaxHp(float num) {Hp = MaxHp = num;}
    public void AddHp(float num, bool damageSource = true) {SetHp(Hp+num, damageSource);}
    public void ToggleInvulnerability(){Invulnerable = !Invulnerable;}
    //TODO: Replace or remove the following four methods!
    //  Instead of directly checking whether an event is occuring, we should have it so that the logic of checking when one dies is here
  //  public bool IsDead(){if(Hp == 0 && !Invulnerable && !IsDeadFired) {IsDeadFired = true; return true;} return false;}
//    public bool TookDamage(){bool state = Damaged; Damaged = false; return state && !Invulnerable && Hp>0;}
//    public bool HasMoved(){ if(Vector3.Distance(PreviousPosition,HumanoidTransform.position) < 0.1) return false; PreviousPosition = HumanoidTransform.position; return true; }
//    public bool IsMoving() {
        //if(HumanoidBody == null){Debug.Log("No RB!"); return false;} return HumanoidBody.linearVelocity.sqrMagnitude > velocityThreshold * velocityThreshold;
//        return isMoving;
//    }
    private protected virtual void OnMoved(){}
    private protected virtual void OnMoving(){}
    private protected virtual void OnDeath(){}
    //If overrided, this method must call its super. Otherwise, OnDeath() may not be called correctly!
    private protected virtual void OnTakeDamage(){
        if(Hp < 0) OnDeath();
    }
}
