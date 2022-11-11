using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    //Grid Creation Master Class
    public Transform player;
    public LayerMask unwalkableLayerMask;
    public Vector2 gridWorldSize;
    public float nodeRadius;
    Node[,] grid;

    float nodeDiameter;
    int gridsizeX, gridsizeY;

    // Start is called before the first frame update
    void Start()
    {
        nodeDiameter = nodeRadius * 2;
        gridsizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridsizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        CreateGrid();
    }

    public Node GetNodeFromWorldPosition(Vector3 worldPosition)//percentage method??
    {
        float percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float percentY = (worldPosition.z + gridWorldSize.y / 2) / gridWorldSize.y;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);
        int x = Mathf.RoundToInt((gridsizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridsizeY - 1) * percentY);
        return grid[x, y];
    }

    void CreateGrid()
    {
        grid = new Node[gridsizeX, gridsizeY];

        Vector3 worldBL = transform.position - Vector3.right * gridsizeX / 2 - Vector3.forward * gridsizeY / 2;

        for (int x = 0; x < gridsizeX; x++)
        {
            for (int y = 0; y < gridsizeY; y++)
            {
                Vector3 worldPoint = worldBL +
                Vector3.right * (x * nodeDiameter + nodeRadius) +
                Vector3.forward * (y * nodeDiameter + nodeRadius);

                bool walkableGridBox = !Physics.CheckSphere(worldPoint, nodeRadius, unwalkableLayerMask);
                grid[x, y] = new Node(walkableGridBox, worldPoint,x,y); 
            }
        }
    }

    public List<Node> GetNeighbourNodes(Node node)
    {
        List<Node> neighbours = new List<Node>();
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <=1; y++)
            { 
                if (x == 0 && y == 0)
                {
                    continue;
                }

                //check the surrounding take the origin node as an example below:
                // checkx = 19 because 20 + (-1) if greater than or equal to 0 it means it is in side the grid (this check is to see if it goes left over the boundary of the grid(negatives of the map))
                // && the same check is 19 < gridsizeX(40) yes all true so we continue do the same for y.
                // this is to check the surroundings. (this speccifically goes right to see the if it goes over the positive regions of the boundary/grid)
                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if (checkX >= 0 && checkX < gridsizeX && checkY >= 0 && checkY < gridsizeY)
                {
                    neighbours.Add(grid[checkX, checkY]);
                }
            }
        }
        return neighbours;
    }


    public List<Node> path;
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));

        if(grid != null)
        {
            Node playerNode = GetNodeFromWorldPosition(player.position);
            foreach (Node n in grid)
            {
                Gizmos.color = n.walkable ? Color.green : Color.red;
                if(path != null)
                {
                    if(path.Contains(n))
                    {
                        Gizmos.color = Color.black;
                    }
                }
                if(playerNode == n)
                {
                    Gizmos.color = Color.white;
                }
                Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - 0.1f));  // - 0.1f));
            }
        }
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
