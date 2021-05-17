using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

public class DungeonGenerator : MonoBehaviour
{
    public int rectNumber = 50;
    public int radius = 25;
    public int hallwayWidth = 1;
    public int largestRoomWallSize = 20;
    public int smallestRoomWallSize = 1;
    public GameObject rectangle;
    List<GameObject> rectangles;
    FancyTriangleMaker ftm;
    Vector3[][] edges;
    public Tilemap wallTilemap;
    public Tilemap floorTilemap;
    public Tile wallTile;
    public TileBase floorTile;
    Vector3 playerSpawnPoint;
    public GameObject player;
    public MST mst;
    public Vector2Int bottomLeft;
    public Vector2Int topRight;
    public GameObject enemyPathfinder;
    public List<Bounds> rooms;
    public GameObject enemySpawner;
    public GameObject generatingMapText;
    public GameObject[] textToShow;
    public int level;
    public GameObject goal;
    public GameObject aiPlayerPathfinder;


    // Start is called before the first frame update
    void Start()
    {
        // Get the current level from the GameDataObject
        level = GameData.level;
        ftm = gameObject.GetComponent<FancyTriangleMaker>();
        rectangles = new List<GameObject>();
        for (int i = 0; i < rectNumber; i++)
        {
            GameObject rect = Instantiate(rectangle, GetPointInRadius(), Quaternion.identity);
            rect.transform.localScale = new Vector3(Mathf.RoundToInt(RandomGaussian(smallestRoomWallSize, largestRoomWallSize)), Mathf.RoundToInt(RandomGaussian(smallestRoomWallSize, largestRoomWallSize)), 1);
            rectangles.Add(rect);
        }
        // Set the timescale to a large value
        Time.timeScale = 15;
        StartCoroutine("StoppedMoving");
    }

