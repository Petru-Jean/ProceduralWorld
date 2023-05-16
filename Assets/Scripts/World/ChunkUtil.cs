using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ChunkUtil
{   
    public const string worldDataPath  = "saves/data/";   
    public const string blockIdMapPath = "blockIdMap.xml";
    
    
    public const int chunkWidth  = 32;
    public const int chunkHeight = 32;

    public const int MinY             = -1800;
    public const int MinChunkY        = (MinY / chunkHeight);

    public const int RenderDistance = 6;

    public const float TicksPerSecond = 24;
    public const float TickRate = 1.0f / TicksPerSecond;
  
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

    public static float SecondsToTicks(float seconds)
    {
        return seconds * 24.0f;
    }   

    public static float TicksToSeconds(int ticks)
    {
        return ticks / TicksPerSecond;
    }

    public static string PosToFileName(Vector2 worldPos)
    {
        return worldDataPath + "[" + worldPos.x + "," + worldPos.y + "]";
    }

    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public static float Rand(Vector4 vect)
    {
        unsafe {
            byte[] data = new byte[sizeof(float)*4];         
            System.Buffer.BlockCopy( System.BitConverter.GetBytes( vect.x ), 0, data, 0,  4 );
            System.Buffer.BlockCopy( System.BitConverter.GetBytes( vect.y ), 0, data, 4,  4 );
            System.Buffer.BlockCopy( System.BitConverter.GetBytes( vect.z ), 0, data, 8,  4 );
            System.Buffer.BlockCopy( System.BitConverter.GetBytes( vect.w ), 0, data, 12, 4 );

            fixed (byte* bytes = &data[0])
            {
                return (float)Unity.Core.XXHash.Hash32(bytes, data.Length) / (float)System.UInt32.MaxValue;
            }

        }
    }
    
    // iteration based so no need for vector 2 input every call
    // perfromance same as above, don't use specifically

    // [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    // public static float Rand(int value, uint seed)
    // {

    //     // unsafe {
    //     //     byte[] data = new byte[sizeof(float)*4];         
    //     //     System.Buffer.BlockCopy( System.BitConverter.GetBytes( vect.x ), 0, data, 0,  4 );
    //     //     System.Buffer.BlockCopy( System.BitConverter.GetBytes( vect.y ), 0, data, 4,  4 );
    //     //     System.Buffer.BlockCopy( System.BitConverter.GetBytes( vect.z ), 0, data, 8,  4 );
    //     //     System.Buffer.BlockCopy( System.BitConverter.GetBytes( vect.w ), 0, data, 12, 4 );

    //     //     fixed (byte* bytes = &data[0])
    //     //     {
    //     //         return (float)Unity.Core.XXHash.Hash32(bytes, data.Length) / (float)System.UInt32.MaxValue;
    //     //     }

    //     // }

    //     byte[] bytes = System.BitConverter.GetBytes(value);
    //     if (System.BitConverter.IsLittleEndian)
    //         System.Array.Reverse(bytes);

    //     unsafe
    //     {
    //         fixed(byte* bytePtr = &bytes[0])
    //         {
    //             return (float)Unity.Core.XXHash.Hash32(bytePtr, bytes.Length, seed) / (float)System.UInt32.MaxValue;
    //         }
    //     }

    // }


    // [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    // public static uint SeedFromPos(Vector2 pos)
    // {
    //     unsafe 
    //     {
    //         byte[] data = new byte[sizeof(float)*4];         
    //         System.Buffer.BlockCopy( System.BitConverter.GetBytes( pos.x ), 0, data, 0,  4 );
    //         System.Buffer.BlockCopy( System.BitConverter.GetBytes( pos.y ), 0, data, 4,  4 );

    //         fixed (byte* bytes = &data[0])
    //         {
    //             return Unity.Core.XXHash.Hash32(bytes, data.Length);
    //         }
    //     }
    // }   

}   
