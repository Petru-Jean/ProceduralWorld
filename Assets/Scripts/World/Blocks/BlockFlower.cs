using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BlockFlower : IBlockVegetation
{
    public override int TextureId()
    {
        return 5;
    }

    public override void OnBreak(Entity player)
    {
        
    }
}
