using UnityEngine;
using TMPro;
using Mirror;
using System.Net;

public class ClientJoinUI : MonoBehaviour
{
    public TMP_InputField codeInput;
    public TMP_Text errorText;
    string targetCode;
    public NetworkManager networkManager;
    public CustomNetworkDiscovery discovery;
    private bool matchFound = false;
    public HostLobby lobbyHoster;
    void Start()
    {
        discovery.OnServerFoundCallback = OnServerFound;
        codeInput.onEndEdit.AddListener(delegate {JoinLobby();});
    }

    public void JoinLobby()
    {
        networkManager.StopHost();
        Debug.LogWarning("Trying to Find lobby with this ID");
        targetCode = codeInput.text.ToUpper();         Debug.LogWarning(targetCode);
        errorText.text = "Searching for lobby...";
        matchFound = false;
        discovery.StartDiscovery(); // begin scanning for LAN hosts
    }
    void OnServerFound(DiscoveryResponse response, IPEndPoint endpoint)    {
        if (matchFound) return;

        if (response.lobbyCode == targetCode)
        {
            matchFound = true;
            errorText.text = "Joining lobby...";

            networkManager.networkAddress = endpoint.Address.ToString();
            networkManager.StartClient();
        } else{
            errorText.text = "Failed to find lobby. Starting to host again.";
            lobbyHoster.CreateLobby();
        }
    }
}
