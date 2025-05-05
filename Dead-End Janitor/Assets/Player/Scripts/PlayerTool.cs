using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerTool : MonoBehaviour
{
    [SerializeField] private List<Dirty> DirtTypes;
    public List<Dirty> GetDirtTypes(){
        return DirtTypes;
    }
    public bool CanCleanType(Dirty dirtType){
        return DirtTypes.Contains(dirtType);
    }
    public Dirty GetPrimaryCleanType(){
        if(DirtTypes.Count != 0) return DirtTypes[0];
        return Dirty.none;
    }
    public abstract void StopTool();
    public abstract void ActivateTool(Transform Origin);
}
