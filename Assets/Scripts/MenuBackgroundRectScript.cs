using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuBackgroundRectScript : MonoBehaviour
{
    public float speed = 1f;
    public Vector3 direction;
    public Color[] rectColors;
    public Vector2Int sizeBounds;
    Vector3Int[] directionPossibilities;
    Vector3Int[] startPositionPossibilities;

    // Start is called before the first frame update
    void Start()
    {
        // Set up arrays to store the possible directions and start positions
        directionPossibilities = new Vector3Int[] { new Vector3Int(1, 0, 0), new Vector3Int(-1, 0, 0), new Vector3Int(0, 1, 0), new Vector3Int(0, -1, 0) };
        startPositionPossibilities = new Vector3Int[] { new Vector3Int(-20, Random.Range(-5, 6), 0), new Vector3Int(20, Random.Range(-5, 6), 0), 
            new Vector3Int(Random.Range(-8, 9), -12, 0), new Vector3Int(Random.Range(-8, 9), 12, 0) };

        // Reset the position, scale, and color of the rectangle
        Reset();
    }

    private void FixedUpdate()
    {
        // Move the rectangle in the direction at the speed
        transform.Translate(direction * speed);
    }

    // When the rectangle is no longer visible
    private void OnBecameInvisible()
    {
        // Reset the position, scale, and color of the rectangle
        Reset();
    }

    private void Reset()
    {
        // Randomize the scale and color
        transform.localScale = new Vector3Int(Random.Range(1, sizeBounds.x), Random.Range(1, sizeBounds.y), 0);
        GetComponent<SpriteRenderer>().color = rectColors[Random.Range(0, rectColors.Length)];
        // Pick a random direction
        int dir = Random.Range(0, 3);
        // Set the direction vector and start position accordingly
        direction = directionPossibilities[dir];
        transform.position = startPositionPossibilities[dir];
    }
}
