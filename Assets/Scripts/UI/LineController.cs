using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineController : Singleton<LineController>
{
    [SerializeField]
    private LineRenderer lineRenderer;

    private List<Vector3> points = new List<Vector3>();

    private void Start()
    {
        lineRenderer.useWorldSpace = false;
    }

    public void ShowLineRenderer()
    {
        lineRenderer.enabled = !lineRenderer.enabled;
    }

    public void SetupLine(List<Vector3> points)
    {
        lineRenderer.positionCount = points.Count;
        this.points = points;
        for (int i = 0; i < points.Count; i++)
        {
            lineRenderer.SetPosition(i, new Vector3(points[i].x, points[i].y, 0));
        }
    }

    //private void Update()
    //{
    //    for (int i = 0; i < points.Count; i++)
    //    {
    //        lineRenderer.SetPosition(i, new Vector3(points[i].x, points[i].y, 0));
    //    }
    //}
}
