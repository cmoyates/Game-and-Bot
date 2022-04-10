using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnemySpawner : MonoBehaviour
{
    public DungeonGenerator dg;
    int spawnCount = 0;
    public int aliveCount;
    public GameObject enemy;
    public TextMeshProUGUI roundText;

    // Start is called before the first frame update
    void Start()
    {
        // Get references to the dungeon generator, and get the current level
        dg = GameObject.FindGameObjectWithTag("Level Generator").GetComponent<DungeonGenerator>();
        spawnCount = dg.level;
        // Get a reference to the "Round" text
        roundText = GameObject.Find("Level Text").GetComponent<TextMeshProUGUI>();
        // Spawn the enemies
        Spawn();
    }

    void Spawn() 
    {
        // Change the round text to reflect the current level
        roundText.text = "Level: " + spawnCount;
        // For each enemy to spawn
        for (int i = 0; i < spawnCount; i++)
        {
            // Get the bounds of a random room
            Bounds room = dg.rooms[Random.Range(0, dg.rooms.Count)];
            // Get a random position in that room
            Vector2 randPosInRoom = new Vector2(Random.Range(room.min.x + 1, room.max.x - 1), Random.Range(room.min.y + 1, room.max.y - 1));
            // Spawn the enemy there
            Instantiate(enemy, new Vector3(randPosInRoom.x, randPosInRoom.y, 0), Quaternion.identity).GetComponent<EnemyAI>().es = this;
        }
    }
}
