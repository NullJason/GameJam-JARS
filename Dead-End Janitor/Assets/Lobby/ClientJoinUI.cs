using UnityEngine;
using TMPro;
using Mirror;

public class ClientJoinUI : MonoBehaviour
{
    public TMP_InputField codeInput;
    public TMP_Text errorText;
    //public GameObject lobbyScenePrefab;
    public NetworkManager networkManager;

    public void JoinLobby()
    {
        string code = codeInput.text.ToUpper();
        if (LobbyManager.Instance.TryJoinLobby(code, out NetworkManager hostManager))
        {
            // Connect tohost for LAN/same machine
            networkManager.networkAddress = "localhost"; 
            networkManager.StartClient();
        }
        else
        {
            errorText.text = "Invalid lobby code.";
        }
    }

    public void OnClientConnected()
    {
        //Instantiate(lobbyScenePrefab); // Spawn lobby UI/character
    }

    public void OnClientDisconnected()
    {
        errorText.text = "Failed to connect to host.";
    }
}
