using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System;

//TODO: When things to clean are created, increment howManyToClean! When they are destroyed, decrement howManyToClean!

public class GameplayManager : MonoBehaviour
{
  private SaveData saveFile;
  private SaveDataHandler saveHandler;

  private List<Spawner> spawners;
  //private int wave = 1;
  private int howManyLeftInWave; //remaining number of zombies that will be spawned as part of this wave.
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
  //bool music;
  //bool sfx;
  //float lastKnownPlayerHealth = 100;
//  public Settings settings;
  void Awake(){
    if(main == null){
      main = this;
      DontDestroyOnLoad(this);
    }
    spawners = new List<Spawner>();
    saveHandler = new SaveDataHandler(Application.persistentDataPath, "save"); //TODO: Remove magic numbers!
    waveTimer = waveTimerReset;
    LoadGame();
  }
  void Update()
  {
    Debug.Log("Wave: " + GetWave() + "\nZombies left: "+ howManyLeftInWave + "\nTotal cleaning objects: " + howManyToClean + "\nOnscreen cleaning objects: " + howManyToCleanOnScreen);
    if(gameIsActive){
      TrySpawn();
      if(howManyLeftInWave == 0 && howManyToClean == 0){
        Debug.Log("=D");
        waveTimer--;
        if(waveTimer == 0) StartNextWave();
      }
    }
    Debug.Log("Sound: \n music=" + GetMusic() + ", \n sfx=" + GetSfx());
  }
  void TrySpawn(){
    spawnTimer--;
    if(spawnTimer <= 0){
      Spawn(GetWave(), zombie);
      spawnTimer = spawnTimerReset;
    }
  }
  void Spawn(int limit, GameObject zombie){
    if(limit > howManyLeftInWave) limit = howManyLeftInWave;
    if(howManyToCleanOnScreen > 15 + 5 * GetWave()) limit = 0;
    int howMany = UnityEngine.Random.Range(0, limit + 1);
    Spawner whichOne = spawners[UnityEngine.Random.Range(0, spawners.Count)];
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
    SetWavePlus(1);
    SetUpWave();
    SpeechHandler.Instance.AcceptNew("Bob", "Wave Complete!\nWave: " + GetWave() + "\n  The game has been saved.");
    SaveGame();
  }

  //Meant to be called at the start of a wave.
  //TODO: Rebalance?
  public void SetUpWave(){
    howManyLeftInWave = 10 + (GetWave() * GetWave());
    //TODO: Increase player speed?
    waveTimer = waveTimerReset;
    howManyToClean = zombie.GetComponent<Zombie>().HowManyDroppedTotal() * howManyLeftInWave; //TODO: Remove magic numbers, as well as unreliable formula!
  }

  //Loads the saveFile stored in this class from the saveFile stored on the user's computer.
  public void LoadGame(){
    saveFile = saveHandler.Load();
    if(saveFile == null) saveFile = new SaveData();
//    wave = saveFile.wave;
//    music = saveFile.music;
//    sfx = saveFile.sfx;
//    lastKnownPlayerHealth = saveFile.health;
    //TODO: Set Player's health to saveFile.health!
  }

  //Loads the saveFile stored in this class onto the saveFile stored on the user's computer.
  public void SaveGame(){
    if(hunter != null) saveFile.health = GetPlayerHealth();
//    else saveFile.health = lastKnownPlayerHealth;
//    saveFile.wave = wave;
//    saveFile.music = music;
//    saveFile.sfx = sfx;
    saveHandler.Save(saveFile);
  }

  //(Apparently) meant to be called when the user starts a new game.
  //Resets the player's health and the wave number, and saves the game.
  //Apparently, leaves the music and sound effects unchanged.
  public void ResetSave(){
//    bool tempSfx = sfx;
//    bool tempMusic = music;
    SetWave(1);
    saveFile.health = 100; //TODO:Remove magic numbers!
//    saveFile.music = music;
//    saveFile.sfx = sfx;
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
  //Returns how many DirtyObjects exist on screen at the moment.
  public float GetVisibleCleanables(){
    return howManyToCleanOnScreen;
  }
  //Returns how many DirtyObjects must been cleaned to complete the wave.
  public float GetCleanables(){
    return howManyToClean;
  }
  public int GetWave(){
    return saveFile.wave;
  }
  private void SetWave(int i){
    saveFile.wave = i;
  }
  private void SetWavePlus(int i = 1){
    SetWave(GetWave() + i);
  }
  public bool GetMusic(){
    return saveFile.music;
  }
  public bool GetSfx(){
    return saveFile.sfx;
  }

  //Setter method for music and sfx.
  public void SetSettings(bool music, bool sfx){
    saveFile.sfx = sfx;
    saveFile.music = music;
  }

  //Returns the health of the player at the last save.
  public float SavedHealth(){
    return saveFile.health;
  }
  //Returns the player's hp. TODO: Make sure that this works properly, particularly when multiplayer rolls around!
  public float GetPlayerHealth(){
    return hunter.GetComponent<Hunter>().GetPlayer().GetComponent<Player>().GetHp();
  }

  public int GetZombiesLeftInWave(){
    return howManyLeftInWave;
  }
  //Returns the number of points or score currently accumulated between games.
  public int GetPoints(){
    return saveFile.accumulatedPoints;
  }

  //TODO: Maybe make this public, or add more limited setters?
  private void SetPoints(int i){
    saveFile.accumulatedPoints += i;
  }

  //Returns the number of points or score currently accumulated in this game.
  public int GetMatchPoints(){
    return saveFile.points;
  }

  //Attempts to remove a certain number of points.
  //If the player does not have enough points, does nothing to points.
  //If the player does have enough points, subtracts that many points from the player.
  //In either case, returns the number of points the player has minus the cost.
  //  Note that a negative score indicates that the transaction failed.
  //  Note that a positive score indicates that the transaction succeeded, and represents the remaining points.
  //TODO: Test!
  public int TryRemovePoints(int cost){
    int result = GetPoints() - cost;
    if(result >= 0) SetPoints(result);
    return result;
  }

  //To be called on the death of the player.
  public void OnDeath(){
    SceneManager.LoadScene("Death Screen");
    ShowCursor();
  }

  //Basic getter for checking whether a certain Tool has been unlocked in the current save data.
  public bool CheckToolUnlocked(Tool tool){
    return saveFile.items.Contains(tool);
  }

  //Adds a tool to the set of tools possessed, and saves the data.
  //Will not throw an error if the tool is already locked.
  //When implementing a tool shop, this should not be preferred! Instead, use TryUnlockTool!
  public void ForceUnlockTool(Tool tool){
    saveFile.items.Add(tool);
    SaveGame();
  }

  //TODO
  public bool TryUnlockTool(Tool tool, int cost){
    if(CheckToolUnlocked(tool)){
      throw new ToolAlreadyOwnedException("Tool already unlocked, cannot unlock again!");
    }
    if(TryRemovePoints(cost) < 0){
      return false;
    }
    ForceUnlockTool(tool);
    return true;
  }
  private class ToolAlreadyOwnedException : Exception{
    public ToolAlreadyOwnedException(String s) : base(s){ //Honestly, not quite sure how this line works.

    }
  }
  public void ShowCursor(bool show = true){
    if(show) Cursor.lockState = CursorLockMode.None;
    else Cursor.lockState = CursorLockMode.Locked;
    Cursor.visible = show;
  }
  public bool SwapShowingCursor(){
    ShowCursor(!Cursor.visible);
    return !Cursor.visible;
  }
}
