using Mirror;
using System.Collections.Generic;
using UnityEngine;

public class LobbyManager : MonoBehaviour
{
    public static LobbyManager Instance;
    public string CurrentLobbyCode;
    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    public string GenerateUniqueLobbyCode()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        Debug.Log("finding unique lobby code");
        string code;
        do
        {
            code = "";
            for (int i = 0; i < 8; i++)
                code += chars[Random.Range(0, chars.Length)];
        } while (code == CurrentLobbyCode); 
        Debug.Log("found unique lobby code");
        return code;
    }
}
