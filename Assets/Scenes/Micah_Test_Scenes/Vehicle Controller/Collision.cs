using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collision : MonoBehaviour
{
    public enum Type
    {
        Circle, // 0
        Rectangle, // 1 
        Banana // 2
    };

    public Type type;

    public float radius;
    public float mass = 1.0f;

    void Start()
    {
        radius = transform.localScale.x / 2;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
