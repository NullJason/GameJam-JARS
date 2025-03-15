using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class OpenTasks : MonoBehaviour
{
    Button OpenTasksButton;
    Transform Canvas;
    GameObject TaskContainer;
    GameObject TaskInfo;
    int count = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Canvas = transform.parent;
        TaskContainer = Canvas.Find("Container").gameObject;
        TaskInfo = Canvas.Find("TaskInfo").gameObject;
        OpenTasksButton = transform.GetComponent<Button>();
        OpenTasksButton.onClick.AddListener(ToggleTasksUi);
    }
    private void Update() {
        if(Input.GetKeyDown(KeyCode.T)) ToggleTasksUi();
    }
    void ToggleTasksUi(){        
        switch (count){
            case 0:
            TaskContainer.SetActive(true);
            TaskInfo.SetActive(false);
            break;
            case 1:
            TaskContainer.SetActive(false);
            TaskInfo.SetActive(true);
            break;
            case 2:
            TaskContainer.SetActive(false);
            TaskInfo.SetActive(false);
            break;
        }
        count+=1; if(count==3) count=0;
    }  
}
