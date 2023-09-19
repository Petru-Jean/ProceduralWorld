using System.Collections.Generic;
using UnityEngine;
using System.Collections.Concurrent;
using Unity.Netcode;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(ChunkData))]
[RequireComponent(typeof(CustomCollider2D))]

public class ChunkRenderer : MonoBehaviour
{
    ChunkData    chunkData;

    PhysicsShapeGroup2D colliderShapeGroup;
    Mesh                mesh;

    List<Vector3> vertices = new List<Vector3>();
    List<Vector2> uvs       = new List<Vector2>();
    List<int>     triangles = new List<int>();

    Dictionary<Vector2, GameObject> cache;
    CustomCollider2D    customCollider;
    MeshFilter          meshFilter;

    ChunkData[] neighbourData;

    private void Awake()
    {
        mesh = new Mesh();
        mesh.indexFormat   = UnityEngine.Rendering.IndexFormat.UInt32;
        
        colliderShapeGroup = new PhysicsShapeGroup2D(ChunkUtil.chunkWidth * ChunkUtil.chunkHeight);

        chunkData       = GetComponent<ChunkData>();
 
        customCollider  = GetComponent<CustomCollider2D>();
        meshFilter      = GetComponent<MeshFilter>();
        
        cache           = ChunkCache.GetCacheObject().Get();

        // Keep this until you make sure uvmapper is initialied before calling its methods
        UvMapper.GetUvs(0,false,false,false,false);

        neighbourData = new ChunkData[4];
    }

    public void Start()
    {
        UpdateMesh();
    }

    public void UpdateMesh()
    {
        vertices.Clear();
        triangles.Clear();
        uvs.Clear();

        mesh.Clear();
        
        colliderShapeGroup.Clear();

        IBlock[,][] blocks = chunkData.GetBlocks();

        for (int i = 0; i < 4; i++)
        {
            GameObject neighbour = null;
 
            if (cache.TryGetValue(new Vector2(transform.position.x, transform.position.y) + ChunkUtil.ChunkNeighbours[i], out neighbour))
            {
                neighbourData[i] = neighbour.GetComponent<ChunkData>();
            }
            
        }

        for (int x = 0;  x < ChunkUtil.chunkWidth;     x++)
        {
            for(int y = 0; y < ChunkUtil.chunkHeight; y++)
            {
                Polygonize(x, y, ChunkData.BlockLayer.Wall);
                Polygonize(x, y, ChunkData.BlockLayer.Block);
            }
        }

        mesh.vertices  = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv        = uvs.ToArray();

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        meshFilter.sharedMesh = mesh;
        customCollider.SetCustomShapes(colliderShapeGroup);
        customCollider.offset = new Vector2(transform.position.x, transform.position.y);

    }

    private void Polygonize(int x, int y, ChunkData.BlockLayer layer)
    {
        IBlock block = chunkData.GetBlock(x, y, layer);

        if (block == FlyweightBlock.blockAir) 
        {
            return;
        }

        if(block.HasCollider())
        {
            colliderShapeGroup.AddBox(new Vector2(x + 0.5f, y + 0.5f), Vector2.one);
        }


        int vertIndex = vertices.Count;

        vertices.Add(new Vector3(x, y));
        vertices.Add(new Vector3(x + 1, y));
        vertices.Add(new Vector3(x, y + 1));
        vertices.Add(new Vector3(x + 1, y + 1));

        triangles.Add(vertIndex);
        triangles.Add(vertIndex + 2);
        triangles.Add(vertIndex + 3);

        triangles.Add(vertIndex);
        triangles.Add(vertIndex + 3);
        triangles.Add(vertIndex + 1);


        IBlock leftBlock, rightBlock, topBlock, bottomBlock;

        rightBlock   = chunkData.GetBlock(x + 1, y, layer);
        leftBlock    = chunkData.GetBlock(x - 1, y, layer);
        topBlock     = chunkData.GetBlock(x, y + 1, layer);
        bottomBlock  = chunkData.GetBlock(x, y - 1, layer);

        if (x + 1 >= ChunkUtil.chunkWidth && neighbourData[1] != null)
        {
            rightBlock = neighbourData[1].GetBlock(0, y, layer);
        }
            
        if (x - 1 < 0 && neighbourData[3] != null)
        {
            leftBlock = neighbourData[3].GetBlock(ChunkUtil.chunkWidth - 1, y, layer);
        }

        if (y + 1 >= ChunkUtil.chunkHeight && neighbourData[0] != null)
        {
            topBlock = neighbourData[0].GetBlock(x, 0, layer);
        }

        else if (y - 1 < 0 && neighbourData[2] != null)
        {
            bottomBlock = neighbourData[2].GetBlock(x, ChunkUtil.chunkHeight - 1, layer);
        }

        bool padTop    = topBlock    == FlyweightBlock.blockAir;
        bool padRight  = rightBlock  == FlyweightBlock.blockAir;
        bool padLeft   = leftBlock   == FlyweightBlock.blockAir;
        bool padBottom = bottomBlock == FlyweightBlock.blockAir;

        Vector2[] uvs = UvMapper.GetUvs(block.TextureId(), padTop, padRight, padLeft, padBottom);

        for(int i = 0; i < 4; i++)
        {
            this.uvs.Add(uvs[i]);
        }
    }   


}
