using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public static class ChunkSaver
{   
    public static void Save(Vector2 pos, BlockData[,] blocks)
    {
        return;
        
        string fileName = ChunkUtil.PosToFileName(pos);

        if(File.Exists(fileName))  
             System.IO.File.WriteAllText(fileName, string.Empty);

        using(var stream = File.Open(fileName, FileMode.OpenOrCreate))
        {
            using(var writer = new BinaryWriter(stream))
            {   
                foreach(BlockData data in blocks)
                {
                    writer.Write(FlyweightBlock.GetId(data.block));
                }
            }
        }
    }

    public static void Save(Vector2 pos, Chunk chunk)
    {
        Save(pos, chunk.GetBlocks());
    }   
    
}
