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
            noise = ChunkUtil.PerlinNoise((float)x1 / 1024.0f);
            noise += 0.5f * ChunkUtil.PerlinNoise((float)x1 / 128.0f);
            noise += 0.25f * ChunkUtil.PerlinNoise((float)x1 / 16.0f);

            noise = (noise / (1.0f + 0.5f + 0.25f)) * biomeMaxHeight;

            heightmap[x] = (int)noise;
        }
        
        return heightmap;
    }

    public override IBlock[,] GenerateBlockData(Chunk chunk, Vector2 worldPos, int[] heightmap, IBlock blendingBlock = null)
    {
        IBlock[,] blocks = new IBlock[ChunkUtil.chunkWidth, ChunkUtil.chunkHeight];
        IBlock[,] walls  = new IBlock[ChunkUtil.chunkWidth, ChunkUtil.chunkHeight];

        int[,] map = GenerateCaveHeightmap(worldPos, defaultCaveCAMapConfig);

        Hasher hasher   = new Hasher(worldPos, Hasher.HashType.BiomeBlendHash);
        Hasher treeHash = new Hasher(worldPos, Hasher.HashType.TreeHash);
        Hasher vegetationHash = new Hasher(worldPos, Hasher.HashType.VegetationHash);

        int leftTreex = int.MinValue;

        for(int x = 0; x < ChunkUtil.chunkWidth; x++)
        {
            for(int y = 0; y < ChunkUtil.chunkHeight; y++)
            { 
                blocks[x,y] = FlyweightBlock.blockAir;

                if(worldPos.y == 0)
                {
                    blocks[x,y] = y <= heightmap[x] ? FlyweightBlock.Get<BlockDirt>() : FlyweightBlock.blockAir;

                    if(blendingBlock != null && y<= heightmap[x])
                    {
                        float horizontalBlendChance = 1.0f - (float) (1.5f * (x+1) / (float)ChunkUtil.chunkWidth);

                        if(hasher.Next() <= horizontalBlendChance)
                        {
                            blocks[x,y] = blendingBlock;
                        } 
                    }

                    if(x > 0 && x < ChunkUtil.chunkWidth - 1 && x >= leftTreex + 9)
                    {
                        if((y-1 <= heightmap[x-1] && y > heightmap[x-1]) && (y-1 <= heightmap[x] && y > heightmap[x]) && (y-1 <= heightmap[x+1] && y > heightmap[x+1]))
                        {
                            if(treeHash.Next() <= 0.1f)
                            {
                                GameObject treeObj = GameObject.Instantiate((GameObject) Resources.Load("Textures/Tree"), new Vector3(worldPos.x + x + 0.5f, worldPos.y + y + 4.15f, 1), Quaternion.identity);
                                treeObj.transform.tag = "Tree";
                                treeObj.layer         = LayerMask.NameToLayer("Foreground"); //"Foreground";
                                chunk.RegisterTree(treeObj);
                                
                                leftTreex = x;
                            }                  
                        }
                    }
                    
                }   
                else if (worldPos.y == -ChunkUtil.chunkHeight)
                {
                    blocks[x,y] = FlyweightBlock.Get<BlockDirt>();
                    
                    if(map[x, y] == 1)
                    {
                        blocks[x,y] = FlyweightBlock.blockAir;
                        chunk.SetWall(x,y, FlyweightBlock.Get<BlockWallDirt>(), false);
                    }   

                    float verticalBlendChance = 1.0f - (float) ((float)y / (float)ChunkUtil.chunkHeight);

                    if(hasher.Next() <= verticalBlendChance)
                    {
                        blocks[x,y] = FlyweightBlock.Get<BlockStone>();
                    } 

                    if(blendingBlock != null)
                    {
                        //blocks[x,y] = new BlockData(FlyweightBlock.Get<BlockDirt>());

                        float horizontalBlendChance = 1.0f - (float) (1.5f * (x+1) / (float)ChunkUtil.chunkWidth);

                        if(hasher.Next() <= horizontalBlendChance)
                        {
                            blocks[x,y] = blendingBlock;
                        } 
                    }
                }
                else if(worldPos.y < 0)
                {
                    blocks[x,y] = FlyweightBlock.Get<BlockStone>();

                    if(map[x, y] == 1)
                    {
                        blocks[x,y] = FlyweightBlock.blockAir;
                        chunk.SetWall(x,y, FlyweightBlock.Get<BlockWallDirt>(), false);
                    }   

                }
                else   
                {         
                    blocks[x,y] = FlyweightBlock.blockAir;
                }

            }
        }

        if(worldPos.y == 0)
        {          
            for(int x = 0; x < ChunkUtil.chunkWidth; x++)
            {
                int y = ChunkUtil.chunkHeight - 1;
                
                bool hasGrass    = vegetationHash.Next() <= 0.25f;
                bool defaultType = vegetationHash.Next() >= 0.5f;

                while(y > 0 && hasGrass)
                {
                    if(blocks[x, y-1] != FlyweightBlock.blockAir)
                    {
                        blocks[x, y] = defaultType ? FlyweightBlock.Get<BlockGrassWeed>() : FlyweightBlock.Get<BlockFlower>();    
                        break; 
                    }

                    y--;
                }
                
            }
        }

        //if (worldPos.y != -ChunkUtil.chunkHeight)
        GenerateOres(worldPos, blocks, defaultOreDistrib);

        return blocks;
    }

}
