using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
#pragma warning disable CS0219, CS0414 // disables assigned but not used variable and field warning.

public class AOEObject : MonoBehaviour
{
    private Dictionary<int,string> AOEEffect = new Dictionary<int, string>{
        [1] = "Instant", // effects happen once.
        [2] = "Sustain" // effects are sustained.
    };
    [SerializeField] private float EffectDuration = 1; // how long the effect stays.
    [SerializeField] private float EffectApplySpeed = 1; // how often, in seconds.
    [SerializeField] private GameObject VisualEffectObject;
    [SerializeField] private float Damage = 1;
    [SerializeField] private float Clean = 1;

    // Anims.
    [SerializeField] private float StartScale = 0.1f;
    [SerializeField] private float EndScale = 1;
    [SerializeField] private Vector3 Vibration = new Vector3(0,0,0); // vibration force in all directions. makes obj vibrate back and forth on axis not 0.
    [SerializeField] private float AnimStart = 0;
    [SerializeField] private float AnimEnd = 0;
    [SerializeField] private int AnimRepeats = 0; // -1 = keeps repeating til end of anim.
    [SerializeField] private float AnimSpeed = 0;
    [SerializeField] private int AnimTweenType = 0; // 0 = linear.
    private HashSet<Transform> WithinAOETransforms = new HashSet<Transform>();
    void Start()
    {
        if(AnimEnd == 0 || AnimEnd>EffectDuration) AnimEnd = EffectDuration;
        transform.GetOrAddComponent<Rigidbody>();
        transform.GetOrAddComponent<Collider>();
    }
    void OnEnable()
    {
        StartCoroutine(DoCoolDownLoop());
        
        // play an animation, such as a bubble slowly expanding and contracting before popping, could make a anim util script. 
    }
    void OnDestroy()
    {
        // add mor projectiles!?!?
    }
    public void Ignore(GameObject objectToIgnore)
    {
        if(objectToIgnore == null) {Debug.Log("Tried to ignore null."); return;}
        Collider[] ObjectColliders = objectToIgnore.GetComponentsInChildren<Collider>();
        Collider[] projectileColliders = transform.GetComponentsInChildren<Collider>();

        foreach (var projCol in projectileColliders)
        {
            foreach (var Col in ObjectColliders)
            {
                Physics.IgnoreCollision(projCol, Col);
                Debug.Log(Col.name);
            }
        }
    }
    void OnCollisionEnter(Collision collision)
    {
        WithinAOETransforms.Add(collision.transform);
    }
    void OnCollisionStay(Collision collision)
    {
        WithinAOETransforms.Add(collision.transform); // might not detect coll entered when first enabled if objects inside.
    }
    void OnCollisionExit(Collision collision)
    {
        WithinAOETransforms.Remove(collision.transform);
    }
    void OnTriggerEnter(Collider other)
    {
        WithinAOETransforms.Add(other.transform);

    }
    void OnTriggerStay(Collider other)
    {
        WithinAOETransforms.Add(other.transform); // might not detect coll entered when first enabled if objects inside.

    }
    void OnTriggerExit(Collider other)
    {
        WithinAOETransforms.Remove(other.transform);
    }
    private void ApplyEffects(){
        foreach(Transform t in WithinAOETransforms){
            if(t == null) continue;
            if(t.TryGetComponent<Humanoid>(out Humanoid human)){
                human.AddHp(-Damage);
            }
            if(t.TryGetComponent<DirtyObject>(out DirtyObject dirty)){
                dirty.Clean(Clean);
            }
        }
    }
    private IEnumerator DoCoolDownLoop(){
        for(float i=0; i<EffectDuration; i+=EffectApplySpeed){
            ApplyEffects();
            yield return new WaitForSeconds(EffectApplySpeed);
        }
        Destroy(gameObject);
    }
}
