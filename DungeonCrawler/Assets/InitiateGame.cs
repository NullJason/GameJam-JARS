using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InitiateGame : MonoBehaviour
{
    // TODO..   known bug: getting a random map sometimes returns null within THIS script...
    float GameStartDelay; // delay after pressing start. This is initially set at equal to particle2 duration.
    Button StartButton; // start button.
    GameObject StartBackground; // bg at start screen
    GameObject PlayGround; // area where everything is loaded into.
    GameObject PlayGroundEnemies; // enemies currently on the map the player is on.
    GameObject Player;
    Transform Particle1; // the particles you see on the start menu.
    Transform Particle2; // the particles you see after pressing start button.
    GameObject MapFolder; // folder containing mapdata/structure.
    GameObject CurrentMap = null;
    static int EnemyCount = 4; // could customize eneemy count and other stats for each level using a dictionary instead once we have the details.
    static int EnemySpawnRate = 5; // ESR in seconds.
    bool GameStarted = false; // mark game started

    // bool PlayerAtExit = false; // if we implement level/stage clear exits.
    
    // Note: GameObject.Find(string) does not work with inactive objects.
    void Start()
    {
        // init values.
        Player = GameObject.Find("Player");
        MapFolder = GameObject.Find("Maps");
        PlayGround = GameObject.Find("Playground");
        PlayGroundEnemies = PlayGround.transform.Find("Enemies").gameObject;
        Debug.Log(PlayGroundEnemies.name);
        StartBackground = GameObject.Find("DefaultBackground");
        Transform StartText = StartBackground.transform.Find("StartCanvas").Find("StartText");
        StartButton = StartText.GetComponent<Button>();
        Particle1 = StartBackground.transform.Find("NormalParticle");
        Particle2 = StartBackground.transform.Find("StartParticle");
        GameStartDelay = Particle2.GetComponent<ParticleSystem>().main.duration;

        // init visibility.
        Player.SetActive(false); // player invisible at start.
        MapFolder.SetActive(false); // this is the folder for map templates.

        // init start button
        StartButton.onClick.AddListener(DoDelayedStartAction);
    }
    void DoDelayedStartAction(){
        if (GameStarted) return;
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
        GameStarted = true;
    }
    void StartGame(){
        Player.transform.position = new Vector3(0,0);
        Player.SetActive(true);
        PlayGround.SetActive(true);

        // add map names to randomizer
        int childCount = MapFolder.transform.childCount;
        string[] MapNames = new string[childCount];
        for (int i = 0; i < childCount; i++)
        {
            MapNames[i] = MapFolder.transform.GetChild(i).name;
        }
        foreach (string map in MapNames){
            // Add maps to the luck table
            Debug.Log(map);
            WeightedLuckManager.Instance.Append("Map", map, 10); // placeholder, no set chance for maps yet.
        }
        CreateNewMap();
    }
    // Get a random map and add to playground.
    void CreateNewMap(){
        string mapName = WeightedLuckManager.Instance.Get("Map"); if (mapName == null){return;}
        GameObject MapClone = Instantiate(MapFolder.transform.Find(mapName).gameObject);
        Transform SpawnPointsFolder = MapClone.transform.Find("SpawnPoints");
        Queue<Vector3> SpawnPoints = new Queue<Vector3>();

        if(CurrentMap != null) { Destroy(CurrentMap);}
        CurrentMap = MapClone;

        foreach(Transform sp in SpawnPointsFolder){
            SpawnPoints.Enqueue(sp.position);
        }

        Transform EnemyFolder = MapClone.transform.Find("Enemies");
        int RngEnemyCount = EnemyFolder.childCount;
        string[] ENames = new string[RngEnemyCount];

        // get the names of valid entities for this map.
        for (int i = 0; i < RngEnemyCount; i++)
        {
            ENames[i] = EnemyFolder.GetChild(i).name;
        }
        // Add the map's enemies to the luck table
        foreach (string entityName in ENames){
            WeightedLuckManager.Instance.Append("Entity", entityName, 10); // placeholder luck value 10.
        }
        // append random enemies to a queue.
        Queue<GameObject> EnemyQueue = new Queue<GameObject>();
        for (int i = 0; i < EnemyCount; i++){
            string EName = WeightedLuckManager.Instance.Get("Entity");
            GameObject Enemy = Instantiate(EnemyFolder.Find(EName).gameObject);
            EnemyQueue.Enqueue(Enemy);
            Enemy.transform.SetParent(PlayGroundEnemies.transform); 
            Enemy.SetActive(false);
        }
        // clear entity table to prevent conflicts with other map enemy indexes. spamming createnewmap() will cause this to throw an error somewhere when Get()-ing.
        // WeightedLuckManager.Instance.Clear("Entity");
        
        // init new map, set the new map's entity index visibility to false.
        EnemyFolder.gameObject.SetActive(false);
        MapClone.SetActive(true);
        MapClone.transform.SetParent(PlayGround.transform);
        // start spawning entities.
        StartDelayedEntitySpawner(EnemyQueue, SpawnPoints);
    }
    void StartDelayedEntitySpawner(Queue<GameObject> queue, Queue<Vector3> sp){
        StartCoroutine(DelayedSpawnEntity(queue, sp));
    }
    IEnumerator DelayedSpawnEntity(Queue<GameObject> queue, Queue<Vector3> sp){
        while(sp.Count>0){
            WeightedLuckManager.Instance.Append("EntitySpawnPos",sp.Dequeue().ToString(), 1); // each spawn pos has even chance.
        }
        
        while(queue.Count>0){
            yield return new WaitForSeconds(EnemySpawnRate);
            SpawnEnemy(queue.Dequeue(), StringToVector3(WeightedLuckManager.Instance.Get("EntitySpawnPos")));
        }
    }
    Vector3 StringToVector3(string str) // if i allow any type as key in luck table i wouldn't need this conversion.
    {
        // assumes string is from vector3.tostring
        str = str.Trim(new char[] { '(', ')' });
        string[] values = str.Split(',');
        float x = float.Parse(values[0]);
        float y = float.Parse(values[1]);
        float z = float.Parse(values[2]);

        return new Vector3(x, y, z);
    }
    void SpawnEnemy(GameObject entity, Vector3 spawn){
        entity.transform.localScale = new Vector3(1,1);
        entity.transform.position = spawn;
        entity.SetActive(true);
    }
    // check when to create a new map. currently checks when there are no enemies on playground.
    void CheckClearStatus(){
        if(PlayGroundEnemies.transform.childCount == 0){
           CreateNewMap();
           Debug.Log("this should only print ONCE per clear.");
        }
    }
    private void FixedUpdate() {
        if (GameStarted){
            CheckClearStatus();
        }
    }
    
}
