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
  private Player player;
  //bool music;
  //bool sfx;
  //float lastKnownPlayerHealth = 100;
//  public Settings settings;
  void Awake(){
    if(main == null){
      main = this;
      DontDestroyOnLoad(this);
    }
    else Destroy(this.gameObject);
    spawners = new List<Spawner>();
    saveHandler = new SaveDataHandler(Application.persistentDataPath, "save"); //TODO: Remove magic numbers!
    waveTimer = waveTimerReset;
    LoadGame();
    SaveGame();
  }
  void Update()
  {
    Debug.Log("liquid Tool: " + GetLiquidTool());
    Debug.Log("Wave: " + GetWave() + "\nZombies left: "+ howManyLeftInWave + "\nTotal cleaning objects: " + howManyToClean + "\nOnscreen cleaning objects: " + howManyToCleanOnScreen);
    if(gameIsActive){
      TrySpawn();
      if(howManyLeftInWave == 0 && howManyToClean == 0){
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

  private void SetGameActive(bool active){
    gameIsActive = active;
  }

  //Meant to be called when the player completes a wave.
  private void StartNextWave(){
    SetWavePlus(1);
    SetUpWave();
    SpeechHandler.Instance.AcceptNew("Bob", "Wave Complete!\nWave: " + GetWave() + "\n  The game has been saved.");
    SaveGame();
  }

  //Sets up a new game using preexisting save data.
  public void SetUpNewRound(String sceneName = "Demo Scene"){
    //TODO: Check if sceneName corresponds to a valid scene, and moreover, if that scene is meant to run the game.
    LoadGame();
    AsyncOperation loadScene = SceneManager.LoadSceneAsync(sceneName);
    loadScene.completed += (x) => {
      SetGameActive(true);
      SetUpWave();
      SetUpTools();
    };
  }

  //Meant to be called at the beginning of a new round, but after the player has been assigned.
  private void SetUpTools(){
    if(CheckToolUnlocked(GetSolidTool())) GetPlayer().SetTool(GetSolidTool());
    else Debug.LogError("Could not equip tool " + GetSolidTool() + " because it was not yet unlocked!");
    if(CheckToolUnlocked(GetLiquidTool())) GetPlayer().SetTool(GetLiquidTool());
    else Debug.LogError("Could not equip tool " + GetLiquidTool() + " because it was not yet unlocked!");
  }

  private Tool GetLiquidTool(){
    return saveFile.liquidTool;
  }
  private Tool GetSolidTool(){
    return saveFile.solidTool;
  }
  public Tool GetTool(Dirty dirty){
    if(dirty == Dirty.solid) return GetSolidTool();
    else if(dirty == Dirty.liquid) return GetLiquidTool();
    else Debug.LogError("Failed to get tool: Player only has a solid and liquid tool, not a " + dirty + " tool!");
    return new Tool();
  }
  public void SetLiquidTool(Tool tool){
    //TODO: check if tools are proper, ie if this tool is a liquid tool.
    if(Tools.GetDirtType(tool) != Dirty.liquid) Debug.LogError("Could not assign liquid tool to " + tool + " because it wasn't a liquid tool!");
    else saveFile.liquidTool = tool;
  }
  public void SetSolidTool(Tool tool){
    //TODO: check if tools are proper, ie if this tool is a solid tool.
    if(Tools.GetDirtType(tool) != Dirty.solid) Debug.LogError("Could not assign solid tool to " + tool + " because it wasn't a solid tool!");
    else saveFile.solidTool = tool;
  }
  public void SetTool(Tool tool, Dirty dirtType){
    if(dirtType == Dirty.solid) SetSolidTool(tool);
    else if(dirtType == Dirty.liquid) SetLiquidTool(tool);
    else Debug.LogError("Failed to get tool: Player only has a solid and liquid tool, not a " + dirtType + " tool!");
  }

  //Meant to be called at the start of a wave.
  //TODO: Rebalance?
  private void SetUpWave(){
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

  //Creates a new save file and saves it.
  //Used to reset all values to default.
  public void FullResetSave(){
    Debug.LogWarning("Full data wipe occurring! Was this intended?");
    saveFile = null;
    Save();
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
    return GetPlayer().GetHp();
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
  public void AddPoints(int i = 1, bool announce = false){
    if(i < 0) Debug.LogWarning("Adding a negative number of points (i)! This may result in a negative total points! If this was not intended, use TryRemovePoints()!");
    SetPoints(GetPoints() + i);
    if(announce){
      Debug.Log("  +" + i + " points!");//TODO: Add better implementation!
    }
  }

  public float GetBaseLuck(){
    return GetPlayer().GetBaseLuck();
  }

  //Removes points from match points and moves them to accumulated points.
  //TODO!
  public void ShuntPoints(){
    Debug.LogError("TODO!");
  }
  //To be called on the death of the player. (TODO: How should this work/be called in multiplayer modes?)
  public void OnDeath(){
    SceneManager.LoadScene("Death Screen");
    ShowCursor();
    player = null;
    SetGameActive(false);
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

  //TODO: Make sure this works properly, particularly when multiplayer rolls around!
  public Player GetPlayer(){
    if(player == null) Debug.LogError("The player is null! Has the intended player called SetPlayer(Player) yet?");
    return player;
  }

  public void SetPlayer(Player player, bool printOnSuccess = false){
    if(this.player == null){
      this.player = player;
      if(printOnSuccess) Debug.Log("Successfully set player to " + player);
    }
    else Debug.LogError("Attempted to set player, but a player had already been specified!");
  }
}
