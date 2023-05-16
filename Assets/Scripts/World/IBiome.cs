using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IBiome
{   
    public abstract int[]        GenerateHeightmap(Vector2 worldPos);
    public abstract BlockData[,] GenerateBlockData(Vector2 worldPos, int[] heightmap, IBlock blendingBlock = null);
    public abstract IBlock       GetBiomeBlockType();

    public struct CAMapConfig
    {
        public int mapWidth, mapHeight;
        public int iterations;
        public float caMin, caMax;
    };

    public struct OreBlockConfig
    {
        public CAMapConfig mapConfig;

        public IBlock block;
        public float minDepth, maxDepth;
        public float chance;

    }

    protected static readonly CAMapConfig defaultCaveCAMapConfig = new CAMapConfig()
    {
        mapWidth  = ChunkUtil.chunkWidth,
        mapHeight = ChunkUtil.chunkHeight,

        iterations = 4,

        caMin = 0.4f,
        caMax = 0.53f
    };

    protected static readonly CAMapConfig defaultOreCAMapConfig = new CAMapConfig()
    {
        mapWidth  = 10,
        mapHeight = 10,

        iterations = 6,

        caMin = 0.55f,
        caMax = 0.75f
    };
    
    protected List<OreBlockConfig> defaultOreDistrib = new List<OreBlockConfig>()
    {
        // Stone needs to be first
        // before implementing priority system
        // otherwise most of ores get overriten

        new OreBlockConfig()
        {
            mapConfig = new CAMapConfig()
            {
                mapWidth  = (ChunkUtil.chunkWidth -  1),
                mapHeight = (ChunkUtil.chunkHeight - 1),

                iterations = 5,

                caMin = 0.33f,
                caMax = 0.41f,
            },

            block = FlyweightBlock.Get<BlockDirt>(),
            minDepth = -5, 
            maxDepth = ChunkUtil.MinY,
            chance   = 0.2f
        },

        new OreBlockConfig()
        {
            mapConfig = defaultOreCAMapConfig,
            block     = FlyweightBlock.Get<BlockGold>(),
            minDepth  = -200,
            maxDepth  = -1500,
            chance    = 1.0f / 9.0f
        },

        new OreBlockConfig()
        {
            mapConfig = defaultOreCAMapConfig,
            block = FlyweightBlock.Get<BlockIron>(),
            minDepth = -40,
            maxDepth = -500,
            chance   = 1.0f / 3.0f
        },

    };

    private int[,] GenerateCAMap(CAMapConfig config, int[,] blocks)
    {
        for(int t = 0; t < config.iterations; t++)
        {
            for(int x = 0; x < config.mapWidth; x++)
            {
                for(int y = 0; y < config.mapHeight; y++)
                {
                    int nv = (y + 1 >= config.mapHeight || x - 1 < 0) ? 0 : blocks[x-1,y+1];
                    int n =  (y + 1 >= config.mapHeight) ? 0 : blocks[x,y+1];
                    int ne = (y + 1 >= config.mapHeight || x + 1 >= config.mapWidth) ? 0 : blocks[x+1,y+1];

                    int v = (x - 1 <  0) ? 0 : blocks[x - 1,   y];
                    int e = (x + 1 >= config.mapWidth) ? 0 :   blocks[x + 1,   y];
                    
                    int sv = (y - 1 < 0 || x - 1 < 0) ? 0 : blocks[x-1,y-1];
                    int s  = (y - 1 < 0) ? 0 : blocks[x, y-1];
                    int se = (y - 1 < 0 || x + 1 >= config.mapWidth) ? 0 : blocks[x+1,y-1];

                    int numNeighbours = (nv+n+ne+e+v+sv+s+se);

                    if(numNeighbours >= 6 && numNeighbours <= 8) blocks[x,y] = 1;
                    else if(numNeighbours <= 2) blocks[x,y] = 0;

                }
            }
        }

        return blocks;
    }   
    
    public  int[,] GenerateOreHeightmap(Vector2 worldPos, CAMapConfig config)
    {
        int[,] blocks = new int[config.mapWidth, config.mapHeight];
        Hasher hash   = new Hasher(worldPos, Hasher.HashType.OreHash);

        float chance = Mathf.Clamp(hash.Next(), config.caMin, config.caMax);

        for(int x = 0; x < config.mapWidth; x++)
        {
            for(int y = 0; y < config.mapHeight; y++)
            {
                blocks[x,y] = 0;
                
                if(hash.Next() <= chance)
                {
                    blocks[x,y] = 1;
                }
         
            }
        }

        return GenerateCAMap(config, blocks);
    }

    public int[,] GenerateCaveHeightmap(Vector2 worldPos, CAMapConfig config)
    {
        int[,] blocks = new int[config.mapWidth, config.mapHeight];
        Hasher hash   = new Hasher(worldPos, Hasher.HashType.CaveHash);

        // chance of adding additional size to the ca map
        // how large can the increase be range(caExtraMin, caExtraMax) * TotalPossibleSize

        // 20% chance of adding extra size to the ca map
        float caExtra = 0.2f;
        float caDiff  = config.caMax - config.caMin;

        float distribution = Mathf.Abs(worldPos.y / (float)ChunkUtil.MinY);
        distribution       = config.caMin + (distribution * caDiff);
        
        // add a chance for slightly larger caves
        // as to create less uniformity
        if(hash.Next() <= caExtra)
        {
            distribution += Mathf.Clamp(hash.Next() * caDiff, 0.15f * caDiff, 0.25f * caDiff);
        }

        distribution = Mathf.Clamp(distribution, config.caMin, config.caMax);

        for(int x = 0; x < config.mapWidth; x++)
        {
            for(int y = 0; y < config.mapHeight; y++)
            {
                blocks[x,y] = 0;
                
                if(hash.Next() <= distribution)
                {
                    blocks[x,y] = 1;
                }
         
            }
        }

        return GenerateCAMap(config, blocks);
    }
    
    public void GenerateOres(Vector2 worldPos, BlockData[,] blocks, List<OreBlockConfig> oreDistribution)
    {        
        Hasher hasher       = new Hasher(worldPos, Hasher.HashType.OreBlockHash);
        
        foreach(OreBlockConfig oreDistrib in oreDistribution)
        {
            if( !(worldPos.y <= oreDistrib.minDepth && worldPos.y >= oreDistrib.maxDepth) )
                continue;
                
            if(hasher.Next() <= oreDistrib.chance)
            {
                Vector2Int goldBlockPos = Vector2Int.zero;
                int[,]     goldOreMap   = new int[oreDistrib.mapConfig.mapWidth, oreDistrib.mapConfig.mapHeight];
                
                goldOreMap   = GenerateOreHeightmap(worldPos, oreDistrib.mapConfig);
                goldBlockPos = new Vector2Int( (int) (hasher.Next() * ChunkUtil.chunkWidth),(int) (hasher.Next() * ChunkUtil.chunkHeight));
                
                goldBlockPos.x = Mathf.Clamp(goldBlockPos.x, 0, ChunkUtil.chunkWidth  - 1 - oreDistrib.mapConfig.mapWidth);
                goldBlockPos.y = Mathf.Clamp(goldBlockPos.y, 0, ChunkUtil.chunkHeight - 1 - oreDistrib.mapConfig.mapHeight);
            
                for(int i = 0; i < oreDistrib.mapConfig.mapWidth; i++)
                {
                    for(int j = 0; j < oreDistrib.mapConfig.mapHeight; j++)
                    {
                        if(goldOreMap[i, j] == 1)
                        {
                            blocks[goldBlockPos.x + i, goldBlockPos.y + j] = new BlockData(oreDistrib.block);
                        }
                    }
                } 

                // if we return here theres one one ore vein per chunk
                //return;
            }       
        }
    }
    
    public int[] BlendHeightmapData(int[] heightmap1, int[] heightmap2)
    {
        int[] blockData = new int[ChunkUtil.chunkWidth];

        float weight = 0.01f;

        for(int i = 0; i < heightmap1.Length; i++)
        {   
            if(i != 0) 
            {
                weight = (float)i / (float)heightmap1.Length;
            }

            blockData[i] = (int) ((heightmap1[i] + (heightmap2[heightmap1.Length - i - 1] * weight)) / (1.0f + weight));

        }

        return blockData;
    }
}
