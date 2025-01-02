using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InitiateGame : MonoBehaviour
{
    float GameStartDelay;
    Button StartButton;
    GameObject StartBackground;
    GameObject PlayGround;
    GameObject Player;
    Transform Particle1;
    Transform Particle2;
    GameObject MapFolder;
    Boolean GameStarted = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // GO.Find(string) does not work with inactive objects.
        Player = GameObject.Find("Player");
        MapFolder = GameObject.Find("Maps");
        PlayGround = GameObject.Find("Playground");
        StartBackground = GameObject.Find("DefaultBackground");
        StartButton = transform.GetComponent<Button>();
        Particle1 = StartBackground.transform.Find("NormalParticle");
        Particle2 = StartBackground.transform.Find("StartParticle");
        GameStartDelay = Particle2.GetComponent<ParticleSystem>().main.duration;

        Player.SetActive(false); // player invisible at start.
        MapFolder.SetActive(false); // this is the folder for map templates.
        StartButton.onClick.AddListener(DoDelayedAction);
    }
    void DoDelayedAction(){
        if (GameStarted) return;
        GameStarted = true;
        Particle1.gameObject.SetActive(false);
        Particle2.gameObject.SetActive(true);
        StartCoroutine(DelayAction(GameStartDelay));
    }
    IEnumerator DelayAction(float delay){
        yield return new WaitForSeconds(delay);
        StartButton.gameObject.SetActive(false);
        StartBackground.SetActive(false);
        Particle2.gameObject.SetActive(false);
        StartGame();
    }
    void StartGame(){
        Player.transform.position = new Vector3(0,0);
        Player.SetActive(true);
        PlayGround.SetActive(true);

        int childCount = MapFolder.transform.childCount;
        string[] MapNames = new string[childCount];
        for (int i = 0; i < childCount; i++)
        {
            MapNames[i] = MapFolder.transform.GetChild(i).name;
        }
        foreach (string map in MapNames){
            // Add maps to the luck table
            WeightedLuckManager.Instance.Append("Map", map, 10); // placeholder, no set chance for maps yet.
        }
        
        // Get a random map
        string mapName = WeightedLuckManager.Instance.Get("Map");
        Debug.Log("Random map: " + mapName);
        GameObject MapClone = Instantiate(MapFolder.transform.Find(mapName).gameObject);
        MapClone.SetActive(true);
        MapClone.transform.SetParent(PlayGround.transform);
    }
}