    IEnumerator StoppedMoving()
    {
        // Wait for one second in real time
        yield return new WaitForSecondsRealtime(1);
        // Put the timescale back to normal
        Time.timeScale = 1;
        // Reduce the size of all of the rectangles by 2 in both directions and make them stop moving
        for (int i = 0; i < rectangles.Count; i++)
        {
            rectangles[i].transform.localScale -= Vector3.one * 2;
            rectangles[i].GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        }
        // Sort the list of rectangles by area (this goes from lowest to highest)
        rectangles.Sort(SortByArea);
        // Reverse the list (making it go from highest to lowest)
        rectangles.Reverse();
        // Add the first 30% of rectangles in the list to another list 
        int cutoff = Mathf.RoundToInt(rectangles.Count * 0.3f);
        List<GameObject> tempRects = new List<GameObject>();
        for (int i = 0; i < cutoff; i++)
        {
            tempRects.Add(rectangles[i]);
        }
        // Make any rectangles that were not in that top 30% blue and add Vec2s containing the positions of the rest to an array
        Vector2[] points = new Vector2[cutoff];
        int counter = 0;
        rectangles.ForEach((item) =>
        {
            if (!tempRects.Contains(item))
            {
                item.GetComponent<SpriteRenderer>().color = Color.blue;
            }
            else
            {
                points[counter] = new Vector2(item.transform.position.x, item.transform.position.y);
                counter++;
            }
        });

        // Wait for a eighth of a second for asthetic purposes
        yield return new WaitForSeconds(0.125f);

        // Get the edges from the Delaunay Triangulation (using DelaunatorSharp)
        edges = ftm.MakeFancyTriangles(points).ToArray();

        // Make an array with all of the edges being represented by arrays of two integers, each of which represents a vertex
        int[][] edgesWithVertIndecies = new int[edges.Length][];
        for (int i = 0; i < edges.Length; i++)
        {
            edgesWithVertIndecies[i] = new int[2];
            for (int j = 0; j < 2; j++)
            {
                for (int k = 0; k < tempRects.Count; k++)
                {
                    if (Vector3.SqrMagnitude(tempRects[k].transform.position - edges[i][j]) < 0.01f) 
                    {
                        edgesWithVertIndecies[i][j] = k;
                        break;
                    }
                }
            }
        }

        // Get a minimum spanning tree from the edges and vertices
        MST.Edge[] mstEdges = mst.calculateMST(points, edgesWithVertIndecies).ToArray();

        // For each edge from the minimum spanning tree draw lines from one point to another
        List<LineRenderer> hallwayLines = new List<LineRenderer>();
        for (int i = 0; i < mstEdges.Length; i++)
        {
            float xDiff = Mathf.Abs(tempRects[mstEdges[i].src].transform.position.x - tempRects[mstEdges[i].dest].transform.position.x);
            float yDiff = Mathf.Abs(tempRects[mstEdges[i].src].transform.position.y - tempRects[mstEdges[i].dest].transform.position.y);
            Vector3 midwayPoint = new Vector3(0, 0, 0);
            bool xDiffSmaller = xDiff < yDiff;
            if (xDiffSmaller)
            {
                midwayPoint.x = tempRects[mstEdges[i].dest].transform.position.x;
                midwayPoint.y = tempRects[mstEdges[i].src].transform.position.y;
            }
            else 
            {
                midwayPoint.x = tempRects[mstEdges[i].src].transform.position.x;
                midwayPoint.y = tempRects[mstEdges[i].dest].transform.position.y;
            }

            Vector3[] srcToMid = new Vector3[] { tempRects[mstEdges[i].src].transform.position, midwayPoint };
            Vector3[] midToDest = new Vector3[] { midwayPoint, tempRects[mstEdges[i].dest].transform.position };

            hallwayLines.Add(ftm.CreateLine(ftm.lineContainer, "mstLine", srcToMid, Color.green, 0.5f, 0));
            hallwayLines.Add(ftm.CreateLine(ftm.lineContainer, "mstLine", midToDest, Color.green, 0.5f, 0));
        }

        // Wait for a eighth of a second for asthetic purposes
        yield return new WaitForSeconds(0.125f);

        // For every rectangle previously generated
        for (int i = 0; i < rectangles.Count; i++)
        {
            // If the rectangle is a main rectangle already, continue
            if (tempRects.Contains(rectangles[i])) { continue; }
            // Otherwise, get the bounds of that rectangle
            Bounds rectBounds = rectangles[i].GetComponent<BoxCollider2D>().bounds;
            // Make an array of four Vec2s, with each one representing a corner of the rectangle
            Vector2[] corners = new Vector2[] { new Vector2(rectBounds.min.x, rectBounds.min.y), new Vector2(rectBounds.max.x, rectBounds.min.y),
                new Vector2(rectBounds.max.x, rectBounds.max.y), new Vector2(rectBounds.min.x, rectBounds.max.y) };
            // Assume that the rectangle is not valid
            bool isValid = false;
            // For every line previously drawn
            for (int j = 0; j < hallwayLines.Count; j++)
            {
                // If the line intersects one of the edges of the rectangle, change it's color and consider it valid
                Vector2 src = new Vector2(hallwayLines[j].GetPosition(0).x, hallwayLines[j].GetPosition(0).y);
                Vector2 dest = new Vector2(hallwayLines[j].GetPosition(1).x, hallwayLines[j].GetPosition(1).y);
                if (LineIntersect(src, dest, corners[0], corners[1]) || LineIntersect(src, dest, corners[1], corners[2]) 
                    || LineIntersect(src, dest, corners[2], corners[3]) || LineIntersect(src, dest, corners[3], corners[0])) 
                {
                    rectangles[i].GetComponent<SpriteRenderer>().color = Color.magenta;
                    isValid = true;
                    break;
                }
            }
            // If it's not valid after cycling through every line, deactivate it
            if (!isValid) 
            {
                rectangles[i].SetActive(false);
            }
        }

        // Initialize some temp variables
        int maxX = 0;
        int minX = 0;
        int maxY = 0;
        int minY = 0;

        Vector3Int tilePos = new Vector3Int(0, 0, 0);

        // Wait for a eighth of a second for asthetic purposes
        yield return new WaitForSeconds(0.125f);

        // For every rectangle
        for (int i = 0; i < rectangles.Count; i++)
        {
            // If it's currently active
            if (rectangles[i].activeInHierarchy) 
            {
                // Get the bounds of the rectangle
                Bounds bounds = rectangles[i].GetComponent<BoxCollider2D>().bounds;
                rooms.Add(bounds);
                // Update the mins and maxes accordingly
                maxX = Mathf.Max(maxX, Mathf.RoundToInt(bounds.max.x));
                minX = Mathf.Min(minX, Mathf.RoundToInt(bounds.min.x));
                maxY = Mathf.Max(maxY, Mathf.RoundToInt(bounds.max.y));
                minY = Mathf.Min(minY, Mathf.RoundToInt(bounds.min.y));
                // Place a floor tile everywhere on the floor tilemap that the rectangle overlaps
                for (int x = Mathf.RoundToInt(bounds.min.x); x < Mathf.RoundToInt(bounds.max.x); x++)
                {
                    tilePos.x = x;
                    for (int y = Mathf.RoundToInt(bounds.min.y); y < Mathf.RoundToInt(bounds.max.y); y++)
                    {
                        tilePos.y = y;
                        floorTilemap.SetTile(tilePos, floorTile);
                    }
                }
            }
        }
        // For every "hallway line", place a line of floor tiles (of a specified width) following that line
        for (int i = 0; i < hallwayLines.Count; i++)
        {
            if (hallwayLines[i].GetPosition(0).x != hallwayLines[i].GetPosition(1).x)
            {
                int maxXPoint = Mathf.RoundToInt(Mathf.Max(hallwayLines[i].GetPosition(0).x, hallwayLines[i].GetPosition(1).x));
                int minXPoint = Mathf.RoundToInt(Mathf.Min(hallwayLines[i].GetPosition(0).x, hallwayLines[i].GetPosition(1).x));
                for (int x = minXPoint-2; x < maxXPoint+3; x++)
                {
                    tilePos.x = x;
                    for (int y = -hallwayWidth; y < 1 + hallwayWidth; y++)
                    {
                        tilePos.y = y + Mathf.RoundToInt(hallwayLines[i].GetPosition(0).y);
                        floorTilemap.SetTile(tilePos, floorTile);
                    }
                }
            }
            else 
            {
                int maxYPoint = Mathf.RoundToInt(Mathf.Max(hallwayLines[i].GetPosition(0).y, hallwayLines[i].GetPosition(1).y));
                int minYPoint = Mathf.RoundToInt(Mathf.Min(hallwayLines[i].GetPosition(0).y, hallwayLines[i].GetPosition(1).y));
                for (int x = -hallwayWidth; x < 1 + hallwayWidth; x++)
                {
                    tilePos.x = x + Mathf.RoundToInt(hallwayLines[i].GetPosition(0).x);
                    for (int y = minYPoint-2; y < maxYPoint+3; y++)
                    {
                        tilePos.y = y;
                        floorTilemap.SetTile(tilePos, floorTile);
                    }
                }
            }
        }
        // Wherever there is not a floor tile place a wall tile
        for (int x = minX - 1; x < maxX + 1; x++)
        {
            tilePos.x = x;
            for (int y = minY - 1; y < maxY + 1; y++)
            {
                tilePos.y = y;
                if (floorTilemap.GetTile(tilePos) != floorTile) 
                {
                    wallTilemap.SetTile(tilePos, wallTile);
                }
            }
        }
        // Store the top-right and bottom-left of the grid
        topRight = new Vector2Int(maxX+1, maxY+1);
        bottomLeft = new Vector2Int(minX-1, minY-1);

        // Set the players spawn point to the position of the rectangle at index 0
        playerSpawnPoint = rectangles[0].transform.position;
        // Destroy all of the rectangles and lines
        for (int i = 0; i < rectangles.Count; i++)
        {
            Destroy(rectangles[i]);
        }
        for (int i = 0; i < hallwayLines.Count; i++)
        {
            Destroy(hallwayLines[i]);
        }
        // Hide the text shown when generating the map and show the text that should be shown during gameplay
        generatingMapText.SetActive(false);
        for (int i = 0; i < textToShow.Length; i++)
        {
            textToShow[i].SetActive(true);
        }
        // If the player is a bot, spawn in the AI player pathfinder
        if (SceneManager.GetActiveScene().buildIndex == 2)
        {
            Instantiate(aiPlayerPathfinder);
        }
        // Spawn the player
        SpawnPlayer();
        // Spawn the enemy's pathfinder and spawner
        Instantiate(enemyPathfinder);
        Instantiate(enemySpawner);
        // Get the bounds of a random room, get a random position in that room, and place the goal there
        Bounds room = rooms[Random.Range(0, rooms.Count)];
        Vector2 randPosInRoom = new Vector2(Random.Range(room.min.x + 1, room.max.x - 1), Random.Range(room.min.y + 1, room.max.y - 1));
        Instantiate(goal, new Vector3(randPosInRoom.x, randPosInRoom.y, 0), Quaternion.identity);
    }

