using Mirror;
using Mirror.Discovery;
using UnityEngine;
using System.Net;

public class CustomNetworkDiscovery : NetworkDiscoveryBase<DiscoveryRequest, DiscoveryResponse>
{
    public string currentLobbyCode;
    public System.Action<DiscoveryResponse, IPEndPoint> OnServerFoundCallback;

    protected override DiscoveryRequest GetRequest() => new DiscoveryRequest();

    protected override DiscoveryResponse ProcessRequest(DiscoveryRequest request, IPEndPoint endpoint)
    {
        return new DiscoveryResponse
        {
            uri = transport.ServerUri().ToString(),
            lobbyCode = currentLobbyCode,
        };
    }

     protected override void ProcessResponse(DiscoveryResponse response, IPEndPoint endpoint)
    {
        OnServerFoundCallback?.Invoke(response, endpoint);
    }

}
