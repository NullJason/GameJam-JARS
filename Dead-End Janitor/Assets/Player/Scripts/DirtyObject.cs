using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirtyObject : MonoBehaviour
{
	// under any dirty obj.
	[SerializeField] private List<bool> DirtType = new List<bool>(){true, true}; // 1 = liquid, 2 = solid.
	[SerializeField] private float MaxHp = 10;
	[SerializeField] private int TaskID = 1;
	private float Hp;
	private string DirtyLayer = "Dirty";
	private Vector3 InitialSize;
	Transform Effects;
    ParticleSystem Particles;
	public bool CleanProcessed = false;
	private AudioSource audioSource;
	private AudioClip Bubbles_AC;
	GameplayManager GPM;

	public bool IsDirtType(int index){return DirtType[index];}
	private void SetLayerRecursive(GameObject parent, int index){
		parent.layer = index;
		foreach (Transform child in transform){
				SetLayerRecursive(child.gameObject, index);
		}
	}
    void Start()
    {
		Effects = transform.Find("Effects");
		if(Effects == null) Effects = Instantiate(Resources.Load<GameObject>("Particles/Effects")).transform;
		Effects.SetParent(transform);
        Particles = Effects.GetComponent<ParticleSystem>();

		Particles.Stop(true);

		Hp = MaxHp;
		InitialSize = transform.localScale;

        int layerIndex = LayerMask.NameToLayer(DirtyLayer);

        if (layerIndex != -1)
        {
            gameObject.layer = layerIndex;
//      TODO: Repair the SetLayerRecursive method!
//			SetLayerRecursive(gameObject, layerIndex);
		}

		if(TaskID < 1) TaskID = 1;
		Tasks.Instance.AddTask(TaskID, gameObject, Hp, MaxHp);
		GPM = FindFirstObjectByType<GameplayManager>();
		if(GPM) GPM.AddToCleanOnScreen(); else{Debug.LogError("Dirty Object Cannot Find GameplayManager");}

		audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.Stop();
		Bubbles_AC = Resources.Load<AudioClip>("Sounds/bubble");
    }
	public void Clean(float Strength){
		Hp -= Strength;
		if(Hp <= 0) { if(CleanProcessed) return; CleanProcessed = true;
		if(DirtType[0]) {audioSource.clip = Bubbles_AC; audioSource.Play();}
		else if(DirtType[1]){} //vacuum dirt type death noise
		Tasks.Instance.CompleteTask(gameObject); Particles.Play(); StartDelayedDestroy(Particles.main.duration); return;}

		Tasks.Instance.UpdateTask(gameObject, Hp);

		Vector3 previousScale = transform.localScale;

		if(transform.localScale.magnitude > 1) transform.localScale = InitialSize * (Hp/MaxHp);
		else return;

		RaycastHit hit;

		Vector3 rayDirection = -transform.up;
		float rayLength = GetRayLengthForObject(previousScale, transform.rotation);

		// prevent floating away from wall while scaling.
		if (Physics.Raycast(transform.position, rayDirection, out hit, rayLength))
		{
			transform.position = hit.point;
		}
	}
	float GetRayLengthForObject(Vector3 objectScale, Quaternion objectRotation)
	{
		Vector3 localDown = Vector3.down;
		Vector3 worldDown = objectRotation * localDown;

		Bounds bounds = GetObjectBounds(objectScale);
		float extent = Vector3.Dot(worldDown.normalized, bounds.extents);

		return extent + 0.5f;
	}
	Bounds GetObjectBounds(Vector3 objectScale)
	{

		Collider collider = GetComponent<Collider>();
		if (collider != null)
		{
			Bounds bounds = collider.bounds;
			bounds.size = Vector3.Scale(bounds.size, objectScale);
			return bounds;
		}
		else
		{
			return new Bounds(Vector3.zero, objectScale);
		}
	}
	void StartDelayedDestroy(float time){
        StartCoroutine(DelayedDestroyObject(time));
		this.enabled = false;
    }
    IEnumerator DelayedDestroyObject(float time){
        yield return new WaitForSeconds(time);
				if(GPM){
						GPM.Clean();
						float luck = GameplayManager.main.GetBaseLuck(); //TODO: Draw from the tool luck stat, as well as any modifier!
						float random = Random.Range(0, 3);
						Debug.Log("Random:" + random + " + luck:" + luck);
						if(luck >= random) GameplayManager.main.AddPoints(1, true);
				}
        Destroy(gameObject);
    }
}
