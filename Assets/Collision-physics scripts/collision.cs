using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class collision : MonoBehaviour
{
    // enum to govern which type of shape object is
    public enum Type
    {
        Circle,    // 0
        Rectangle, // 1
        Triangle,  // 2
        line,      // 3
        Point      // 4
    };

    // declarable, assignable "Type" for current object
    [Header("Shape")]
    public Type type;

    // Variables for circle
    [Header("Circle Attributes")]
    public float radius;
    public float mass = 1.0f;

    //Variables for Rectangle
    [Header("Rectangle Attributes")]
    public float X_coordinate;
    public float Y_coordinate;
    public float rect_width;
    public float rect_height;
    float bottom_left_corner_X;
    float bottom_left_corner_Y;

    void Start()
    {
        // Determine Radius
        if (type == Type.Circle) // if its a circle
        {
            // find the radius of circle by getting diameter(length of x) / 2
            radius = transform.localScale.x / 2;
        }
        else if (type == Type.line) // if its a line
        {

        }
        else if (type == Type.Point) // if its a point
        {

        }

        //print(radius);
    }

    void Update()
    {
        // constantly update RECTANGLE position
        if (type == Type.Rectangle) // if its a rectangle
        {
            rect_width = transform.localScale.x; // how wide on x axis
            rect_height = transform.localScale.y; // how tall on y axis

            // find bottom left corner & track
            bottom_left_corner_X = this.transform.localPosition.x - (rect_width / 2);
            bottom_left_corner_Y = this.transform.localPosition.y - (rect_height / 2);

            X_coordinate = bottom_left_corner_X; // X coordinate
            Y_coordinate = bottom_left_corner_Y; // y coordinate
        }
        else if (type == Type.line) // of rectangle...
        {

        }
        else if(type == Type.Point)
        {

        }

    }
}
