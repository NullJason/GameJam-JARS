using UnityEngine;
using System.Collections.Generic;

//TODO: When things to clean are created, increment howManyToClean! When they are destroyed, decrement howManyToClean!

public class GameplayManager : MonoBehaviour
{
  private SaveData saveFile;
  private SaveDataHandler saveHandler;

  private List<Spawner> spawners;
  private int wave = 1;
  private int howManyLeftInWave;
  private bool gameIsActive = false; //TODO!
  [SerializeField] GameObject zombie;
  [SerializeField] int spawnTimerReset = 300; //How long it takes for the Manager to summon a new set of zombies.
  int spawnTimer;
  private int howManyToClean = 0;
  private int howManyToCleanOnScreen = 0;
  [SerializeField] int waveTimerReset = 300; //How long it takes for a new wave to begin after finishing a wave.
  int waveTimer;
  public static GameplayManager main;
  public static GameObject hunter;
  void Awake(){
    if(main == null){
      main = this;
      DontDestroyOnLoad(this);
    }
    spawners = new List<Spawner>();
    saveHandler = new SaveDataHandler(Application.persistentDataPath, "save"); //TODO: Remove magic numbers!
    waveTimer = waveTimerReset;
  }
  void Update()
  {
    Debug.Log("Wave: " + wave + "\nZombies left: "+ howManyLeftInWave + "\nTotal cleaning objects: " + howManyToClean + "\nOnscreen cleaning objects: " + howManyToCleanOnScreen);
    if(gameIsActive){
      TrySpawn();
      if(howManyLeftInWave == 0 && howManyToClean == 0){
        waveTimer--;
        if(waveTimer == 0) StartNextWave();
      }
    }
  }
  void TrySpawn(){
    spawnTimer--;
    if(spawnTimer <= 0){
      Spawn(wave, zombie);
      spawnTimer = spawnTimerReset;
    }
  }
  void Spawn(int limit, GameObject zombie){
    if(limit > howManyLeftInWave) limit = howManyLeftInWave;
    if(howManyToCleanOnScreen > 15 + 5 * wave) limit = 0;
    int howMany = Random.Range(0, limit + 1);
    Spawner whichOne = spawners[Random.Range(0, spawners.Count)];
    whichOne.Spawn(zombie, howMany);
    howManyLeftInWave -= howMany;
  }
  public void AddSpawner(Spawner spawner){
    spawners.Add(spawner);
  }

  public void SetGameActive(bool active){
    gameIsActive = active;
  }

  public void StartNextWave(){
    wave++;
    SetUpWave();
    SaveGame();
//    Debug.Log("Wave: " + wave);
  }

  //Meant to be called at the start of a wave.
  //TODO: Rebalance?
  public void SetUpWave(){
    howManyLeftInWave = 10 + (wave * wave);
    //TODO: Increase player speed?
    waveTimer = waveTimerReset;
    howManyToClean = zombie.GetComponent<Zombie>().HowManyDroppedTotal() * howManyLeftInWave; //TODO: Remove magic numbers, as well as unreliable formula!
  }

  public void LoadGame(){
    saveFile = saveHandler.Load();
    if(saveFile == null) saveFile = new SaveData();
    wave = saveFile.wave;
    //TODO: Set Player's health to saveFile.health!
  }

  public void SaveGame(){
//    saveFile.health = //TODO: Get player's health!
    saveFile.wave = wave;
    saveHandler.Save(saveFile);
  }

  public void ResetSave(){
    saveFile = new SaveData();
    SaveGame();
  }

  public bool SaveExists(){
    return saveHandler.Load().wave != 1; //TODO: Remove magic numbers!
  }
  //Meant to be called by a DirtyObject when cleaned.
  public void Clean(){
    howManyToClean--;
    howManyToCleanOnScreen--;
  }

  public void AddToCleanOnScreen(){
    howManyToCleanOnScreen++;
  }
}
