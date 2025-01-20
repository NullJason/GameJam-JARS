using UnityEngine;

public class Humanoid : MonoBehaviour
{
    [SerializeField] private float MaxHp = 100;
    private float Hp = 100;
    Transform HumanoidTransform;
    Vector3 PreviousPosition;
    private bool Invulnerable = false;
    private bool Damaged = false;
    private bool IsDeadFired = false;
    private void Start() {
        SetHp(MaxHp);
        HumanoidTransform = transform;
        PreviousPosition = transform.position;
    }
    public float GetHp() {return Hp;}
    public float GetMaxHp() {return MaxHp;}
    public void SetHp(float num) {float previous = Hp; Hp=num; if(Hp>MaxHp) Hp = MaxHp; if(Hp<0) Hp=0; if(previous>Hp) Damaged = true;}
    public bool IsDead(){if(Hp == 0 && !Invulnerable && !IsDeadFired) {IsDeadFired = true; return true;} return false;}
    public void SetMaxHp(float num) {Hp = MaxHp = num;}
    public void AddHp(float num) {SetHp(Hp+num);}
    public void ToggleInvulnerability(){Invulnerable = !Invulnerable;}
    public bool TookDamage(){bool state = Damaged; Damaged = false; return state && !Invulnerable && Hp>0;}
    public bool HasMoved(){ if(PreviousPosition.Equals(HumanoidTransform.position)) return false; PreviousPosition = HumanoidTransform.position; return true; }
}
