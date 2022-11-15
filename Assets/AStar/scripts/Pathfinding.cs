using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System;

public class Pathfinding : MonoBehaviour
{
    PathRequestManager requestManager;
    //public Transform StartTransform, EndTransform;
    //Vector3 oldTargetPosition = new Vector3();
    Grid grid;

    private void Awake()
    {
        requestManager = GetComponent<PathRequestManager>();
        grid = GetComponent<Grid>();
    }
    private void Start()
    {
        //oldTargetPosition = EndTransform.position;
    }
    private void Update()
    {
        /* //compare the nodes instead of this method...
        if (Input.GetKeyDown(KeyCode.Space))
        {
            FindPath(StartTransform.position, EndTransform.position);
        }

        if(EndTransform.position != oldTargetPosition)
        {
            print("Finding & Updating Old Target...");
            oldTargetPosition = EndTransform.position;
            FindPath(StartTransform.position, EndTransform.position);
        }
        */
    }

    public void StartFindPath(Vector3 startPos, Vector3 endPos)
    {
        StartCoroutine(FindPath(startPos, endPos));
    }    

    IEnumerator FindPath(Vector3 startPos, Vector3 endPos)
    {
        Stopwatch sw = new Stopwatch();
        sw.Start();//from 5-4 ms to <4

        Vector3[] waypoints = new Vector3[0];
        bool pathSuccess = false;

        Node startNode = grid.GetNodeFromWorldPosition(startPos);
        Node endNode = grid.GetNodeFromWorldPosition(endPos);

        if(startNode.walkable && endNode.walkable)
        {
            Heap<Node> openSet = new Heap<Node>(grid.MaxSize);
            HashSet<Node> closedSet = new HashSet<Node>();

            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                Node currentNode = openSet.RemoveFirst();//openSet[0];

                /*
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
                */

                closedSet.Add(currentNode);

                if (currentNode == endNode)
                {
                    sw.Stop();
                    print("Path found in time of ms = " + sw.ElapsedMilliseconds);
                    pathSuccess = true;

                    break;
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

                        if (!openSet.Contains(neighbour))
                        {
                            openSet.Add(neighbour);
                        }
                    }

                }
            }
        }

        yield return null;
        if(pathSuccess)
        {
            waypoints = RetraceFinalPath(startNode, endNode);
        }
        requestManager.FinishedProcessingPath(waypoints, pathSuccess);
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

    Vector3[] RetraceFinalPath(Node start, Node end)
    {
        List<Node> path = new List<Node>();
        Node currentNode = end;

        while(currentNode != start)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }

        Vector3 [] waypoints = SimplifyPath(path);

        path.Add(currentNode);
        //path.Reverse();
        Array.Reverse(waypoints);
        //grid.path = path;
        return waypoints;
    }

    Vector3[] SimplifyPath(List<Node> path)
    {
        List<Vector3> waypoints = new List<Vector3>();
        Vector2 directionOld = Vector2.zero;

        for (int i = 1; i < path.Count; i++)
        {
            Vector2 directionNew = new Vector2(path[i - 1].gridX - path[i].gridX,
                                            path[i - 1].gridY - path[i].gridY);

            if(directionNew != directionOld)
            {
                waypoints.Add(path[i].worldPosition);
            }
            directionOld = directionNew;
        }

        return waypoints.ToArray();
    }

}
