using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Core;

public static class ChunkUtil
{   
    public static string savePath      = "Saves/";
    
    public static string WorldDataPath 
    {
        get { return savePath + WorldSettings.WorldName + "/"; }
    }  

    public static string GetWorldDataPath(string WorldName)
    {
        return savePath + WorldName + "/";
    }

    public const int chunkWidth  = 32;
    public const int chunkHeight = 32;

    public const int MinY             = -1800;
    public const int MinChunkY        = (MinY * chunkHeight);

    public const int RenderDistance = 6;
  
    static ChunkUtil()
    {
    }

    public static bool IsPosValid(Vector2Int pos)
    {
        return IsPosValid(pos.x, pos.y);
    }

    public static bool IsPosValid(int x, int y)
    {
        if(x >= chunkWidth || y >= chunkHeight || x < 0 || y < 0)
            return false;
        
        return true;
    }

    public static Vector2Int WorldToChunkPos(float x, float y)
    {
        int x1 = (int) x; 
        int y1 = (int) y;

        x1 -= x1 % chunkWidth;
        y1 -= y1 % chunkHeight;

        return new Vector2Int(x1, y1);
    }


    public static Vector2[] BlockNeighbours = new Vector2[8]
    {
        new Vector2(-1, -1),
        new Vector2(0,  -1),
        new Vector2(1,  -1),

        new Vector2(-1, 0),
        new Vector2(1,  0),

        new Vector2(-1, 1),
        new Vector2(0,  1),
        new Vector2(1,  1)
    };

    public static Vector2[] ChunkNeighbours = new Vector2[4]
    {
        new Vector2(0,  chunkHeight),
        new Vector2(chunkWidth, 0),
        new Vector2(0,  -chunkHeight),
        new Vector2(-chunkWidth, 0),
    };


    public static string GetChunkRegionPath(Vector2 chunkPos)
    {
        int regX, regY;

        regX = (int)chunkPos.x / (16 * chunkWidth);
        regY = (int)chunkPos.y / (16 * chunkHeight);

        string region = WorldDataPath + "Region[" + regX + "," + regY + "]/";

        return region;
    }

    public static string ChunkPosToSavePath(Vector2 worldPos)
    {
        return GetChunkRegionPath(worldPos) + "[" + worldPos.x + "," + worldPos.y + "]";
    }

    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public static float Rand(Vector4 vect)
    {
        unsafe {
            byte[] data = new byte[sizeof(float)*5]; 

            System.Buffer.BlockCopy( System.BitConverter.GetBytes( vect.x ), 0, data, 0,  4 );
            System.Buffer.BlockCopy( System.BitConverter.GetBytes( vect.y ), 0, data, 4,  4 );
            System.Buffer.BlockCopy( System.BitConverter.GetBytes( vect.z ), 0, data, 8,  4 );
            System.Buffer.BlockCopy( System.BitConverter.GetBytes( vect.w ), 0, data, 12, 4 );
            System.Buffer.BlockCopy( System.BitConverter.GetBytes(WorldSettings.Seed), 0, data, 16, 4);

            fixed (byte* bytes = &data[0])
            {
                return (float)Unity.Core.XXHash.Hash32(bytes, data.Length) / (float)System.UInt32.MaxValue;
            }
        }
    }

    public static int GetHashCode(string name)
    {
        byte[] data = System.Text.Encoding.ASCII.GetBytes(name);
        
        unsafe
        {
            fixed (byte* bytes = &data[0])
            {
                return (int)Unity.Core.XXHash.Hash32(bytes, data.Length);
            }
        }

    }

    public static float PerlinNoise(float x)
    {
        return Perlin.Noise(x, 0, (float) WorldSettings.Seed);
    }
    
    public static void LoadSave(string worldName)
    {
        WorldSettings.Change(worldName);
        UnityEngine.SceneManagement.SceneManager.LoadScene("Game");

    }

    public static IBlock[,][] InitBlockData()
    {
        IBlock[,][] blocks  = new IBlock[ChunkUtil.chunkWidth, ChunkUtil.chunkHeight][];

        for (int x = 0; x < ChunkUtil.chunkWidth; x++)
        {
            for (int y = 0; y < ChunkUtil.chunkHeight; y++)
            {
                blocks[x, y] = new IBlock[2];

                blocks[x,y][(int)ChunkData.BlockLayer.Wall]  = FlyweightBlock.blockAir;
                blocks[x,y][(int)ChunkData.BlockLayer.Block] = FlyweightBlock.blockAir;
            }
        }

        return blocks;
    }

}   
