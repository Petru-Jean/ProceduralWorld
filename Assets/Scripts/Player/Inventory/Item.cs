using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item
{
    public abstract string      Name();
    public abstract void        Use(Entity entity);


    public virtual IBlock BlockType()
    {
        return FlyweightBlock.blockAir;
    }
    
    public virtual Sprite Icon()
    {
        Texture2D icon = (Texture2D)Resources.Load("Textures/Items/ItemPlaceholder");
        //return (Sprite) Resources.Load("Textures/ItemPlaceholder");
        return Sprite.Create(icon, new Rect(0, 0, icon.width, icon.height), Vector2.zero);
    }

    public virtual int MaxStacks()
    {
        return 1;
    }

}
