using UnityEngine;
using Mirror;

public class CustomNetworkManager : NetworkRoomManager
{
    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnServerAddPlayer(conn);
        Debug.Log("A player has joined the lobby.");

        // You could also notify the host player here
        GameObject hostPlayer = NetworkServer.connections[0].identity.gameObject;
        hostPlayer.GetComponent<HostPlayer>()?.OnNewPlayerJoined(conn);
    }
}
