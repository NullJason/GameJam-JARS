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

	public bool IsDirtType(int index){return DirtType[index];}
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
		}

		if(TaskID < 1) TaskID = 1;
		Tasks.Instance.AddTask(TaskID, gameObject, Hp, MaxHp);
    }
	public void Clean(float Strength){
		Hp -= Strength;
		if(Hp <= 0) { if(CleanProcessed) return; CleanProcessed = true; Tasks.Instance.CompleteTask(gameObject); Particles.Play(); StartDelayedDestroy(Particles.main.duration); return;}
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
    }
    IEnumerator DelayedDestroyObject(float time){
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }
}