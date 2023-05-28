using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockTreePlains : IBlockTree
{
    public override int[] TextureIds()
    {
        return new int[]{
        0,
        0,
        0,
        7,
        4,
        6,
        5
        };
    }
    /*
        Stump = 0,
        LeftStump = 1, 
        RightStump = 2, 
        Trunk = 3,
        Leaf = 4,
        LeftBranch = 5, 
        RightBranch = 6

    */
}
