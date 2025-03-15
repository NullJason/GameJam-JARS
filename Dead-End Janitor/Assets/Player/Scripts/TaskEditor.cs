using System.Collections.Generic;
using UnityEngine;

public class TaskEditor : MonoBehaviour
{
    // made redundant. VALUES HAVE TO BE EDITED THROUGH TASKS.cs
    [SerializeField] private List<int> taskIds = new List<int>();
    [SerializeField] private List<string> taskTitles = new List<string>();
    [SerializeField] private List<string> taskDescriptions = new List<string>();

    private Dictionary<int, string> TaskTitles = new Dictionary<int, string>();
    private Dictionary<int, string> TaskDescriptions = new Dictionary<int, string>();

    private void OnValidate()
    {
        // Rebuild dictionaries whenever values are changed in the Inspector
        TaskTitles.Clear();
        TaskDescriptions.Clear();

        for (int i = 0; i < taskIds.Count; i++)
        {
            if (i < taskTitles.Count)
                TaskTitles[taskIds[i]] = taskTitles[i];
            if (i < taskDescriptions.Count)
                TaskDescriptions[taskIds[i]] = taskDescriptions[i];
            else TaskDescriptions[taskIds[i]] = "";
        }
    }

    public Dictionary<int, string> GetTitles() => TaskTitles;
    public Dictionary<int, string> GetDescriptions() => TaskDescriptions;
}