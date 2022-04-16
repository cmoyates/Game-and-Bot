using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using UnityEngine.InputSystem;

public class AIPlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    int dashMultiplier = 1;
    bool isDashing = false;
    public Rigidbody2D rb;
    public SpriteRenderer sr;
    public int health = 3;
    bool invincible = false;
    public GameObject[] healthBar;
    SpriteRenderer[] healthBarSRs;
    public GameObject backgroundMusic;
    Color tempColor;
    float dashCooldown = 0.0f;
    public float dashCooldownScale = 1.0f;
    public Camera mainCamera;
    public Tilemap floor;
    public DungeonGenerator dg;
    public GameObject explorationSquare;
    public List<Transform> explSquares;
    public bool goalPosKnown = false;
    public Transform goal;
    public Transform currentTarget;
    Vector3 goalPos;
    public Vector3 currentTargetPos;
    AIPlayerPathfinding aipf;
    Vector2Int offset;
    public List<Transform> visibleEnemyTransforms;
    public int shootingCooldown = 1;
    int currentShootingCooldown = 0;
    public Shooting shootingScript;
    bool goalIsTarget = false;
    ScreenShake ss;
    PauseMenu pauseMenu;

    private void Start()
    {
        // Get references to the screen shake component, the background music player, and the AI player pathfinder
        ss = gameObject.GetComponentInChildren<ScreenShake>();
        backgroundMusic = GameObject.FindGameObjectWithTag("Music");
        aipf = GameObject.FindGameObjectWithTag("AI Player Pathfinder").GetComponent<AIPlayerPathfinding>();
        // Get a reference to the dungeon gnerator and get the offset needed to convert from positions to grid positions
        dg = GameObject.FindGameObjectWithTag("Level Generator").GetComponent<DungeonGenerator>();
        offset = dg.bottomLeft;
        // Get a reference to the floor tilemap
        floor = GameObject.FindGameObjectWithTag("Floor").GetComponent<Tilemap>();
        // Record the color of the player at the beginning of play
        tempColor = sr.color;
        // Get references to the three sections of the healthbar
        healthBarSRs = new SpriteRenderer[3];
        for (int i = 0; i < 3; i++)
        {
            healthBarSRs[i] = healthBar[i].GetComponent<SpriteRenderer>();
        }
        // Instantiate a list of visible enemies
        visibleEnemyTransforms = new List<Transform>();
        // Create a list of "explore squares", and place them everywhere on the map
        explSquares = new List<Transform>();
        for (int x = dg.bottomLeft.x; x < dg.topRight.x; x++)
        {
            for (int y = dg.bottomLeft.y; y < dg.topRight.y; y++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);
                if (floor.GetTile(pos)) 
                {
                    GameObject square = Instantiate(explorationSquare, pos, Quaternion.identity);
                    square.GetComponent<ExplorationSquare>().ai = this;
                    explSquares.Add(square.transform);
                }
            }
        }
        // Get a reference to the pause menu script so pausing works
        pauseMenu = GameObject.Find("Canvas").GetComponent<PauseMenu>();
    }

    // Update is called once per frame
    void Update()
    {
        // Try to dash every frame
        if (dashCooldown <= 0.0f && !isDashing)
        {
            StartCoroutine("Dash");
        }
        // Do increment the dash cooldown if it's not at 0
        if (dashCooldown >= 0)
        {
            dashCooldown -= Time.deltaTime * dashCooldownScale;
            tempColor.a = 1.0f - dashCooldown;
            sr.color = tempColor;
        }
    }

    IEnumerator Dash()
    {
        // Shake the screen
        StartCoroutine(ss.Shake(0.1f, 0.1f));
        // Make the player invincible and "dashing"
        invincible = true;
        isDashing = true;
        dashMultiplier = 2;
        // Change the players color and set the cooldown
        sr.color = tempColor;
        dashCooldown = 0.7f;
        // Wait for a quarter of a second
        yield return new WaitForSeconds(0.25f);
        // Remove the invincibility and make the player no longer dashing
        isDashing = false;
        dashMultiplier = 1;
        invincible = false;
    }

    public void TakeDamage()
    {
        // If the player gets hit and isn't currently invincible
        if (!invincible)
        {
            // Decrease the players health
            health--;
            // If the players health is now below 0
            if (health < 0)
            {
                // Stop the music and go to the game over scene
                Destroy(backgroundMusic);
                SceneManager.LoadScene(sceneBuildIndex: 3);
            }
            else
            {
                // Otherwise, decrease the healthbar and give the player temporary invincibility
                healthBar[health].SetActive(false);
                StartCoroutine("PostDamageInvincibility");
            }
        }
    }

    IEnumerator PostDamageInvincibility()
    {
        // Make the player invincible and change its opacity
        invincible = true;
        Color tempColor = sr.color;
        tempColor.a = 0.3f;
        sr.color = tempColor;
        // Wait two seconds
        yield return new WaitForSeconds(2);
        // Remove the players invincibility and reset its color
        invincible = false;
        tempColor.a = 1;
        sr.color = tempColor;
    }

    private void FixedUpdate()
    {
        // If the game isn't paused, the shooting isn't cooling down, and there is a visible enemy
        if (!PauseMenu.isPaused && currentShootingCooldown >= shootingCooldown && visibleEnemyTransforms.Count > 0)
        {
            // Reset the shooting cooldown and shoot at the enemy
            currentShootingCooldown = 0;
            // Get the direction from the player to the enemy
            Vector2 dir = (visibleEnemyTransforms[0].position - transform.position).normalized;
            // Shoot in that direction
            shootingScript.Shoot(dir);
        }
        else 
        {
            // Otherwise increment the shooting cooldown
            ++currentShootingCooldown;
        }

        // If the position of the goal is known and the goal isn't currently the target, make the goal the target
        if (goalPosKnown && !goalIsTarget) 
        {
            // Remember that the goal is the target from now on
            goalIsTarget = true;
            // Make the goal the target
            currentTarget = goal;
            currentTargetPos = goal.position;
            // Recalculate the pathfinding
            aipf.DirectionGridCalc();
        }
        // If there is no current target, pick a random unexplored "explore square" and make that the target
        if (currentTarget == null) 
        {
            // Get a random "explore square" and make it the target
            int index = Random.Range(0, explSquares.Count);
            currentTarget = explSquares[index];
            currentTargetPos = currentTarget.position;
            // Recalculate the pathfinding
            aipf.DirectionGridCalc();
        }

        // Get the enemy's position in a way that the pathfinding system can understand
        Vector2Int pos = new Vector2Int(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.y));
        pos -= offset;
        // Get the direction to the player from the pathfinding system
        bool[] directions = aipf.m_directions[pos.x][pos.y];
        // Turn the array of bools into a Vec2 representing a direction ([0] = Up, [1] = Down, [2] = Left, [3] = Right)
        Vector2 movement = new Vector2Int(0, 0);
        if (directions[0]) { movement.y -= 1; }
        if (directions[1]) { movement.y += 1; }
        if (directions[2]) { movement.x -= 1; }
        if (directions[3]) { movement.x += 1; }

        // Normalize the movement vector so the player isn't faster diagonally
        movement.Normalize();
        // Move the player according to the movement vector, the players speed, and whether or not the player was dashing
        rb.MovePosition(rb.position + movement * (moveSpeed * dashMultiplier) * Time.fixedDeltaTime);
    }

    public void Pause(InputAction.CallbackContext context)
    {
        // If the pause button is pressed
        if (context.performed)
        {
            // Call the pause function on the pause menu script
            pauseMenu.Pause();
        }
    }
}
