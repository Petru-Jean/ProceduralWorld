using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EntityPlayer : MonoBehaviour
{
    ChunkCache   cache;
    ChunkFactory factory;

    void Awake()
    {
        cache = ChunkCache.GetCacheObject();
        
        factory = new ChunkFactory();
    }

    void Update()
    {
        StartCoroutine(UpdateChunks());
    }

    IEnumerator UpdateChunks()
    {
        while(true)
        {
            Vector2 startPos =  ChunkUtil.WorldToChunkPos(transform.position.x - (ChunkUtil.RenderDistance / 2) * (ChunkUtil.chunkWidth), transform.position.y - (ChunkUtil.RenderDistance / 2) * (ChunkUtil.chunkHeight)); 

            List<Vector2> chunkMap  = new List<Vector2>();

            for(int x = 0; x < ChunkUtil.RenderDistance; x++)   
            {
                for(int y = 0; y < ChunkUtil.RenderDistance; y++)
                {
                    chunkMap.Add(startPos + new Vector2(( x * ChunkUtil.chunkWidth), y * ChunkUtil.chunkHeight)); 
                }           
            }

            Vector2[] unusedChunks = cache.Get().Keys.Except(chunkMap).ToArray();

            cache.Remove(unusedChunks);
            
            Vector2[] neededChunks = chunkMap.Except(cache.Get().Keys).ToArray();

            cache.Cache(factory.Create(neededChunks));

            yield return new WaitForSecondsRealtime(0.5f);
        }
        
    }


}
