using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuBackgroundEffect : MonoBehaviour
{
    public Color[] rectColors;
    public int rectCount = 1;
    public float rectSpeedScale = 1.0f;
    public Vector2Int rectSizeBounds;
    public GameObject rectPrefab;
    
    // Start is called before the first frame update
    void Start()
    {
        // For a specified number of rectangles
        for (int i = 0; i < rectCount; i++)
        {
            // Spawn them in and store a reference to their controller script
            MenuBackgroundRectScript mbRect = Instantiate(rectPrefab, Vector3.zero, Quaternion.identity).GetComponent<MenuBackgroundRectScript>();
            // Set the controllers speed, possible colors, and size bounds
            mbRect.speed = rectSpeedScale;
            mbRect.rectColors = rectColors;
            mbRect.sizeBounds = rectSizeBounds;
        }
    }
}
