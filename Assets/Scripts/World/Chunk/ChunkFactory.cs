using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
//using Unity.Core;
using System.Linq;
using Unity.Netcode;

public class ChunkFactory 
{
    GameObject chunkPrefab;

    public ChunkFactory()
    {
        chunkPrefab = (GameObject) Resources.Load("Prefabs/Chunk");
    }

    public List<GameObject> Create(Vector2[] worldPosArr)
    {
        float time = Time.realtimeSinceStartup;

        List<GameObject> chunks = new List<GameObject>();

        foreach(Vector2 chunkPos in worldPosArr)
        {
            GameObject chunk = CreateChunkGameObject(chunkPos);

            if(chunkPos.y >= ChunkUtil.MinChunkY)
            {
                chunks.Add(chunk);
            }
        }                     

        foreach(GameObject chunk in chunks)
        {
            IBlock[,][] loadedData = ChunkLoader.Load(chunk.transform.position);
            ChunkData   chunkData  = chunk.GetComponent<ChunkData>();

            if(loadedData != null)
            {
                chunkData.SetBlocks(loadedData, false);
            }
            else
            {
                Vector2     chunkPos  = chunk.transform.position;
                IBlock[,][] blocks    = ChunkUtil.InitBlockData();

                IBiome biome = GetBiome(chunkPos);

                if(NeedsBiomeBlending(chunk.transform.position))
                {
                    IBiome blendingBiome = GetBlendingBiome(chunkPos);
                    IBlock blendBlock    = NeedsBiomeBlendingLeft(chunkPos) ? blendingBiome.GetBiomeBlockType() : null;

                    blocks = biome.GenerateBlockData(chunkData, biome.BlendHeightmapData(biome.GenerateHeightmap(chunkPos), 
                    blendingBiome.GenerateHeightmap(chunkPos)), blendBlock);      
                }
                else
                {
                    blocks = biome.GenerateBlockData(chunkData, biome.GenerateHeightmap(chunkPos));
                }   
                
                chunkData.SetBlocks(blocks, false);
                chunk.name = biome.ToString();

            }

        }
        
        //Debug.Log("Generated " + chunks.Count + " chunks: " + (Time.realtimeSinceStartup - time));

        return chunks;

    }

    private GameObject CreateChunkGameObject(Vector2 worldPos)
    {
        GameObject chunk = GameObject.Instantiate(chunkPrefab, new Vector3(worldPos.x, worldPos.y, -1), Quaternion.identity);
        chunk.GetComponent<Renderer>().material.mainTexture.wrapMode = TextureWrapMode.Clamp;
        //GameObject chunk = new GameObject();        
        //chunk.transform.position = new Vector3(worldPos.x, worldPos.y, 0);

        //chunk.transform.tag = "Chunk";
        //chunk.transform.name = "Chunk";

        //chunk.AddComponent<MeshFilter>();
        //chunk.AddComponent<MeshRenderer>().material = (Material)Resources.Load("Materials/Terrain");
        //chunk.AddComponent<ChunkRenderer>();
        //chunk.AddComponent<ChunkData>();

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
        return worldPos.y != 0 && worldPos.x % (4 * ChunkUtil.chunkWidth) == 0 && worldPos.y % (4 * ChunkUtil.chunkHeight) == 0 &&
         ChunkUtil.Rand(worldPos) <= 0.33f;
    }

}
