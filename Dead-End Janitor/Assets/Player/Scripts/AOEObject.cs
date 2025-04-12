using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

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
    private List<Transform> WithinAOETransforms = new List<Transform>();
    void Start()
    {
        transform.GetOrAddComponent<Rigidbody>();
        transform.GetOrAddComponent<Collider>();
        StartCoroutine(DoCoolDownLoop());
    }
    void OnCollisionEnter(Collision collision)
    {
        WithinAOETransforms.Add(collision.transform);
    }
    void OnCollisionExit(Collision collision)
    {
        WithinAOETransforms.Remove(collision.transform);
    }
    private void ApplyEffects(){
        foreach(Transform t in WithinAOETransforms){
            if(TryGetComponent<Humanoid>(out Humanoid human)){
                human.AddHp(Damage);
            }
            if(TryGetComponent<DirtyObject>(out DirtyObject dirty)){
                dirty.Clean(Clean);
            }
        }
    }
    private IEnumerator DoCoolDownLoop(){
        for(float i=0; i<EffectDuration; i+=EffectApplySpeed){
            yield return new WaitForSeconds(EffectApplySpeed);
            ApplyEffects();
        }
        Destroy(gameObject);
    }
}
