using UnityEngine;
using System;
using System.IO;

public class SaveDataHandler
{
  private string path = "";
  private string fileName = "";
  private string codeword = "JAS:interactive";

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
        string decryptedData = Crypt(dataToLoad);
        loaded = JsonUtility.FromJson<SaveData>(decryptedData);
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
      string dataEncrypted = Crypt(dataAsJson);
      using(FileStream stream = new FileStream(location, FileMode.Create)){
        using(StreamWriter writer = new StreamWriter(stream)){
          writer.Write(dataEncrypted);
        }
      }
    }
    catch(Exception e){
      Debug.LogError("Failed to save to " + location + "\n" + e);
    }
  }

  //Used for both encrypting and decrypting data.
  private string Crypt(string data){
    string result = "";
    for(int i = 0; i < data.Length; i++){
      result += (char) (data[i] ^ codeword[i % codeword.Length]);
    }
    return result;
  }
}
