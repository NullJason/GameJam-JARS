using UnityEngine;
using TMPro;
using Mirror;

public class HostLobby : MonoBehaviour
{
    public TMP_Text lobbyCodeDisplay;
    public NetworkManager networkManager;
    public CustomNetworkDiscovery discovery;

    private string lobbyCode;

    public void CreateLobby()
    {
        lobbyCode = LobbyManager.Instance.GenerateUniqueLobbyCode();
        LobbyManager.Instance.CurrentLobbyCode = lobbyCode;
        lobbyCodeDisplay.text = "Lobby Code: " + lobbyCode;

        networkManager.StartHost();
        discovery.currentLobbyCode = lobbyCode;
        discovery.AdvertiseServer(); // broadcast lobby code over LAN
    }
    void OnEnable()
    {
        CreateLobby();
    }
    private void OnDisable()
    {
        networkManager.StopHost();
    }
}
