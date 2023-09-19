using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ItemStack
{
    Item item;
    int  stacks;

    public Item Item
    {
        get { return item; }
    }

    public int Stacks
    {
        get { return stacks;  }
        set { stacks = value < 0 ? 0 : value; }
    }

    public ItemStack(Item item, int stacks)
    {
        this.item   = item;
        this.stacks = Mathf.Clamp(stacks, 1, item.MaxStacks());
    }

    // public static bool operator ==(ItemStack first, ItemStack second)
    // {

    //     return false;
    // }

    // public static bool operator !=(ItemStack first, ItemStack second) => !(first == second);

    public static readonly ItemStack empty = new ItemStack(FlyweightItem.itemEmpty, 1); 
}
