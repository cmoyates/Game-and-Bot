using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSelfDestruct : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // Get a reference to the particle system
        ParticleSystem particleSystem = GetComponent<ParticleSystem>();
        // Calculate the total amount of time that it would take to play the effect
        float totalLifetime = particleSystem.main.duration + particleSystem.main.startLifetime.constantMax;
        // Destry the particle system object after that amount of time
        Destroy(gameObject, totalLifetime);
    }
}
