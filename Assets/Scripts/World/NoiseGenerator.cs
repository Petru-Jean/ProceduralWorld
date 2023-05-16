using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NoiseGenerator
{
    public static float GenSurfaceNoise(Vector2 worldPos)
    {
        float noise = 0;

        return noise;
    }


    public static float GenCaveNoise(Vector2 worldPos)
    {
        float noise = 0;

        noise = Mathf.PerlinNoise(worldPos.x / 16, worldPos.y / 16 );

        return noise;
    }

}
