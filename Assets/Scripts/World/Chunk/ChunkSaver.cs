using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public static class ChunkSaver
{   
    public static void Save(Vector2 pos, ChunkData chunk)
    {   
        System.IO.Directory.CreateDirectory("Saves/");
        System.IO.Directory.CreateDirectory(ChunkUtil.GetChunkRegionPath(pos));

        string fileName = ChunkUtil.ChunkPosToSavePath(pos);
        
        if (File.Exists(fileName))
        {
            System.IO.File.WriteAllText(fileName, string.Empty);
        }

        IBlock[,][] blocks = chunk.GetBlocks();
        
        using(var stream = File.Open(fileName, FileMode.OpenOrCreate))
        { 
            using(var writer = new BinaryWriter(stream))
            {

                for (int x = 0; x < ChunkUtil.chunkWidth; x++)
                {
                    for (int y = 0; y < ChunkUtil.chunkHeight; y++)
                    {
                        writer.Write(FlyweightBlock.GetId(blocks[x, y][(int)ChunkData.BlockLayer.Wall]));
                        writer.Write(FlyweightBlock.GetId(blocks[x, y][(int)ChunkData.BlockLayer.Block]));
                    }
                }
          
            }
        }

    }

    
}
