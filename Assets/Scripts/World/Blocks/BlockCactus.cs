using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BlockCactus : IBlockVegetation
{
    public override int TextureId()
    {
        return 7;
    }

    public override void OnBreak(Entity player)
    {
        
    }

}
