using Mirror;
using System.Collections.Generic;
using UnityEngine;

public class LobbyManager : MonoBehaviour
{
    public static LobbyManager Instance;

    // all active lobbies keyed by unique code
    private Dictionary<string, NetworkManager> activeLobbies = new Dictionary<string, NetworkManager>();

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    public string GenerateUniqueLobbyCode()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        string code;
        do
        {
            code = "";
            for (int i = 0; i < 5; i++)
                code += chars[Random.Range(0, chars.Length)];
        } while (activeLobbies.ContainsKey(code));
        return code;
    }

    public void RegisterLobby(string code, NetworkManager manager)
    {
        if (!activeLobbies.ContainsKey(code))
            activeLobbies.Add(code, manager);
    }

    public bool TryJoinLobby(string code, out NetworkManager manager)
    {
        return activeLobbies.TryGetValue(code, out manager);
    }

    public void UnregisterLobby(string code)
    {
        if (activeLobbies.ContainsKey(code))
            activeLobbies.Remove(code);
    }
}
