using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiomePlains : IBiome
{
    float biomeMaxHeight = 64.0f;//48.0f;

    public override IBlock GetBiomeBlockType()
    {
        return FlyweightBlock.Get<BlockStone>();
    }

    public override int[] GenerateHeightmap(Vector2 worldPos)
    {
        worldPos.x += 50000;
        int[] heightmap = new int[ChunkUtil.chunkWidth];

        for(int x = 0; x < ChunkUtil.chunkWidth; x++)
        {
            float noise = 0;

            float x1 = worldPos.x + x;
            //noise = Mathf.PerlinNoise((float)x1 / 4096.0f,          0.0f);
            noise = ChunkUtil.PerlinNoise((float)x1 / 8192.0f);
            noise += 0.5f * ChunkUtil.PerlinNoise((float)x1 / 8.0f);
            noise += 0.25f * ChunkUtil.PerlinNoise((float)x1 / 1.0f);

            noise = (noise / (1.0f + 0.5f + 0.25f)) * biomeMaxHeight;

            heightmap[x] = (int)noise;
        }

        return heightmap;
    }

    public override IBlock[,] GenerateBlockData(Chunk chunk, Vector2 worldPos, int[] heightmap, IBlock blendingBlock = null)
    {
        IBlock[,] blocks = new IBlock[ChunkUtil.chunkWidth, ChunkUtil.chunkHeight];

        int[,] map = GenerateCaveHeightmap(worldPos, defaultCaveCAMapConfig);
        Hasher hasher = new Hasher(worldPos, Hasher.HashType.BiomeBlendHash);


        for(int x = 0; x < ChunkUtil.chunkWidth; x++)
        {
            for(int y = 0; y < ChunkUtil.chunkHeight; y++)
            { 
                blocks[x,y] = FlyweightBlock.blockAir;
                

                if(worldPos.y == 0)
                {
                    blocks[x,y] = y <= heightmap[x] ? (FlyweightBlock.Get<BlockStone>()) : FlyweightBlock.blockAir;
                    
                    if(blendingBlock != null && y<= heightmap[x])
                    {
                        float horizontalBlendChance = 1.0f - (float) ((float)x / (float)ChunkUtil.chunkWidth);

                        if(hasher.Next() <= horizontalBlendChance)
                        {
                            blocks[x,y] = (blendingBlock);
                        } 
                    }
                }   
                else if (worldPos.y == -ChunkUtil.chunkHeight)
                {
                    blocks[x,y] = (GetBiomeBlockType());
                    
                    if(map[x, y] == 1)
                    {
                        blocks[x,y] = FlyweightBlock.blockAir;
                    }   

                    if(blendingBlock != null)
                    {
                        blocks[x,y] = (GetBiomeBlockType());

                        float horizontalBlendChance = 1.0f - (float) ((float)x / (float)ChunkUtil.chunkWidth);

                        if(hasher.Next() <= horizontalBlendChance)
                        {
                            blocks[x,y] = (blendingBlock);
                        } 

                        float verticalBlendChance = 1.0f - (float) ((float)y / (float)ChunkUtil.chunkHeight);

                        if(hasher.Next() <= verticalBlendChance)
                        {
                            blocks[x,y] = (GetBiomeBlockType());
                        } 

                    }

                }
                else if(worldPos.y < 0)
                {
                    blocks[x,y] = (FlyweightBlock.Get<BlockStone>());//new BlockData(FlyweightBlock.Get<block>()); 

                    if(map[x, y] == 1)
                        blocks[x,y] = FlyweightBlock.blockAir;
                }
                else   
                {
                    blocks[x,y] = FlyweightBlock.blockAir;
                }

            }
        }

        GenerateOres(worldPos, blocks, defaultOreDistrib);

        return blocks;
    }

}
