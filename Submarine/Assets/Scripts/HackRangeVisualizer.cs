using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class HackRangeVisualizer : MonoBehaviour
{
    public int segments = 100;  // 원의 세부 분할 수
    public float radius = 5f;   // 반지름

    private LineRenderer line;

    void Start()
    {
        line = GetComponent<LineRenderer>();
        line.positionCount = segments + 1;
        line.useWorldSpace = false;

        CreateCircle();
    }

    void CreateCircle()
    {
        float angle = 0f;
        for (int i = 0; i <= segments; i++)
        {
            float x = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;
            float y = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;
            line.SetPosition(i, new Vector3(x, y, 0));  
            angle += 360f / segments;
        }
    }
}
