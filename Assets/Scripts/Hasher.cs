using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hasher
{
    public enum HashType
    {
        CaveHash, 
        OreHash,
        CaHash,
        OreBlockHash
    };

    // public static float Hash(Vector2 worldPosition, HashType hashType, int iteration)
    // {
    //     return ChunkUtil.Rand(new Vector4(worldPosition.x, worldPosition.y, (int) hashType, iteration));
    // }

    HashType hashType_;
    Vector2  worldPos_;
    int      iteration_;

    public Hasher(Vector2 worldPos, HashType hashType)
    {
        worldPos_ = worldPos;
        hashType_ = hashType;
    }

    public float Next()
    {
        return ChunkUtil.Rand(new Vector4(worldPos_.x, worldPos_.y, (int) hashType_, iteration_++));
    }


}
