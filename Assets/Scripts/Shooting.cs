using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooting : MonoBehaviour
{
    public GameObject bulletPrefab;
    public float bulletForce = 20f;
    ScreenShake ss;
    public float shakeDuration = 1f;
    public float shakeMagnitude = 1f;
    public AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        ss = gameObject.GetComponentInChildren<ScreenShake>();
    }

    // Update is called once per frame
    void Update()
    {
        // If the player clicks the left mouse button and it's not an AI controlled game
        if (Input.GetButtonDown("Fire1") && !PauseMenu.isPaused && GameData.previousScene == 1) 
        {
            // Shoot at the mouse position
            Shoot(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        }
    }

    public void Shoot(Vector2 mousePos) 
    {
        // Play the shooting sound and shake the screen
        audioSource.PlayOneShot(audioSource.clip); 
        StartCoroutine(ss.Shake(shakeDuration, shakeMagnitude));
        // Get the vector from the mouse position to the position of the player
        Vector2 pos = new Vector2(transform.position.x, transform.position.y);
        Vector2 dir = (mousePos - pos).normalized;
        // Spawn the bullet
        GameObject bullet = Instantiate(bulletPrefab, new Vector3(dir.x + transform.position.x, dir.y + transform.position.y, 0), Quaternion.identity);
        // Add the appropriate force to the bullet
        Rigidbody2D bulletRB = bullet.GetComponent<Rigidbody2D>();
        bulletRB.AddForce(dir * bulletForce, ForceMode2D.Impulse);
    }
}
