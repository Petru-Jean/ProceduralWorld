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
        OreBlockHash,
        BiomeHash,
        BiomeBlendHash,
        TreeHash,
        VegetationHash
    };

    // public static float Hash(Vector2 worldPosition, HashType hashType, int iteration)
    // {
    //     return ChunkUtil.Rand(new Vector4(worldPosition.x, worldPosition.y, (int) hashType, iteration));
    // }

    HashType hashType_;
    Vector2  worldPos_;
    int      iteration_;

    int seed;

    public Hasher(Vector2 worldPos, HashType hashType, int seed = 69420911)
    {
        worldPos_ = worldPos;
        hashType_ = hashType;

        this.seed = seed;
    }

    public float Next()
    {
        return ChunkUtil.Rand(new Vector4(worldPos_.x, worldPos_.y, (int) hashType_, iteration_++));
    }


}
