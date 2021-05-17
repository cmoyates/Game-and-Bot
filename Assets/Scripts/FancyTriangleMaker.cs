using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DelaunatorSharp;
using System.Linq;
using DelaunatorSharp.Unity.Extensions;
using System;

public class FancyTriangleMaker : MonoBehaviour
{
    Delaunator delaunator;
    List<IPoint> points;
    public Transform lineContainer;
    public Material lineMaterial;

    // Start is called before the first frame update
    void Start()
    {
        // Set "points" to a new list of Delaunator IPoints
        points = new List<IPoint>();
    }

    public List<Vector3[]> MakeFancyTriangles(Vector2[] originalPoints) 
    {
        // Add the points from the parameter to an array of Delaunator IPoints
        for (int i = 0; i < originalPoints.Length; i++)
        {
            points.Add(new Point(originalPoints[i].x, originalPoints[i].y));
        }

        // Create a new Delaunator object with an array made of the points list from before
        delaunator = new Delaunator(points.ToArray());
        // Create a new empty list of arrays of Vector3s
        List<Vector3[]> outputEdges = new List<Vector3[]>();
        // For every edge that the Delaunator found
        delaunator.ForEachTriangleEdge((edge) => 
        {
            // Add a new array of 2 Vector3s to the list made up of the Vector3 positions of the two ends of the edge
            Vector3[] edgePoints = new Vector3[2];
            edgePoints[0] = edge.P.ToVector3();
            edgePoints[1] = edge.Q.ToVector3();
            outputEdges.Add(edgePoints);
        });

        // Return the list of Vector3 arrays
        return outputEdges;
    }

    // Pretty sure I got this function from the Delaunator-Sharp examples
    public LineRenderer CreateLine(Transform container, string name, Vector3[] points, Color color, float width, int order = 1)
    {
        var lineGameObject = new GameObject(name);
        lineGameObject.transform.parent = container;
        var lineRenderer = lineGameObject.AddComponent<LineRenderer>();

        lineRenderer.SetPositions(points);

        lineRenderer.material = lineMaterial ?? new Material(Shader.Find("Standard"));
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;
        lineRenderer.startWidth = width;
        lineRenderer.endWidth = width;
        lineRenderer.sortingOrder = order;
        return lineRenderer;
    }
}
