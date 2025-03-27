using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class SaveData
{
  public int wave;
  public float health;
  public bool music;
  public bool sfx;
  public int points;
  public int accumulatedPoints;
  public HashSet<Tool> items;

  public SaveData(){
    wave = 1;
    health = 100;
    music = true;
    sfx = true;
    points = 0;
    accumulatedPoints = 0;
    items = new HashSet<Tool>();
    items.Add(Tool.mop);
    items.Add(Tool.vacuum);
  }
}
