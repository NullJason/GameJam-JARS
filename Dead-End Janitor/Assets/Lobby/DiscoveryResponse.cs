using Mirror;
using System.Net;

public struct DiscoveryResponse : NetworkMessage
{
    public string uri;           // Used by the client to connect
    public string lobbyCode;     // used to match lobbies

    // todo: Add more data (e.g., number of players, host name, etc.)
}
