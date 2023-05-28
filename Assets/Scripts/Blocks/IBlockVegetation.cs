using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IBlockVegetation : IBlock
{
    public override bool HasCollider()
    {
        return false;
    }

    public override bool ForcePadding()
    {
        return true;
    }
    
}
