using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{

    public Transform StartTransform, EndTransform;
    Grid grid;

    private void Awake()
    {
        grid = GetComponent<Grid>();
    }

    private void Update()
    {
        FindPath(StartTransform.position, EndTransform.position);
    }

    void FindPath(Vector3 startPos, Vector3 endPos)
    {
        Node startNode = grid.GetNodeFromWorldPosition(startPos);
        Node endNode = grid.GetNodeFromWorldPosition(endPos);

        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();

        openSet.Add(startNode);

        while(openSet.Count > 0)
        {
            Node currentNode = openSet[0];

            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < currentNode.fCost || //whole if is unoptimized
                    openSet[i].fCost == currentNode.fCost &&
                    openSet[i].hCost < currentNode.hCost)
                {
                    currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode == endNode)
            {
                RetraceFinalPath(startNode, endNode);
                return;
            }

            foreach (Node neighbour in grid.GetNeighbourNodes(currentNode))
            {
                if (neighbour.walkable == false || closedSet.Contains(neighbour))
                {
                    continue;
                }

                int newMoveCostToNeighbour = currentNode.gCost + GetDistanceFromNodes(currentNode, neighbour);
                if (newMoveCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newMoveCostToNeighbour;
                    neighbour.hCost = GetDistanceFromNodes(neighbour, endNode);
                    neighbour.parent = currentNode;

                    if(!openSet.Contains(neighbour))
                    {
                        openSet.Add(neighbour);
                    }
                }
                  
            }
        }
    }

    //? 17.00
    int GetDistanceFromNodes(Node node1, Node node2)
    {
        int distX = Mathf.Abs(node1.gridX - node2.gridX);
        int distY = Mathf.Abs(node1.gridY - node2.gridY);

        if (distX > distY)
        {
            return 14 * distY + 10 * (distX - distY);
        }
        return 14 * distX + 10 * (distY - distX);
    }

    void RetraceFinalPath(Node start, Node end)
    {
        List<Node> path = new List<Node>();
        Node currentNode = end;

        while(currentNode != start)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        path.Add(currentNode);
        path.Reverse();

        grid.path = path;
    }
}
