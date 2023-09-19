using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EntityScript))]

public class PlayerActionController : MonoBehaviour
{
    // GameObject rendered over the selected block to indicate 
    // how long until the block is broken
    [SerializeField]  GameObject  blockCover;

    // The material of the Cover-Block
    Material    coverMat;

    BlockBreakData breakData;

    ChunkCache   chunkCache;
    Entity       entity;
        
    public void Awake()
    {
        entity     = GetComponent<EntityScript>().Entity;
        breakData  = new BlockBreakData();
    }

    private struct BlockPosInfo
    {
        public Vector2Int chunkPos;
        public Vector2Int blockPos;

        public static bool operator ==(BlockPosInfo b1, BlockPosInfo b2)
        {
            return b1.chunkPos == b2.chunkPos && b1.blockPos == b2.blockPos;
        }

        public static bool operator !=(BlockPosInfo b1, BlockPosInfo b2)
        {
            return !(b1 == b2);
        }

        public BlockPosInfo(Vector2Int chunkPos, Vector2Int blockPos)
        {
            this.chunkPos = chunkPos;
            this.blockPos = blockPos;
        }
           
    };

    struct BlockBreakData
    {
        public BlockPosInfo block;

        public float timeElapsed;
        public float breakTime;

        public bool isBreaking;
    };

    void Start()
    {
        chunkCache = ChunkCache.GetCacheObject();
        

        blockCover = Instantiate(blockCover);
        blockCover.SetActive(false);

        coverMat   = blockCover.GetComponent<SpriteRenderer>().material;
    }

    /// <summary>
    /// Calculates world position of the block hit by the cursor.
    /// </summary>
    /// <returns>The World Position of the Chunk GameObject and the Block's position in the Chunk.</returns>
    BlockPosInfo GetHoveredBlockPos()
    {
        Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Vector2Int chunkPos = Vector2Int.zero;
        Vector2Int blockPos = Vector2Int.zero;

        float x1 = Mathf.Abs(pos.x) % ChunkUtil.chunkWidth;
        float y1 = Mathf.Abs(pos.y) % ChunkUtil.chunkHeight;

        chunkPos.x = pos.x < 0 ? (int)(pos.x - (ChunkUtil.chunkWidth - x1)) : (int)(pos.x - x1);
        blockPos.x = pos.x < 0 ? ChunkUtil.chunkWidth - Mathf.CeilToInt(Mathf.Abs(pos.x % ChunkUtil.chunkWidth)) : (int)Mathf.Abs(pos.x % ChunkUtil.chunkWidth);
        
        chunkPos.y = pos.y < 0 ? (int)(pos.y - (ChunkUtil.chunkHeight - y1)) : (int)(pos.y - y1);
        blockPos.y = pos.y < 0 ?  ChunkUtil.chunkHeight - Mathf.CeilToInt(Mathf.Abs(pos.y % ChunkUtil.chunkHeight)) : (int)Mathf.Abs(pos.y % ChunkUtil.chunkHeight);

        return new BlockPosInfo(chunkPos, blockPos);
    }

    void BreakBlock(BlockPosInfo blockPos)
    {
        GameObject chunkObject = chunkCache.Get(blockPos.chunkPos);

        if (chunkObject == null)
            return;

        ChunkData chunkData = chunkObject.GetComponent<ChunkData>();

        chunkData.GetBlock(blockPos.blockPos.x, blockPos.blockPos.y, ChunkData.BlockLayer.Block).OnBreak(entity);
        chunkData.SetBlock(blockPos.blockPos.x, blockPos.blockPos.y, FlyweightBlock.blockAir, ChunkData.BlockLayer.Block);
    }
    
    /// <returns>True if the block was placed, false otherwise.</returns>
    bool PlaceBlock(BlockPosInfo blockInfo, IBlock block)
    {
        GameObject chunkObject = chunkCache.Get(blockInfo.chunkPos);

        if (chunkObject == null || block == FlyweightBlock.blockAir)
            return false;

        ChunkData chunkData = chunkObject.GetComponent<ChunkData>();

        // Check if there's any block placed at that position
        if (chunkData.GetBlock(blockInfo.blockPos.x, blockInfo.blockPos.y, ChunkData.BlockLayer.Block) != FlyweightBlock.blockAir)
            return false;

        chunkData.SetBlock(blockInfo.blockPos.x, blockInfo.blockPos.y, block, ChunkData.BlockLayer.Block );

        return true;
    }

    void Update()
    {
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

        float dist = Vector2.Distance(transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition));

        if (dist > 8.0f)
        {
            blockCover.SetActive(false);
            breakData.isBreaking = false;
            return;
        }

        BlockPosInfo currHitBlock = GetHoveredBlockPos();

        blockCover.SetActive(true);

        blockCover.transform.position = currHitBlock.chunkPos + currHitBlock.blockPos + new Vector2(0.5f, 0.5f);
        blockCover.transform.position = new Vector3(blockCover.transform.position.x, blockCover.transform.position.y, -3.0f);

        if (Input.GetMouseButton(0))
        {
            if (breakData.isBreaking && breakData.block == currHitBlock)
            {
                // Time spent breaking >= block break time

                if (breakData.timeElapsed >= breakData.breakTime)
                {
                    breakData.isBreaking = false;

                    BreakBlock(breakData.block);
                }
                else
                {
                    breakData.timeElapsed += Time.deltaTime;

                    float alpha = 0.2f + ((float)(breakData.timeElapsed / breakData.breakTime));
                    coverMat.color = new Color(coverMat.color.r, coverMat.color.g, coverMat.color.b, alpha);

                }

            }
            else
            {
                GameObject chunkObject = chunkCache.Get(currHitBlock.chunkPos);

                if (chunkObject == null)
                    return;

                breakData.block      = currHitBlock;
                breakData.isBreaking = true;
                breakData.timeElapsed = 0;

                breakData.breakTime = chunkObject.GetComponent<ChunkData>().GetBlock(currHitBlock.blockPos.x, currHitBlock.blockPos.y, ChunkData.BlockLayer.Wall).BreakTime();
            }

        }
        else
        {
            breakData.isBreaking = false;

            if (Input.GetMouseButtonDown(1))
            {
                Inventory inventory = entity.Inventory;
                Item      currentItem = inventory.GetItemStack(inventory.SelectedItemSlot).Item;

                IBlock    block = currentItem.BlockType();

                if (PlaceBlock(currHitBlock, block))
                {
                    currentItem.Use(entity);
                }
    
            }

        }

        // Reset Block highlight intensity
        if(Input.GetMouseButton(0) == false || breakData.isBreaking == false)
        {
            coverMat.color = new Color(coverMat.color.r, coverMat.color.g, coverMat.color.b, 0.2f);
        }       

    }

}
