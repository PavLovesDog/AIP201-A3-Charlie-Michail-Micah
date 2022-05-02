using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControllerM : MonoBehaviour
{
    public Transform player;
    public Vector3 offset;
    public float focusSpeed = 0.2f;

    Vector3 velocity = Vector3.zero;

    /*
     * NOTE: for smooth translation
     * 
     * Late Update - for OWN physics movement
     * FixedUpdate - for rigid body
     * 
     */
    void LateUpdate()
    {
        Vector3 playerPosition = player.position + offset;
        transform.position = Vector3.SmoothDamp(transform.position, playerPosition, ref velocity, focusSpeed);
    }
}
