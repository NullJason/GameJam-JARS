using UnityEngine;

public class Tutorial : MonoBehaviour
{

    public SpeechHandler SH;
    Color ZombieTextColor = new Color(0,100f/255f,50f/255f,255f/255f);
    Color ZombieBarColor = new Color(0,0,0,85f/255f);
    //public Dialogue(string speaker, string text, float? textSize, Color? barColor, Color? textColor, int? flavor, float? autoPlay, float? animDelay)
    public void DoGameStartText(){
        // 0 = instant, 1 = slow character append, 2 = shake text and ui.
        // anim delay depends on flavor.
        // after the animation is done, adds autoPlay time before next dialogue is auto played.
        SH.AcceptNew("You", "Where Am I?",20,ZombieBarColor,ZombieTextColor,1,3);
        SH.AcceptNew("You", "...",20,ZombieBarColor,ZombieTextColor,1,3f);
        SH.AcceptNew("You", "This Place is Absolutely FILTHY.",20,ZombieBarColor,ZombieTextColor,2,5,0.2f);
        SH.AcceptNew("You", "IT MUST BE CLEANSED!",20,ZombieBarColor,ZombieTextColor,2,5,0.2f);
        SH.AcceptNew("You", "...Who's that?",20,ZombieBarColor,ZombieTextColor,1,2,0.25f);
        SH.PlayNext();
    }
    void Start()
    {
        if (SH == null)
        {
            SH = FindFirstObjectByType<SpeechHandler>();
            if (SH == null)
            {
                Debug.LogError("SpeechHandler not found in the scene!");
                return;
            }
        }

        DoGameStartText();
    }

}
