using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BlockGrassWeed : IBlockVegetation
{
    public override int TextureId()
    {
        return 4;
    }

    public override void OnBreak(Entity player)
    {
        
    }


}
