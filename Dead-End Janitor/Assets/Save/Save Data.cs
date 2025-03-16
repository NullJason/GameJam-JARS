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
  public HashSet<Item> items;
  public enum Item{
    mop = 1,
    vacuum = 2
  }
  public SaveData(){
    wave = 1;
    health = 100;
    music = true;
    sfx = true;
    points = 0;
    accumulatedPoints = 0;
    items = new HashSet<Item>();
    items.Add(Item.mop);
    items.Add(Item.vacuum);
  }
}
