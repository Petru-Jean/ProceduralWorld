using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IBlock
{
    public abstract int TextureId();
    
    public virtual bool HasCollider()
    {
        return true;
    }

    public virtual bool ForcePadding()
    {
        return false;
    }

}
