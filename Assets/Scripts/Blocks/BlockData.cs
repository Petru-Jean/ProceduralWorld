using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockData
{
    public IBlock block;

    public BlockData(IBlock block)
    {
        this.block = block;
    }

    public void SetBlock(IBlock block)
    {
        this.block = block;
    }

    public IBlock Block()
    {
        return this.block;
    }

}
