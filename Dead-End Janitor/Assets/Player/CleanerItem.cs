using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CleanerItem : MonoBehaviour
{
	// under any item that can clean.
	
	// each second, add 5 cleaniness to dirt.
	[SerializeField] private int Speed = 1;
	[SerializeField] private int Strength = 5;
	[SerializeField] Camera PlayerCamera;

	public float detectionRange = 10f;
    public LayerMask DirtyLayer;
	private OnCooldown = false;

    void Update()
    {
		if(Input.GetMouseButtonDown(0)) DetectLookAt();
    }

    void DetectLookAt()
    {
		if(PlayerCamera == null) { Debug.Log("PlayerCamera DNE, Please assign."); return;}
        
		Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, detectionRange, DirtyLayer))
        {
            Debug.Log("Looking at the mess: " + hit.collider.gameObject.name);
			TryCleanUp(hit.collider.gameObject);
        }
		else{
			
		}
    }
	void TryCleanUp(Gameobject mess){
		if(!OnCooldown) {OnCooldown = true; StartCoroutine(CleanUp);}
	}
		
	IEnumerator CleanUp(Gameobject mess){
		DirtyObject MessScript = mess.GetComponent<DirtyObject>();
		yield return new WaitForSecondsRealtime(Speed);
		MessScript.Clean(Strength);
		OnCooldown = false;
	}
}