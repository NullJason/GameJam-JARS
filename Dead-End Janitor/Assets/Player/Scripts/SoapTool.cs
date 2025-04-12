using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class SoapTool : MonoBehaviour
{
	[SerializeField] private GameObject Player = null;
	[SerializeField] private Transform PlayerCameraTransform = null;
	[SerializeField] private ParticleSystem Effects;
	[SerializeField] private PlayerTool ToolMonobehavior;
	private Camera PlayerCamera;

	private void Start() {
		if(Player == null) Player = GameObject.Find("Player");
		if(PlayerCameraTransform == null && Player) PlayerCameraTransform = Player.transform.Find("PlayerCamera");
		PlayerCamera = PlayerCameraTransform.GetComponent<Camera>();
		
	}
	
    void Update()
    {
		if(Input.GetMouseButtonDown(0)) 
        {
			ToolMonobehavior.ActivateTool(PlayerCameraTransform);
		}
    }
}
