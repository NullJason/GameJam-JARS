using UnityEngine;

public class DisplayToolName : DisplayMessage
{
  public void SetUp(Tool tool){
    message = "" + tool;
  }
}
