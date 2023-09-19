using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IBiome
{   
    public abstract int[]        GenerateHeightmap(Vector2 worldPos);
    public abstract IBlock[,][]  GenerateBlockData(ChunkData chunk, int[] heightmap, IBlock blendingBlock = null);
    public abstract IBlock       GetBiomeBlockType();

    // Represents a generic Celular Automation map, used for generating any type of structure: ore vein, cave, ...
    public struct CelularMapConfig
    {
        public int mapWidth, mapHeight;

        // Chaning the iterations param differs greatly based on the generation rules used.
        public int iterations;

        // The minimum and maximum surface area occupied by cells (0 - 100%)
        public float caMin, caMax;
    };

    // Blueprint for generating ore veins in a Chunk.
    public struct OreVein
    {
        // Dimensions and shape of the ore vein is determined by the Celular Map config
        public CelularMapConfig mapConfig;

        public IBlock block;

        public float minDepth, maxDepth;
        
        // Chance for the ore vein to spawn in a given chunk (0 - 100%)
        public float chance;
    }

    // Configuration of the default cave structure that is generated within chunks
    protected static readonly CelularMapConfig defaultCaveCelularMap      = new CelularMapConfig()
    {
        mapWidth  = ChunkUtil.chunkWidth,
        mapHeight = ChunkUtil.chunkHeight,

        iterations = 4,

        caMin = 0.4f,
        caMax = 0.53f
    };

    protected static readonly CelularMapConfig defaultOreVeinCelularMap   = new CelularMapConfig()
    {
        mapWidth  = 10,
        mapHeight = 10,

        iterations = 6,

        caMin = 0.50f,
        caMax = 0.70f
    };
    
    protected static readonly List<OreVein>    defaultOreVeinDistribution = new List<OreVein>()
    {
        // Stone needs to be first
        // before implementing priority system
        // otherwise most of ores get overriten
        // ---> Remove this comment ?

        new OreVein()
        {
            mapConfig = new CelularMapConfig()
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

        new OreVein()
        {
            mapConfig = defaultOreVeinCelularMap,
            block     = FlyweightBlock.Get<BlockGold>(),
            minDepth  = -200,
            maxDepth  = -1500,
            chance    = 1.0f / 9.0f
        },

        new OreVein()
        {
            mapConfig = defaultOreVeinCelularMap,
            block = FlyweightBlock.Get<BlockIron>(),
            minDepth = -40,
            maxDepth = -500,
            chance   = 1.0f / 3.0f
        },

    };

    /// <summary>
    /// Generates a Celular Automation map by iterating the rule set 
    /// on the provided heightmap
    /// </summary>
    /// <param name="config">Structure of the Celular Map</param>
    /// <param name="blocks">Initial heightmap that that is iteratively evolved</param>
    /// <returns></returns>
    private int[,] GenerateCelularMap(CelularMapConfig config, int[,] blocks)
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

    /// <summary>
    /// Generates the Celular Map for Ore Veins corresponding to the provided world position.
    /// </summary>
    /// <param name="worldPos"></param>
    /// <param name="config"></param>
    /// <returns></returns>
    public int[,] GenerateOreCelularMap(Vector2 worldPos, CelularMapConfig config)
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

        return GenerateCelularMap(config, blocks);
    }

    /// <summary>
    /// Generates the Celular Map for Caves corresponding to the provided world position.
    /// </summary>
    /// <param name="worldPos"></param>
    /// <param name="config"></param>
    /// <returns></returns>
    public int[,] GenerateCaveCelularMap(Vector2 worldPos, CelularMapConfig config)
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

        return GenerateCelularMap(config, blocks);
    }
    
    public void GenerateOres(Vector2 worldPos, IBlock[,][] blocks, List<OreVein> oreDistribution)
    {        
        Hasher hasher   = new Hasher(worldPos, Hasher.HashType.OreVeinHash);
        
        foreach(OreVein oreDistrib in oreDistribution)
        {
            if( !(worldPos.y <= oreDistrib.minDepth && worldPos.y >= oreDistrib.maxDepth) )
                continue;
                
            if(hasher.Next() <= oreDistrib.chance)
            {
                //Vector2Int goldBlockPos = Vector2Int.zero;
                //int[,]     goldOreMap   = new int[oreDistrib.mapConfig.mapWidth, oreDistrib.mapConfig.mapHeight];

                int[,]      oreMap = GenerateOreCelularMap(worldPos, oreDistrib.mapConfig);
                Vector2Int  orePos = new Vector2Int( (int) (hasher.Next() * ChunkUtil.chunkWidth),(int) (hasher.Next() * ChunkUtil.chunkHeight));

                orePos.x = Mathf.Clamp(orePos.x, 0, ChunkUtil.chunkWidth  - 1 - oreDistrib.mapConfig.mapWidth);
                orePos.y = Mathf.Clamp(orePos.y, 0, ChunkUtil.chunkHeight - 1 - oreDistrib.mapConfig.mapHeight);
            
                for(int i = 0; i < oreDistrib.mapConfig.mapWidth; i++)
                {
                    for(int j = 0; j < oreDistrib.mapConfig.mapHeight; j++)
                    {
                        if(oreMap[i, j] == 1)
                        {
                            blocks[orePos.x + i, orePos.y + j][(int)ChunkData.BlockLayer.Block] = oreDistrib.block;
                        }
                    }
                } 

                // if we return here theres one one ore vein per chunk
                //return;
            }       
        }
    }
    
    // Used for interpolating between two Chunks that have different Biomes
    public int[] BlendHeightmapData(int[] heightmap1, int[] heightmap2)
    {
        int[] heightmap = new int[ChunkUtil.chunkWidth];

        float weight = 0.01f;

        for(int i = 0; i < heightmap1.Length; i++)
        {   
            if(i != 0) 
            {
                weight = (float)i / (float)heightmap1.Length;
            }

            heightmap[i] = (int) ((heightmap1[i] + (heightmap2[heightmap1.Length - i - 1] * weight)) / (1.0f + weight));

        }

        return heightmap;
    }
    
}
