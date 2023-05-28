using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Chunk : MonoBehaviour
{
    IBlock[,]     blocks;
    IBlock[,]     walls;

    List<GameObject> trees;

    float _tick = 0;


    void Awake()
    {   
        blocks  = new IBlock[ChunkUtil.chunkWidth, ChunkUtil.chunkHeight];
        walls   = new IBlock[ChunkUtil.chunkWidth, ChunkUtil.chunkHeight];

        trees = new List<GameObject>();

    }
    
    public void RegisterTree(GameObject treeObject)
    {
        trees.Add(treeObject);
    }

    public IBlock[,] GetWalls()
    {
        return walls;
    }

    public IBlock GetWall(int x, int y)
    {
        if(ChunkUtil.IsPosValid(x, y))
            return walls[x, y];
        
        return FlyweightBlock.blockAir;
    }

    public void SetWalls(IBlock[,] walls, bool notifyRenderer = true)
    {
        this.walls = walls;

        if(notifyRenderer)
        {
            NotifyRenderer();
        }
    }

    public void SetWall(int x, int y, IBlock wall, bool notifyRenderer = true)
    {
        if(!ChunkUtil.IsPosValid(x, y)) 
            return;

         walls[x, y] = wall;

        if(notifyRenderer)
        {
            NotifyRenderer();
        }
    }


    public IBlock[,] GetBlocks()
    {
        return blocks;
    }

    public IBlock GetBlock(int x, int y)
    {
        if(ChunkUtil.IsPosValid(x, y))
            return blocks[x, y];
        
        return FlyweightBlock.blockAir;
    }   

    public void SetBlocks(IBlock[,] blocks, bool notifyRenderer = true)
    {
        this.blocks = blocks;

        if(notifyRenderer)
        {
            NotifyRenderer();
        }
            
    }

    public void SetBlock(int x, int y, IBlock block, bool notifyRenderer = true)
    {
        if(!ChunkUtil.IsPosValid(x, y)) 
            return;

         blocks[x, y] = block;

        if(notifyRenderer)
        {
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
        //ChunkSaver.Save(new Vector2Int((int)transform.position.x, (int)transform.position.y), this);

        foreach(GameObject tree in trees)
        {
            Destroy(tree);
        }
    }


}
