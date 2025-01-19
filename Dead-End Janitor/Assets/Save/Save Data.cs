using UnityEngine;

[System.Serializable]
public class SaveData
{
  public int wave;
  public int health;
  public SaveData(){
    wave = 1;
    health = 10;
  }
}
