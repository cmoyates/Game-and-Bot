using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemyPathfinder : MonoBehaviour
{
    class Node
    {
        // Info about the node
        public int x = 0;
        public int y = 0;
        public int distance = 0;

        // Constructors
        public Node(int xPos, int yPos, int dis)
        {
            x = xPos;
            y = yPos;
            distance = dis;
        }
        public Node() { }

        // SetNode function to get around having to create new nodes every time
        public void SetNode(int xPos, int yPos, int dis)
        {
            x = xPos;
            y = yPos;
            distance = dis;
        }
    }

    int[][] m_grid;
    int[][] m_distance;
    public bool[][][] m_directions;
    List<Node> m_open;
    int width;
    int height;
    GameObject player;
    DungeonGenerator dg;
    Tilemap walls;
    Vector2Int offset;
    public AIPlayerController playerAI;

    // Start is called before the first frame update
    void Start()
    {
        // Get references to the player (and the players AI if it's a bot), the walls tilemap, and the dungeon generator
        player = GameObject.FindGameObjectWithTag("Player");
        playerAI = player.GetComponent<AIPlayerController>();
        walls = GameObject.FindGameObjectWithTag("Walls").GetComponent<Tilemap>();
        dg = GameObject.FindGameObjectWithTag("Level Generator").GetComponent<DungeonGenerator>();
        // Get the offset needed to convert a world position to a grid position, as well as the width and height of the grid
        offset = dg.bottomLeft;
        width = dg.topRight.x - dg.bottomLeft.x;
        height = dg.topRight.y - dg.bottomLeft.y;
        // Create the "open list" and populate it with nodes
        m_open = new List<Node>();
        for (int i = 0; i < width * height; i++)
        {
            m_open.Add(new Node());
        }
        // Get the position of the player on the grid
        int gx = Mathf.FloorToInt(player.transform.position.x) - offset.x;
        int gy = Mathf.FloorToInt(player.transform.position.y) - offset.y;

        // Populate the grid, the distances and the directions with their starting variables
        m_grid = new int[width][];
        m_distance = new int[width][];
        m_directions = new bool[width][][];
        for (int x = 0; x < width; x++)
        {
            m_grid[x] = new int[height];
            m_distance[x] = new int[height];
            m_directions[x] = new bool[height][];
            for (int y = 0; y < height; y++)
            {
                m_grid[x][y] = (walls.GetTile(new Vector3Int(x + dg.bottomLeft.x, y + dg.bottomLeft.y, 0)) != null) ? 1 : 0;
                if (x == gx && y == gy) { m_grid[x][y] = 7; }
                m_distance[x][y] = 0;
                m_directions[x][y] = new bool[] { false, false, false, false };
            }
        }
    }

    private void FixedUpdate()
    {
        // Run the direction calculations
        DirectionGridCalc();
    }

    void DirectionGridCalc() 
    {
        // Reset all of the directions that were calculated previously
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                m_distance[x][y] = (m_grid[x][y] == 1) ? -2 : -1;
                m_directions[x][y] = new bool[]{ false, false, false, false };
            }
        }
        
        // Get the player's position on the grid
        int gx = Mathf.FloorToInt(player.transform.position.x) - offset.x;
        int gy = Mathf.FloorToInt(player.transform.position.y) - offset.y;

        // Starting node added to open
        m_open[0].SetNode(gx, gy, 0);
        m_distance[gx][gy] = 0;
        int openCount = 1;
        int openIndex = 0;

        // While open is not empty
        while (openCount - openIndex != 0)
        {
            // Current node is popped from open
            int x = m_open[openIndex].x, y = m_open[openIndex].y, dist = m_open[openIndex].distance;
            ++openIndex;
            // Have a bitset available to record the directions
            bool[] dirBits = new bool[4];
            // Expand to the neighbor nodes

            // Check that the current node is not at the top of the grid
            if (y != 0)
            {
                // If the node above has an assigned distance, do the direction calculation for up
                if ((m_distance[x][y-1] + 1u) != 0)
                {
                    dirBits[0] = !((m_distance[x][y - 1] + 1u - dist) != 0);
                }
                // Otherwise add it to the open list and set the distance
                else
                {
                    m_open[openCount].SetNode(x, y - 1, dist + 1);
                    m_distance[x][y - 1] = dist + 1;
                    ++openCount;
                }
            }
            // Check that the current node is not at the bottom of the grid
            if (height - (y + 1) != 0)
            {
                // If the node below has an assigned distance, do the direction calculation for down
                if ((m_distance[x][y + 1] + 1u) != 0)
                    
                {
                    dirBits[1] = !(m_distance[x][y + 1] + 1u - dist != 0);
                }
                // Otherwise add it to the open list and set the distance
                else
                {
                    m_open[openCount].SetNode(x, y + 1, dist + 1);
                    m_distance[x][y + 1] = dist + 1;
                    ++openCount;
                }
            }
            // Check that the current node is not at the far left of the grid
            if (x != 0)
            {
                // If the node to the left has an assigned distance, do the direction calculation for left
                if ((m_distance[x - 1][y] + 1u) != 0)
                {
                    dirBits[2] = !(m_distance[x - 1][y] + 1u - dist != 0);
                }
                // Otherwise add it to the open list and set the distance
                else
                {
                    m_open[openCount].SetNode(x - 1, y, dist + 1);
                    m_distance[x - 1][y] = dist + 1;
                    ++openCount;
                }
            }
            // Check that the current node is not at the far right of the grid
            if (width - (x + 1) != 0)
            {
                // If the node to the right has an assigned distance, do the direction calculation for right
                if ((m_distance[x + 1][y] + 1u) != 0)
                {
                    dirBits[3] = !(m_distance[x + 1][y] + 1u - dist != 0);
                }
                // Otherwise add it to the open list and set the distance
                else
                {
                    m_open[openCount].SetNode(x + 1, y, dist + 1);
                    m_distance[x + 1][y] = dist + 1;
                    ++openCount;
                }
            }
            // Assign the calcualted directions to the appropriate place on the directions grid
            m_directions[x][y] = dirBits;
        }
    }
}
