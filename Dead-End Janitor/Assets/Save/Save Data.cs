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
  public List<Tool> items;
  public Tool liquidTool;
  public Tool solidTool;
  public Tool special;

  public SaveData(){
    wave = 1;
    health = 100;
    music = true;
    sfx = true;
    points = 0;
    accumulatedPoints = 0;
    items = new List<Tool>();
    items.Add(Tool.mop);
    items.Add(Tool.vacuum);
    liquidTool = Tool.mop;
    liquidTool = Tool.vacuum;
  }
}
