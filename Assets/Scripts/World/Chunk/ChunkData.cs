using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

[RequireComponent(typeof(ChunkRenderer))]

public class ChunkData : MonoBehaviour
{
    IBlock[,][]   blocks;
    
    ChunkRenderer chunkRenderer;

    public enum BlockLayer : int
    {
        Wall  = 0,
        Block = 1
    };

    void Awake()
    {
       blocks = ChunkUtil.InitBlockData();
    }

    public IBlock GetBlock(int x, int y, BlockLayer layer)
    {
 
        if (ChunkUtil.IsPosValid(x, y))
            return blocks[x, y][(int)layer];
        
        return FlyweightBlock.blockAir;
    }   

    public IBlock[,][] GetBlocks()
    {
        return blocks;
    }

    public void SetBlocks(IBlock[,][] blocks, bool notifyRenderer = true)
    {
        this.blocks = blocks;

        if(notifyRenderer)
        {
            NotifyRenderer();
        }
            
    }

    public void SetBlock(int x, int y, IBlock block, BlockLayer layer, bool notifyRenderer = true)
    {
        if(!ChunkUtil.IsPosValid(x, y)) 
            return;

         blocks[x, y][(int)layer] = block;

        if(notifyRenderer)
        {
            NotifyRenderer();
        }
    }
    
    public void NotifyRenderer()
    {
        if(chunkRenderer == null)
        {
            chunkRenderer = GetComponent<ChunkRenderer>();
        }

        chunkRenderer.UpdateMesh();
    }

}
