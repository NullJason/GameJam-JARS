using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

        // init vars.
        TaskEditor taskEditor = transform.GetComponent<TaskEditor>();
        TaskTitles = taskEditor.GetTitles();
        TaskDescriptions = taskEditor.GetDescriptions();

        Canvas = GameObject.Find("TaskCanvas").transform;
        TaskContainer = Canvas.Find("Container");
        TaskButton = TaskContainer.transform.Find("TaskButton");
        TaskInfo = Canvas.Find("TaskInfo");
        TaskTitle = TaskInfo.Find("TaskTitle").GetChild(0).GetComponent<TMP_Text>();
        TaskDesc = TaskInfo.Find("TaskDescription").GetChild(0).GetComponent<TMP_Text>();

        TaskButton.gameObject.SetActive(false);
        TaskContainer.gameObject.SetActive(false);
        TaskInfo.gameObject.SetActive(false);
    }
    private Tasks(){}
    private Dictionary<int, string> TaskTitles; // TaskID to task title...
    private Dictionary<int, string> TaskDescriptions; // TaskID to task descriptions...
    private Dictionary<GameObject, TaskProgress> TaskObjects = new Dictionary<GameObject, TaskProgress>(); // List of GOs.
    private Dictionary<int, int> TIDCount = new Dictionary<int, int>();
    private Dictionary<int, RectTransform> TIDToButton = new Dictionary<int, RectTransform>();
    private int PreviousNumTasks = 0;

    Transform Canvas;
    Transform TaskContainer;
    Transform TaskButton;
    Transform TaskInfo;
    TMP_Text TaskTitle;
    TMP_Text TaskDesc;
    public void RefreshTasks(){
        float currentY = 0f;
        foreach (RectTransform rect in TIDToButton.Values)
        {
            rect.anchoredPosition = new Vector2(0f, currentY);
            currentY -= rect.sizeDelta.y;
        }
    }
    public void AddTask(int TID, GameObject TaskObject, float progress, float condition){
        if (TID < 0) return;
        if(!TIDCount.TryAdd(TID, 1)) TIDCount[TID] += 1;
        string TaskName = TaskTitles[TID];
        TaskProgress tp = new TaskProgress(progress, condition, TID);
        TaskObjects.Add(TaskObject, tp);
        // set ui button text here;
        if(TIDCount[TID] == 1){
            GameObject button = Instantiate(TaskButton).gameObject;
            TMP_Text txt = button.transform.GetChild(0).GetComponent<TMP_Text>();
            txt.SetText(TaskName+": "+TIDCount[TID]);
            button.transform.SetParent(TaskContainer, false);
            TIDToButton.Add(TID, button.GetComponent<RectTransform>());
            button.GetComponent<Button>().onClick.AddListener(() => ShowTask(TaskObject));
            button.SetActive(true);
        }else{
            GameObject button = TIDToButton[TID].gameObject;
            TMP_Text txt = button.transform.GetChild(0).GetComponent<TMP_Text>();
            txt.SetText(TaskName+": "+TIDCount[TID]);
        }
        RefreshTasks();
    }
    private void ShowTask(GameObject TObj){
        TaskInfo.gameObject.SetActive(!TaskInfo.gameObject.activeSelf);
        UpdateTaskInfoUI(TObj);
    }
    private void UpdateTaskInfoUI(GameObject TObj){
        // if(!TaskInfo.gameObject.activeSelf) return;
        // note: this will change the current screen.
        int TID = TaskObjects[TObj].GetTID();
        TaskTitle.SetText(TaskTitles[TID]+": "+TaskObjects[TObj].GetPercentCleared()+"%");
        TaskDesc.SetText(TaskDescriptions[TID]);
    }
    public void UpdateTask(GameObject TaskObj, float Progress){
        TaskObjects[TaskObj].SetProgress(Progress);
        UpdateTaskInfoUI(TaskObj);
    }
    public void CompleteTask(GameObject TaskObj){
        int TID = TaskObjects[TaskObj].GetTID();
        TIDCount[TID] -= 1;
        if(TIDCount[TID] == 0) {TIDCount.Remove(TID); Destroy(TIDToButton[TID].gameObject); TIDToButton.Remove(TID);}
        else { string TaskName = TaskTitles[TID];
        GameObject button = TIDToButton[TID].gameObject;
        TMP_Text txt = button.transform.GetChild(0).GetComponent<TMP_Text>();
        txt.SetText(TaskName+": "+TIDCount[TID]); }
        TaskObjects.Remove(TaskObj);
        TaskInfo.gameObject.SetActive(false);
        RefreshTasks();
    }
    public List<GameObject> GetTaskObjects(){
        return TaskObjects.Keys.ToList();
    }
    public bool NumTaskObjectsChanged(){
        if(PreviousNumTasks != TaskObjects.Count) {PreviousNumTasks = TaskObjects.Count; return true;}
        return false;
    }
    public int NumTaskObjects(){return TaskObjects.Count;}
}
class TaskProgress{
    private int TID;
    private float Progress;
    private float Condition;
    public TaskProgress(float current, float max, int tid){ Progress = current; Condition = max; TID = tid;}
    public float GetProgress() {return Progress;}
    public void SetProgress(float p) { Progress = p; }
    public float GetMax() {return Condition;}
    public float GetPercentCleared() {return (1 - (Progress / Condition)) * 100;}
    public int GetTID(){return TID;}
    public bool IsCleared(){
        float percent = GetPercentCleared();
        if(percent >= 1 || percent < 0) return true;
        return false;
    }
}
