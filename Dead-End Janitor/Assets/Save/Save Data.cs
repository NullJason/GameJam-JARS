using UnityEngine;

[System.Serializable]
public class SaveData
{
  public int wave;
  public int health;
  public bool music;
  public bool sfx;
  public SaveData(){
    wave = 1;
    health = 10;
    music = true;
    sfx = true;
  }
}
