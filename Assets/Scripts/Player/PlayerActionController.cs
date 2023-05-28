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

        if(hit && hit.transform.CompareTag("Chunk") == false)
        {
            if(hit.transform.CompareTag("Tree"))
            {
                if(Input.GetMouseButtonDown(0))
                {
                    Destroy(hit.transform.gameObject);
                }
            }
        }
        else
        {
            hit.point = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            Vector2Int blockPos = Vector2Int.zero;
            Vector2Int chunkPos = Vector2Int.zero;

            float x1 = Mathf.Abs(hit.point.x) % ChunkUtil.chunkWidth;
            float y1 = Mathf.Abs(hit.point.y) % ChunkUtil.chunkHeight;

            if(hit.point.x < 0)
            {
                chunkPos.x = (int) (hit.point.x - (ChunkUtil.chunkWidth  - x1));
                blockPos.x = ChunkUtil.chunkWidth - Mathf.CeilToInt(Mathf.Abs(hit.point.x % ChunkUtil.chunkWidth));
            }
            else
            {
                chunkPos.x = (int) (hit.point.x - x1);
                blockPos.x = (int) Mathf.Abs(hit.point.x % ChunkUtil.chunkWidth);
            }

            if(hit.point.y < 0 )
            {
                chunkPos.y = (int) (hit.point.y - (ChunkUtil.chunkHeight - y1));
                blockPos.y =  ChunkUtil.chunkHeight - Mathf.CeilToInt(Mathf.Abs(hit.point.y % ChunkUtil.chunkHeight));
            }
            else
            {
                chunkPos.y = (int) (hit.point.y - y1);
                blockPos.y = (int) Mathf.Abs(hit.point.y % ChunkUtil.chunkHeight);
            }

            if(Input.GetMouseButtonDown(0))
            {
                GameObject chunkObject = _cache.GetChunk(chunkPos);

                if(chunkObject == null) 
                    return;

                chunkObject.GetComponent<Chunk>().SetBlock(blockPos.x, blockPos.y, FlyweightBlock.blockAir);
            }
        }

    }
}
