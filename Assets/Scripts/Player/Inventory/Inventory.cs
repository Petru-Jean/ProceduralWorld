using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory
{
    int slots;
    int selectedSlot;

    ItemStack[] items;

    public int SelectedItemSlot
    {
        get { return selectedSlot;  }
        set { selectedSlot = Mathf.Clamp(value, 0, 8);  }
    }

    public Inventory(int slots = 36+9)
    {
        this.slots = slots;
        this.items = new ItemStack[slots];

        System.Random rand = new System.Random();

        for(int i = 0; i < slots; i++)
        {
            items[i] = ItemStack.empty;

            // if(rand.Next(0,4) == 0)  
            //     TryAddItemStack(new ItemStack(FlyweightItem.Get<ItemPlaceholder>(), 1));
        }

    }

    public int Slots()
    {
        return slots;
    }

    public ItemStack GetItemStack(int slot)
    {
        if(slot < 0 || slot >= slots)
        {
            return ItemStack.empty;
        }

        return items[slot];
    }
    
    public ItemStack[] GetItemStacks()
    {
        return items;
    }

    public void SetItemStack(int slot, ItemStack itemStack)
    {
        if(slot < 0 || slot >= slots)
        {
            return;
        }

        // if(itemStack.Stacks <= 0)
        // {
        //     items[slot] = ItemStack.empty;
        // }

        items[slot] = itemStack;
    }

    /// <summary>
    /// Tries to fill up stacks of inventory slots that have the same item type
    /// otherwise it fills up an empty inventory slot, if it exist.
    /// </summary>
    /// <param name="itemStack">The provided ItemStack</param>
    /// <returns>The number of stacks that couldn't fill any slot.</returns>
    
    public int TryAddItemStack(ItemStack itemStack)
    {
        int maxStacks  = itemStack.Item.MaxStacks();
        int stacksLeft = itemStack.Stacks;

        for(int i = 0; i < slots; i++)
        {
            if (items[i].Item == itemStack.Item)
            {
                // check how many stacks are left
                // in the current item
                int remainingStacks = maxStacks - items[i].Stacks;

                if(remainingStacks < stacksLeft )
                {
                    stacksLeft -= remainingStacks;
                    items[i].Stacks = maxStacks;
                }
                else
                {
                    items[i].Stacks += stacksLeft;
                    return 0;
                }

            }
        }

        for (int i = 0; i < slots; i++)
        {
            if(items[i].Item == FlyweightItem.itemEmpty)
            {
                items[i] = new ItemStack(itemStack.Item, stacksLeft);
                return 0;
            }
        }   

        return stacksLeft;
    }

    /// <summary>
    /// Checks if the inventory contains the specified amount of item stacks of the provided Item type.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="stacks"></param>
    /// <returns>Return true if the inventory contains the number of stacks (or more) of the provided item type.</returns>
    public bool ContainsItemStacks(Item item, int stacks)
    {
        int stacksFound = 0;

        for (int i = 0; i < slots; i++)
        {
            if(items[i].Item == item)
            {
                stacksFound += items[i].Stacks;
            }

            if(stacksFound >= stacks)
            {
                return true;
            }
        }
        
        return false;
    }

    /// <summary>
    /// Decreases stacks from existing item stacks. Does NOT guarantee that the inventory contains those items.
    /// </summary>
    /// <param name="item">The type of item.</param>
    /// <param name="stacks">The number of stacks to be decreased.</param>
    public void DecreaseItemStack(Item item, int stacks)
    {
        int  stacksLeft = stacks;

        for (int i = 0; i < slots; i++)
        {
            if(stacksLeft <= 0)
            {
                    return;
            }

            if(items[i].Item == item)
            {

                if(items[i].Stacks > stacksLeft)
                {
                    items[i].Stacks -= stacksLeft;
                    stacksLeft      = 0;
                }
                else
                {
                    stacksLeft -= items[i].Stacks;
                    items[i]    = ItemStack.empty;
                }   
            }

        }

    }


}
