using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public bool walkable;
    public Vector3 worldPosition;

    public int gCost; // far away from START node
    public int hCost; // opp of gcost how far the node is from the END node

    public int gridX;
    public int gridY;

    public Node (bool _walkable, Vector3 _worldPosition, int _gridX, int _gridY)
    {
        walkable = _walkable;
        worldPosition = _worldPosition;
        gridX = _gridX;
        gridY = _gridY;
    }

    public int fCost //total cost
    {
        get
        {
            return gCost + hCost;
        }
    }
}
