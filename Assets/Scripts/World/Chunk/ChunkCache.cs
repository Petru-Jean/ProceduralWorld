using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Netcode;

public class ChunkCache : MonoBehaviour
{
    private Dictionary<Vector2, GameObject> cache;

    public void Awake()
    {
        cache = new Dictionary<Vector2, GameObject>();
    }

    public void Add(Vector2 pos, GameObject chunk)
    {
        cache[pos] = chunk; 
    }

    public void Add(List<GameObject> chunks)
    {
        foreach(GameObject obj in chunks)
        {
            cache[obj.transform.position] = obj;
        }
        
    }

    public void Remove(Vector2[] chunks)
    {
        foreach (Vector2 element in chunks)
        {
            Remove(element);
        }
    }


    public void Remove(Vector2 chunk)
    {
        if(!cache.ContainsKey(chunk)) 
            return;

        ChunkSaver.Save(chunk, cache[chunk].GetComponent<ChunkData>());

        Destroy(cache[chunk].gameObject);

        cache.Remove(chunk);
    }

    public bool Contains(Vector2 chunk)
    {
        return cache.ContainsKey(chunk);
    }

    public Dictionary<Vector2, GameObject> Get()
    {
        return cache;
    }

    public GameObject Get(Vector2 pos)
    {
        if(cache.ContainsKey(pos))
            return cache[pos];

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
