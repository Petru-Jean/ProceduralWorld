using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System.Linq;

public class PlayerMapUpdater : MonoBehaviour
{
    private ChunkCache   cache;
    private ChunkFactory factory;

    void Awake()
    {
        cache   = ChunkCache.GetCacheObject();
        factory = new ChunkFactory();
    }

    public void Start()
    {
        InvokeRepeating("UpdateChunkMap", 0.0f, 0.5f);
    }

    private void UpdateChunkMap()
    {
        // Calculate left-down corner of the Chunk Map
        Vector2 startPos = ChunkUtil.WorldToChunkPos(transform.position.x - (ChunkUtil.RenderDistance / 2) * (ChunkUtil.chunkWidth),
                                transform.position.y - (ChunkUtil.RenderDistance / 2) * (ChunkUtil.chunkHeight));

        List<Vector2> chunkMap = new List<Vector2>();
       
        // Calculate chunks that need to be active
        for (int x = 0; x < ChunkUtil.RenderDistance; x++)
        {
            for (int y = 0; y < ChunkUtil.RenderDistance; y++)
            {
                chunkMap.Add(startPos + new Vector2(x * ChunkUtil.chunkWidth, y * ChunkUtil.chunkHeight));
            }
        }

        Vector2[] unusedChunks = cache.Get().Keys.Except(chunkMap).ToArray();

        if (unusedChunks.Length > 0)
            cache.Remove(unusedChunks);

        Vector2[] neededChunks = chunkMap.Except(cache.Get().Keys).ToArray();

        if (neededChunks.Length == 0)
            return;

        // Create Chunks and store them in the cache
        cache.Add(factory.Create(neededChunks));

        // Update first and last row of existing chunks
        // Otherwise there would be visual seems between the old and new chunks

        List<Vector2> rowsToUpdate = new List<Vector2>();

        rowsToUpdate.Add(new Vector2(neededChunks[0].x - ChunkUtil.chunkWidth, neededChunks[0].y));
        rowsToUpdate.Add(new Vector2(neededChunks[0].x + ChunkUtil.chunkWidth, neededChunks[0].y));

        foreach (Vector2 row in rowsToUpdate)
        {
            if (cache.Contains(row))
            {
                for (int i = 0; i < ChunkUtil.RenderDistance; i++)
                {
                    GameObject chunk = cache.Get(new Vector2(row.x, row.y + i * ChunkUtil.chunkHeight));

                    if (chunk != null)
                        chunk.GetComponent<ChunkRenderer>().UpdateMesh();
                }
            }
        }
        

    }

    void OnApplicationQuit()
    {
        foreach (KeyValuePair<Vector2, GameObject> pair in cache.Get())
        {
           // ChunkSaver.Save(pair.Key, pair.Value.GetComponent<ChunkData>());
        }
    }

}
