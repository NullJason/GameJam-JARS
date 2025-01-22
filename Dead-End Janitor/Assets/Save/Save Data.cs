using UnityEngine;

[System.Serializable]
public class SaveData
{
  public int wave;
  public float health;
  public bool music;
  public bool sfx;
  public SaveData(){
    wave = 1;
    health = 100;
    music = true;
    sfx = true;
  }
}
