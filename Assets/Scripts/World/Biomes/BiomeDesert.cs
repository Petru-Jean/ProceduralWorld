using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiomeDesert : IBiome
{
    float biomeMaxHeight = 48.0f;//64.0f;
    
    public override IBlock GetBiomeBlockType()
    {
        return FlyweightBlock.Get<BlockSand>();
    }

    public override int[] GenerateHeightmap(Vector2 worldPos)
    {
        worldPos.x += 50000;
        
        int[] heightmap = new int[ChunkUtil.chunkWidth];

        for(int x = 0; x < ChunkUtil.chunkWidth; x++)
        {
            float noise = 0;

            float x1 = worldPos.x + x;
            noise = ChunkUtil.PerlinNoise((float)x1 / 1024.0f);
            noise += 0.5f * ChunkUtil.PerlinNoise((float)x1 / 128.0f);
            noise += 0.25f * ChunkUtil.PerlinNoise((float)x1 / 8.0f);

            noise = (noise / (1.0f + 0.5f + 0.25f)) * biomeMaxHeight;

            heightmap[x] = (int)noise;
        }
        
        return heightmap;

    }

    public override IBlock[,][] GenerateBlockData(ChunkData chunk, int[] heightmap, IBlock blendingBlock = null)
    {
        Vector2 worldPos   = new Vector2(chunk.transform.position.x, chunk.transform.position.y);
        IBlock[,][] blocks = ChunkUtil.InitBlockData();

        int[,] map = GenerateCaveCelularMap(worldPos, defaultCaveCelularMap);

        Hasher hasher         = new Hasher(worldPos, Hasher.HashType.BiomeBlendHash);
        Hasher vegetationHash = new Hasher(worldPos, Hasher.HashType.VegetationHash);

        for(int x = 0; x < ChunkUtil.chunkWidth; x++)
        {
            for(int y = 0; y < ChunkUtil.chunkHeight; y++)
            { 
                blocks[x,y][(int)ChunkData.BlockLayer.Block] = FlyweightBlock.blockAir;
                
                // Generate Terrain Surface
                if(worldPos.y == 0)
                {
                    blocks[x,y] [(int)ChunkData.BlockLayer.Block] = y <= heightmap[x] ? GetBiomeBlockType(): FlyweightBlock.blockAir;

                    if(blendingBlock != null && y<= heightmap[x])
                    {
                        float horizontalBlendChance = 1.0f - (float) (1.5f * (x+1) / (float)ChunkUtil.chunkWidth);

                        if(hasher.Next() <= horizontalBlendChance)
                        {
                            blocks[x,y][(int)ChunkData.BlockLayer.Block] = blendingBlock;
                        } 
                    }
                }   
                else if (worldPos.y == -ChunkUtil.chunkHeight)
                {
                    blocks[x,y][(int)ChunkData.BlockLayer.Block] = GetBiomeBlockType();

                    if (map[x, y] == 1)
                    {
                        blocks[x, y][(int)ChunkData.BlockLayer.Block] = FlyweightBlock.blockAir;
                    }

                    float verticalBlendChance = 1.0f - (float) ((float)y / (float)ChunkUtil.chunkHeight);

                    if(hasher.Next() <= verticalBlendChance)
                    {
                        blocks[x,y][(int)ChunkData.BlockLayer.Block] = FlyweightBlock.Get<BlockStone>();
                    } 

                    if(blendingBlock != null)
                    {
                        //blocks[x,y] = new BlockData(FlyweightBlock.Get<BlockDirt>());

                        float horizontalBlendChance = 1.0f - (float) (1.5f * (x+1) / (float)ChunkUtil.chunkWidth);

                        if(hasher.Next() <= horizontalBlendChance)
                        {
                            blocks[x,y][(int)ChunkData.BlockLayer.Block] = blendingBlock;
                        } 
                    }
                }
                else if(worldPos.y < 0)
                {
                    blocks[x,y][(int)ChunkData.BlockLayer.Block] = FlyweightBlock.Get<BlockStone>();

                    if(map[x, y] == 1)
                    {
                        blocks[x,y][(int)ChunkData.BlockLayer.Block] = FlyweightBlock.blockAir;
                    }   

                }
                else   
                {         
                    blocks[x,y][(int)ChunkData.BlockLayer.Block] = FlyweightBlock.blockAir;
                }

            }
        }

        if(worldPos.y == 0)
        {          
            for(int x = 0; x < ChunkUtil.chunkWidth; x++)
            {
                int y = ChunkUtil.chunkHeight - 1;
                
                bool hasGrass    = vegetationHash.Next() <= 0.25f;
                //bool defaultType = vegetationHash.Next() >= 0.5f;
                
                int height      = Mathf.Clamp(Mathf.RoundToInt(vegetationHash.Next() * 8), 1, 8);

                while(y > 0 && hasGrass)
                {
                    if(blocks[x, y-1][(int)ChunkData.BlockLayer.Block] != FlyweightBlock.blockAir)
                    {
                        for(int i = 0; i < height; i++)
                        {
                            if(y + i  >= ChunkUtil.chunkHeight)
                                break;

                            blocks[x, y + i][(int)ChunkData.BlockLayer.Block] = FlyweightBlock.Get<BlockCactus>();   
                        } 
                        
                        break; 
                    }

                    y--;
                }
                
            }
        }

        //if (worldPos.y != -ChunkUtil.chunkHeight)
        GenerateOres(worldPos, blocks, defaultOreVeinDistribution);

        return blocks;
    }

}
