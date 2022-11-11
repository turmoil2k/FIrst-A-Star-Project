using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MathGraph : MonoBehaviour
{
    [SerializeField] Transform pointPrefabTransform;
    [SerializeField] [Range(10,100)] int resolution;
    [SerializeField] [Range(1, 2)] int funtionGraph;
    [SerializeField] bool inverseTime;
    Vector3 scaleAwake;
    Vector3 positionAwake;
    float step;

    Transform[] points;

    // Start is called before the first frame update
    void Awake()
    {
        step = 2f / resolution;
        positionAwake = Vector3.zero;
        scaleAwake = Vector3.one * step;
        points = new Transform[resolution];
        for (int i = 0; i < points.Length; i++)
        {
            Transform point = Instantiate(pointPrefabTransform);
            points[i] = point;
            positionAwake.x = (i + 0.5f) * step - 1f;
            //position.y = Mathf.Pow(position.x,2);
            point.localPosition = positionAwake;
            point.localScale = scaleAwake;
            point.SetParent(transform, false);

        }
    }

    // Update is called once per frame
    void Update()
    {
        float time;
        if (!inverseTime)
        {
            time = Time.time;
        }
        else
        {
            time = -Time.time;
        }

        for (int i = 0; i < points.Length; i++)
        {
            Transform point = points[i];
            Vector3 pos = point.localPosition;
            if (funtionGraph == 1)
            {
                pos.y = SinWave(pos.x, time);
            }
            else
            {
                pos.y = Ripple(pos.x, time);
            }
            point.localPosition = pos;
        }
    }

    public static float Ripple (float x, float t)
    {
        float d = Mathf.Abs(x);
        float y = Mathf.Sin(Mathf.PI * (4f * d - t));
        return y / (1f + 5f * d);
    }

    public static float SinWave(float posX, float t)
    {
        return Mathf.Sin(Mathf.PI * (posX + t));
    }
}
