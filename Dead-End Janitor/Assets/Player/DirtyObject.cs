using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirtyObject : MonoBehaviour
{
	// under any dirty obj.
	[SerializeField] private float MaxHp = 10;
	private float Hp;
	private string DirtyLayer = "DirtyLayer";
	private Vector3 InitialSize;

    void Start()
    {
		Hp = MaxHp;
		InitialSize = transform.localScale;
		
        int layerIndex = LayerMask.NameToLayer(DirtyLayer);
        
        if (layerIndex != -1)
        {
            gameObject.layer = layerIndex;
		}
    }
	public void Clean(int Strength){
		Hp -= Strength;
		if(Hp <= 0) {Destroy(gameObject); return}
		transform.localScale = InitialSize * (Hp/MaxHp);
		
		RaycastHit hit;
		
		// prevent floating away from wall while scaling.
		if (Physics.Raycast(transform.position, Vector3.down, out hit, 1f))
		{
			transform.position = hit.point;
		}
	}
}