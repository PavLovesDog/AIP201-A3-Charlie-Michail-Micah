using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowFieldDetector : MonoBehaviour
{

    Vector3 force = Vector3.zero;

    public int flowForce;
    public FlowFieldManager ffM1;
    public FlowFieldManager ffM2;
    public FlowFieldManager ffM3;
    public FlowFieldManager ffM4;
    public FlowFieldManager ffM5;
    public FlowFieldManager ffM6;
    public FlowFieldManager ffM7;
    public FlowFieldManager ffM8;

    public Vector3 FlowBoundry()
    {
        if (transform.position.x > ffM1.gridStartX && transform.position.x < ffM1.width + ffM1.gridStartX && transform.position.y > ffM1.gridStartY && transform.position.y < ffM1.height + ffM1.gridStartY)
        {
            force = ffM1.GetForceForPosition(transform.position);
            return force * flowForce;
        }

        if (transform.position.x > ffM2.gridStartX && transform.position.x < ffM2.width + ffM2.gridStartX && transform.position.y > ffM2.gridStartY && transform.position.y < ffM2.height + ffM2.gridStartY)
        {
            force = ffM2.GetForceForPosition(transform.position);
            return force * flowForce;
        }

        if (transform.position.x > ffM3.gridStartX && transform.position.x < ffM3.width + ffM3.gridStartX && transform.position.y > ffM3.gridStartY && transform.position.y < ffM3.height + ffM3.gridStartY)
        {
            force = ffM3.GetForceForPosition(transform.position);
            return force * flowForce;
        }

        if (transform.position.x > ffM4.gridStartX && transform.position.x < ffM4.width + ffM4.gridStartX && transform.position.y > ffM4.gridStartY && transform.position.y < ffM4.height + ffM4.gridStartY)
        {
            force = ffM4.GetForceForPosition(transform.position);
            return force * flowForce;
        }

        if (transform.position.x > ffM5.gridStartX && transform.position.x < ffM5.width + ffM5.gridStartX && transform.position.y > ffM5.gridStartY && transform.position.y < ffM5.height + ffM5.gridStartY)
        {
            force = ffM5.GetForceForPosition(transform.position);
            return force * flowForce;
        }

        if (transform.position.x > ffM6.gridStartX && transform.position.x < ffM6.width + ffM6.gridStartX && transform.position.y > ffM6.gridStartY && transform.position.y < ffM6.height + ffM6.gridStartY)
        {
            force = ffM6.GetForceForPosition(transform.position);
            return force * flowForce;
        }

        if (transform.position.x > ffM7.gridStartX && transform.position.x < ffM7.width + ffM7.gridStartX && transform.position.y > ffM7.gridStartY && transform.position.y < ffM7.height + ffM7.gridStartY)
        {
            force = ffM7.GetForceForPosition(transform.position);
            return force * flowForce;
        }

        if (transform.position.x > ffM8.gridStartX && transform.position.x < ffM8.width + ffM8.gridStartX && transform.position.y > ffM8.gridStartY && transform.position.y < ffM8.height + ffM8.gridStartY)
        {
            force = ffM8.GetForceForPosition(transform.position);
            return force * flowForce;
        }

        return Vector3.zero; // return nothing if not in field
    }
}
