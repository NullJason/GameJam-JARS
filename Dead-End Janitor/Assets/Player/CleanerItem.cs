using System.Collections;
using UnityEngine;

public class CleanerItem : MonoBehaviour
{
	// under any item that can clean.
	
	// each second, add 5 cleaniness to dirt.
	[SerializeField] private int Speed = 1;
	[SerializeField] private int Strength = 5;
	[SerializeField] private Vector3 ToolPositionOffset = new Vector3(1, -0.5f, 1.3f); // Position offset (adjust for bottom-right corner)
	[SerializeField] private Quaternion ToolRotationOffset = Quaternion.Euler(80, 0, 0); // Position offset (adjust for bottom-right corner)
	[SerializeField] private GameObject Player = null;
	[SerializeField] private Transform PlayerCameraTransform = null;
	public float detectionRange = 10f;
    public LayerMask DirtyLayer;
	private Camera PlayerCamera;
	private bool OnCooldown = false;
	private bool ToolInterrupted = false; // stops cleaning if you look away or stop holding down m1.
	private bool isAnimating = false;

	private void Start() {
		if(Player == null) Player = GameObject.Find("Player");
		if(PlayerCameraTransform == null && Player) PlayerCameraTransform = Player.transform.Find("PlayerCamera");
		PlayerCamera = PlayerCameraTransform.GetComponent<Camera>();
	}

    void Update()
    {
		if(Input.GetMouseButton(0)) DetectLookAt();
		else{ ToolInterrupted = true; }

		// Animator animator = GetComponent<Animator>();
		// isAnimating = animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f;
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
		if(!OnCooldown && !ToolInterrupted) {OnCooldown = true; StartCoroutine(CleanUp(mess));}
	}
		
	IEnumerator CleanUp(GameObject mess){
		DirtyObject MessScript = mess.GetComponent<DirtyObject>();
		yield return new WaitForSecondsRealtime(Speed);
		if(!ToolInterrupted) MessScript.Clean(Strength);
		OnCooldown = false;
	}

	private void OnEnable() {
		if (PlayerCameraTransform == null) return;
		transform.SetParent(PlayerCameraTransform);
		if (isAnimating) return;
		transform.position = PlayerCameraTransform.position + ToolPositionOffset;
        transform.rotation = PlayerCameraTransform.rotation * ToolRotationOffset;
	}
}