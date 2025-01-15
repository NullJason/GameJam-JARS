using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InitiateGame : MonoBehaviour
{
    float GameStartDelay; // delay after pressing start. This is initially set at equal to particle2 duration.
    Button StartButton; // start button.
    GameObject StartBackground; // bg at start screen
    GameObject PlayGround; // area where everything is loaded into.
    GameObject Player;
    Transform Particle1; // the particles you see on the start menu.
    Transform Particle2; // the particles you see after pressing start button.
    GameObject MapFolder; // folder containing mapdata/structure.
    private bool GameStarting = false;

    void Start()
    {
        // init values.
        Player = GameObject.Find("Player");
        MapFolder = GameObject.Find("Maps");
        PlayGround = GameObject.Find("Playground");
        StartBackground = GameObject.Find("DefaultBackground");
        Transform StartText = StartBackground.transform.Find("StartCanvas").Find("StartText");
        StartButton = StartText.GetComponent<Button>();
        Particle1 = StartBackground.transform.Find("NormalParticle");
        Particle2 = StartBackground.transform.Find("StartParticle");
        GameStartDelay = Particle2.GetComponent<ParticleSystem>().main.duration;

        GameObject obbg = Instantiate(Resources.Load<GameObject>("MapResource/OutOfBoundsBg"));
        obbg.transform.SetParent(PlayGround.transform);
        obbg.SetActive(true);

        // add map names to randomizer
        foreach(Transform room in MapFolder.transform){
            Debug.Log(room.name);
            WeightedLuckManager.Instance.Append("Map", room.name, 1); // placeholder, no set chance for maps yet.
        }

        // init visibility.
        Player.SetActive(false); // player invisible at start.
        MapFolder.SetActive(false); // this is the folder for map templates.

        // init start button
        StartButton.onClick.AddListener(DoDelayedStartAction);
    }
    void DoDelayedStartAction(){
        if (GameStarting || MapManager.Instance.GameStarted) return;
        GameStarting = true;
        Particle1.gameObject.SetActive(false);
        Particle2.gameObject.SetActive(true);
        StartCoroutine(DelayedStartAction(GameStartDelay));
    }
    IEnumerator DelayedStartAction(float delay){
        yield return new WaitForSeconds(delay);
        StartButton.gameObject.SetActive(false);
        StartBackground.SetActive(false);
        Particle2.gameObject.SetActive(false);
        StartGame();
        MapManager.Instance.UpdateGameStartedStatus(true);
    }
    void StartGame(){
        Player.transform.position = new Vector3(0,0);
        Player.SetActive(true);
        PlayGround.SetActive(true);
        MapManager.Instance.StartMapCreation();
        MapManager.Instance.UpdateProgressText();
    }
}