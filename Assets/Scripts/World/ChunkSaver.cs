using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public static class ChunkSaver
{   
    public static void Save(Vector2 pos, Chunk chunk)
    {
        IBlock[,] blocks = chunk.GetBlocks();

        string fileName = ChunkUtil.PosToFileName(pos);

        if(File.Exists(fileName))  
             System.IO.File.WriteAllText(fileName, string.Empty);


        using(var stream = File.Open(fileName, FileMode.OpenOrCreate))
        {
            using(var writer = new BinaryWriter(stream))
            {   
                foreach(IBlock block in blocks)
                {
                    writer.Write(FlyweightBlock.GetId(block));
                }
            }
        }
    }

    
}
