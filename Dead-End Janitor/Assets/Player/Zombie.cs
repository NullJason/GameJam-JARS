using System.Collections;
using UnityEngine;

public class Zombie : Humanoid
{
    Transform Effects;
    ParticleSystem Particles;
    public bool DeathProcessed = false;
    void Start()
    {
        Effects = transform.Find("Effects");
        Particles = Effects.GetComponent<ParticleSystem>();
        Particles.Stop(true);
    }

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
    void UponDeath(){
        Debug.Log(gameObject.name + " Has Died and left a big mess.");
        if(DeathProcessed) return;
        DeathProcessed = true;
        if(Particles.isPlaying) Particles.Stop();
        var mainModule = Particles.main;
        mainModule.startSize = 0.5f;
        mainModule.startSpeed = 5;
        Particles.Play();
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
        rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
        StartDelayedDestroy(Particles.main.duration);
    }
    void OnHit(){
        Debug.Log(gameObject.name + " :" + GetHp() + ": " + "OUCH! >:(");
        Particles.Play();
    }
    void StartDelayedDestroy(float time){
        StartCoroutine(DelayedDestroyObject(time));
    }
    IEnumerator DelayedDestroyObject(float time){
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }
}
