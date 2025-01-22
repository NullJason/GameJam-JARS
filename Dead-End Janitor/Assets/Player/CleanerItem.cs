using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class CleanerItem : MonoBehaviour
{
	// under any item that can clean.

	// each second, add 5 cleaniness to dirt.
	[SerializeField] private float Speed = 0.1f;
	[SerializeField] private float Strength = 0.25f;
	//[SerializeField] private Vector3 ToolPositionOffset = new Vector3(1, -0.5f, 1.3f); // Position offset (adjust for bottom-right corner)
	//[SerializeField] private Quaternion ToolRotationOffset = Quaternion.Euler(80, 0, 0); // Position offset (adjust for bottom-right corner)
	[SerializeField] private GameObject Player = null;
	[SerializeField] private Transform PlayerCameraTransform = null;
	public float detectionRange = 10f;
    private LayerMask DirtyLayer;
	private string LayerName = "Dirty";
	private Camera PlayerCamera;
	private bool OnCooldown = false;
	private bool ToolInterrupted = false; // stops cleaning if you look away or stop holding down m1.
	private bool isAnimating = false;
	[SerializeField] private ParticleSystem Effects;
	[SerializeField] private List<bool> DirtType = new List<bool>(){true, true}; // 1 = liquid, 2 = solid.
	[SerializeField] private List<bool> CleanMethod = new List<bool>(){true, true}; // 1 = mop, 2 = vacuum.
	private AudioMixer Mixer;
	private AudioSource audioSource;
    private AudioClip Washing_AC;
	private AudioClip Vacuum_AC;
	[Range(0f, 1f)]
	public float Vacuum_Volume = 0.6f;
	[Range(0f, 1f)]
	public float Mop_Volume = 1f;
	private float vacuumloopStart = 2f;
	private float vacuumloopEnd = 11.5f;
	private float currentStart = 0;
	private float currentEnd = 1;


	private void Start() {
		if(Player == null) Player = GameObject.Find("Player");
		if(PlayerCameraTransform == null && Player) PlayerCameraTransform = Player.transform.Find("PlayerCamera");
		PlayerCamera = PlayerCameraTransform.GetComponent<Camera>();
		if(Speed <= 0) Speed = 0.1f; if(Strength <= 0) Strength = 0.25f;
		int dirtyLayerId = LayerMask.NameToLayer(LayerName);
        if (dirtyLayerId != -1) // -1 indicates the layer doesn't exist
        {
            DirtyLayer = 1 << dirtyLayerId;
        }
		Mixer = Resources.Load<AudioMixer>("Sounds/MainMixer");

		audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.Stop();
        Washing_AC = Resources.Load<AudioClip>("Sounds/wash"); 
		Vacuum_AC = Resources.Load<AudioClip>("Sounds/vacuum"); 
		audioSource.outputAudioMixerGroup = Mixer.FindMatchingGroups("Master")[0];
		audioSource.loop = true;
	}
	void PlaySoundEnding(){
		audioSource.loop = false;
		audioSource.time = currentEnd;
	}
	void SetLoopStartEnd(float start = 0, float end = 0){
		if(end == 0) end = audioSource.clip.length-1;
		currentStart = start;
		currentEnd = end;
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

		if(Input.GetMouseButtonDown(0)) if(Effects.main.loop) {
			Effects.Play(); audioSource.loop = true; Mixer.SetFloat("MainMusicVolume", -20f);
			if(CleanMethod[1]) {audioSource.clip = Vacuum_AC; SetLoopStartEnd(vacuumloopStart,vacuumloopEnd); audioSource.volume = Vacuum_Volume;}
			else if(CleanMethod[0]) {audioSource.clip = Washing_AC; SetLoopStartEnd(); audioSource.volume = Mop_Volume;} audioSource.Play();
		}
		if(Input.GetMouseButtonUp(0)) if(Effects.main.loop) {Effects.Stop(); PlaySoundEnding(); Mixer.SetFloat("MainMusicVolume", 0f);}
		if(Input.GetMouseButton(0)) ToolInterrupted = false;
		else{ ToolInterrupted = true; }

		// Animator animator = GetComponent<Animator>();
		// isAnimating = animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f;
    }
	public void ConnectCleanerCollision(Collision other) {
		if((DirtyLayer.value & (1 << other.gameObject.layer)) != 0) {
			TryCleanUp(other.gameObject);
		}

	}

    void DetectLookAt()
    {
		if(PlayerCamera == null) { Debug.Log("PlayerCamera DNE, Please assign."); return;}

		Ray ray = new Ray(PlayerCamera.transform.position, PlayerCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, detectionRange, DirtyLayer))
        {
            Debug.Log("Looking at the huge mess: " + hit.collider.gameObject.name);
			ToolInterrupted = false;
			TryCleanUp(hit.collider.gameObject);
        }
		else{
			Debug.Log(" Nothing to Clean! :( ");
			ToolInterrupted = true;
		}
    }
	void TryCleanUp(GameObject mess){
		// double tool interrupt check is necessary, trust.
		DirtyObject MessScript = mess.GetComponent<DirtyObject>();
		bool CanClean = false;
		for(int i=0; i<DirtType.Count; i++) {if(DirtType[i] && MessScript.IsDirtType(i)) CanClean = true;} if(!CanClean) return;
		if(!OnCooldown && !ToolInterrupted) {OnCooldown = true; StartCoroutine(CleanUp(mess));}
	}

	IEnumerator CleanUp(GameObject mess){
		DirtyObject MessScript = mess.GetComponent<DirtyObject>();
		yield return new WaitForSecondsRealtime(Speed);
		if(!ToolInterrupted) MessScript.Clean(Strength);
		OnCooldown = false;
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
