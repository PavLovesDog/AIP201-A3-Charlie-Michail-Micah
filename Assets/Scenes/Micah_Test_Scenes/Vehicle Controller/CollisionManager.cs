using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionManager : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        collision[] collisions = FindObjectsOfType<collision>();

        foreach (collision this_collision in collisions)
        {
            foreach (collision other_collision in collisions)
            {
                // Do not detect collision with yourself.
                if (this_collision == other_collision) continue;

                // Circle to Circle collision.
                if (this_collision.type == collision.Type.Circle && other_collision.type == collision.Type.Circle)
                {
                    float this_radius = this_collision.radius;
                    float other_radius = other_collision.radius;

                    Vector3 this_to_other = other_collision.transform.position - this_collision.transform.position;
                    Vector3 other_to_this = this_collision.transform.position - other_collision.transform.position;
                    float distance_this_to_other = this_to_other.magnitude;

                    // A negative intersection depth means no intersection.
                    float intersection_depth = (this_radius + other_radius) - distance_this_to_other;

                    if (intersection_depth > 0.0f)
                    {
                        // AI Resolution: Create a collision event that the gameobjects can respond to

                        float total_mass = this_collision.mass + other_collision.mass;

                        float other_mass_ratio = other_collision.mass / total_mass;
                        float this_mass_ratio = this_collision.mass / total_mass;

                        // Move other using this_to_other vector
                        other_collision.transform.position += this_to_other.normalized * intersection_depth * this_mass_ratio;

                        // Move this using other_to_this vector
                        this_collision.transform.position += other_to_this.normalized * intersection_depth * other_mass_ratio;


                        /* if (other_collision.GetComponent<wander_AI>())
                         {
                             other_collision.GetComponent<wander_AI>().ResetAngle();
                         }
                         if (this_collision.GetComponent<wander_AI>())
                         {
                             this_collision.GetComponent<wander_AI>().ResetAngle();
                         } */
                    }
                }

                // Rectangle to rectangle collisions
                //if (this_collision.type == collision.Type.Rectangle && other_collision.type == collision.Type.Rectangle)
                //{

                //    // Use this:  float width = GetComponent<SpriteRenderer>().bounds.size.x;
                //}
            }


        }
    }
}
