using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Zombie : Humanoid
{
    Transform Effects;
    ParticleSystem Particles;
    public bool DeathProcessed = false;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private GameObject blood;
    [SerializeField] private GameObject solid;
    private Animator animator;
    private AudioSource audioSource;
    private AudioClip audioClip;



    void Start()
    {
        if(agent == null) agent = GetComponent<NavMeshAgent>();
        Effects = transform.Find("Effects");
        Particles = Effects.GetComponent<ParticleSystem>();
        Particles.Stop(true);
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.Stop();

        // Load the audio clip from the Resources folder
        audioClip = Resources.Load<AudioClip>("Sounds/FleshHit"); // Replace "AudioFileName" with your MP3's file name (without extension)
        audioSource.playOnAwake = false;
        if (audioClip == null)
        {
            Debug.LogError("Audio file not found in Resources folder.");
        }
        audioSource.clip = audioClip;
        animator = GetComponent<Animator>();
        transform.Find("Armature").rotation = Quaternion.Euler(-90, 180, 0);
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
        if(animator) {animator.SetBool("IsWalking", false); animator.SetTrigger("DoDeath");}
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
        audioClip = Resources.Load<AudioClip>("Sounds/BloodExplosion");
        audioSource.clip = audioClip;
        audioSource.Play();
        StartDelayedDestroy(Particles.main.duration);
        this.enabled = false;
    }
    void OnHit(){
        Debug.Log(gameObject.name + " :" + GetHp() + ": " + "OUCH! >:(");
        Particles.Play();
//        Instantiate(blood, transform.position - new Vector3(0, transform.localScale.y, 0), transform.rotation); //TODO: Replace this with solids, once we have them!
        Instantiate(blood, transform.position/* - new Vector3(0, transform.localScale.y, 0)*/, transform.rotation);
        audioSource.Play();
        DoAttack();
    }
    void DoAttack(){
        animator.SetTrigger("DoAttack");
    }
    void StartDelayedDestroy(float time){
        StartCoroutine(DelayedDestroyObject(time));
    }
    IEnumerator DelayedDestroyObject(float time){
        yield return new WaitForSeconds(time);
        Instantiate(solid, transform.position, transform.rotation);
        Destroy(gameObject);
    }
    public virtual int HowManyDroppedTotal(){
        return (int)Mathf.Ceil(GetMaxHp()) + 1;
    }
}
