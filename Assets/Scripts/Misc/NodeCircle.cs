using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeCircle : MonoBehaviour
{
    [Range(0, 50)]
    public int segments = 50;
    [Range(0, 5)]
    public float xradius = 0.5f;
    [Range(0, 5)]
    public float yradius = 0.5f;
    LineRenderer line;

    void Start()
    {
        line = gameObject.GetComponent<LineRenderer>();

        line.positionCount = segments + 1;
        line.useWorldSpace = false;
        CreatePoints();
    }

    void CreatePoints()
    {
        float x;
        float y;

        float angle = 20f;

        for (int i = 0; i < (segments + 1); i++)
        {
            x = Mathf.Sin(Mathf.Deg2Rad * angle) * xradius;
            y = Mathf.Cos(Mathf.Deg2Rad * angle) * yradius;

            line.SetPosition(i, new Vector3(x, y, 0));

            angle += (360f / segments+1);
        }
    }
}
