using UnityEngine;
using System;
using System.IO;

public class SaveDataHandler
{
  private string path = "";
  private string fileName = "";

  public SaveDataHandler(string path, string fileName){
    this.path = path;
    this.fileName = fileName;
  }

  public SaveData Load(){
    string location = Path.Combine(path, fileName);
    SaveData loaded = null;
    if(File.Exists(location)){
      try{
        string dataToLoad = "";
        using(FileStream stream = new FileStream(location, FileMode.Open)){
          using(StreamReader reader = new StreamReader(stream)){
            dataToLoad = reader.ReadToEnd();
          }
        }
        loaded = JsonUtility.FromJson<SaveData>(dataToLoad);
      }
      catch(Exception e){
        Debug.LogError("Failed to load " + location + "\n" + e);
      }
    }
    return loaded;
  }

  public void Save(SaveData data){
    string location = Path.Combine(path, fileName);
    try{
      Directory.CreateDirectory(Path.GetDirectoryName(location));
      string dataAsJson = JsonUtility.ToJson(data, true);

      using(FileStream stream = new FileStream(location, FileMode.Create)){
        using(StreamWriter writer = new StreamWriter(stream)){
          writer.Write(dataAsJson);
        }
      }
    }
    catch(Exception e){
      Debug.LogError("Failed to save to " + location + "\n" + e);
    }
  }
}
