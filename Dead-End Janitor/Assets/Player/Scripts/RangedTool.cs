using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RangedTool : MonoBehaviour
{
    // Jason: If we ever want a Projectile ID to Projectile Object structure. Should contain a list of all avaliable Projectile Assets in Game. This would only exist to make it more customizable and as backup.
    // 0 = default_Projectile, 1 = soap, 2 = vry slip soap
    public Transform ProjectileFolder; // folder all projectiles get parented to.
    Dictionary<int, Transform> ProjectileAssets = new Dictionary<int, Transform>();

    [Header("RangedTool Properties")]
    [SerializeField] private bool CanClean = true;
    [SerializeField] private bool CanDamage = true;
    [SerializeField] private int CleanType = 0; // 0 = liquid, 1 = solid here
    [SerializeField] private float ToolCooldown = 1;
    [SerializeField] private float ShootDelay = 0.5f;    
    [SerializeField] private bool isAuto = false; // determines if the weapon is togglable instead.
    [SerializeField] private GameObject Projectile;
    [SerializeField] private int ProjectileID_BackUp; // in case for wahtever reason can't find the projectile or too lazy to parent the projectile => uses ProjectileAssets instead.
    [SerializeField] private (int,int) Accuracy = (1,1); // 1 (max) = shoots directly at the cursor, -1 (min) = shoots behind the player. x = horizontal alignment, y = verticle alignment.
    [SerializeField] private int ProjectileCount = 1;
    [SerializeField] private float ProjectileSpeed = 10;


    [Header("Projectile Percent Properties")]
    // these properties are transferred as a scale factor to the actual projectile.
    // Ex: if a Projectile originally has a dmg of 5 and the tool a dmg scale of 2 it will do 200% more dmg then it usually does.
    [SerializeField] private int ProjectileImpact = 1;
    [SerializeField] private float ProjectileDamage = 1; 
    [SerializeField] private int ProjectilePierce = 1; 
    [SerializeField] private int ProjectileWallPierce = 1; 
    [SerializeField] private float ProjectileLifeTime = 1;
    [SerializeField] private float ProjectileScale = 1;
    [SerializeField] private float ProjectileImpactDelay = 1;
    [SerializeField] private int ProjectileGravity = 1;

    private bool OnCooldown = false;
    private GameObject AppliedProjectile;

    // Call whenever you want to shoot a projectile.
    public void ShootProjectile(Transform origin){
        if (OnCooldown) return;
        else OnCooldown = true;
        if (origin == null) origin = transform;
        StartCoroutine(ProjectileStart(origin));
        
    }
    // Stops a toggleable tool (a tool using isAuto).
    public void StopTool(){

    }
    private IEnumerator ProjectileStart(Transform shootOrigin){
        yield return new WaitForSeconds(ShootDelay);
        for (int i = 0; i<ProjectileCount; i++){
            GameObject projectile = CreateProjectile();
            projectile.transform.SetParent(ProjectileFolder);
            // TODO: delays and such
            // TODO: actually shoot the projectile
            projectile.transform.SetPositionAndRotation(shootOrigin.position, shootOrigin.rotation);

            projectile.SetActive(true);
        }
    }
    private void ApplyProjectile(GameObject projectile){
        bool exists = projectile.TryGetComponent(out Projectile projectileMono);
        if (!exists) {projectile.AddComponent<Projectile>(); projectileMono = projectile.GetComponent<Projectile>();}
        ApplyToolInfluenceToMono(projectileMono);
        projectileMono.PassFolder(ProjectileFolder);
        AppliedProjectile = projectile;
    }
    private void ApplyToolInfluenceToMono(Projectile mono){
       mono.InitFromToolValues(this);
    }
    private GameObject CreateProjectile(){
        return Instantiate(AppliedProjectile);
    }
    private void InitProjectile(){
        // note: default int value is 0 even if not explicitly set in C#
        GameObject newProjectile;
        if (Projectile){
            newProjectile = Instantiate(Projectile);
        }
        else newProjectile = Instantiate(ProjectileAssets[ProjectileID_BackUp].gameObject);
        newProjectile.SetActive(false);
        ApplyProjectile(newProjectile);
    }
    private void ChargeUp(){}
    
    void Start()
    {
        if (ProjectileFolder == null) ProjectileFolder = new GameObject("ProjectileFolder").transform;
        GameObject DefaultProjectile = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        DefaultProjectile.transform.localScale = new Vector3(0.25f,0.25f,0.25f);
        DefaultProjectile.AddComponent<Collider>();
        DefaultProjectile.AddComponent<Rigidbody>();
        ProjectileAssets.Add(0, DefaultProjectile.transform);
        InitProjectile();
    }

}
