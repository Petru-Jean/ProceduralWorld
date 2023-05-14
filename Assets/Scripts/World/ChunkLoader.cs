using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public static class ChunkLoader
{
    public static bool Exists(Vector2 pos)
    {
        return File.Exists(ChunkUtil.PosToFileName(pos));
    }

    public static BlockData[,] Load(Vector2 pos)
    {
        BlockData[,] blocks = null;
      
        if(Exists(pos))
        {
            blocks = new BlockData[ChunkUtil.chunkWidth, ChunkUtil.chunkHeight];

            using(var stream = File.Open(ChunkUtil.PosToFileName(pos), FileMode.Open))
            {
                using(var reader = new BinaryReader(stream))
                {   
                    for(int x = 0; x < ChunkUtil.chunkWidth; x++)
                    {
                        for(int y = 0; y < ChunkUtil.chunkHeight; y++)
                        {
                            //ushort readVal = reader.ReadUInt16();

                            // if(readVal == ushort.MaxValue)
                            //     break;
                            
                            IBlock block = FlyweightBlock.Get(reader.ReadUInt16());
                            
                            blocks[x, y] = new BlockData(block);
                        }
                    }
                }

            }
        }

        return blocks;
    }
}
