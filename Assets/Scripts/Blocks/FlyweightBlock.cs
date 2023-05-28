using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public static class FlyweightBlock
{   
    private static Dictionary<System.Type, IBlock>    blockCache;
    private static Dictionary<System.Type, ushort>    typeIdCache;
    private static Dictionary<ushort, System.Type>    idTypeCache;
    
    private static ushort lastId = 0;

    static FlyweightBlock()
    {
        blockCache   = new Dictionary<System.Type, IBlock>();
        typeIdCache  = new Dictionary<System.Type, ushort>();
        idTypeCache  = new Dictionary<ushort, System.Type>();

        if(!File.Exists("Blocks.txt"))
        {
            File.Create("Blocks.txt").Close();
        }

        foreach(string row in File.ReadLines("Blocks.txt"))
        {
            string[] data = row.Split(" ");
            
            if(data.Length != 2)
            {
                Debug.LogError("Corrupt block-id map data");
                break;
            }

            System.Type type = System.Type.GetType(data[0]);
            lastId           = ushort.Parse(data[1]);

            AddTypeIdPair(type, lastId);
        }

        // AddTypeIdPair(typeof(BlockAir),   0);
        // AddTypeIdPair(typeof(BlockDirt),  1);
        // AddTypeIdPair(typeof(BlockGrassWeed), 2);
        // AddTypeIdPair(typeof(BlockStone), 3);
        // AddTypeIdPair(typeof(BlockGold),  4);
        // AddTypeIdPair(typeof(BlockDirt),  5);
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

        return blockAir;
    }

    public static ushort GetId(IBlock block)
    {
        ushort id = 0;
        
        if(typeIdCache.TryGetValue(block.GetType(), out id))
        {
            return id;
        }
        else
        {  
            using (StreamWriter sw = !File.Exists("Blocks.txt") ?  File.CreateText("Blocks.txt") : File.AppendText("Blocks.txt"))
            {
                lastId++;

                sw.WriteLine(block.GetType() + " " + lastId);
                AddTypeIdPair(block.GetType(), lastId);

                return lastId;
            }
        }
        
    }

    public static ushort GetId<T>() where T : IBlock
    {
        ushort id = 0;
        
        if(typeIdCache.TryGetValue(typeof(T), out id))
        {
            return id;
        }
        else
        {  
            using (StreamWriter sw = !File.Exists("Blocks.txt") ?  File.CreateText("Blocks.txt") : File.AppendText("Blocks.txt"))
            {
                lastId++;

                sw.WriteLine(typeof(T) + " " + lastId);
                AddTypeIdPair(typeof(T), lastId);

                return lastId;
            }
        }

    }

    public static readonly IBlock blockAir = new BlockAir();
}
