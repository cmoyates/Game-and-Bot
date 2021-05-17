using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShake : MonoBehaviour
{
    // The original local position of the camera
    Vector3 originalPos = new Vector3(0, 0, -10);

    // The coroutine that shakes the camera
    public IEnumerator Shake(float duration, float magnitude) 
    {
        // For the set amount of time
        float elapsed = 0.00f;
        while (elapsed < duration) 
        {
            // Get an offset multiplied by the magnitude specified
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;
            // Set the local position of the camera to that offset
            transform.localPosition = new Vector3(x, y, originalPos.z);

            // Count forward in time
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Once the time has elapsed return the camera to its original position
        transform.localPosition = originalPos;
    }
}
