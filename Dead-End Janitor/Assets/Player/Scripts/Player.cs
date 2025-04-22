using UnityEngine;

public class Player : Humanoid
{
    // Checkout Humanoid.cs!

    // Stats
    [SerializeField] private float PlayerLuck = 1;

    private AudioSource audioSource;
    private AudioClip audioClip;
    [SerializeField] private SwapWeapons toolSystem;

    Color ZombieTextColor = new Color(0,100f/255f,0,230f/255f);
    Color ZombieBarColor = new Color(0,0,0,85f/255f);
    Color MCTextColor = new Color(200f/255f,25f/255f,50f/255f,230f/255f);
    Color MCBarColor = new Color(200f/255f,25f/255f,50f/255f,85f/255f);
    //public Dialogue(string speaker, string text, float? textSize, Color? barColor, Color? textColor, int? flavor, float? autoPlay, float? animDelay)
    void Awake(){
        if(GameplayManager.main != null){
            GameplayManager.main.SetPlayer(this, true);
            SetHp(GameplayManager.main.SavedHealth());
        } else Debug.LogWarning("GAMEPLAYMANAGER IS NULL.");
        if(toolSystem == null) toolSystem = GetComponent<SwapWeapons>();
        if(toolSystem == null) Debug.LogError("Could not find the SwapWeapons component!");
        // Add an AudioSource component if not already attached
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.Stop();

        // Load the audio clip from the Resources folder
        audioClip = Resources.Load<AudioClip>("Sounds/ConcreteFootSteps"); // Replace "AudioFileName" with your MP3's file name (without extension)

        if (audioClip == null)
        {
            Debug.LogError("Audio file not found in Resources folder.");
        }
        audioSource.clip = audioClip;
    }

    void Update()
    {
        if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D)) OnMoving();
        else {audioSource.loop = false; audioSource.Stop();}
    }
    private protected override void OnMoving(){
        // play sound
        Debug.Log("moving");
        if(!audioSource.isPlaying) {audioSource.loop = true; audioSource.Play();}
    }
    private protected override void OnDeath(){
        int deaths = Tasks.Instance.GetPlayerDeathCount();
        switch(deaths){
            case 0:
                SpeechHandler.Instance.AcceptNew("Her", "You are an odd one.",20,MCBarColor,MCTextColor,1,2,0.01f);
                SpeechHandler.Instance.AcceptNew("Her", "However.",20,MCBarColor,MCTextColor,1,2,0.01f);
                SpeechHandler.Instance.AcceptNew("Her", "YOU ARE MERELY A ZOMBIE.",20,MCBarColor,MCTextColor,1,2,0.05f);
                SpeechHandler.Instance.AcceptNew("Her", "DO NOT GET IN MY WAY AGAIN.",20,MCBarColor,MCTextColor,1,0,0.05f);
            break;
            case 1:
                SpeechHandler.Instance.AcceptNew("Her", "Another janitor?",20,MCBarColor,MCTextColor,1,2,0.01f);
                SpeechHandler.Instance.AcceptNew("Her", "No, it's you again.",20,MCBarColor,MCTextColor,1,2,0.01f);
                SpeechHandler.Instance.AcceptNew("Her", "I will make sure to kill you this time. Goodbye.",20,MCBarColor,MCTextColor,1,2,0.01f);
            break;
            case 2:
                SpeechHandler.Instance.AcceptNew("Her", "How embarrasing.",20,MCBarColor,MCTextColor,1,2,0.01f);
                SpeechHandler.Instance.AcceptNew("Her", "Die.",20,MCBarColor,MCTextColor,1,2,0.01f);
                SpeechHandler.Instance.AcceptNew("Her", "...3",20,MCBarColor,MCTextColor,1,2,0.01f);
            break;
            case 9:
                SpeechHandler.Instance.AcceptNew("Her", "...",20,MCBarColor,MCTextColor,1,2,0.01f);
                SpeechHandler.Instance.AcceptNew("Her", "How long have I been here?",20,MCBarColor,MCTextColor,1,2,0.01f);
            break;
            default:
                SpeechHandler.Instance.AcceptNew("Her", "..."+deaths ,20,MCBarColor,MCTextColor,1,2,0.01f);
            break;
        }
        Tasks.Instance.AddPlayerDeath();

        SpeechHandler.Instance.PlayNext();
        if (GameplayManager.main != null) GameplayManager.main.OnDeath();
    }
    public float GetBaseLuck(){
        return PlayerLuck;
    }

    //Causes the player to equip the passed tool. Does not guarantee that the player will be holding the tool.
    public void SetTool(Tool tool){
        SetTool(Tools.Get(tool));
    }
    public void SetTool(GameObject tool){
        SetTool(tool.transform);
    }
    public void SetTool(Transform tool){
        toolSystem.SetTool(tool.transform);
    }
}
