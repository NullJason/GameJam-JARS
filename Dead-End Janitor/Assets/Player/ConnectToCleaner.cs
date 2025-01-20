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
    private void OnEnable(){
      Debug.Log("=D");
      transform.localRotation = Quaternion.Euler(-89.921f, 0, 0);
      transform.localPosition = new Vector3(4.30742233f, 2.57828106f, 4.50259023e-07f);
    }
}
