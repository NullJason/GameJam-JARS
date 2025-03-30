using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class SoapTool : MonoBehaviour
{
	[SerializeField] private float CoolDown = 5f;
	[SerializeField] private float ThrowStrength = 10f;
	[SerializeField] private GameObject Player = null;
	[SerializeField] private Transform PlayerCameraTransform = null;
    private LayerMask DirtyLayer;
	private string LayerName = "Dirty";
	private Camera PlayerCamera;
	private bool OnCooldown = false;
	private bool ToolInterrupted = false; // stops cleaning if you look away or stop holding down m1.
	private bool isAnimating = false;
	[SerializeField] private ParticleSystem Effects;
    [SerializeField] private Transform ObjectThrown;
	private AudioMixer Mixer;
	private AudioSource audioSource;
	private float currentStart = 0;
	private float currentEnd = 1;


	private void Start() {
		if(Player == null) Player = GameObject.Find("Player");
		if(PlayerCameraTransform == null && Player) PlayerCameraTransform = Player.transform.Find("PlayerCamera");
		PlayerCamera = PlayerCameraTransform.GetComponent<Camera>();
		int dirtyLayerId = LayerMask.NameToLayer(LayerName);
        if (dirtyLayerId != -1) // -1 indicates the layer doesn't exist
        {
            DirtyLayer = 1 << dirtyLayerId;
        }
		Mixer = Resources.Load<AudioMixer>("Sounds/MainMixer");

		audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.Stop();
        // Washing_AC = Resources.Load<AudioClip>("Sounds/wash");
		audioSource.outputAudioMixerGroup = Mixer.FindMatchingGroups("Master")[0];
		audioSource.loop = true;
	}
	void PlaySoundEnding(){
		audioSource.loop = false;
		audioSource.time = currentEnd;
	}
    void Update()
    {
		if (audioSource.isPlaying && audioSource.loop)
        {
            if (audioSource.time > currentEnd)
            {
                audioSource.time = currentStart;
            }
        }

		if(Input.GetMouseButtonDown(0) && !OnCooldown) //if(Effects.main.loop) 
        {
			//Effects.Play(); audioSource.loop = true; Mixer.SetFloat("MainMusicVolume", -20f);
            OnCooldown = true;
			GameObject Throwable = GameObject.Instantiate(ObjectThrown.gameObject);
            Throwable.SetActive(true);
            StartCoroutine(DoCooldown());
		}
		if(Input.GetMouseButtonUp(0)) //if(Effects.main.loop) {Effects.Stop(); PlaySoundEnding(); Mixer.SetFloat("MainMusicVolume", 0f);}
		if(Input.GetMouseButton(0)) ToolInterrupted = false;
		else{ ToolInterrupted = true; }

		// Animator animator = GetComponent<Animator>();
		// isAnimating = animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f;
    }
    IEnumerator DoCooldown(){
		yield return new WaitForSecondsRealtime(CoolDown);
		OnCooldown = false;
	}
	public void SoapThrowableCollision(Collision other) {

	}

    private IEnumerator FadeMixerGroup(AudioMixer mixer, string parameter = "MainMusicVolume", float target = -20f, float duration = 2f)
    {
        float currentTime = 0f;
        float currentVolume;
        mixer.GetFloat(parameter, out currentVolume);

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            float newVolume = Mathf.Lerp(currentVolume, target, currentTime / duration);
            mixer.SetFloat(parameter, newVolume);
            yield return null;
        }

        mixer.SetFloat(parameter, target);
    }
}
