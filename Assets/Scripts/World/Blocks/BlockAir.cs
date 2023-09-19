using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockAir : IBlock
{
    public override int TextureId()
    {
        return -1;
    }

    public override void OnBreak(Entity player)
    {
        
    }

}
