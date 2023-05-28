using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldSettings : MonoBehaviour
{
    private static int    seed;
    private static string worldName;

    public static int Seed
    {
        get { return seed; }
    }

    public static string WorldName
    {
        get { return worldName; }
    }

    public static void Change(string newName)
    {
        seed      = newName.GetHashCode() / 1024;
        worldName = newName;
    }

    void Awake()
    {
        // Debug.Log("Started world with seed " + seed + " and name: " + worldName);
    }



}
