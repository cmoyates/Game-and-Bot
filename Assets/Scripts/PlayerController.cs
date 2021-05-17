using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
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
    ScreenShake ss;
    Vector2 movement;

    private void Start()
    {
        // Get references to the screen shake component and the background music player
        ss = gameObject.GetComponentInChildren<ScreenShake>();
        backgroundMusic = GameObject.FindGameObjectWithTag("Music");
        // Record the color of the player at the beginning of play
        tempColor = sr.color;
        // Get references to the three sections of the healthbar
        healthBarSRs = new SpriteRenderer[3];
        for (int i = 0; i < 3; i++)
        {
            healthBarSRs[i] = healthBar[i].GetComponent<SpriteRenderer>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Get the inputs from the player
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        // If the player presses shift and the dash is not on cooldown, start dashing
        if (Input.GetKeyDown(KeyCode.LeftShift) && dashCooldown <= 0.0f && !isDashing) 
        {
            StartCoroutine("Dash");
        }
        // Update the dashes cooldown timer and the players color
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
        // Normalize the movement vector so the player isn't faster diagonally
        movement.Normalize();
        // Move the player according to the movement vector, the players speed, and whether or not the player was dashing
        rb.MovePosition(rb.position + movement * (moveSpeed * dashMultiplier) * Time.fixedDeltaTime);
    }
}
