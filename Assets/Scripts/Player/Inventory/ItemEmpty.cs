using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemEmpty : Item
{
    public override Sprite Icon()
    {
        return null;
    }

    public override string Name()
    {
        return "Empty";
    }

    public override void Use(Entity player)
    {

    }

}
