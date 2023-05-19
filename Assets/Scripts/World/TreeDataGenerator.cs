using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeDataGenerator : MonoBehaviour
{
    public struct Tree
    {
        public int treeType;
        public int baseType;

        public int height;
        
        public List<int> leftBranches, rightBranches;

        public int topType;

    };

    public static Tree Generate(Vector2 worldPos, int treeType)
    {
        Hasher hasher = new Hasher(worldPos, Hasher.HashType.TreeHash);

        Tree tree = new Tree();
        tree.treeType   = treeType;

        tree.baseType   = (int) (hasher.Next() * 3.0f);
        tree.height     = Mathf.Clamp((int) (hasher.Next() * 18.0f), 10, 18);
        
        tree.leftBranches  = new List<int>();
        tree.rightBranches = new List<int>();

        for(int i = 1; i < tree.height - 1; i++)
        {
            if(hasher.Next() >= 0.75f)
                tree.leftBranches.Add(i);
            
            if(hasher.Next() >= 0.75f)
                tree.rightBranches.Add(i);
        }

        tree.topType = (int) (hasher.Next() * 3.0f);

        return tree;
    }


}
