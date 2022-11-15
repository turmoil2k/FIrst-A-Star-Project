using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bot : MonoBehaviour
{
    public Transform target;
    [SerializeField] float speed = 5f;
    Vector3[] path;
    int targetIndex;

    private void Start()
    {
        PathRequestManager.RequestPath(transform.position, target.position, OnPathFound);
    }
    public void OnPathFound(Vector3[] newPath, bool pathSuccess)
    {
        if (pathSuccess == true)
        {
            path = newPath;
            StopCoroutine("FollowPath");
            StartCoroutine("FollowPath");
        }
    }

    IEnumerator FollowPath()
    {
        Vector3 currentWaypoint = path[0];
        Vector3 transformXZ;
        Vector3 waypointXZ;

        while (true)
        {
            transformXZ = transform.position;
            waypointXZ = currentWaypoint;
            transformXZ.y = 0;
            waypointXZ.y = 0;
            if (transformXZ == waypointXZ)//literally inside the node pixel perfect lets add distance
            //if(Vector3.Distance(transform.position,currentWaypoint) <= 1f) // if distance is implemented obstacle avoidance must also be implemented.
            {
                targetIndex++;
                if(targetIndex >= path.Length)//TARGET? ERROR
                {
                    yield break;
                }
                currentWaypoint = path[targetIndex];
            }
            transform.position = Vector3.MoveTowards(transform.position, currentWaypoint + Vector3.up, speed * Time.deltaTime);
            yield return null;
        }
    }

    public void OnDrawGizmos()
    {
        if(path != null)
        {
            for (int i = targetIndex; i < path.Length; i++)
            {
                Gizmos.color = Color.black;
                Gizmos.DrawCube(path[i], new Vector3(0.9f,2.5f,0.9f));
                if(i == targetIndex)
                {
                    Gizmos.color = Color.cyan;
                    Gizmos.DrawLine(transform.position + Vector3.up, path[i] + Vector3.up);
                }
                else
                {
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawLine(path[i - 1] + Vector3.up, path[i] + Vector3.up);
                }
            }
        }
    }

}
