using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public static class ChunkLoader
{
    public static IBlock[,][] Load(Vector2 pos)
    {
        if(!File.Exists(ChunkUtil.ChunkPosToSavePath(pos)))
            return null;

        IBlock[,][] blocks = ChunkUtil.InitBlockData();
        
        using(var stream = File.Open(ChunkUtil.ChunkPosToSavePath(pos), FileMode.Open))
        {
            using(var reader = new BinaryReader(stream))
            {   

                for (int x = 0; x < ChunkUtil.chunkWidth; x++)
                {
                    for (int y = 0; y < ChunkUtil.chunkHeight; y++)
                    {
                        IBlock wall  = FlyweightBlock.Get(reader.ReadInt32());
                        IBlock block = FlyweightBlock.Get(reader.ReadInt32());

                        blocks[x, y][(int)ChunkData.BlockLayer.Wall]  = wall;
                        blocks[x, y][(int)ChunkData.BlockLayer.Block] = block;
                    }
                }
                

            }
        }
        
        return blocks;
    }
}
