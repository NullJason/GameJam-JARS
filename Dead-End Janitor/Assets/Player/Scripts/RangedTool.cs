using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
#pragma warning disable CS0219 // disables assigned but not used variable warning.
public class RangedTool : PlayerTool
{
    // If we ever want a Projectile ID to Projectile Object structure. Should contain a list of all avaliable Projectile Assets in Game. This would only exist to make it more customizable and as backup.
    // 0 = default_Projectile, 1 = soap, 2 = vry slip soap
    public Transform ProjectileFolder; // folder all projectiles get parented to.
    Dictionary<int, Transform> ProjectileAssets = new Dictionary<int, Transform>();

    [Header("RangedTool Properties")]
    [SerializeField] private bool CanClean = true;
    [SerializeField] private bool CanDamage = true; // dmg entities
    [SerializeField] private int CleanType = 0; // 0 = liquid, 1 = solid here
    [SerializeField] private float ToolCooldown = 1;
    [SerializeField] private float ShootDelay = 0.5f;    
    [SerializeField] private bool isAuto = false; // determines if the weapon is togglable instead.

    [SerializeField] private GameObject Projectile;
    [SerializeField] private int ProjectileID_BackUp; // in case for wahtever reason can't find the projectile or too lazy to parent the projectile => uses ProjectileAssets instead.
    [SerializeField] private bool ShootPhysicalProjectile = false; // acts like a normal fps if false.
    [SerializeField] private (int,int) Accuracy = (1,1); // 1 (max) = shoots directly at the cursor, -1 (min) = shoots behind the player. x = horizontal alignment, y = verticle alignment.
    [SerializeField] private int ProjectileCount = 1;
    [SerializeField] private float ProjectileSpeed = 60; // force


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
    public override void ActivateTool(Transform origin){
        if (OnCooldown) return;
        OnCooldown = true;
        if (origin == null) origin = transform;
        StartCoroutine(ProjectileStart(origin));
        StartCoroutine(DoCoolDown());
    }
    // Stops a toggleable tool (a tool using isAuto).
    public override void StopTool(){

    }
    private IEnumerator DoCoolDown(){
        yield return new WaitForSeconds(ToolCooldown);
        OnCooldown = false;
    }
    private IEnumerator ProjectileStart(Transform shootOrigin){
        yield return new WaitForSeconds(ShootDelay);
        for (int i = 0; i<ProjectileCount; i++){
            GameObject projectile = CreateProjectile();
            projectile.transform.SetParent(ProjectileFolder);
            projectile.SetActive(true);
            // TODO: different shooting mechanisms
            if(ShootPhysicalProjectile){
               Rigidbody rb = projectile.GetOrAddComponent<Rigidbody>();
               rb.AddForce(shootOrigin.forward * ProjectileSpeed);
            } else{
                Ray ray = new Ray(shootOrigin.position, shootOrigin.forward);
                if (Physics.Raycast(ray, out RaycastHit hit, 50f))
                {
                    // TODO: activate projectile effects. assumes projectile is anchored.
                    if(CanDamage && hit.transform.TryGetComponent<Humanoid>(out Humanoid human)){
                        human.AddHp(ProjectileDamage);
                    }
                    if(CanClean && hit.transform.TryGetComponent<DirtyObject>(out DirtyObject dirty)){
                        dirty.Clean(ProjectileDamage);
                    }  
                }
            }
            projectile.transform.SetPositionAndRotation(shootOrigin.position, shootOrigin.rotation);
            
            yield return new WaitForSeconds(ShootDelay);
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
       mono.InitFromToolValues(this,CanClean,CanDamage);
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
    private IEnumerator ChargeUp(){
        yield return new WaitForSeconds(ToolCooldown);
        OnCooldown = false;
    }
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
