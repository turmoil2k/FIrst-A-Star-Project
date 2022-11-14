using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MathGraph : MonoBehaviour
{
    [SerializeField] Transform pointPrefabTransform;
    [SerializeField] [Range(10,100)] int resolution;
    [SerializeField] FunctionLibrary.FunctionName function;
    [SerializeField] bool inverseTime;
    Vector3 scaleAwake;
    Vector3 positionAwake;
    float step;

    Transform[] points;

    // Start is called before the first frame update
    void Awake()
    {
        step = 2f / resolution;
        scaleAwake = Vector3.one * step;
        points = new Transform[resolution * resolution];
        for (int i = 0; i < points.Length; i++)
        {
            Transform point = Instantiate(pointPrefabTransform);
            points[i] = point;
            point.localScale = scaleAwake;
            point.SetParent(transform, false);

        }
    }

    // Update is called once per frame
    void Update()
    { 
        FunctionLibrary.Function f = FunctionLibrary.GetFunction(function);

        float time;
        if (!inverseTime)
        {
            time = Time.time;
        }
        else
        {
            time = -Time.time;
        }

        float step = 2f / resolution;

        float v = 0.5f * step - 1f;
        for (int i = 0, x = 0, z = 0; i < points.Length; i++, x++)
        {
            if (x == resolution)
            {
                x = 0;
                z += 1;
                v = (z + 0.5f) * step - 1f;
            }
            float u = (x + 0.5f) * step - 1f;
            points[i].localPosition = f(u, v, time);
        }
    }
}

public static class FunctionLibrary
{

    public delegate Vector3 Function(float u,float v, float t);

    public enum FunctionName { SinWave, MultiWave, Ripple, Sphere, Torus }

    static Function[] functions = { SinWave, MultiWave, Ripple, Sphere, Torus };

    public static Function GetFunction(FunctionName name)
    {
        return functions[(int)name];
    }


    public static Vector3 Ripple(float u, float v, float t)
    {
        float d = Mathf.Sqrt(u * u + v * v);
        Vector3 p;
        p.x = u;
        p.y = Mathf.Sin(Mathf.PI * (4f * d - t));
        p.y /= 1f + 10f * d;
        p.z = v;
        return p;
    }

    public static Vector3 SinWave(float u, float v, float t)
    {
        Vector3 p;
        p.x = u;
        p.y = Mathf.Sin(Mathf.PI * (u + v + t));
        p.z = v;
        return p;
    }

    public static Vector3 MultiWave(float u, float v, float t)
    {
        Vector3 p;
        p.x = u;
        p.y = Mathf.Sin(Mathf.PI * (u + 0.5f * t));
        p.y += 0.5f * Mathf.Sin(2f * Mathf.PI * (v + t));
        p.y += Mathf.Sin(Mathf.PI * (u + v + 0.25f * t));
        p.y *= 1f / 2.5f;
        p.z = v;
        return p;
    }

    public static Vector3 Sphere(float u, float v, float t)
    {
        float r = 0.5f + 0.5f * Mathf.Sin(Mathf.PI * t);
        float s = r * Mathf.Cos(0.5f * Mathf.PI * v);
        Vector3 p;
        p.x = s * Mathf.Sin(Mathf.PI * u);
        p.y = r * Mathf.Sin(0.5f * Mathf.PI * v);
        p.z = s * Mathf.Cos(Mathf.PI * u);
        return p;
    }

    public static Vector3 Torus(float u, float v, float t)
    {
        float r1 = 0.7f + 0.1f * Mathf.Sin(Mathf.PI * (6f * u + 0.5f * t));
        float r2 = 0.15f + 0.05f * Mathf.Sin(Mathf.PI * (8f * u + 4f * v + 2f * t));
        float s = r1 + r2 * Mathf.Cos(Mathf.PI * v);
        Vector3 p;
        p.x = s * Mathf.Sin(Mathf.PI * u);
        p.y = r2 * Mathf.Sin(Mathf.PI * v);
        p.z = s * Mathf.Cos(Mathf.PI * u);
        return p;
    }
}
