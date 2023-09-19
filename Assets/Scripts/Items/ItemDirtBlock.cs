using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDirtBlock : Item
{
    public override string Name()
    {
        return "Dirt Block";
    }

    public override IBlock BlockType()
    {
        return FlyweightBlock.Get<BlockDirt>();
    }
    
    public override void Use(Entity entity)
    {
        entity.Inventory.DecreaseItemStack(FlyweightItem.Get<ItemDirtBlock>(), 1);
    }

    public override Sprite Icon()
    {
        Texture2D icon = (Texture2D)Resources.Load("Textures/Items/ItemDirtBlock");
        //return (Sprite) Resources.Load("Textures/ItemPlaceholder");
        return Sprite.Create(icon, new Rect(0, 0, icon.width, icon.height), Vector2.zero);
    }

    public override int MaxStacks()
    {
        return 64;
    }

}
