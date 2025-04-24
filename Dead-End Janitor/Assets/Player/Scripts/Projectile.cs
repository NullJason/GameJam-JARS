using UnityEngine;
using System.Reflection;
using Unity.VisualScripting;
using Unity.Mathematics;
using System.Collections;
#pragma warning disable CS0219, CS0414 // disables assigned but not used variable and field warning.

public class Projectile : MonoBehaviour
{
    [Header("Projectile Behavior / Base Stats")] // The basic stat of the projectile. Tools can add to a projectile's stats through the Projectile Percent Properties.
    private int CleanType = 1; // DO NOT ALTER. Change in RangedTool instead, value reflected. altering this WILL break cleaning.
    [SerializeField] private Vector2 ProjectilePath = new Vector2(0,0); // randomizes how the projectile travels. (animation only)
    [SerializeField] private int ProjectileBounceCount = 0; // bounces off walls?
    [SerializeField] private int ProjectileBounceForce = 1;
    [SerializeField] private int ProjectileImpact = 1;
    [SerializeField] private float ProjectileDamage = 5; 
    [SerializeField] private int ProjectilePierce = 0; // determines how many entities can be pierced where -1 = infinite pierce. For example if the tool shoots a big bubble (AOE) you would want -1 so the bubble doesn't 'pop' when it hits an entity or dirty object.
    [SerializeField] private int ProjectileWallPierce = 0; // 0 = walls always stops projectiles.
    [SerializeField] private float ProjectileLifeTime = 1;
    [SerializeField] private float ProjectileScale = 1;
    [SerializeField] private float ProjectileImpactDelay = 0;
    [SerializeField] private int ProjectileGravity = 0; // influence of world gravity. can be any int.
    [SerializeField] private GameObject AOEObject; // the AOE effect that spawns upon the end of the projectile's lifespan.
    [SerializeField] private GameObject PlayerWhoFired;
    [SerializeField] private bool FriendlyFire = false;
    [SerializeField] private bool CanClean = true;
    [SerializeField] private bool CanDamage = true;
    private Transform ProjectileFolder;
    private bool ProjectileEnabled = false;
    private Rigidbody ProjectileRB;
    private Collider ProjectileColl;
    private const string DirtyLayerName = "Dirty";
    private int DirtyLayerIndex;
    private GameObject QueuedAOEObject;
    void Start()
    {
        int layerIndex = LayerMask.NameToLayer(DirtyLayerName);

        if (layerIndex != -1)
        {
            DirtyLayerIndex = layerIndex;
		} else Debug.Log("DIRTY LAYER DNE!!!");
    }
    public void InitFromToolValues(RangedTool source){
        // getting values via reflection so I dont have to manually apply every single thing using parameters. fortunately not computationally expensive if just for initializing.
        // this.CanClean = CanClean;
        // this.CanDamage = CanDamage;
        FieldInfo[] fields = typeof(RangedTool).GetFields();
        foreach (FieldInfo field in fields){
            FieldInfo targetField = typeof(Projectile).GetField(field.Name);
            if (targetField != null && targetField.FieldType == field.FieldType){
                object toolValue = field.GetValue(source);
                object projectileValue = targetField.GetValue(this);
                if(toolValue is int intvalue && projectileValue is int targetint){
                    targetField.SetValue(this, targetint + intvalue * targetint);
                }
                else if(toolValue is float floatvalue && projectileValue is float targetfloat){
                    targetField.SetValue(this, targetfloat + floatvalue * targetfloat);
                }
                else if(toolValue is bool boolvalue && projectileValue is bool targetbool){
                    targetField.SetValue(this, boolvalue);
                }
            }
        }
        transform.localScale *= ProjectileScale;
        PlayerWhoFired = source.GetOwner();
    }
    public void PassFolder(Transform folder){
        ProjectileFolder = folder;
        transform.SetParent(folder);
    }
    public void Ignore(GameObject objectToIgnore)
    {
        if(objectToIgnore == null) {Debug.Log("Tried to ignore null."); return;}
        if(gameObject.activeSelf) Debug.Log("Projectile Active before ignore collision.");
        Collider[] ObjectColliders = objectToIgnore.GetComponentsInChildren<Collider>();
        Collider[] projectileColliders = transform.GetComponentsInChildren<Collider>();
        if(QueuedAOEObject != null && QueuedAOEObject.TryGetComponent<AOEObject>(out AOEObject aoeMono)) aoeMono.Ignore(objectToIgnore);

        foreach (var projCol in projectileColliders)
        {
            foreach (var Col in ObjectColliders)
            {
                Physics.IgnoreCollision(projCol, Col);
                Debug.Log(Col.name);
            }
        }
    }
    void OnEnable()
    {
        if (!ProjectileEnabled && transform.parent == ProjectileFolder){
            ProjectileEnabled = true; 
            QueuedAOEObject = Instantiate(AOEObject);
            transform.SetParent(ProjectileFolder);
            if(transform.TryGetComponent<Rigidbody>(out Rigidbody rb)) ProjectileRB = rb;
            if(transform.TryGetComponent<Collider>(out Collider c)) {ProjectileColl = c; Ignore(PlayerWhoFired);}
            if (ProjectileGravity != 0) rb.useGravity = true;
            StartCoroutine(DoDelayedDestroy());
        }
    }
    private const string collidedText = "Projectile Collided with something."; // debug
    private const string collidedFailText = "Projectile Collided while !ProjectileEnabled, folder prob not set up.";
    private const string collidedBehaviorNotSetText = "Unspecified Behavior.";
    
    private IEnumerator DoDelayedDestroy(){
        yield return new WaitForSeconds(ProjectileLifeTime);
        Destroy(gameObject);
    }
    void OnCollisionEnter(Collision collision)
    {

        Transform CollT = collision.transform;
        int layer = collision.gameObject.layer;

        if (ProjectileEnabled){
            Debug.LogWarning($"Projectile Collided with {collision.gameObject.name}");
            if(layer == 0){ // default unity layer (walls, floors, etc.)
                if (ProjectileBounceCount != 0){
                    ProjectileBounceCount -= 1;
                } else {Destroy(gameObject);}
                Debug.LogWarning($"Projectile Bounced Off {collision.gameObject.name}");
            }
            else if(layer == DirtyLayerIndex && CanClean){
                if(CollT.TryGetComponent(out DirtyObject dirt)){
                    if (dirt.IsDirtType(CleanType)) dirt.Clean(ProjectileDamage);
                    Debug.LogWarning($"Projectile tried to clean {collision.gameObject.name}");
                }
            } 
            else if(CanDamage && CollT.TryGetComponent(out Humanoid humanoid)){
                humanoid.AddHp(-ProjectileDamage); 
                if (ProjectilePierce != 0) ProjectilePierce -= 1;
                else Destroy(gameObject);
                Debug.LogWarning($"Projectile tried to damage {collision.gameObject.name}");
            }
            else Debug.Log(collidedBehaviorNotSetText);
        } 
        else{
            Debug.Log(collidedFailText);
        }
    }
    void OnDestroy()
    {
        Debug.LogWarning("Projectile was destroyed!");
        if (AOEObject == null) return;
        Vector3 pos = transform.position;
        quaternion rot = transform.rotation;
        QueuedAOEObject.transform.SetParent(ProjectileFolder);
        QueuedAOEObject.transform.SetPositionAndRotation(pos, rot);
        QueuedAOEObject.SetActive(true);
    }
}
