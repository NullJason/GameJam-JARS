using UnityEngine;

//This class can be used to line up several RectTransforms in a column under a parent RectTransform. 
//The parent will be resized automatically according to the number of RectTransforms added. 

//TODO: Make sure that the RectTransforms of the parent and the children are configured correctly!

public class ListBuilder : MonoBehaviour
{
  [SerializeField] float header; //How much space above the list. 
  [SerializeField] float footer; //How much space below the list.
  [SerializeField] float margin; //How much vertical space separates entries.
  [SerializeField] float width; //How wide this should be.
  [SerializeField] float offsetX = 0; //What ratio of width should an item be centered on.
  [SerializeField] float space = -1; //How wide a single entry is, excluding margin. Negative values specify dynamically checking the height of each thing added.
  [SerializeField] RectTransform[] items;
  [SerializeField] RectTransform thisTransform; 
  // Start is called once before the first execution of Update after the MonoBehaviour is created
  void Start()
  {
    if(thisTransform == null){
      thisTransform = GetComponent<RectTransform>();
      if(thisTransform == null) Debug.LogError("Could not find a RectTransform for " + gameObject.name + ", and none was provided!");
    }
    thisTransform.sizeDelta = new Vector2(width, header + footer);
    foreach(RectTransform t in items){
      Add(t);
    }
  }

  //Adds t to the listbar.
  public void Add(RectTransform t){
    t.SetParent(thisTransform);
    float distance;
    if(space < 0) distance = t.sizeDelta.y + margin;
    else distance = space + margin;
    t.localPosition = new Vector3(offsetX * thisTransform.sizeDelta.x / 2, -thisTransform.sizeDelta.y + footer, t.localPosition.z);
    thisTransform.sizeDelta += new Vector2(0, distance);
  }
}
