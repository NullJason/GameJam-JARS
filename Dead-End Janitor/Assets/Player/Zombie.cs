using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Zombie : Humanoid
{
    Transform Effects;
    ParticleSystem Particles;
    public bool DeathProcessed = false;
    [SerializeField] private NavMeshAgent agent;
    void Start()
    {
        if(agent == null) agent = GetComponent<NavMeshAgent>();
        Effects = transform.Find("Effects");
        Particles = Effects.GetComponent<ParticleSystem>();
        Particles.Stop(true);
    }

    // Update is called once per frame
    void Update()
    {
        agent.SetDestination(GameplayManager.hunter.transform.position);
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
