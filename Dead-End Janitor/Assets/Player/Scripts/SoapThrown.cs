using UnityEngine;

public class SoapThrown : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
       collision.transform.TryGetComponent<DirtyObject>(out DirtyObject dirtyObject);
       // TODO: instead of direct contact, create a splash gameobject which slowly drains anything in oncollisionstay, checks dirtyobject component there instead.
       if (dirtyObject){
            Debug.Log("Splash!");
       }
    }
}
