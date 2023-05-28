using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkCache : MonoBehaviour
{
    Dictionary<Vector2, GameObject> cache_;

    void Awake()
    {
        cache_ = new Dictionary<Vector2, GameObject>();
    }

    public void Cache(Vector2 pos, GameObject chunk)
    {
        cache_[pos] = chunk; 
    }

    public void Cache(List<GameObject> chunks)
    {
        List<GameObject> objsToUpdate = new List<GameObject>();

        foreach(GameObject obj in chunks)
        {
            for(int i = 0; i < 4; i++)
            {
                if(cache_.ContainsKey(new Vector2(obj.transform.position.x, obj.transform.position.y) + ChunkUtil.ChunkNeighbours[i]))
                {
                    objsToUpdate.Add(cache_[new Vector2(obj.transform.position.x, obj.transform.position.y) + ChunkUtil.ChunkNeighbours[i]]);
                }
            }
            
        }
        foreach(GameObject obj in chunks)
        {
            cache_[obj.transform.position] = obj;
        }
    
        foreach(GameObject obj in chunks)
        {
            obj.GetComponent<ChunkRenderer>().UpdateMesh();
        }
        
        foreach(GameObject obj in objsToUpdate)
        {
            obj.GetComponent<ChunkRenderer>().UpdateMesh();
        }

    }

    public void Remove(Vector2[] chunks)
    {
        foreach(Vector2 element in chunks)
        {
            Remove(element);
        }
    }
    
    public void Remove(Vector2 chunk)
    {
        if(!cache_.ContainsKey(chunk)) 
            return;

        ChunkSaver.Save(chunk, cache_[chunk].GetComponent<Chunk>());

        Destroy(cache_[chunk].gameObject);

        cache_.Remove(chunk);
    }

    void OnApplicationQuit()
    {
        foreach(Vector2 chunk in cache_.Keys)
        {
            ChunkSaver.Save(chunk, cache_[chunk].GetComponent<Chunk>());
        }

    }

    public bool Contains(Vector2 chunk)
    {
        return cache_.ContainsKey(chunk);
    }

    public Dictionary<Vector2, GameObject> Get()
    {
        return cache_;
    }

    public GameObject GetChunk(Vector2 pos)
    {
        if(cache_.ContainsKey(pos))
            return cache_[pos];

        return null;
    }

    public static ChunkCache GetCacheObject()
    {
        GameObject cacheObj = GameObject.Find("WorldManager");

        if(cacheObj == null)
            Debug.LogError("WorldManager gameObject doesn't exist.");

        ChunkCache cache = cacheObj.GetComponent<ChunkCache>();

        if(cache == null)
        {
            cache = cacheObj.AddComponent<ChunkCache>();
        }

        return cache;
    }   

}
