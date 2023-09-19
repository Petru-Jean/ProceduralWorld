using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine;

[Serializable] 
public abstract class IBlock
{
    public abstract int  TextureId();
    public abstract void OnBreak(Entity player);
    
    //public abstract int GetId(); // ID = HASH CODE
 
    public virtual float BreakTime()
    {
        return 1.0f;
    }

    public virtual bool HasCollider()
    {
        return true;
    }

    public virtual bool Padded()
    {
        return true;
    }

}
