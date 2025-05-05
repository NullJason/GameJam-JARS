using UnityEngine;
using TMPro;
using Mirror;

public class HostLobbyUI : MonoBehaviour
{
    public TMP_Text lobbyCodeDisplay;
    public GameObject lobbyPanel;
    public NetworkManager networkManager;

    private string lobbyCode;

    public void CreateLobby()
    {
        lobbyCode = LobbyManager.Instance.GenerateUniqueLobbyCode();
        lobbyCodeDisplay.text = "Lobby Code: " + lobbyCode;

        networkManager.StartHost();

        LobbyManager.Instance.RegisterLobby(lobbyCode, networkManager);

        lobbyPanel.SetActive(true);
    }
    void OnEnable()
    {
        CreateLobby();
    }
    private void OnDisable()
    {
        if (!string.IsNullOrEmpty(lobbyCode))
            LobbyManager.Instance.UnregisterLobby(lobbyCode);
    }
}
