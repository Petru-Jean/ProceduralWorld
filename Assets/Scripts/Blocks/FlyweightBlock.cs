using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FlyweightBlock
{   
    private static Dictionary<System.Type, IBlock>    blockCache;
    private static Dictionary<System.Type, ushort>    typeIdCache;
    private static Dictionary<ushort, System.Type>    idTypeCache;

    static FlyweightBlock()
    {
        blockCache   = new Dictionary<System.Type, IBlock>();
        typeIdCache  = new Dictionary<System.Type, ushort>();
        idTypeCache  = new Dictionary<ushort, System.Type>();

        AddTypeIdPair(typeof(BlockAir),   0);
        AddTypeIdPair(typeof(BlockDirt),  1);
        AddTypeIdPair(typeof(BlockGrass), 2);
        AddTypeIdPair(typeof(BlockStone), 3);
        AddTypeIdPair(typeof(BlockGold),  4);
        AddTypeIdPair(typeof(BlockDirt),  5);
    }
    
    private static void AddTypeIdPair(System.Type T, ushort id)
    {
        if(!T.IsSubclassOf(typeof(IBlock)))
        {
            throw new UnityException("Trying to cache type not derived from IBlock");
        }

        typeIdCache[T] = id;
        idTypeCache[id] = T;
    }

    public static IBlock Get<T>() where T : IBlock, new()
    {
        IBlock block = null;

        if(blockCache.TryGetValue(typeof(T), out block))
        {
            return block;
        }

        block = new T();
        blockCache[typeof(T)] = block;

        return block;
    }

    public static IBlock Get(ushort id)
    {
        System.Type type;
        IBlock      block = null;

        if(idTypeCache.TryGetValue(id, out type))
        {
            if(blockCache.TryGetValue(type, out block))
            {
                return block;
            }
        }

        return Get<BlockAir>();
    }

    public static ushort GetId(IBlock block)
    {
        ushort id = 0;
        
        if(typeIdCache.TryGetValue(block.GetType(), out id))
        {
            
            return id;
        }

        Debug.Log("id not found" + block.GetType());
        return 0;
    }

    public static ushort GetId<T>() where T : IBlock
    {
        ushort id = 0;
        
        if(typeIdCache.TryGetValue(typeof(T), out id))
        {
            return id;
        }

        // Debug.Log(id);
        return 0;
    }

    public static BlockData blockDataAir = new BlockData(new BlockAir());
}
