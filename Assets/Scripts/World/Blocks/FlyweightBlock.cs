using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public static class FlyweightBlock
{   
    private static Dictionary<System.Type, IBlock> typeBlockCache = new Dictionary<System.Type, IBlock>();

    private static Dictionary<System.Type, int>    typeIdCache = new Dictionary<System.Type, int>(); 
    private static Dictionary<int, System.Type>    idTypeCache = new Dictionary<int, System.Type>();

    static FlyweightBlock()
    {
        CacheBlock(new BlockCactus());
        CacheBlock(new BlockDirt());
        CacheBlock(new BlockFlower());
        CacheBlock(new BlockGold());
        CacheBlock(new BlockGrassWeed());
        CacheBlock(new BlockIron());
        CacheBlock(new BlockSand());
        CacheBlock(new BlockStone());
    } 
    
    private static void CacheBlock(IBlock block)
    {
        int id = ChunkUtil.GetHashCode(block.GetType().ToString());

        typeBlockCache[block.GetType()]     = block;
        typeIdCache[block.GetType()]        = id;
        idTypeCache[id] = block.GetType();

    }
    
    public static IBlock Get<T>() where T : IBlock, new()
    {
        IBlock block = null;

        if(typeBlockCache.TryGetValue(typeof(T), out block))
        {
            return block;
        }

        CacheBlock(new T());

        return typeBlockCache[typeof(T)];
    }

    public static IBlock Get(int id)
    {
        System.Type type;
        IBlock      block = null;

        if(idTypeCache.TryGetValue(id, out type))
        {
            if(typeBlockCache.TryGetValue(type, out block))
            {
                return block;
            }
        }

        return blockAir;
    }
 
    public static int GetId(IBlock block)
    {
        // cache this, also
        return ChunkUtil.GetHashCode(block.GetType().ToString());
    }

    public static int GetId<T>()
    {
        return ChunkUtil.GetHashCode(typeof(T).ToString());
    }

    public static readonly IBlock blockAir = new BlockAir();

}
