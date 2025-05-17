using UnityEngine;

public class DisplayMessage : MonoBehaviour
{
  [SerializeField] private protected string message;
  [SerializeField] private protected ResultsPanel display;
  public void Display(){
    display.Hijack(message);
  }
}
