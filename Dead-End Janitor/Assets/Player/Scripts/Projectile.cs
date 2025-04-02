using UnityEngine;
using System.Reflection;
using UnityEngine.WSA;

public class Projectile : MonoBehaviour
{
    [Header("Projectile Behavior")]
    [SerializeField] private Vector2 ProjectilePath = new Vector2(0,0); // randomizes how the projectile travels.
    [SerializeField] private int ProjectileBounceCount = 0; // bounces off walls?
    [SerializeField] private int ProjectileImpact = 1;
    [SerializeField] private float ProjectileDamage = 5; 
    [SerializeField] private int ProjectilePierce = 0; // determines how many entities can be pierced where -1 = infinite pierce. For example if the tool shoots a big bubble (AOE) you would want -1 so the bubble doesn't 'pop' when it hits an entity or dirty object.
    [SerializeField] private int ProjectileWallPierce = 0; // 0 = walls always stops projectiles.
    [SerializeField] private float ProjectileLifeTime = 1;
    [SerializeField] private float ProjectileScale = 1;
    [SerializeField] private float ProjectileImpactDelay = 0;
    [SerializeField] private int ProjectileGravity = 0; // influence of world gravity. can be any int.
    private Transform ProjectileFolder;
    private bool ProjectileEnabled = false;
    public void InitFromToolValues(RangedTool source){
        // getting values via reflection so I dont have to manually apply every single thing using parameters. fortunately not computationally expensive if just for initializing.
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
    }
    public void PassFolder(Transform folder){
        ProjectileFolder = folder;
    }

    void OnEnable()
    {
        if (transform.parent = ProjectileFolder) ProjectileEnabled = true;
    }
    private const string collidedText = "Projectile Collided."; // debug
    void OnCollisionEnter(Collision collision)
    {
        if (ProjectileEnabled){
            Debug.Log(collidedText);
        }
    }
}
