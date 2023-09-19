using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BlockGold : IBlock
{
    public override int TextureId()
    {
        return 2;
    }

    public override void OnBreak(Entity player)
    {
        
    }

}
