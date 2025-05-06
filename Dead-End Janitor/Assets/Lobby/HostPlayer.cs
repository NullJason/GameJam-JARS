using UnityEngine;
using Mirror;

public class HostPlayer : NetworkBehaviour
{
    public void OnNewPlayerJoined(NetworkConnection conn)
{
    if (!isServer) return;

    var connToClient = conn as NetworkConnectionToClient;
    if (connToClient != null)
    {
        Debug.Log("New player joined! Connection ID: " + connToClient.connectionId);
    }
}

}
