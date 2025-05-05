using UnityEngine;
using UnityEngine.UI;

public class TransitionUI : MonoBehaviour
{
    [SerializeField] private Button TheButton;
    [SerializeField] private GameObject ThisCanvas;
    [SerializeField] private GameObject NextCanvas;
    [SerializeField] private bool ReverseOnAction = false;
    void Start()
    {
        if (TheButton==null) TheButton = GetComponent<Button>();
        TheButton.onClick.AddListener(DoAction);
    }
    void DoAction(){
        if(NextCanvas != null) NextCanvas.SetActive(true);
        if(ThisCanvas != null) ThisCanvas.SetActive(false);
        if(ReverseOnAction) {GameObject temp = NextCanvas; NextCanvas = ThisCanvas; ThisCanvas = temp;}
    }
}
