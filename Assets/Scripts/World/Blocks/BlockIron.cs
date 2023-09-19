using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BlockIron : IBlock
{
    public override int TextureId()
    {
        return 3;
    }

    public override void OnBreak(Entity player)
    {
        
    }
}
