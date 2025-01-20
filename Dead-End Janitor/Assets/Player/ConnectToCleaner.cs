using UnityEngine;

public class ConnectToCleaner : MonoBehaviour
{
    public CleanerItem CleanerItemScript;
    private Vector3 position;
    private Quaternion rotation;
    private void Start() {
        position = transform.localPosition;
        rotation = transform.localRotation;
        if(CleanerItemScript==null) CleanerItemScript = transform.parent.GetComponent<CleanerItem>();
    }
    private void OnCollisionEnter(Collision other) {
        CleanerItemScript.ConnectCleanerCollision(other);
    }
    private void OnCollisionExit(Collision other) {
        CleanerItemScript.ConnectCleanerCollision(other);
    }
    private void OnDisable(){
      Debug.Log("=D");
      transform.localRotation = rotation;
      transform.localPosition = position;
    }
}
