using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : Humanoid
{
    // Checkout Humanoid.cs!

    private AudioSource audioSource;
    private AudioClip audioClip;

    Color ZombieTextColor = new Color(0,100f/255f,0,230f/255f);
    Color ZombieBarColor = new Color(0,0,0,85f/255f);
    Color MCTextColor = new Color(200f/255f,25f/255f,50f/255f,230f/255f);
    Color MCBarColor = new Color(200f/255f,25f/255f,50f/255f,85f/255f);
    //public Dialogue(string speaker, string text, float? textSize, Color? barColor, Color? textColor, int? flavor, float? autoPlay, float? animDelay)

    void Start()
    {
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
        SetHp(GameplayManager.main.GetLastPlayerHealth());
    }
    void Update()
    {
        if(IsDead()){
            UponDeath();
        }
        if(TookDamage()){
            OnHit();
        }
        //if(IsMoving())
        if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))OnMove(); else {audioSource.loop = false; audioSource.Stop();}
    }
    void OnMove(){
        // play sound
        Debug.Log("moving");
        if(!audioSource.isPlaying) {audioSource.loop = true; audioSource.Play();}
    }
    void UponDeath(){
        int deaths = Tasks.Instance.GetPlayerDeathCount();
        switch(deaths){
            case 1:
                SpeechHandler.Instance.AcceptNew("Her", "You are an odd one.",20,MCBarColor,MCTextColor,1,2,0.01f);
                SpeechHandler.Instance.AcceptNew("Her", "However.",20,MCBarColor,MCTextColor,1,2,0.01f);
                SpeechHandler.Instance.AcceptNew("Her", "YOU ARE MERELY A ZOMBIE.",20,MCBarColor,MCTextColor,1,2,0.05f);
                SpeechHandler.Instance.AcceptNew("Her", "DO NOT GET IN MY WAY AGAIN.",20,MCBarColor,MCTextColor,1,0,0.05f);
            break;
            case 2:
                SpeechHandler.Instance.AcceptNew("Her", "Another janitor?",20,MCBarColor,MCTextColor,1,2,0.01f);
                SpeechHandler.Instance.AcceptNew("Her", "No, it's you again.",20,MCBarColor,MCTextColor,1,2,0.01f);
                SpeechHandler.Instance.AcceptNew("Her", "I will make sure to kill you this time. Goodbye.",20,MCBarColor,MCTextColor,1,2,0.01f);
            break;
            case 3:
                SpeechHandler.Instance.AcceptNew("Her", "How embarrasing.",20,MCBarColor,MCTextColor,1,2,0.01f);
                SpeechHandler.Instance.AcceptNew("Her", "Die.",20,MCBarColor,MCTextColor,1,2,0.01f);
                SpeechHandler.Instance.AcceptNew("Her", "...3",20,MCBarColor,MCTextColor,1,2,0.01f);
            break;
            case 10:
                SpeechHandler.Instance.AcceptNew("Her", "...",20,MCBarColor,MCTextColor,1,2,0.01f);
                SpeechHandler.Instance.AcceptNew("Her", "How long have I been here?",20,MCBarColor,MCTextColor,1,2,0.01f);
            break;
            default:
                SpeechHandler.Instance.AcceptNew("Her", "..."+deaths ,20,MCBarColor,MCTextColor,1,2,0.01f);
            break;
        }
        Tasks.Instance.AddPlayerDeath();

        SpeechHandler.Instance.PlayNext();
        SceneManager.LoadScene("Death Screen");
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    void OnHit(){

    }
}
