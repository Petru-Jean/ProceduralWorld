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
        OreVeinHash,
        BiomeHash,
        BiomeBlendHash,
        TreeHash,
        VegetationHash
    };

    HashType hashType;
    Vector2  worldPos;
    int      iteration;

    public Hasher(Vector2 worldPos, HashType hashType)
    {
        this.worldPos = worldPos;
        this.hashType = hashType;
    }

    public float Next()
    {
        return ChunkUtil.Rand(new Vector4(worldPos.x, worldPos.y, (int) hashType, iteration++));
    }


}