    // A function that gets a random point within a random radius (from 0 to 2) from another point
    // https://www.gamasutra.com/blogs/AAdonaac/20150903/252889/Procedural_Dungeon_Generation_Algorithm.php
    Vector3Int GetPointInRadius()
    {
        float t = Mathf.PI * 2 * UnityEngine.Random.value;
        float u = UnityEngine.Random.value + UnityEngine.Random.value;
        float r = (u > 1) ? 2 - u : u;
        return new Vector3Int(Mathf.RoundToInt(radius * r * Mathf.Cos(t)), Mathf.RoundToInt(radius * r * Mathf.Sin(t)), 0);
    }

    // A function that spawns the player
    void SpawnPlayer() 
    {
        // Deactivate the current main camera and spawn in the player
        GameObject.FindGameObjectWithTag("MainCamera").SetActive(false);
        GameObject playerInstance = Instantiate(player, playerSpawnPoint, Quaternion.identity);
    }

    // A function that checks if a line from point a to point b intersects a line from point c to point d
    bool LineIntersect(Vector2 a, Vector2 b, Vector2 c, Vector2 d) 
    {
        Vector2 r = b - a;
        Vector2 s = (d - c);
        float rxs = r.x * s.y - s.x * r.y;
        Vector2 cma = c - a;
        float t = (cma.x * s.y - s.x * cma.y) / rxs;
        float u = (cma.x * r.y - r.x * cma.y) / rxs;
        return t >= 0 && t <= 1 && u >= 0 && u <= 1;
    }

    // A function that returns a random float between a specified min and max
    // https://answers.unity.com/questions/421968/normal-distribution-random.html
    public static float RandomGaussian(float minValue = 0.0f, float maxValue = 1.0f)
    {
        float u, v, S;

        do
        {
            u = 2.0f * UnityEngine.Random.value - 1.0f;
            v = 2.0f * UnityEngine.Random.value - 1.0f;
            S = u * u + v * v;
        }
        while (S >= 1.0f);

        // Standard Normal Distribution
        float std = u * Mathf.Sqrt(-2.0f * Mathf.Log(S) / S);

        // Normal Distribution centered between the min and max value
        // and clamped following the "three-sigma rule"
        float mean = (minValue + maxValue) / 2.0f;
        float sigma = (maxValue - mean) / 3.0f;
        return Mathf.Clamp(std * sigma + mean, minValue, maxValue);
    }

    // This is a function used to sort two game objects by their "area" as determined by their scale
    static int SortByArea(GameObject r1, GameObject r2)
    {
        return r1.transform.localScale.sqrMagnitude.CompareTo(r2.transform.localScale.sqrMagnitude);
    }
}