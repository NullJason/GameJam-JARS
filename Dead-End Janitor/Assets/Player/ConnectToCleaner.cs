using System.Collections.Generic;
using UnityEngine;

public class ConnectToCleaner : MonoBehaviour
{
    public CleanerItem CleanerItemScript;
    private Vector3 position;
    private Quaternion rotation;
    [SerializeField] private List<bool> CleanMethod = new List<bool>(){false, false}; // 1 = mop, 2 = vacuum. 
    private void Start() {
        position = transform.localPosition;
        rotation = transform.localRotation;
        if(CleanerItemScript==null) CleanerItemScript = transform.parent.GetComponent<CleanerItem>();
    }
    private void OnCollisionEnter(Collision other) {
        if(CleanMethod[0]) CleanerItemScript.ConnectCleanerCollision(other);
    }
    private void OnCollisionStay(Collision other) {
        if(CleanMethod[1]) CleanerItemScript.ConnectCleanerCollision(other);
    }
    private void OnCollisionExit(Collision other) {
        if(CleanMethod[0]) CleanerItemScript.ConnectCleanerCollision(other);
    }
    private void OnDisable(){
      Debug.Log("=D");
      transform.localRotation = rotation;
      transform.localPosition = position;
    }
}
