using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathRequestManager : MonoBehaviour
{
    Queue<PathRequest> pathRequestQueue = new Queue<PathRequest>();
    PathRequest currentPathRequest;

    static PathRequestManager instance;
    Pathfinding pathfinding;

    bool isProcessingPath;

    private void Awake()
    {
        instance = this;
        pathfinding = GetComponent<Pathfinding>();
    }

    public static void RequestPath(Vector3 pathStart, Vector3 pathEnd,
                                    Action<Vector3[],bool> actionCallback)
    {
        PathRequest newReq = new PathRequest(pathStart, pathEnd, actionCallback);
        instance.pathRequestQueue.Enqueue(newReq);
        instance.TryProcessNext();
    }

    void TryProcessNext()
    {
        if(!isProcessingPath && pathRequestQueue.Count > 0)
        {
            currentPathRequest = pathRequestQueue.Dequeue();
            isProcessingPath = true;
            pathfinding.StartFindPath(currentPathRequest.pathStart, currentPathRequest.pathEnd);
        }
    }

    public void FinishedProcessingPath(Vector3 [] path, bool success)
    {
        currentPathRequest.actionCallback(path, success);
        isProcessingPath = false;
        TryProcessNext();
    }


    struct PathRequest
    {
        public Vector3 pathStart;
        public Vector3 pathEnd;
        public Action<Vector3[], bool> actionCallback;

        public PathRequest(Vector3 _start, Vector3 _end, Action<Vector3[], bool> _actionCallback)
        {
            pathStart = _start;
            pathEnd = _end;
            actionCallback = _actionCallback;
        }
    }
}
