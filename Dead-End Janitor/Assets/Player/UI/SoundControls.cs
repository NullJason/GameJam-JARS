using UnityEngine;
using UnityEngine.UI;

public class SoundControls : MonoBehaviour
{
    [SerializeField] Toggle musc;
    [SerializeField] Toggle sfx;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {
        if(musc == null) Debug.LogWarning("No value was provided to " + gameObject.name + " for music toggle! ");
        if(sfx == null) Debug.LogWarning("No value was provided to " + gameObject.name + " for sfx toggle! ");
        musc.isOn = GameplayManager.main.GetMusic();
        sfx.isOn = GameplayManager.main.GetSfx();
    }

    // Update is called once per frame
    void Update()
    {
        GameplayManager.main.SetSettings(musc.isOn, sfx.isOn);
    }
    void OnDisable(){
        GameplayManager.main.SaveGame();
    }
}
