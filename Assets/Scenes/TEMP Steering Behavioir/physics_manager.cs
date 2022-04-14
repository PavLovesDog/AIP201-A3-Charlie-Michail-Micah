using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class physics_manager : MonoBehaviour
{

    void Update()
    {
        // for all objects that have colliders, detect collision
        collision[] collisions = FindObjectsOfType<collision>();

        foreach(collision this_collision in collisions)
        {
            foreach (collision other_collision in collisions)
            {
                // do not detect collision with self
                if (this_collision == other_collision) continue;

                // detect collisions!

                //CIRCLE TO CIRCLE COLLISION ======================================================================
                if (this_collision.type == collision.Type.Circle && // if this object is a circle
                    other_collision.type == collision.Type.Circle) // if other object colliding with is a circle
                {
                    // set both radius
                    float this_radius = this_collision.radius;
                    float other_radius = other_collision.radius;

                    // use radius of both objects to see if there has been a collision

                    //vector to track distance from current gameObject to other collidable gameObject and vice versa
                    Vector3 this_to_other = other_collision.transform.position - this_collision.transform.position;
                    Vector3 other_to_this = this_collision.transform.position - other_collision.transform.position;
                    // store distance in a float
                    float distance_this_to_other = this_to_other.magnitude;

                    // to find intersection depth, the radius combined minus the total distance is what is left over
                    // NOTE: a negative intersection depth means NO intersection
                    float intersection_depth = this_radius + other_radius - distance_this_to_other;

                    // I.E. if the radius of both circles combined is greater than the total distance,
                    // objects MUST be colliding

                    if (intersection_depth > 0.0f) 
                    {
                        print("COLLISION!");
                        // A.I resolution...

                        // Handle mass properties
                        float total_mass = this_collision.mass + other_collision.mass; // find total mass involved in colilsion
                        // find ratios of who takes more or less of the collision direction
                        float other_mass_ratio = other_collision.mass / total_mass;
                        float this_mass_ratio = this_collision.mass / total_mass;


                        // Move other object using this_to_other vector direction multiplied by the intersection depth & this objects mass
                        other_collision.transform.position += this_to_other.normalized * intersection_depth * this_mass_ratio;

                        // move this object using other_to_this vector direction multiplied by the intersection depth & other objects mass
                        this_collision.transform.position += other_to_this.normalized * intersection_depth * other_mass_ratio;
                    }
                }

                // RECT TO RECT COLLISION
                if (this_collision.type == collision.Type.Rectangle && // if this object is a circle
                    other_collision.type == collision.Type.Rectangle)
                {
                    if (this_collision.X_coordinate < other_collision.X_coordinate + other_collision.rect_width &&
                        this_collision.X_coordinate + this_collision.rect_width > other_collision.X_coordinate &&
                        this_collision.Y_coordinate < other_collision.Y_coordinate + other_collision.rect_height &&
                        this_collision.Y_coordinate + this_collision.rect_height > other_collision.Y_coordinate)
                    {
                        print("Rectangles are COLLIDING");

                        // Handle mass properties
                        float total_mass = this_collision.mass + other_collision.mass; // find total mass involved in colilsion
                        // find ratios of who takes more or less of the collision direction
                        float other_mass_ratio = other_collision.mass / total_mass;
                        float this_mass_ratio = this_collision.mass / total_mass;

                        //TODO Move accordingly!
                    }
                }

            }
        }

    }
}
