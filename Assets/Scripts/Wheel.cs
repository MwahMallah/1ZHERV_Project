using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wheel : MonoBehaviour
{
    [SerializeField] private float rotationSpeed;

    // Update is called once per frame
    void Update() {
        if (!GameManager.Instance.IsGamePaused()) {
            float baseMultiplier = 1000f;
            // Create a rotation quaternion based on the rotation speed and time
            Quaternion deltaRotation = Quaternion.Euler(0, 0, -Time.deltaTime * baseMultiplier * rotationSpeed);

            // Apply the rotation to the current rotation
            transform.rotation *= deltaRotation;

            if (transform.rotation.eulerAngles.z >= 360f) {
                // Reset the rotation to prevent accumulation of rotation errors
                transform.rotation = Quaternion.identity;
            }
        }
    }
}
