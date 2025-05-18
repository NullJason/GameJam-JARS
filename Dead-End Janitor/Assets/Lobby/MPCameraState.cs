using Mirror;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class MPCameraState : NetworkBehaviour
{
    void Start()
    {
        if (!isLocalPlayer)
        {
            Debug.Log("Detected as non-local player.");
            // transform.GetComponent<Camera>().enabled = false;
            // transform.GetComponent<AudioListener>().enabled = false;
            // transform.GetComponent<UniversalAdditionalCameraData>().enabled = false;
        }
    }
}
