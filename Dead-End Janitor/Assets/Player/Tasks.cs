using System.Collections.Generic;
using UnityEngine;

public class Tasks : MonoBehaviour
{
    // Unity Singleton instance.
    public static Tasks Instance { get; private set; }
    private void Awake()
    {
        // Check if there is already an instance of this class
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Destroy duplicate
            return;
        }

        Instance = this; // Set the instance
        DontDestroyOnLoad(gameObject); // Make persistent across scenes

       // init vars if any.
    }
    private Tasks(){}
    [SerializeField] private Dictionary<int, string> TaskTitles = new Dictionary<int, string>(); // TaskID to task title...
    [SerializeField] private Dictionary<int, string> TaskDescriptions = new Dictionary<int, string>(); // TaskID to task descriptions...
    private Dictionary<GameObject, TaskProgress> TaskObjects = new Dictionary<GameObject, TaskProgress>(); // List of GOs.
    public void RefreshTasks(){
       // update ui once, this method just reorganizes the list of tasks if any were removed / added so holes and overlaps of ui dont happen.
       //foreach() 
    }
    public void RenewCompass(){
        // update direction of dirt to clean, should point to random dirty obj. Possible upgrade: point to closest.
    }
    public void AddTask(int TID, GameObject TaskObject, float progress, float condition){
        if (TID < 0) return;
        string TaskName = TaskTitles[TID];
        string TaskDescription = TaskDescriptions[TID];
        TaskProgress tp = new TaskProgress(progress, condition);
        TaskObjects.Add(TaskObject, tp);
        // set ui text here;
    }
    public void UpdateTask(GameObject TaskObj, float Progress){
        TaskObjects[TaskObj].SetProgress(Progress);
        // alter progress ui text here
    }
    public void Remove(GameObject TaskObj){
        TaskObjects.Remove(TaskObj);
        RefreshTasks();
    }
}
class TaskProgress{
    private float Progress;
    private float Condition;
    public TaskProgress(float current, float max){ Progress = current; Condition = max; }
    public float GetProgress() {return Progress;}
    public void SetProgress(float p) { Progress = p; }
    public float GetMax() {return Condition;}
    public float GetPercentCleared() {return Progress/Condition;}
    public bool IsCleared(){
        float percent = GetPercentCleared();
        if(percent >= 1 || percent < 0) return true;
        return false;
    }
}
