using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Unity.Core;
using System.Linq;

public class ChunkFactory 
{
    ChunkCache cache_;
    
    public ChunkFactory()
    {
        cache_ = ChunkCache.GetCacheObject();
    }

    //First generate all chunks objects
    //Then, assign an heightmap to each
    //And   afterwards update them all visually    
    public List<GameObject> Create(Vector2[] worldPosArr)
    {
        float time  = Time.realtimeSinceStartup;

        List<GameObject> chunks = new List<GameObject>();

        foreach(Vector2 chunkPos in worldPosArr)
        {
            GameObject chunk = CreateChunkGameObject(chunkPos);

            // if(chunkPos.y >= ChunkUtil.MinChunkY)
            // {
                chunks.Add(chunk);
            // }
        }   

        foreach(GameObject chunk in chunks)
        {
            IBlock[,] loadedData = ChunkLoader.Load(chunk.transform.position);

            if(loadedData != null)
            {   
                IBlock[,] walls1 = new IBlock[ChunkUtil.chunkWidth, ChunkUtil.chunkHeight];

                chunk.GetComponent<Chunk>().SetBlocks(loadedData, false);
                for(int x = 0; x < ChunkUtil.chunkWidth; x++)
                    for(int y =0 ;y < ChunkUtil.chunkHeight;y ++)
                        walls1[x,y] = FlyweightBlock.blockAir;

                chunk.GetComponent<Chunk>().SetWalls(walls1, false);
                continue;
            }

            Vector2      chunkPos = chunk.transform.position;
            IBlock[,] blocks   = new IBlock[ChunkUtil.chunkWidth,ChunkUtil.chunkHeight];
            
            Chunk chunkScript     = chunk.GetComponent<Chunk>();

            IBiome biome = GetBiome(chunkPos);

            chunk.name = biome.ToString();

            IBlock[,] walls = new IBlock[ChunkUtil.chunkWidth, ChunkUtil.chunkHeight];

            for(int x = 0; x < ChunkUtil.chunkWidth; x++) 
                for(int y = 0; y < ChunkUtil.chunkHeight;y++)
                    walls[x,y] = FlyweightBlock.blockAir;

            chunk.GetComponent<Chunk>().SetWalls(walls, false);

            if(NeedsBiomeBlending(chunk.transform.position))
            {
                IBiome blendingBiome = GetBlendingBiome(chunkPos);
                IBlock blendBlock    = NeedsBiomeBlendingLeft(chunkPos) ? blendingBiome.GetBiomeBlockType() : null;

                blocks = biome.GenerateBlockData(chunkScript, chunkPos, biome.BlendHeightmapData(biome.GenerateHeightmap(chunkPos), blendingBiome.GenerateHeightmap(chunkPos)), blendBlock);      
            }
            else
            {
                blocks = biome.GenerateBlockData(chunkScript, chunkPos, biome.GenerateHeightmap(chunkPos));
            }   

            chunk.GetComponent<Chunk>().SetBlocks(blocks, false);

            
        }

        // foreach(GameObject chunk in chunks)
        // {
        //     chunk.GetComponent<ChunkRenderer>().UpdateMesh();
        // }

        if(Time.realtimeSinceStartup - time >= 0.001f)
            Debug.Log(Time.realtimeSinceStartup - time);

        return chunks;

    }

    private GameObject CreateChunkGameObject(Vector2 worldPos)
    {
        GameObject chunk = new GameObject();        
        chunk.transform.position = new Vector3(worldPos.x, worldPos.y, 0);

        chunk.AddComponent<MeshFilter>();
        chunk.AddComponent<MeshRenderer>().material = (Material)Resources.Load("Materials/Terrain");
        chunk.AddComponent<ChunkRenderer>();
        chunk.AddComponent<Chunk>();

        chunk.transform.tag = "Chunk";
        
        return chunk;
    }
    
    bool NeedsBiomeBlendingLeft(Vector2 worldPos)
    {
        return GetBiome(worldPos) != GetBiome(worldPos - new Vector2(ChunkUtil.chunkWidth, 0));
    }

    bool NeedsBiomeBlending(Vector2 worldPos)
    {
        if(GetBiome(worldPos) != GetBiome(worldPos + new Vector2(ChunkUtil.chunkWidth, 0)))
            return true;

        return GetBiome(worldPos) != GetBiome(worldPos - new Vector2(ChunkUtil.chunkWidth, 0));
    }

    static int biomeLength = 6 * ChunkUtil.chunkWidth;

    private static IBiome GetBiome(Vector2 worldPos)
    {
        if(worldPos.x < 0) worldPos.x -= biomeLength;

        int divs = (int) worldPos.x / biomeLength;

        if(divs == 0) return FlyweightBiomes.biomeHills;
        if(divs == 1) return FlyweightBiomes.biomePlains;
        else return FlyweightBiomes.biomeDesert;

        //return divs % 2 == 0 ? FlyweightBiomes.biomeHills : FlyweightBiomes.biomePlains; 
    }

    IBiome GetBlendingBiome(Vector2 worldPos)
    {
        if(GetBiome(worldPos) != GetBiome(worldPos + new Vector2(ChunkUtil.chunkWidth, 0)))
            return GetBiome(worldPos + new Vector2(ChunkUtil.chunkWidth, 0));

        return GetBiome(worldPos - new Vector2(ChunkUtil.chunkWidth, 0));
    }

    private static bool IsCaveChunk(Vector2 worldPos)
    {
        return worldPos.y != 0 && worldPos.x % (4 * ChunkUtil.chunkWidth) == 0 && worldPos.y % (4 * ChunkUtil.chunkHeight) == 0 && ChunkUtil.Rand(worldPos) <= 0.33f;
    }

}
