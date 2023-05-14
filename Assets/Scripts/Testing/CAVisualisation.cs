using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CAVisualisation : MonoBehaviour
{
    int mapWidth  = 10;
    int mapHeight = 10;

    int[,] blocks;

    void Start()
    {
        //a
        blocks = new int[mapWidth, mapHeight];

        for(int i = 0; i < mapWidth; i++)
        {
            for(int j = 0; j < mapHeight; j++)
            {
                blocks[i, j] = UnityEngine.Random.Range(0.0f, 1.0f) <= 0.5f ? 1 : 0;
            }
        }
    }

    void AdavanceCA(int numTimes)
    {
        for(int t = 0; t < numTimes; t++)
        {
            for(int x = 0; x < mapWidth; x++)
            {
                for(int y = 0; y < mapHeight; y++)
                {
                    int nv = (y + 1 >= mapHeight || x - 1 < 0) ? 0 : blocks[x-1,y+1];
                    int n =  (y + 1 >= mapHeight) ? 0 : blocks[x,y+1];
                    int ne = (y + 1 >= mapHeight || x + 1 >= mapWidth) ? 0 : blocks[x+1,y+1];

                    int v = (x - 1 <  0) ? 0 : blocks[x - 1,   y];
                    int e = (x + 1 >= mapWidth) ? 0 :   blocks[x + 1,   y];
                    
                    int sv = (y - 1 < 0 || x - 1 < 0) ? 0 : blocks[x-1,y-1];
                    int s  = (y - 1 < 0) ? 0 : blocks[x, y-1];
                    int se = (y - 1 < 0 || x + 1 >= mapWidth) ? 0 : blocks[x+1,y-1];

                    int numNeighbours = (nv+n+ne+e+v+sv+s+se);

                    // if(numNeighbours == 3) blocks[x,y] = 1;
                    // else if(numNeighbours <  2) blocks[x,y] = 0;
                    // else if(numNeighbours >  3) blocks[x,y] = 0;

                    //blocks[x,y] = 0;
                    if(numNeighbours >= 6 && numNeighbours <= 8) blocks[x,y] = 1;
                    else if(numNeighbours <= 2) blocks[x,y] = 0;

                    // if(numNeighbours > 4) blocks[x,y]       = 1;
                    // else if (numNeighbours < 4) blocks[x,y] = 0;

                }
            }

        }

    }

    void Update()
    {

        if(Input.GetKeyDown(KeyCode.K))
        {
            AdavanceCA(1);
        }

    }

    void OnDrawGizmos()
    {
        for(int i = 0; i < mapWidth; i++)
        {
            for(int j = 0; j < mapHeight; j++)
            {
                if(blocks[i, j] == 1)
                {
                    Gizmos.DrawCube(new Vector3(i, j), Vector3.one);
                }
            }
        }
        
        
    }

}
