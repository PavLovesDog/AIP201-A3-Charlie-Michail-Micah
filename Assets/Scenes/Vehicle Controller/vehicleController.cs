using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class vehicleController : MonoBehaviour
{

    public float maxSpeed = 1.0f;
    public float acceleration = 3.0f;
    public float rotationSpeed = 90.0f;
    public float dragForce = -3.0f;

    float speed = 3.0f;

    //velocity
    public float currentDirectionDeg = 90.0f;
    public Vector3 velocityPerSecond = Vector3.zero;
    public float currentVelocityPerSecond = 0.0f;
    public Vector3 driftVector = Vector3.zero;
    public float driftAmount = 1.0f;
    public float driftTime = 0.0f;
    public float timeTurning = 0.0f;

        
    public bool isAccelerating;
    public bool isDeccelerating;

    bool turningLeft;
    bool turningRight;
    bool pressedDrift;

    // Update is called once per frame
    void Update()
    {
        driftVector = new Vector3(Mathf.Cos(currentDirectionDeg * Mathf.Deg2Rad) * 0.75f, Mathf.Sin(currentDirectionDeg * Mathf.Deg2Rad) * 0.75f); 
        velocityPerSecond = new Vector3(Mathf.Cos(currentDirectionDeg * Mathf.Deg2Rad), Mathf.Sin(currentDirectionDeg * Mathf.Deg2Rad));

        #region Rotation

        turningLeft = Input.GetKey(KeyCode.A);
        turningRight = Input.GetKey(KeyCode.D);
        pressedDrift = Input.GetKey(KeyCode.Space);

        //bool headingDiagUpLeft = velocityPerSecond.x < 0 && velocityPerSecond.y > 0 && currentVelocityPerSecond > (maxSpeed / 2);
        //bool headingDiagDownLeft = velocityPerSecond.x < 0 && velocityPerSecond.y < 0 && currentVelocityPerSecond > (maxSpeed / 2);
        //bool headingDiagUpRight = velocityPerSecond.x > 0 && velocityPerSecond.y > 0 && currentVelocityPerSecond > (maxSpeed / 2);
        //bool headingDiagDownRight = velocityPerSecond.x > 0 && velocityPerSecond.y < 0 && currentVelocityPerSecond > (maxSpeed / 2);


        // Attach turning rotation to speed, if going to slow, no turn
        float minSpeedForTurn = currentVelocityPerSecond / 8;
        minSpeedForTurn = Mathf.Clamp01(minSpeedForTurn);

        Vector3 Rotation = new Vector3(0.0f, 0.0f, currentDirectionDeg - 90.0f);
        transform.eulerAngles = Rotation;
        if (turningLeft) // LEFT
        {
            ////Start turning counter for drift
            //timeTurning += Time.deltaTime;

            if(Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.S))
            {
                currentDirectionDeg += rotationSpeed * Time.deltaTime;
            }
            else
            {
                currentDirectionDeg += rotationSpeed * Time.deltaTime * minSpeedForTurn; 
            }

            #region Left Turn Drift
                HandleDriftLeftTurn(pressedDrift);

            #region old drifting repeat
            /* 
                if (headingDiagUpLeft)
                {
                    // create drift force
                    //Vector3 driftForce = new Vector3(velocityPerSecond.x * lowerLimit, velocityPerSecond.y * upperLimit, 0.0f);
                    //Vector3 Force = Vector2.Perpendicular(-driftForce);
                    
                    Vector3 driftForce = Vector2.Perpendicular(velocityPerSecond);
                    Debug.DrawLine(transform.position, driftForce, Color.magenta);
                    // add it to drift vector
                    //timeTurning > 0.5
                    if (pressedDrift)
                    {
                        driftVector += driftForce;
                        transform.position -= driftVector * Time.deltaTime * driftAmount;
                    }
                    else
                    {
                        Vector3 driftDecay = driftVector * Time.deltaTime * driftAmount;
                        transform.position += driftDecay;
                    }
                
                }
                else if (headingDiagDownLeft)
                {
                    //Vector3 driftForce = new Vector3(velocityPerSecond.x * upperLimit, velocityPerSecond.y * lowerLimit, 0.0f);
                    Vector3 driftForce = Vector2.Perpendicular(velocityPerSecond);
                    Debug.DrawLine(transform.position, driftForce, Color.magenta);
                
                    if (pressedDrift)
                    {
                        driftVector += driftForce;
                        transform.position -= driftVector * Time.deltaTime * driftAmount;
                    }
                }
                else if(headingDiagDownRight)
                {
                    //Vector3 driftForce = new Vector3(velocityPerSecond.x * lowerLimit, velocityPerSecond.y * upperLimit, 0.0f);
                    Vector3 driftForce = Vector2.Perpendicular(velocityPerSecond);
                    Debug.DrawLine(transform.position, driftForce, Color.magenta);
                
                    if (pressedDrift)
                    {
                        driftVector += driftForce;
                        transform.position -= driftVector * Time.deltaTime * driftAmount;
                    }
                }
                else if (headingDiagUpRight)
                {
                    //Vector3 driftForce = new Vector3(velocityPerSecond.x * upperLimit, velocityPerSecond.y * lowerLimit, 0.0f);
                    Vector3 driftForce = Vector2.Perpendicular(velocityPerSecond);
                    Debug.DrawLine(transform.position, driftForce, Color.magenta);
                
                    if (pressedDrift)
                    {
                        driftVector += driftForce;
                        transform.position -= driftVector * Time.deltaTime * driftAmount;
                    }
                }
             */
            #endregion

            #endregion

        }
        else if (!turningLeft && !turningRight)
        {
            timeTurning = 0.0f; // reset for next turn!
            driftTime = 0.0f;
        }

        if (turningRight) // RIGHT
        {
            ////Start turning counter for drift
            //timeTurning += Time.deltaTime;

            if (Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.S))
            {
                currentDirectionDeg -= rotationSpeed * Time.deltaTime;
            }
            else
            {
                currentDirectionDeg -= rotationSpeed * Time.deltaTime * minSpeedForTurn;
            }

            #region Right Turn Drift

            HandleDriftRightTurn(pressedDrift);

            #region Old Drift Repetative
            //if (headingDiagUpLeft)
            //{
            //    // create drift force
            //    Vector3 driftForce = Vector2.Perpendicular(-velocityPerSecond);
            //    Debug.DrawLine(transform.position, driftForce, Color.magenta);
            //
            //    // add it to drift vector
            //    if (pressedDrift)
            //    {
            //        driftVector += driftForce;
            //        transform.position -= driftVector * Time.deltaTime * driftAmount;
            //    }
            //}
            //else if (headingDiagDownLeft)
            //{
            //    Vector3 driftForce = Vector2.Perpendicular(-velocityPerSecond);
            //    Debug.DrawLine(transform.position, driftForce, Color.magenta);
            //
            //    if (pressedDrift)
            //    {
            //        driftVector += driftForce;
            //        transform.position -= driftVector * Time.deltaTime * driftAmount;
            //    }
            //}
            //else if (headingDiagDownRight)
            //{
            //    Vector3 driftForce = Vector2.Perpendicular(-velocityPerSecond);
            //    Debug.DrawLine(transform.position, driftForce, Color.magenta);
            //
            //    if (pressedDrift)
            //    {
            //        driftVector += driftForce;
            //        transform.position -= driftVector * Time.deltaTime * driftAmount;
            //    }
            //}
            //else if (headingDiagUpRight)
            //{
            //    Vector3 driftForce = Vector2.Perpendicular(-velocityPerSecond);
            //    Debug.DrawLine(transform.position, driftForce, Color.magenta);
            //
            //    if (pressedDrift)
            //    {
            //        driftVector += driftForce;
            //        transform.position -= driftVector * Time.deltaTime * driftAmount;
            //    }
            //}
            #endregion

            #endregion
        }
        else if (!turningLeft && !turningRight)
        {
            timeTurning = 0.0f;
            driftTime = 0.0f;
        }

        #endregion

        #region Acceleration

        isAccelerating = Input.GetKey(KeyCode.W);
        isDeccelerating = Input.GetKey(KeyCode.S);

        if (isAccelerating)
        {
            // Speed up acceleration when reversing for faster take off
            if(currentVelocityPerSecond < 0.0f)
            {
                acceleration = 20.0f;
            }
            else
            {
                acceleration = 5.0f;
            }

            //Drift vector
            driftVector *= currentVelocityPerSecond;
            Debug.DrawLine(transform.position, transform.position + driftVector, Color.red);

            //turn to direction
            //velocityPerSecond = new Vector3(Mathf.Cos(currentDirectionDeg * Mathf.Deg2Rad), Mathf.Sin(currentDirectionDeg * Mathf.Deg2Rad));
            currentVelocityPerSecond += acceleration * Time.deltaTime;
            currentVelocityPerSecond = Mathf.Clamp(currentVelocityPerSecond, -maxSpeed, maxSpeed);
            velocityPerSecond *= currentVelocityPerSecond;
            Debug.DrawLine(transform.position, transform.position + velocityPerSecond, Color.green); // Draw facing direction



            #region Rigidbody Movement
            ////UNITY RIGIDBODY PHYSICS
            //Vector3 forceDirection = new Vector3(Mathf.Cos(currentDirectionDeg * Mathf.Deg2Rad), Mathf.Sin(currentDirectionDeg * Mathf.Deg2Rad));
            //forceDirection.Normalize();
            //Rigidbody rb = GetComponent<Rigidbody>();
            //rb.AddForce(forceDirection * acceleration * Time.deltaTime, ForceMode.Force);
            #endregion
        }
        else if (!isAccelerating && currentVelocityPerSecond > 0.5f) // if not holding GO & still rolling
        {

            // Drag
            velocityPerSecond = new Vector3(Mathf.Cos(currentDirectionDeg * Mathf.Deg2Rad), Mathf.Sin(currentDirectionDeg * Mathf.Deg2Rad));
            currentVelocityPerSecond = Mathf.Lerp(currentVelocityPerSecond, dragForce, Time.deltaTime);
            //currentVelocityPerSecond -= acceleration * Time.deltaTime;
            velocityPerSecond *= currentVelocityPerSecond;

         
        }
        else if (isDeccelerating)
        {
            if (currentVelocityPerSecond > 0.0f)
            {
                acceleration = 20.0f; //TODO this is never hit? Why?
            }
            else
            {
                acceleration = 5.0f;
            }

            // OWN PHYSICS
            velocityPerSecond = new Vector3(-Mathf.Cos(currentDirectionDeg * Mathf.Deg2Rad), -Mathf.Sin(currentDirectionDeg * Mathf.Deg2Rad));
            currentVelocityPerSecond -= acceleration * Time.deltaTime;
            currentVelocityPerSecond = Mathf.Clamp(currentVelocityPerSecond, -maxSpeed, maxSpeed -5.0f);
            velocityPerSecond *= -currentVelocityPerSecond;
            //transform.position -= velocityPerSecond * currentVelocityPerSecond * Time.deltaTime;


            #region Rigidbody Movement
            ////UNITY RIGIDBODY PHYSICS
            //Vector3 forceDirection = new Vector3(Mathf.Cos(currentDirectionDeg * Mathf.Deg2Rad), Mathf.Sin(currentDirectionDeg * Mathf.Deg2Rad));
            //forceDirection.Normalize();
            //Rigidbody rb = GetComponent<Rigidbody>();
            //rb.AddForce(-forceDirection * acceleration * Time.deltaTime, ForceMode.Force);
            #endregion
        }
        else if (!isDeccelerating && currentVelocityPerSecond < -0.5f)
        {
            velocityPerSecond = new Vector3(Mathf.Cos(currentDirectionDeg * Mathf.Deg2Rad), Mathf.Sin(currentDirectionDeg * Mathf.Deg2Rad));
            currentVelocityPerSecond = Mathf.Lerp(currentVelocityPerSecond, dragForce, Time.deltaTime);
            //currentVelocityPerSecond += acceleration * Time.deltaTime;
            //currentVelocityPerSecond = Mathf.Clamp(currentVelocityPerSecond, -maxSpeed, maxSpeed);
            velocityPerSecond *= currentVelocityPerSecond;
        }
        else
        {
            currentVelocityPerSecond = 0;
            velocityPerSecond = Vector3.zero;
        }

        #endregion

        // add rotation & accel vectors to move
        transform.position += velocityPerSecond * Time.deltaTime;
    }

    //TODO Handle how transform is translated when drift is released...
    void HandleDriftLeftTurn(bool pressedDrift)
    {
        //bool pressedDrift = Input.GetKey(KeyCode.Space);
        // create drift force
        //Vector3 driftForce = new Vector3(velocityPerSecond.x * lowerLimit, velocityPerSecond.y * upperLimit, 0.0f);
        //Vector3 Force = Vector2.Perpendicular(-driftForce);

        Vector3 driftForce = Vector2.Perpendicular(velocityPerSecond);
        //Debug.DrawLine(transform.position, driftForce, Color.magenta);

        // add it to drift vector

        if (pressedDrift)
        {
            driftTime += Time.deltaTime;
            driftTime = Mathf.Clamp(driftTime, 0.0f, 0.5f);
            driftVector += driftForce;
            transform.position -= driftVector * Time.deltaTime * driftAmount;
        }
        else if (!pressedDrift && driftTime > 0.0f)
        {
            driftTime -= Time.deltaTime;

            Vector3 driftDecay = -driftVector * Time.deltaTime * driftAmount;
            transform.position += driftDecay; 
        }
        else
        {
            driftTime = 0.0f;
        }
    }

    //TODO Handle how transform is translated when drift is released...
    void HandleDriftRightTurn(bool pressedDrift)
    {
        //bool pressedDrift = Input.GetKey(KeyCode.Space);
        // create drift force
        Vector3 driftForce = Vector2.Perpendicular(-velocityPerSecond);
        //Debug.DrawLine(transform.position, driftForce, Color.magenta);

        // add it to drift vector
        if (pressedDrift)
        {
            driftTime += Time.deltaTime;
            driftTime = Mathf.Clamp(driftTime, 0.0f, 0.5f);
            driftVector += driftForce;
            transform.position -= driftVector * Time.deltaTime * driftAmount;
        }
        else if (!pressedDrift && driftTime > 0.0f)
        {
            Vector3 driftDecay = -driftVector * Time.deltaTime * driftAmount;
            transform.position += driftDecay;
        }
        else
        {
            driftTime = 0.0f;
        }
    }
}
