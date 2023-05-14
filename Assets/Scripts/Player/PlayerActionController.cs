using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActionController : MonoBehaviour
{
    ChunkCache _cache;

    // Start is called before the first frame update
    void Start()
    {
        _cache = GameObject.Find("WorldManager").GetComponent<ChunkCache>();

    }

    // Update is called once per frame
    void Update()
    {   
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

        if(hit)
        {
            Vector2Int blockPos = new Vector2Int((int) hit.point.x % ChunkUtil.chunkWidth, (int) hit.point.y % ChunkUtil.chunkHeight);

            if(Input.GetMouseButtonDown(0))
            {
                _cache.GetChunk(ChunkUtil.WorldToChunkPos((int)hit.point.x, (int)hit.point.y)).GetComponent<Chunk>().SetBlock(blockPos.x, blockPos.y, FlyweightBlock.blockDataAir);
            }

        }

    }
}
