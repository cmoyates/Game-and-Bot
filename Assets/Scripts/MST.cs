using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MST : MonoBehaviour
{
    // A class representing an edge to be used in the algorithm below
    public class Edge 
    {
        public int src;
        public int dest;
        public float weight;
        public Edge(int srcParam, int destParam, float weightParam) 
        {
            src = srcParam;
            dest = destParam;
            weight = weightParam;
        }
    }

    // Calculate the Minimum Spanning Tree from the given edges and vertices using Prim's algorithm
    public List<Edge> calculateMST(Vector2[] vertices, int[][] edges) 
    {
        // Convert the edges from the parameter into Edge objects and store them in a list
        List<Edge> actualEdges = new List<Edge>();
        for (int i = 0; i < edges.Length; i++)
        {
            int src = edges[i][0];
            int dest = edges[i][1];
            float distance = Vector2.Distance(vertices[src], vertices[dest]);
            actualEdges.Add(new Edge(src, dest, distance));
        }
        
        // Create new lists representing the vertices that are reached and unreached respectively
        List<int> reached = new List<int>();
        List<int> unreached = new List<int>();
        // Create a list to sture the edges that will be returned from this function
        List<Edge> output = new List<Edge>();

        // Consider every vertex unreached to start
        for (int i = 0; i < vertices.Length; i++)
        {
            unreached.Add(i);
        }
        // Add the first vertex to the reached list and remove it from the unreached list
        reached.Add(unreached[0]);
        unreached.RemoveAt(0);

        // While there are still vertices that haven't been reached yet
        while (unreached.Count > 0) 
        {
            // Store all the edges that have one reached vertex and one unreached vertex in a posibilities list
            List<Edge> possibilities = new List<Edge>();
            for (int i = 0; i < actualEdges.Count; i++)
            {
                if (reached.Contains(actualEdges[i].src) != reached.Contains(actualEdges[i].dest)) 
                {
                    possibilities.Add(actualEdges[i]);
                }
            }
            // Find the edge from the possibilities with the lowest weight
            Edge currentBest = possibilities[0];
            for (int i = 1; i < possibilities.Count; i++)
            {
                if (possibilities[i].weight < currentBest.weight) 
                {
                    currentBest = possibilities[i];
                }
            }
            // Add the vertex of the best edge was not in the reached list to it, and remove it from the unreached list
            if (reached.Contains(currentBest.src))
            {
                reached.Add(currentBest.dest);
                unreached.Remove(currentBest.dest);
            }
            else 
            {
                reached.Add(currentBest.src);
                unreached.Remove(currentBest.src);
            }
            // Add the best edge to the output list
            output.Add(currentBest);
        }

        // Return the output list
        return output;
    }
}
