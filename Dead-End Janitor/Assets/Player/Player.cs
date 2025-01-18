using UnityEngine;

public class Player : Humanoid
{
    // Checkout Humanoid.cs!

    // Update is called once per frame
    void Update()
    {
        if(IsDead()){
            UponDeath();
        }
        if(TookDamage()){
            OnHit();
        }
    }
    void UponDeath(){}
    void OnHit(){}
}
