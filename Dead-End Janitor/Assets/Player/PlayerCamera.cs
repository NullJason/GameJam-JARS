using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private Transform PlayerTransform = null;
	public float Sensitivity {
		get { return sensitivity; }
		set { sensitivity = value; }
	}
	[Range(0.1f, 9f)][SerializeField] float sensitivity = 2f;
	[Tooltip("Limits vertical camera rotation. Prevents the flipping that happens when rotation goes above 90.")]
	[Range(0f, 90f)][SerializeField] float yRotationLimit = 88f;

	Vector2 rotation = Vector2.zero;
	const string xAxis = "Mouse X";
	const string yAxis = "Mouse Y";
    private bool isCursorLocked = true;
    private void Start() {
        if (PlayerTransform == null) PlayerTransform = GameObject.Find("Player").transform;
    }
    void HandleCursorLock()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isCursorLocked = !isCursorLocked;
        }

        if (isCursorLocked)
        {
            Cursor.lockState = CursorLockMode.Locked; 
            Cursor.visible = false; 
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true; 
        }
    }

	void Update(){
        HandleCursorLock();
		rotation.x += Input.GetAxis(xAxis) * sensitivity;
		rotation.y += Input.GetAxis(yAxis) * sensitivity;
		rotation.y = Mathf.Clamp(rotation.y, -yRotationLimit, yRotationLimit);
		var xQuat = Quaternion.AngleAxis(rotation.x, Vector3.up);
		var yQuat = Quaternion.AngleAxis(rotation.y, Vector3.left);

        PlayerTransform.rotation = xQuat; // plr handles left/right
		transform.localRotation = yQuat; // cam handles up/down
        
	}
}
