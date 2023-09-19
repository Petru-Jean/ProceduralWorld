using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FlyweightItem
{
    private static Dictionary<System.Type, Item> typeItemCache = new Dictionary<System.Type, Item>();

    public static Item Get<T>() where T : Item, new()
    {
        Item item = null;

        if(typeItemCache.TryGetValue(typeof(T), out item))
        {
            return item;
        }

        typeItemCache[typeof(T)] = new T();

        return typeItemCache[typeof(T)];
    }

    public static Item itemEmpty = new ItemEmpty();

}
