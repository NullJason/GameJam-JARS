using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapManager : MonoBehaviour
{
    // Unity Singleton instance.
    public static MapManager Instance { get; private set; }
    private void Awake()
    {
        // Check if there is already an instance of this class
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Destroy duplicate
            return;
        }

        Instance = this; // Set the instance
        DontDestroyOnLoad(gameObject); // Make persistent across scenes

        // init values
        Player = GameObject.Find("Player");
        MapFolder = GameObject.Find("Maps");
        PlayGround = GameObject.Find("Playground");
        PlayGroundEnemies = PlayGround.transform.Find("Enemies").gameObject;
        PlayGroundPaths = PlayGround.transform.Find("Paths");
        PlayGroundStageExit = PlayGroundPaths.Find("StageExit");
        EntranceObj = Resources.Load<GameObject>("MapResource/Entrance");
        ExitObj = Resources.Load<GameObject>("MapResource/Exit");
        MapProgressText = PlayGround.transform.Find("Progression").GetChild(0).GetComponent<Text>();
    }
    private MapManager(){}
    GameObject PlayGround; // area where everything is loaded into.
    GameObject PlayGroundEnemies; // enemies currently on the map the player is on.
    GameObject Player;
    GameObject MapFolder; // folder containing mapdata/structure.
    GameObject CurrentMap = null;
    Dictionary<GameObject, LevelPath> PathWays = new Dictionary<GameObject, LevelPath>();
    Transform PlayGroundPaths;
    Transform PlayGroundStageExit;
    GameObject EntranceObj;
    GameObject ExitObj;
    Text MapProgressText;
    string LastExitUsed;
    int PlrToPathOffset = 1;
    int TotalRoomsGenerated = 0;
    int NumOfStages = 3;
    int MinRooms = 6;
    int MaxRooms = 8;
    int RoomsCleared = 0; // current stage rooms cleared
    int StagesCleared = 0; // current stages cleared;
    int NumOfRoomsInStage = 7; // current num of rooms in this stage of the current map
    static int EnemyCount = 1;//4; // 1 is easier to test with.
    static int EnemySpawnRate = 5;
    bool BossDefeated = false;
    bool BossSpawned = false; // prevent boss spawning multiple times.
    GameObject BossRoom = null;
    public bool GameStarted = false;
    public void UpdateProgressText(){
        MapProgressText.text = "Map: "+CurrentMap.name.Replace("(Clone)", "").Trim()+"\nFloor: "+(StagesCleared+1)+"\nRoomsCleared: "+RoomsCleared;
    }
    string ProcessMapStatus(){
        if (CurrentMap == null) {return WeightedLuckManager.Instance.Get("Map");}
        // 1: Stage cleared, 2: Map Cleared, 3: Room cleared.
        if(NumOfRoomsInStage <= RoomsCleared || BossSpawned) {
            // TODO.. set Random stage within map.
            NumOfRoomsInStage = Random.Range(MinRooms,MaxRooms);
            RoomsCleared = 0;
            StagesCleared += 1;
            TotalRoomsGenerated = 0;
            BossSpawned = false;
            DestroyStage();
            // TODO.. set new stage type?
            CurrentMap = null;
            return WeightedLuckManager.Instance.Get("Map"); //CurrentMap.name.Replace("(Clone)", "").Trim(); // should also return same as room clear
        }
        else if(NumOfStages <= StagesCleared) { // gonna have to put increased difficulty number for each map cleared.
            StagesCleared = 0;
            RoomsCleared = 0;
            return WeightedLuckManager.Instance.Get("Map");
        }
        else{
            RoomsCleared += 1;
            return CurrentMap.name.Replace("(Clone)", "").Trim(); // TODO.. return a random room from a selected stage within the map.
        }
    }
    void DestroyStage(){
        // destroy every room from previous stage.
        foreach(GameObject obj in PathWays.Keys){
            Destroy(obj);
        }
        // destroy the path objects and refereneces.
        foreach(LevelPath path in PathWays.Values){
            path.Destroy();
        }
        PathWays.Clear(); // clear the references.
    }
    // Get a random room and add to playground. 
    public void CreateNextArea(){
        string RoomName = ProcessMapStatus(); if (RoomName == null){return;}
        GameObject RoomClone = Instantiate(MapFolder.transform.Find(RoomName).gameObject); // when going thro exit, returns null error
        Transform SpawnPointsFolder = RoomClone.transform.Find("SpawnPoints");
        Transform PathPositions = RoomClone.transform.Find("PathPos");
        Transform BossFolder = RoomClone.transform.Find("Boss");
        Queue<Vector3> SpawnPoints = new Queue<Vector3>();
        GameObject PreviousRoom = CurrentMap;
        PathWays.Add(RoomClone, new LevelPath());

        string[] RoomPathDirections = new string[4]{"Left","Up","Right","Down"};
        foreach(string dir in RoomPathDirections){
            WeightedLuckManager.Instance.Append("MapPath", dir, 1);
        }

        // if a previous room exists, use the Player's exit pos to set the new room's entrance pos. 
        if(PreviousRoom != null) { 
            PreviousRoom.SetActive(false); 
            string EntranceDir = LevelPath.GetReversedDirection(LastExitUsed);
            if (EntranceDir == null) {Debug.LogError("EntranceDirection is null"); return;}
            WeightedLuckManager.Instance.Remove("MapPath", EntranceDir); // prevent exit spawning at entrance.

            // create entrance.
            Transform PosOfPath = PathPositions.Find(EntranceDir);
            GameObject NewPath = Instantiate(EntranceObj);
            NewPath.transform.position = PosOfPath.transform.position;
            NewPath.transform.rotation = PosOfPath.transform.rotation;
            NewPath.transform.SetParent(PlayGroundPaths.Find(EntranceDir), true);
            NewPath.SetActive(true);

            PathWays[RoomClone].SetEntrance(EntranceDir, PreviousRoom);
            PathWays[RoomClone].AddPathObj(EntranceDir, NewPath);
        }

        // prevent generating exits if room is last in stage. Current logic makes the first dead-end room generated - always the boss room.
        int NumExits = Random.Range(1,4);
        if (TotalRoomsGenerated + 1 >= NumOfRoomsInStage) {
            NumExits = 0;
            // prevent boss spawning at every dead-end room.
            if (!BossSpawned) {
                BossSpawned = true;
                Transform boss = BossFolder.GetChild(0);
                boss.gameObject.SetActive(false);
                boss.SetParent(PlayGroundEnemies.transform);
                StartSpawnBoss(boss);

                // the below code will be replaced when BossDied() is implemented by boss entity hp.
                GameObject NewPath = Instantiate(ExitObj); NewPath.transform.localScale *= 2;
                NewPath.transform.position = new Vector3(0,0);
                NewPath.transform.SetParent(PlayGroundStageExit, true);
                NewPath.SetActive(true);

                BossRoom = RoomClone;
            }
        }

        // create the exits.
        for(int i=0; i<NumExits; i++){
            TotalRoomsGenerated += 1; // each exit is another room.
            string Direction = WeightedLuckManager.Instance.Get("MapPath"); if (Direction == null) {Debug.LogError("Direction null"); return;}
            WeightedLuckManager.Instance.Remove("MapPath", Direction); // no stacking exits lmao.
            Transform PosOfPath = PathPositions.Find(Direction);
            GameObject NewPath = Instantiate(ExitObj);
            NewPath.transform.position = PosOfPath.transform.position;
            NewPath.transform.rotation = PosOfPath.transform.rotation;
            NewPath.transform.SetParent(PlayGroundPaths.Find(Direction), true);
            NewPath.SetActive(true);
            PathWays[RoomClone].AddPathObj(Direction, NewPath);
        }

        // queue the enemy SPs.
        foreach(Transform sp in SpawnPointsFolder){
            SpawnPoints.Enqueue(sp.position);
        }

        // get room's enemy data.
        Transform EnemyFolder = RoomClone.transform.Find("Enemies");
        int RngEnemyCount = EnemyFolder.childCount;
        string[] ENames = new string[RngEnemyCount];

        // get the names of valid entities for this room.
        for (int i = 0; i < RngEnemyCount; i++)
        {
            ENames[i] = EnemyFolder.GetChild(i).name;
        }

        // Add the room's enemies to the luck table
        foreach (string entityName in ENames){
            WeightedLuckManager.Instance.Append("Entity", entityName, 10); // placeholder luck value 10.
        }

        // append random enemies to a queue.
        Queue<GameObject> EnemyQueue = new Queue<GameObject>();
        for (int i = 0; i < EnemyCount; i++){
            string EName = WeightedLuckManager.Instance.Get("Entity");
            GameObject EnemyOrigin = EnemyFolder.Find(EName).gameObject;
            GameObject EnemyClone = Instantiate(EnemyOrigin);
            EnemyQueue.Enqueue(EnemyClone);
            Vector3 NormalScale = EnemyOrigin.transform.localToWorldMatrix.lossyScale; // get world scale

            EnemyClone.transform.SetParent(PlayGroundEnemies.transform);
            EnemyClone.transform.localScale = NormalScale;
            SpriteRenderer sr = EnemyClone.GetComponent<SpriteRenderer>();
            sr.sortingLayerID = SortingLayer.NameToID("Entity");
            EnemyClone.SetActive(false);
        }

        // clear table to prevent randomizer conflicts with other rooms.
        WeightedLuckManager.Instance.Clear("Entity");
        WeightedLuckManager.Instance.Clear("MapPath"); 
        
        EnemyFolder.gameObject.SetActive(false);
        BossFolder.gameObject.SetActive(false);
        PathPositions.gameObject.SetActive(false);
        SetCurrentRoom(RoomClone);
        StartDelayedEntitySpawner(EnemyQueue, SpawnPoints);
    }
    void StartSpawnBoss(Transform boss){
        StartCoroutine(DelayedBossSpawner(boss));
    }
    IEnumerator DelayedBossSpawner(Transform boss){
        yield return new WaitForSeconds(1); // or use boss anim time.
        SpawnEnemy(boss.gameObject, new Vector3(0,0));
    }
    void StartDelayedEntitySpawner(Queue<GameObject> queue, Queue<Vector3> sp){
        StartCoroutine(DelayedSpawnEntity(queue, sp));
    }
    IEnumerator DelayedSpawnEntity(Queue<GameObject> queue, Queue<Vector3> sp){
        while(sp.Count>0){
            WeightedLuckManager.Instance.Append("EntitySpawnPos",sp.Dequeue().ToString(), 1);
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
        entity.transform.position = spawn;
        entity.SetActive(true);
    }
    // clear condition.
    bool CheckClearStatus(){
        if(PlayGroundEnemies.transform.childCount == 0){
            return true;
        }
        return false;
    }
    // If you want to connect your boss behavior script's HP to this.
    public void BossDied(){
        BossDefeated = true;
        GameObject NewPath = Instantiate(ExitObj); NewPath.transform.localScale *= 2;
        NewPath.transform.position = new Vector3(0,0);
        NewPath.transform.SetParent(PlayGroundStageExit, true);
        NewPath.SetActive(true);
    }

    // CurrentMap == clear && Goes through entrance -> changeMap, exit -> create new map
    public void OnPlrCollided(Collider2D other) {
        if(CurrentMap==null) return;
        if (GameStarted){
            bool IsEntrance = other.gameObject.CompareTag("RoomEntrance");
            bool IsExit = other.gameObject.CompareTag("RoomExit");
            if(!IsEntrance && !IsExit) return;

            bool ClearStatus = CheckClearStatus();
            if (ClearStatus == false) return;

            // if last exit.
            if (other.transform.parent.Equals(PlayGroundStageExit)){
                LastExitUsed = null;
                Destroy(other.gameObject);
                CreateNextArea();
                return;
            }

            string PathDirection = other.transform.parent.name; // exit or entrance's parent should be a empty obj with a direction as the name that is child of PathPos.

            if(IsEntrance){
                SetCurrentRoom(PathWays[CurrentMap].GetEntranceRoom());
            }
            else if(IsExit){
                // check if this exit has a map associated to its path, else, create new room
                GameObject MapConnectedToExit = PathWays[CurrentMap].Get(PathDirection);
                if(MapConnectedToExit != null){
                    SetCurrentRoom(MapConnectedToExit);
                }
                else{
                    LastExitUsed = PathDirection; // so that CNA() knows where the exit in the old room is in direction.
                    GameObject PreviousRoom = CurrentMap;
                    CreateNextArea(); // changes Current
                    PathWays[PreviousRoom].Add(PathDirection, CurrentMap); // connect the old.r exit to the new.r entrance
                }
            }
            
            Vector3 NewPlayerPos = GetPlrPosByDirection(PathDirection, PathWays[CurrentMap].GetPathObj(LevelPath.GetReversedDirection(PathDirection)));
            Player.transform.position = NewPlayerPos; // reset the plr's position to prevent calling this method rapidly. i COULD use a ienum delay.
        }
    }
    Vector3 GetPlrPosByDirection(string dir, GameObject PathObj){
        if (PathObj == null || dir == null) return new Vector3(0,0);
        Vector3 entrancePosition = PathObj.transform.position;
        Vector2 entranceSize = PathObj.GetComponent<SpriteRenderer>().bounds.size; 
        
        Vector2 playerSize = Player.GetComponent<SpriteRenderer>().bounds.size;

        Vector3 newPlayerPosition = entrancePosition;

        switch (dir)
        {
            case "Left":
                newPlayerPosition.x -= (entranceSize.x / 2 + playerSize.x / 2 + PlrToPathOffset);
                break;

            case "Right":
                newPlayerPosition.x += (entranceSize.x / 2 + playerSize.x / 2 + PlrToPathOffset);
                break;

            case "Up":
                newPlayerPosition.y += (entranceSize.y / 2 + playerSize.y / 2 + PlrToPathOffset);
                break;

            case "Down":
                newPlayerPosition.y -= (entranceSize.y / 2 + playerSize.y / 2 + PlrToPathOffset);
                break;

            default:
                newPlayerPosition = new Vector3(0,0);
                break;
        }
        return newPlayerPosition;
    }
    void SetCurrentRoom(GameObject room){
        if (room == null) {Debug.LogError("Cannot Set Room for it's null."); return;}
        GameObject PreviousRoom = CurrentMap;
        CurrentMap = room;
        CurrentMap.transform.SetParent(PlayGround.transform);
        if(PreviousRoom != null) {PreviousRoom.SetActive(false); PathWays[PreviousRoom].SetActive(false);}
        CurrentMap.SetActive(true);
        PathWays[CurrentMap].SetActive(true);
        UpdateProgressText();

        if(BossRoom != null && CurrentMap.Equals(BossRoom)){
            PlayGroundStageExit.gameObject.SetActive(true);
        }else{PlayGroundStageExit.gameObject.SetActive(false);}
    }

    // Util for other scripts.
    public void UpdateGameStartedStatus(bool s){
        GameStarted = s;
    }
    public void StartMapCreation(){
        CreateNextArea();
    }
}
class LevelPath{
    // these should all point to another room if not null. Basically linked list in each directions.
    private Dictionary<string, GameObject> DirToPath = new Dictionary<string, GameObject>(); // key = direction, value = room.
    private Dictionary<string, GameObject> DirToPathObj = new Dictionary<string, GameObject>(); // key = dir, value = pathObj.
    private string Entrance = null;
    private int RoomNumber;
    public LevelPath(){
        DirToPath.Add("Left", null);
        DirToPath.Add("Up", null);
        DirToPath.Add("Right", null);
        DirToPath.Add("Down", null);

        DirToPathObj.Add("Left", null);
        DirToPathObj.Add("Up", null);
        DirToPathObj.Add("Right", null);
        DirToPathObj.Add("Down", null);
    }
    public void Add(string dir, GameObject obj){
        DirToPath[dir] = obj;
    }
    public GameObject Get(string dir){
        return DirToPath[dir];
    }
    public void Destroy(){
        foreach(GameObject pathObj in DirToPathObj.Values){
            GameObject.Destroy(pathObj);
        }
        DirToPath.Clear();
        DirToPathObj.Clear();
    }
    public void AddPathObj(string dir, GameObject path){
        DirToPathObj[dir] = path;
    }
    public GameObject GetPathObj(string dir){
        return DirToPathObj[dir];
    }
    // method if don't want to rely on parent name. more efficient to have a reversed dict? Dictionary<int, string> reverseDictionary = myDictionary.ToDictionary(pair => pair.Value, pair => pair.Key);
    public string GetPathObjDirection(GameObject path){
        foreach(var pair in DirToPathObj){
            if(pair.Value.Equals(path)) return pair.Key;
        }
        return null;
    }
    public void SetActive(bool s){
        foreach(GameObject po in DirToPathObj.Values){
            if(po!=null) po.SetActive(s);
        }
    }
    public bool Contains(string dir){
        return DirToPath[dir] != null;
    }
    public GameObject GetEntranceRoom(){
        return Get(Entrance);
    }
    public static string GetReversedDirection(string dir){
        if(dir.Equals("Left")) return "Right";
        else if (dir.Equals("Right")) return "Left";
        else if (dir.Equals("Up")) return "Down";
        else return "Up";
    }
    public void SetEntrance(string dir, GameObject room){
        Entrance = dir;
        Add(dir, room);
    }
    public bool IsNew(){
        foreach(GameObject nextMap in DirToPath.Values){
            if (nextMap != null) return false;
        } return true;
    }
}