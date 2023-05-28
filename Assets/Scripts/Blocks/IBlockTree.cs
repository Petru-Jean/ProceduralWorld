using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IBlockTree : IBlock
{
    TreeBlockType blockType = TreeBlockType.Trunk;

    public enum TreeBlockType : int
    {
        Stump = 0,
        LeftStump = 1, 
        RightStump = 2, 
        Trunk = 3,
        Leaf = 4,
        LeftBranch = 5, 
        RightBranch = 6
    };

    public abstract int[] TextureIds();
    
    public void SetBlockType(TreeBlockType type)
    {
        this.blockType = type;
    }

    public TreeBlockType GetBlockType()
    {
        return blockType;
    }
    
    public override int TextureId()
    {
        return TextureIds()[(int)GetBlockType()];
    }


}
