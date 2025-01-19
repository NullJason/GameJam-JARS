using UnityEngine;

public class ConnectToCleaner : MonoBehaviour
{
    public CleanerItem CleanerItemScript;
    private void Start() {
        if(CleanerItemScript==null) CleanerItemScript = transform.parent.GetComponent<CleanerItem>();
    }
    private void OnCollisionEnter(Collision other) {
        CleanerItemScript.ConnectCleanerCollision(other);
    }
    private void OnCollisionExit(Collision other) {
        CleanerItemScript.ConnectCleanerCollision(other);
    }
}
