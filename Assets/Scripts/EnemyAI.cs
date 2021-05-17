using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemyAI : MonoBehaviour
{
    public Rigidbody2D rb;
    public float moveSpeed = 5;
    EnemyPathfinder epf;
    DungeonGenerator dg;
    Vector2Int offset;
    public float steeringScale = 0.1f;
    public EnemySpawner es;
    ScreenShake ss;
    public AudioSource audioSource;
    public AudioClip[] clips;
    bool playerIsAI;
    AIPlayerController ai;
    public GameObject dieExplosion;
    Quaternion explosionAngle;

    // Start is called before the first frame update
    void Start()
    {
        // Get references to the screenshake component and the enemy pathfinding system
        ss = Camera.main.GetComponent<ScreenShake>();
        epf = GameObject.FindGameObjectWithTag("Enemy Pathfinder").GetComponent<EnemyPathfinder>();
        // If the player is a bot, get a reference to it's AI
        playerIsAI = epf.playerAI != null;
        if (playerIsAI) 
        {
            ai = epf.playerAI;
        }
        // Get the offset needed to convert the players position into a position that can be used by the pathfinding system
        dg = GameObject.FindGameObjectWithTag("Level Generator").GetComponent<DungeonGenerator>();
        offset = dg.bottomLeft;
        // Set the angle of this enemy's death explosion effect so that it's visible to the camera
        explosionAngle = Quaternion.Euler(90, 0, 0);
    }

    public void GotHit() 
    {
        // Play the death particle effect and sound clip
        Instantiate(dieExplosion, transform.position, explosionAngle);
        audioSource.PlayOneShot(clips[0]);
        // Shake the screen
        StartCoroutine(ss.Shake(0.2f, 0.7f));
        // Get the bounds of a random room
        Bounds room = dg.rooms[Random.Range(0, dg.rooms.Count)];
        // Get a random position in that room
        Vector2 randPosInRoom = new Vector2(Random.Range(room.min.x + 1, room.max.x - 1), Random.Range(room.min.y + 1, room.max.y - 1));
        // Teleport the enemy there
        transform.position = new Vector3(randPosInRoom.x, randPosInRoom.y, 0);
    }

    private void FixedUpdate()
    {
        // Get the enemy's position in a way that the pathfinding system can understand
        Vector2Int pos = new Vector2Int(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.y));
        pos -= offset;
        // Get the direction to the player from the pathfinding system
        bool[] directions = epf.m_directions[pos.x][pos.y];
        // Turn the array of bools into a Vec2 representing a direction ([0] = Up, [1] = Down, [2] = Left, [3] = Right)
        Vector2 movement = new Vector2Int(0, 0);
        if (directions[0]) { movement.y -= 1; }
        if (directions[1]) { movement.y += 1; }
        if (directions[2]) { movement.x -= 1; }
        if (directions[3]) { movement.x += 1; }
        // Normalize the vector so the enemies aren't faster diagonally and multiply by the movement speed
        movement.Normalize();
        movement *= moveSpeed;
        // Steering behavior so the player can out-maneuver them
        Vector2 steering = movement - rb.velocity;
        steering *= steeringScale;
        rb.velocity = (rb.velocity + steering);
    }

    // If the enemy hits the player
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.rigidbody.tag == "Player") 
        {
            // Play the player damage sound and shake the screen
            audioSource.PlayOneShot(clips[1]);
            StartCoroutine(ss.Shake(0.1f, 0.15f));
            // Tell the appropriate player controller that it was hit
            if (playerIsAI)
            {
                collision.rigidbody.GetComponent<AIPlayerController>().TakeDamage();
            }
            else 
            {
                collision.rigidbody.GetComponent<PlayerController>().TakeDamage();
            }
        }
    }

    // When the enemy becomes visible
    private void OnBecameVisible()
    {
        // If the player is a bot, tell the player AI that it can see this enemy 
        if (playerIsAI) 
        {
            ai.visibleEnemyTransforms.Add(transform);
        }
    }
    // When the enemy is no longer visible to the player camera
    private void OnBecameInvisible()
    {
        // If the player is a bot, tell the player AI that it can no longer see this enemy 
        if (playerIsAI) 
        {
            ai.visibleEnemyTransforms.Remove(transform);
        }
    }
}
