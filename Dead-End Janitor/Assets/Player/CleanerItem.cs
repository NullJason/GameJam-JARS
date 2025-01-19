using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CleanerItem : MonoBehaviour
{
	// under any item that can clean.
	
	// each second, add 5 cleaniness to dirt.
	[SerializeField] private float Speed = 0.1f;
	[SerializeField] private float Strength = 0.25f;
	[SerializeField] private Vector3 ToolPositionOffset = new Vector3(1, -0.5f, 1.3f); // Position offset (adjust for bottom-right corner)
	[SerializeField] private Quaternion ToolRotationOffset = Quaternion.Euler(80, 0, 0); // Position offset (adjust for bottom-right corner)
	[SerializeField] private GameObject Player = null;
	[SerializeField] private Transform PlayerCameraTransform = null;
	public float detectionRange = 10f;
    private LayerMask DirtyLayer;
	private string LayerName = "Dirty";
	private Camera PlayerCamera;
	private bool OnCooldown = false;
	private bool ToolInterrupted = false; // stops cleaning if you look away or stop holding down m1.
	private bool isAnimating = false;
	[SerializeField] private ParticleSystem Effects;
	[SerializeField] private List<bool> DirtType = new List<bool>(){true, true}; // 1 = liquid, 2 = solid.
	[SerializeField] private List<bool> CleanMethod = new List<bool>(){true, true}; // 1 = collision, 2 = click.


	private void Start() {
		if(Player == null) Player = GameObject.Find("Player");
		if(PlayerCameraTransform == null && Player) PlayerCameraTransform = Player.transform.Find("PlayerCamera");
		PlayerCamera = PlayerCameraTransform.GetComponent<Camera>();
		if(Speed <= 0) Speed = 0.1f; if(Strength <= 0) Strength = 0.25f;
		int dirtyLayerId = LayerMask.NameToLayer(LayerName);
        if (dirtyLayerId != -1) // -1 indicates the layer doesn't exist
        {
            DirtyLayer = 1 << dirtyLayerId;
        }
	}

    void Update()
    {
		if(CleanMethod[1]){
		if(Input.GetMouseButton(0)) DetectLookAt();
		else{ ToolInterrupted = true; }}

		// Animator animator = GetComponent<Animator>();
		// isAnimating = animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f;
    }
	public void ConnectCleanerCollision(Collision other) {
		if(CleanMethod[0] && (DirtyLayer.value & (1 << other.gameObject.layer)) != 0) {
			ToolInterrupted = false;
			TryCleanUp(other.gameObject);
		}

	}

    void DetectLookAt()
    {
		if(PlayerCamera == null) { Debug.Log("PlayerCamera DNE, Please assign."); return;}
        
		Ray ray = new Ray(PlayerCamera.transform.position, PlayerCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, detectionRange, DirtyLayer))
        {
            Debug.Log("Looking at the huge mess: " + hit.collider.gameObject.name);
			ToolInterrupted = false;
			TryCleanUp(hit.collider.gameObject);
        }
		else{
			Debug.Log(" Nothing to Clean! :( ");
			ToolInterrupted = true;
		}
    }
	void TryCleanUp(GameObject mess){
		// double tool interrupt check is necessary, trust.
		DirtyObject MessScript = mess.GetComponent<DirtyObject>();
		bool CanClean = false;
		for(int i=0; i<DirtType.Count; i++) {if(DirtType[i] && MessScript.IsDirtType(i)) CanClean = true;} if(!CanClean) return;
		if(!OnCooldown && !ToolInterrupted) {OnCooldown = true; StartCoroutine(CleanUp(mess));}
	}
		
	IEnumerator CleanUp(GameObject mess){
		DirtyObject MessScript = mess.GetComponent<DirtyObject>();
		yield return new WaitForSecondsRealtime(Speed);
		if(!ToolInterrupted) MessScript.Clean(Strength);
		OnCooldown = false;
		if(Effects != null) Effects.Play();
	}

	private void OnEnable() {
		if (PlayerCameraTransform == null) return;
		transform.SetParent(PlayerCameraTransform);
		if (isAnimating) return;
		transform.position = PlayerCameraTransform.position + ToolPositionOffset;
        transform.rotation = PlayerCameraTransform.rotation * ToolRotationOffset;
	}
}