using UnityEngine;
using System.Reflection;
using Unity.VisualScripting;
using Unity.Mathematics;
using System.Collections;

public class Projectile : MonoBehaviour
{
    [Header("Projectile Behavior / Base Stats")] // The basic stat of the projectile. Tools can add to a projectile's stats through the Projectile Percent Properties.
    private int CleanType = 1; // DO NOT ALTER. Change in RangedTool instead, value reflected. altering this WILL break cleaning.
    [SerializeField] private Vector2 ProjectilePath = new Vector2(0,0); // randomizes how the projectile travels. (animation only)
    [SerializeField] private int ProjectileBounceCount = 0; // bounces off walls?
    [SerializeField] private int ProjectileImpact = 1;
    [SerializeField] private float ProjectileDamage = 5; 
    [SerializeField] private int ProjectilePierce = 0; // determines how many entities can be pierced where -1 = infinite pierce. For example if the tool shoots a big bubble (AOE) you would want -1 so the bubble doesn't 'pop' when it hits an entity or dirty object.
    [SerializeField] private int ProjectileWallPierce = 0; // 0 = walls always stops projectiles.
    [SerializeField] private float ProjectileLifeTime = 1;
    [SerializeField] private float ProjectileScale = 1;
    [SerializeField] private float ProjectileImpactDelay = 0;
    [SerializeField] private int ProjectileGravity = 0; // influence of world gravity. can be any int.
    [SerializeField] private GameObject AOEObject; // the AOE effect that spawns upon the end of the projectile's lifespan.
    private bool CanClean = true;
    private bool CanDamage = true;
    private Transform ProjectileFolder;
    private bool ProjectileEnabled = false;
    private Rigidbody ProjectileRB;
    private Collider ProjectileColl;
    private const string DirtyLayerName = "Dirty";
    private int DirtyLayerIndex;
    void Start()
    {
        int layerIndex = LayerMask.NameToLayer(DirtyLayerName);

        if (layerIndex != -1)
        {
            DirtyLayerIndex = layerIndex;
		} else Debug.Log("DIRTY LAYER DNE!!!");
    }
    public void InitFromToolValues(RangedTool source, bool CanClean, bool CanDamage){
        // getting values via reflection so I dont have to manually apply every single thing using parameters. fortunately not computationally expensive if just for initializing.
        this.CanClean = CanClean;
        this.CanDamage = CanDamage;
        FieldInfo[] fields = typeof(RangedTool).GetFields();
        foreach (FieldInfo field in fields){
            FieldInfo targetField = typeof(Projectile).GetField(field.Name);
            if (targetField != null && targetField.FieldType == field.FieldType){
                object toolValue = field.GetValue(source);
                object projectileValue = targetField.GetValue(this);
                if(toolValue is int intvalue && projectileValue is int targetint){
                    targetField.SetValue(this, targetint + intvalue * targetint);
                }
                else if(toolValue is float floatvalue && projectileValue is int targetfloat){
                    targetField.SetValue(this, targetfloat + floatvalue * targetfloat);
                }
            }
        }
        transform.localScale *= ProjectileScale;
    }
    public void PassFolder(Transform folder){
        ProjectileFolder = folder;
    }

    void OnEnable()
    {
        if (transform.parent = ProjectileFolder){
            ProjectileEnabled = true;
            if(transform.TryGetComponent<Rigidbody>(out Rigidbody rb)) ProjectileRB = rb;
            if(transform.TryGetComponent<Collider>(out Collider c)) ProjectileColl = c;
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
            Debug.Log(collidedText);
            if(layer == 0){ // default unity layer (walls, floors, etc.)
                if (ProjectileBounceCount != 0){
                    ProjectileBounceCount -= 1;
                } else Destroy(gameObject);
            }
            else if(layer == DirtyLayerIndex && CanClean){
                if(CollT.TryGetComponent(out DirtyObject dirt)){
                    if (dirt.IsDirtType(CleanType)) dirt.Clean(ProjectileDamage);
                }
            } 
            else if(CanDamage && CollT.TryGetComponent(out Humanoid humanoid)){
                humanoid.AddHp(ProjectileDamage); // TODO: doesn't account for friendly fire yet.
                if (ProjectilePierce != 0) ProjectilePierce -= 1;
                else Destroy(gameObject);
            }
            else Debug.Log(collidedBehaviorNotSetText);
        } 
        else{
            Debug.Log(collidedFailText);
        }
    }
    void OnDestroy()
    {
        if (AOEObject == null) return;
        Vector3 pos = transform.position;
        quaternion rot = transform.rotation;
        GameObject AOEobj = Instantiate(AOEObject);
        AOEobj.transform.SetParent(ProjectileFolder);
        AOEobj.transform.SetPositionAndRotation(pos, rot);
        AOEobj.SetActive(true);
    }
}
