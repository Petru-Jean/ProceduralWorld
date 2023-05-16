using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiomePlains : IBiome
{
    float biomeMaxHeight = 48.0f;

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
            noise = Mathf.PerlinNoise((float)x1 / 4096.0f,          0.0f);
            noise += 0.5f * Mathf.PerlinNoise((float)x1 / 8.0f,   0.0f);
            noise += 0.25f * Mathf.PerlinNoise((float)x1 / 1.0f,   0.0f);

            noise = (noise / (1.0f + 0.5f + 0.25f)) * biomeMaxHeight;

            heightmap[x] = (int)noise;
        }

        return heightmap;
    }

    public override BlockData[,] GenerateBlockData(Vector2 worldPos, int[] heightmap, IBlock blendingBlock = null)
    {
        BlockData[,] blocks = new BlockData[ChunkUtil.chunkWidth, ChunkUtil.chunkHeight];

        int[,] map = GenerateCaveHeightmap(worldPos, defaultCaveCAMapConfig);
        Hasher hasher = new Hasher(worldPos, Hasher.HashType.BiomeBlendHash);


        for(int x = 0; x < ChunkUtil.chunkWidth; x++)
        {
            for(int y = 0; y < ChunkUtil.chunkHeight; y++)
            { 
                if(worldPos.y == 0)
                {
                    blocks[x,y] = y <= heightmap[x] ? new BlockData(FlyweightBlock.Get<BlockStone>()) : FlyweightBlock.blockDataAir;
                    
                    if(blendingBlock != null && y<= heightmap[x])
                    {
                        float horizontalBlendChance = 1.0f - (float) ((float)x / (float)ChunkUtil.chunkWidth);

                        if(hasher.Next() <= horizontalBlendChance)
                        {
                            blocks[x,y] = new BlockData(blendingBlock);
                        } 
                    }
                }   
                else if (worldPos.y == -ChunkUtil.chunkHeight)
                {
                    blocks[x,y] = new BlockData(GetBiomeBlockType());
                    
                    if(map[x, y] == 1)
                    {
                        blocks[x,y] = FlyweightBlock.blockDataAir;
                    }   

                    if(blendingBlock != null)
                    {
                        blocks[x,y] = new BlockData(GetBiomeBlockType());

                        float horizontalBlendChance = 1.0f - (float) ((float)x / (float)ChunkUtil.chunkWidth);

                        if(hasher.Next() <= horizontalBlendChance)
                        {
                            blocks[x,y] = new BlockData(blendingBlock);
                        } 

                        float verticalBlendChance = 1.0f - (float) ((float)y / (float)ChunkUtil.chunkHeight);

                        if(hasher.Next() <= verticalBlendChance)
                        {
                            blocks[x,y] = new BlockData(GetBiomeBlockType());
                        } 

                    }

                }
                else if(worldPos.y < 0)
                {
                    blocks[x,y] = new BlockData(FlyweightBlock.Get<BlockStone>());//new BlockData(FlyweightBlock.Get<block>()); 

                    if(map[x, y] == 1)
                        blocks[x,y] = FlyweightBlock.blockDataAir;
                }
                else   
                {
                    blocks[x,y] = FlyweightBlock.blockDataAir;
                }

            }
        }

        GenerateOres(worldPos, blocks, defaultOreDistrib);

        return blocks;
    }

}
