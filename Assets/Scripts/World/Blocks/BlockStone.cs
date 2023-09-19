using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BlockStone : IBlock
{
    public override int TextureId()
    {
        return 1;
    }

    public override void OnBreak(Entity player)
    {
        player.Inventory.TryAddItemStack(new ItemStack(FlyweightItem.Get<ItemStoneBlock>(), 1));
        player.Exp += 2;
    }

    public override float BreakTime()
    {
        return 3.0f;
    }
    
}
