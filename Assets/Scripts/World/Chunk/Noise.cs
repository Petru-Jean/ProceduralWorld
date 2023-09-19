using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise
{
    public static float Perlin(float x, float y)
    {
        x += 500000.0f;
        y += 500000.0f;

        return Mathf.PerlinNoise(x, y);
    }

}
