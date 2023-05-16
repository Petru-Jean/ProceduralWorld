using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(Chunk))]
[RequireComponent(typeof(CustomCollider2D))]

public class ChunkRenderer : MonoBehaviour
{
    Chunk _chunk;

    Mesh _mesh;

    List<Vector3> _vertices;
    List<Vector2> _uvs;
    List<int>     _triangles;


    CustomCollider2D    customCollider;
    PhysicsShapeGroup2D colliderShapeGroup;
    MeshFilter          meshFilter;

    Dictionary<Vector2, GameObject> cache;

    GameObject[] neighbours;

    private void Awake()
    {
        _vertices   = new List<Vector3>();
        _uvs        = new List<Vector2>();
        _triangles  = new List<int>();

        _mesh = new Mesh();
        _mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

        _chunk    = GetComponent<Chunk>();
        

        customCollider     = GetComponent<CustomCollider2D>();
        colliderShapeGroup = new PhysicsShapeGroup2D(ChunkUtil.chunkWidth * ChunkUtil.chunkHeight);
        
        meshFilter         = GetComponent<MeshFilter>();

        cache      = ChunkCache.GetCacheObject().Get();
        neighbours = new GameObject[4];

    }

    public void UpdateMesh()
    {
        _vertices.Clear();
        _triangles.Clear();
        _uvs.Clear();

        _mesh.Clear();
        
        colliderShapeGroup.Clear();
        customCollider.ClearCustomShapes();

        for(int i = 0; i < 4; i ++)
        { 
            GameObject neighbour = null;
            cache.TryGetValue(new Vector2(transform.position.x, transform.position.y) + ChunkUtil.ChunkNeighbours[i], out neighbour);
            neighbours[i] = neighbour;

            //Debug.Log("Neighbour at " + new Vector2(transform.position.x, transform.position.y) + ChunkUtil.ChunkNeighbours[i] + " is: " + neighbour);
        }
        
        for(int x = 0;  x < ChunkUtil.chunkWidth;     x++)
        {
            for(int y = 0; y < ChunkUtil.chunkHeight; y++)
            {
                Polygonize(x, y);
            }
        }

        _mesh.vertices  = _vertices.ToArray();
        _mesh.triangles = _triangles.ToArray();
        _mesh.uv        = _uvs.ToArray();

        _mesh.RecalculateBounds();
        _mesh.RecalculateNormals();

        meshFilter.sharedMesh = _mesh;
        customCollider.SetCustomShapes(colliderShapeGroup);
        customCollider.offset = new Vector2(transform.position.x, transform.position.y);

    }

    private void Polygonize(int x, int y)
    {   
        if(_chunk.GetBlock(x,y).block.GetType() == typeof(BlockAir))
        {
            return;
        }

        colliderShapeGroup.AddBox(new Vector2(x + 0.5f, y + 0.5f), Vector2.one);

        int vertIndex = _vertices.Count;

        _vertices.Add(new Vector3(x,                       y));
        _vertices.Add(new Vector3(x + 1,  y));
        _vertices.Add(new Vector3(x,                       y + 1));
        _vertices.Add(new Vector3(x + 1,  y + 1));

        _triangles.Add(vertIndex);
        _triangles.Add(vertIndex+2);
        _triangles.Add(vertIndex+3);

        _triangles.Add(vertIndex);
        _triangles.Add(vertIndex+3);
        _triangles.Add(vertIndex+1);
        
        float time = Time.realtimeSinceStartup;

        IBlock currBlock = _chunk.GetBlock(x,y).block;
        IBlock leftBlock, rightBlock, topBlock, bottomBlock;

        rightBlock  = _chunk.GetBlock(x+1, y).block;
        leftBlock   = _chunk.GetBlock(x-1, y).block;
        topBlock    = _chunk.GetBlock(x,   y+1).block;
        bottomBlock = _chunk.GetBlock(x,   y-1).block;

        if(x + 1 >= ChunkUtil.chunkWidth && neighbours[1] != null)
        {
            rightBlock = neighbours[1].GetComponent<Chunk>().GetBlock(0,y).block;
        }

        if(x - 1 < 0 && neighbours[3] != null)
        {
            leftBlock = neighbours[3].GetComponent<Chunk>().GetBlock(ChunkUtil.chunkWidth-1, y).block;
        }

        if(y + 1 >= ChunkUtil.chunkHeight && neighbours[0] != null)
        {
             topBlock = neighbours[0].GetComponent<Chunk>().GetBlock(x,0).block;
        }

        else if(y - 1 < 0 && neighbours[2] != null)
        {
            bottomBlock = neighbours[2].GetComponent<Chunk>().GetBlock(x,ChunkUtil.chunkHeight-1).block ;
        }

        bool addTopPadding      = topBlock    != FlyweightBlock.blockDataAir.block;
        bool addRightPadding    = rightBlock  != FlyweightBlock.blockDataAir.block;
        bool addLeftPadding     = leftBlock   != FlyweightBlock.blockDataAir.block;
        bool addBottomPadding   = bottomBlock != FlyweightBlock.blockDataAir.block;

        Vector2[] uvs = UvMapper.GetUvs(currBlock.TextureId(), addTopPadding, addRightPadding, addLeftPadding, addBottomPadding);

        for(int i = 0; i < 4; i++)
        {
            _uvs.Add(uvs[i]);
        }

        
    }   


}
