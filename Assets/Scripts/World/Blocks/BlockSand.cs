using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockSand : IBlock
{
    public override int TextureId()
    {
        return 6;
    }

    public override void OnBreak(Entity player)
    {
        
    }
    
}
