using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    // The number of seconds before the bullet destroys itself
    public int lifespan = 1;
    
    // Start is called before the first frame update
    void Start()
    {
        // Destroy the bullet after a set amount of time
        Destroy(gameObject, lifespan);
    }

    // If the bullet hits the enemy, tell the enemy AI that it got hit
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.rigidbody.tag == "Enemy") 
        {
            collision.gameObject.GetComponent<EnemyAI>().GotHit();
        }
    }
}
