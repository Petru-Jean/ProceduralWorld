using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiomePlains : IBiome
{
    float biomeMaxHeight = 48.0f;//64.0f;//48.0f;

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

    public override IBlock[,][] GenerateBlockData(ChunkData chunk, int[] heightmap, IBlock blendingBlock = null)
    {
        IBlock[,][] blocks = ChunkUtil.InitBlockData();
        Vector2 worldPos   = new Vector2(chunk.transform.position.x, chunk.transform.position.y);

        int[,] map = GenerateCaveCelularMap(worldPos, defaultCaveCelularMap);
        Hasher hasher = new Hasher(worldPos, Hasher.HashType.BiomeBlendHash);
        
        Hasher treeHash = new Hasher(worldPos, Hasher.HashType.TreeHash);

        int leftTreex = int.MinValue;

        for(int x = 0; x < ChunkUtil.chunkWidth; x++)
        {
            for(int y = 0; y < ChunkUtil.chunkHeight; y++)
            { 
                blocks[x,y][(int)ChunkData.BlockLayer.Block] = FlyweightBlock.blockAir;
                

                if(worldPos.y == 0)
                {
                    blocks[x,y][(int)ChunkData.BlockLayer.Block] = y <= heightmap[x] ? (FlyweightBlock.Get<BlockStone>()) : FlyweightBlock.blockAir;
                    
                    if(blendingBlock != null && y<= heightmap[x])
                    {
                        float horizontalBlendChance = 1.0f - (float) ((float)x / (float)ChunkUtil.chunkWidth);

                        if(hasher.Next() <= horizontalBlendChance)
                        {
                            blocks[x,y][(int)ChunkData.BlockLayer.Block] = (blendingBlock);
                        } 
                    }

                    if(x > 0 && x < ChunkUtil.chunkWidth - 1 && x >= leftTreex + 9)
                    {
                        if((y-1 <= heightmap[x-1] && y > heightmap[x-1]) && (y-1 <= heightmap[x] && y > heightmap[x]) &&
                         (y-1 <= heightmap[x+1] && y > heightmap[x+1]))
                        {
                            if(treeHash.Next() <= 0.1f)
                            {
                                // GameObject treeObj = CreateTree(new Vector2(worldPos.x + 0.5f, worldPos.y + 4.15f) + new Vector2(x,y));
                                // chunk.RegisterTree(treeObj);
                                
                                // leftTreex = x;
                            }                  
                        }
                    }
                }   
                else if (worldPos.y == -ChunkUtil.chunkHeight)
                {
                    blocks[x,y][(int)ChunkData.BlockLayer.Block] = (GetBiomeBlockType());
                    
                    if(map[x, y] == 1)
                    {
                        blocks[x,y][(int)ChunkData.BlockLayer.Block] = FlyweightBlock.blockAir;
                    }   

                    if(blendingBlock != null)
                    {
                        blocks[x,y][(int)ChunkData.BlockLayer.Block] = (GetBiomeBlockType());

                        float horizontalBlendChance = 1.0f - (float) ((float)x / (float)ChunkUtil.chunkWidth);

                        if(hasher.Next() <= horizontalBlendChance)
                        {
                            blocks[x,y][(int)ChunkData.BlockLayer.Block] = (blendingBlock);
                        } 

                        float verticalBlendChance = 1.0f - (float) ((float)y / (float)ChunkUtil.chunkHeight);

                        if(hasher.Next() <= verticalBlendChance)
                        {
                            blocks[x,y][(int)ChunkData.BlockLayer.Block] = (GetBiomeBlockType());
                        } 

                    }

                }
                else if(worldPos.y < 0)
                {
                    blocks[x,y][(int)ChunkData.BlockLayer.Block] = (FlyweightBlock.Get<BlockStone>());//new BlockData(FlyweightBlock.Get<block>()); 

                    if(map[x, y] == 1)
                        blocks[x,y][(int)ChunkData.BlockLayer.Block] = FlyweightBlock.blockAir;
                }
                else   
                {
                    blocks[x,y][(int)ChunkData.BlockLayer.Block] = FlyweightBlock.blockAir;
                }

            }
        }

        GenerateOres(worldPos, blocks, defaultOreVeinDistribution);

        return blocks;
    }

}
