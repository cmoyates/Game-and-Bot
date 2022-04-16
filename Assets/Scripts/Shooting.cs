using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Shooting : MonoBehaviour
{
    public GameObject bulletPrefab;
    public float bulletForce = 20f;
    ScreenShake ss;
    public float shakeDuration = 1f;
    public float shakeMagnitude = 1f;
    public AudioSource audioSource;
    Vector2 aim;
    bool isKeyboard = false;

    // Start is called before the first frame update
    void Start()
    {
        ss = gameObject.GetComponentInChildren<ScreenShake>();
        aim = Vector2.down;
    }

    public void TriggerShoot(InputAction.CallbackContext context) 
    {
        // When shoot button pressed and it's not an AI controlled game
        if (context.performed && !PauseMenu.isPaused && GameData.previousScene == 1) 
        {
            // Get look direction
            if (isKeyboard)
            {
                // Get the vector from the mouse position to the position of the player
                aim = Camera.main.ScreenToWorldPoint(aim) - transform.position;
                aim.Normalize();
            }
            // Shoot in that direction
            Shoot(aim);
        }
    }

    public void Shoot(Vector2 lookDir) 
    {
        // Play the shooting sound and shake the screen
        audioSource.PlayOneShot(audioSource.clip); 
        StartCoroutine(ss.Shake(shakeDuration, shakeMagnitude));
       
        // Spawn the bullet
        GameObject bullet = Instantiate(bulletPrefab, new Vector3(lookDir.x + transform.position.x, lookDir.y + transform.position.y, 0), Quaternion.identity);
        // Add the appropriate force to the bullet
        Rigidbody2D bulletRB = bullet.GetComponent<Rigidbody2D>();
        bulletRB.AddForce(lookDir * bulletForce, ForceMode2D.Impulse);
    }

    public void OnDeviceChange(PlayerInput input)
    {
        isKeyboard = input.currentControlScheme.Equals("Keyboard");
        // If changing to a gamepad
        if (!isKeyboard) 
        {
            // Start the aim direction at down
            aim = Vector2.down;
        }
    }

    public void SetAim(InputAction.CallbackContext context) 
    {
        // If it's not trying to set the aim direction to (0, 0) 
        if (context.ReadValue<Vector2>() != Vector2.zero) 
        {
            // Set the aim direction
            aim = context.ReadValue<Vector2>();
        }
    }
}
