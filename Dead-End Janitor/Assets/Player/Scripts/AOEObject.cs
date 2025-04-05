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
    [SerializeField] private float Damage = 1;
    [SerializeField] private float Clean = 1;
    void Start()
    {
        transform.GetOrAddComponent<Rigidbody>();
        transform.GetOrAddComponent<Collider>();
    }
    void OnCollisionStay(Collision collision)
    {
        // apply effects
    }
}
