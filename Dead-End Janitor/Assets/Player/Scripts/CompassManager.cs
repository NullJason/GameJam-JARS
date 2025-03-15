using System.Collections.Generic;
using UnityEngine;

public class CompassManager : MonoBehaviour
{
    public RectTransform BarTransform;
    public RectTransform ObjectiveMarker;
    public List<RectTransform> ObjectiveMarkers;
    public Transform CamObjectTransform;
    public List<GameObject> Objectives;
    private void Start() {
        CamObjectTransform = GameObject.Find("PlayerCamera").transform;
    }
    private void Update() {
        if(Tasks.Instance.NumTaskObjectsChanged()) {
            int Difference = Tasks.Instance.NumTaskObjects() - ObjectiveMarkers.Count ;
            Objectives = Tasks.Instance.GetTaskObjects();
            if(Difference > 0 ) {
                for(int i=0; i<Difference; i++){
                    RectTransform NewMarker = Instantiate(ObjectiveMarker);
                    ObjectiveMarkers.Add(NewMarker);
                    NewMarker.gameObject.SetActive(true);
                }
            }else if(Difference < 0){
                for(int i=0; i<-Difference; i++){
                    ObjectiveMarkers.RemoveAt(0);
                }
            }
        }
        if(Objectives.Count>0){
            for(int i=0; i< Objectives.Count; i++){
                SetMarkerPosition(ObjectiveMarkers[i], Objectives[i].transform.position);
            }
        }
    }
    private void SetMarkerPosition(RectTransform mT, Vector3 wP){
        Vector3 dirToTarget = wP - CamObjectTransform.position;
        float angle = Vector2.SignedAngle(new Vector2(dirToTarget.x, dirToTarget.z), new Vector2(CamObjectTransform.transform.forward.x,CamObjectTransform.transform.forward.z));
        float compassPosX = Mathf.Clamp(2*angle/Camera.main.fieldOfView, -1, 1);
        mT.anchoredPosition = new Vector2(BarTransform.rect.width/2*compassPosX, 0);
    }
}

