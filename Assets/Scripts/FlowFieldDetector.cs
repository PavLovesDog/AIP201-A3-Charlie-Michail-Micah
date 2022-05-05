using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowFieldDetector : MonoBehaviour
{

    Vector3 force = Vector3.zero;

    public int flowForce;
    public List<FlowFieldManager> AIFlowFields = new List<FlowFieldManager>();
    public List<FlowFieldManager> PlayerFlowFields = new List<FlowFieldManager>();

    private void Start()
    {
        //Fill out NPC flow fields
        if (gameObject.CompareTag("NPC"))
        {
            // detect items in scene with "A.I FlowField" tag
            GameObject[] AIflowFields = GameObject.FindGameObjectsWithTag("A.I FlowField");

            // add those items to list
            foreach (GameObject ff in AIflowFields)
            {
                AIFlowFields.Add(ff.GetComponent<FlowFieldManager>());
            }
        }

        //Fill out Player flow fields
        if (gameObject.CompareTag("Player"))
        {
            // detect items in scene with "FlowField" tag
            GameObject[] playerFlowFields = GameObject.FindGameObjectsWithTag("Player FlowField");

            // add those items to list
            foreach (GameObject ff in playerFlowFields)
            {
                PlayerFlowFields.Add(ff.GetComponent<FlowFieldManager>());
            }
        }
    }

    public Vector3 FlowBoundry()
    {
        // Enact flow fields force on NPC's
        if(gameObject.CompareTag("NPC"))
        {
            for (int i = 0; i < AIFlowFields.Count; i++)
            {
                // check if NPC within current checked field
                if (transform.position.x > AIFlowFields[i].gridStartX &&
                    transform.position.x < AIFlowFields[i].width + AIFlowFields[i].gridStartX && 
                    transform.position.y > AIFlowFields[i].gridStartY && 
                    transform.position.y < AIFlowFields[i].height + AIFlowFields[i].gridStartY)
                {
                    force = AIFlowFields[i].GetForceForPosition(transform.position);
                    return force * flowForce;
                }
            }
        }

        // Enact flow fields force on players
        if (gameObject.CompareTag("Player"))
        {
            for (int i = 0; i < PlayerFlowFields.Count; i++)
            {
                // check if player within field
                if (transform.position.x > PlayerFlowFields[i].gridStartX &&
                    transform.position.x < PlayerFlowFields[i].width + PlayerFlowFields[i].gridStartX &&
                    transform.position.y > PlayerFlowFields[i].gridStartY &&
                    transform.position.y < PlayerFlowFields[i].height + PlayerFlowFields[i].gridStartY)
                {
                    force = PlayerFlowFields[i].GetForceForPosition(transform.position);
                    return force * flowForce;
                }
            }
        }

        return Vector3.zero; // return nothing if not in field
    }
}
