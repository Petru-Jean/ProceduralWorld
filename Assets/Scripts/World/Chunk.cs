using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Chunk : MonoBehaviour
{
    BlockData[,]     blocks;
    
    float _tick = 0;

    void Awake()
    {   
        blocks  = new BlockData[ChunkUtil.chunkWidth, ChunkUtil.chunkHeight];
        
    }
    
    public BlockData[,] GetBlocks()
    {
        return blocks;
    }

    public BlockData GetBlock(int x, int y)
    {
        if(ChunkUtil.IsPosValid(x, y))
            return blocks[x, y];
        
        return FlyweightBlock.blockDataAir;
    }

    public void SetBlocks(BlockData[,] blocks, bool notifyRenderer = true)
    {
        this.blocks = blocks;

        if(notifyRenderer)
        {
            NotifyRenderer();
        }
            
    }

    public void SetBlock(int x, int y, BlockData block, bool notifyRenderer = true)
    {
        if(!ChunkUtil.IsPosValid(x, y)) 
            return;

         blocks[x, y] = block;

        if(notifyRenderer)
        {
            // List<int> neighboursToUpdate = new List<int>();

            // if(x == 0) neighboursToUpdate.Add((int)ChunkRenderer.NeighbourIndices.Left);
            // if(x == ChunkUtil.chunkWidth-1) neighboursToUpdate.Add((int)ChunkRenderer.NeighbourIndices.Right);

            // if(y == 0) neighboursToUpdate.Add((int)ChunkRenderer.NeighbourIndices.Down);
            // if(y == ChunkUtil.chunkHeight - 1) neighboursToUpdate.Add((int)ChunkRenderer.NeighbourIndices.Up);

            NotifyRenderer();
        }
    }
    
    public void NotifyRenderer()
    {
        if(GetComponent<ChunkRenderer>() != null)
            GetComponent<ChunkRenderer>().UpdateMesh();
    }

    public void Update()
    {
        if(_tick >= ChunkUtil.TickRate)
        {
            _tick = 0;
            
            OnTick();
        }

        _tick += Time.deltaTime;

    }

    void OnTick()
    {

        // List<Vector2> growableBlocks = new List<Vector2>();

        // foreach(KeyValuePair<Vector2, BlockData> blockPair in _blocks[0])
        // {
        //     blockPair.Value.block.OnTick();
            
        //     IBlock block = blockPair.Value.block;
        //     Vector2 pos  = blockPair.Key;

        //     if(block is IBlockGrowable)
        //     {
        //         growableBlocks.Add(pos);
        //     }

        // }

        // foreach(Vector2 pos in growableBlocks)
        // {
        //     IBlockGrowable block = ((IBlockGrowable) _blocks[0][pos].block);
        //     IBlock         blockBelow = GetBlock(new Vector2(pos.x, pos.y - ChunkUtil.chunkHeight)).block;

        //     if(blockBelow.GetType() != typeof(BlockAir)) 
        //         continue;

        //     if(block.CanGrow())
        //     {
        //         block.OnGrow();

        //         SetBlock(pos - new Vector2(0, ChunkUtil.blockLen), new BlockData(new BlockVine()));
        //     }
        // }

    }

    void OnDestroy()
    {
        ChunkSaver.Save(new Vector2Int((int)transform.position.x, (int)transform.position.y), this);
    }


}
