using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiomeHills : IBiome
{
    float biomeMaxHeight = 48.0f;

    public override IBlock GetBiomeBlockType()
    {
        return FlyweightBlock.Get<BlockDirt>();
    }

    public override int[] GenerateHeightmap(Vector2 worldPos)
    {
        worldPos.x += 50000;
        
        int[] heightmap = new int[ChunkUtil.chunkWidth];

        for(int x = 0; x < ChunkUtil.chunkWidth; x++)
        {
            float noise = 0;

            float x1 = worldPos.x + x;
            noise = Mathf.PerlinNoise((float)x1 / 1024.0f,          0.0f);
            noise += 0.5f * Mathf.PerlinNoise((float)x1 / 128.0f,   0.0f);
            noise += 0.25f * Mathf.PerlinNoise((float)x1 / 16.0f, 0.0f);

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
                    blocks[x,y] = y <= heightmap[x]? new BlockData(FlyweightBlock.Get<BlockDirt>()) : FlyweightBlock.blockDataAir;

                    if(blendingBlock != null && y<= heightmap[x])
                    {
                        float horizontalBlendChance = 1.0f - (float) (1.5f * (x+1) / (float)ChunkUtil.chunkWidth);

                        if(hasher.Next() <= horizontalBlendChance)
                        {
                            blocks[x,y] = new BlockData(blendingBlock);
                        } 
                    }

                }   
                else if (worldPos.y == -ChunkUtil.chunkHeight)
                {
                    blocks[x,y] = new BlockData(FlyweightBlock.Get<BlockDirt>());
                    
                    if(map[x, y] == 1)
                    {
                        blocks[x,y] = FlyweightBlock.blockDataAir;
                    }   

                    float verticalBlendChance = 1.0f - (float) ((float)y / (float)ChunkUtil.chunkHeight);

                    if(hasher.Next() <= verticalBlendChance)
                    {
                        blocks[x,y] = new BlockData(FlyweightBlock.Get<BlockStone>());
                    } 

                    if(blendingBlock != null)
                    {
                        //blocks[x,y] = new BlockData(FlyweightBlock.Get<BlockDirt>());

                        float horizontalBlendChance = 1.0f - (float) (1.5f * (x+1) / (float)ChunkUtil.chunkWidth);

                        if(hasher.Next() <= horizontalBlendChance)
                        {
                            blocks[x,y] = new BlockData(blendingBlock);
                        } 
                    }
                }
                else if(worldPos.y < 0)
                {
                    blocks[x,y] = new BlockData(FlyweightBlock.Get<BlockStone>());

                    if(map[x, y] == 1)
                    {
                        blocks[x,y] = FlyweightBlock.blockDataAir;
                    }   

                }
                else   
                {         
                    blocks[x,y] = FlyweightBlock.blockDataAir;
                }

            }
        }

        //if (worldPos.y != -ChunkUtil.chunkHeight)
            GenerateOres(worldPos, blocks, defaultOreDistrib);


        if(worldPos.y > 0)
        {
            // if(UnityEngine.Random.Range(0,3) == 0)
            // {
            //     TreeDataGenerator.Tree tree = TreeDataGenerator.Generate(worldPos, 0);

            //     for(int i = 0; i < tree.height; i++)
            //     {
            //         blocks[16, i] = new BlockData(FlyweightBlock.Get<BlockTemp4>());

            //         if(i == 0) blocks[16, 0] = new BlockData(FlyweightBlock.Get<BlockTemp4>());
            //         if(i == tree.height - 1) blocks[16, tree.height-1] = new BlockData(FlyweightBlock.Get<BlockTemp1>());

            //         for(int j = 0; j < tree.leftBranches.Count; j++)
            //         {
            //             blocks[15, tree.leftBranches[j]] = new BlockData(FlyweightBlock.Get<BlockTemp2>()); 
            //         }

            //         for(int j = 0; j < tree.rightBranches.Count; j++)
            //         {
            //             blocks[17, tree.rightBranches[j]] = new BlockData(FlyweightBlock.Get<BlockTemp3>()); 
            //         }
                    
            //     }
            // }
        }

        return blocks;
    }

}
