using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable] 
public class BlockDirt : IBlock
{
    public override int TextureId()
    {
        return 0;
    }

    public override void OnBreak(Entity player)
    {
        player.Inventory.TryAddItemStack(new ItemStack(FlyweightItem.Get<ItemDirtBlock>(), 1));
        player.Exp += 1;
    }
    
}
