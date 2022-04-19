using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class vehicleController : MonoBehaviour
{
    //Forces
    public float maxSpeed = 1.0f;
    public float acceleration = 3.0f;
    public float rotationSpeed = 90.0f;
    public float dragForce = -3.0f;

    //Velocity
    public float currentDirectionDeg = 90.0f;
    public Vector3 velocityPerSecond = Vector3.zero;
    public float currentVelocityPerSecond = 0.0f;

    //Drift
    public Vector3 driftVector = Vector3.zero;
    public float driftAmount = 1.0f;
    public float driftAngle = 90.0f;
    public float driftTime = 0.0f;
    public float timeTurning = 0.0f;
    
    //Booleans
    public bool isAccelerating;
    public bool isDeccelerating;
    bool turningLeft;
    bool turningRight;
    bool pressedDrift;

    // Update is called once per frame
    void Update()
    {
        // Track facing direction for vectors
        driftVector = new Vector3(Mathf.Cos(currentDirectionDeg * Mathf.Deg2Rad) * 0.35f, Mathf.Sin(currentDirectionDeg * Mathf.Deg2Rad) * 0.35f); 
        velocityPerSecond = new Vector3(Mathf.Cos(currentDirectionDeg * Mathf.Deg2Rad), Mathf.Sin(currentDirectionDeg * Mathf.Deg2Rad));

        
        //testing SMALLER draw lines for visual aesthetics in debugging...
        Vector3 velocityPerSecondTracker = new Vector3(Mathf.Cos(currentDirectionDeg * Mathf.Deg2Rad) * 0.5f, Mathf.Sin(currentDirectionDeg * Mathf.Deg2Rad) * 0.5f);
        velocityPerSecondTracker *= currentVelocityPerSecond;
        Debug.DrawLine(transform.position, transform.position + velocityPerSecondTracker, Color.green);
        //Vector3 TESTdriftVector = new Vector3(Mathf.Cos((currentDirectionDeg - 90.0f) * Mathf.Deg2Rad) * 0.5f, Mathf.Sin((currentDirectionDeg - 90.0f) * Mathf.Deg2Rad) * 0.5f);
        //TESTdriftVector *= currentVelocityPerSecond;
        //Debug.DrawLine(transform.position, transform.position + TESTdriftVector, Color.cyan);

        // assign bools
        turningLeft = Input.GetKey(KeyCode.A);
        turningRight = Input.GetKey(KeyCode.D);
        pressedDrift = Input.GetKey(KeyCode.Space);
        isAccelerating = Input.GetKey(KeyCode.W);
        isDeccelerating = Input.GetKey(KeyCode.S);

        #region Rotation

        // Attach turning rotation to speed, if going to slow, no turn
        float minSpeedForTurn = currentVelocityPerSecond / 8;
        minSpeedForTurn = Mathf.Clamp01(minSpeedForTurn);

        // attach rotation to vector for steering
        Vector3 Rotation = new Vector3(0.0f, 0.0f, currentDirectionDeg - 90.0f);
        transform.eulerAngles = Rotation;

        //TODO TESTING
        Vector3 dForceLeft = new Vector3(Mathf.Cos((currentDirectionDeg + driftAngle) * Mathf.Deg2Rad) * 0.5f, Mathf.Sin((currentDirectionDeg + driftAngle) * Mathf.Deg2Rad) * 0.5f);
        dForceLeft *= (timeTurning * 5);
        Debug.DrawLine(transform.position, transform.position + dForceLeft, Color.magenta);

        //TODO THIS NEEDS TO HAPPEN REGARDLESS OF TURNING
        Vector3 dForceRight = new Vector3(Mathf.Cos((currentDirectionDeg - driftAngle) * Mathf.Deg2Rad) * 0.5f, Mathf.Sin((currentDirectionDeg - driftAngle) * Mathf.Deg2Rad) * 0.5f);
        dForceRight *= (timeTurning * 5); 
        Debug.DrawLine(transform.position, transform.position + dForceRight, Color.magenta);

        if (turningLeft)
        {
            timeTurning += Time.deltaTime;
            timeTurning = Mathf.Clamp(timeTurning, 0.0f, 1.5f); // clamp it

            if (isDeccelerating)
            {
                // allow turning no matter what
                currentDirectionDeg += rotationSpeed * Time.deltaTime; 
            }
            else // apply rotation speed clamp
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
            timeTurning -= Time.deltaTime;
            driftTime = 0.0f;
        }

        if (turningRight)
        {
            timeTurning += Time.deltaTime;
            timeTurning = Mathf.Clamp(timeTurning, 0.0f, 1.5f); // clamp it

            if (isDeccelerating)
            {
                // allow turning no matter what
                currentDirectionDeg -= rotationSpeed * Time.deltaTime;
            }
            else
            {
                //apply speed turning clamp
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
            timeTurning -= Time.deltaTime;
            driftTime = 0.0f;
        }

        #endregion

        #region Acceleration

        if (isAccelerating)
        {
           
            // set speeds, for smooth changes between reverse to accelerate
            AdjustForces(7.5f, 15.0f, 20.0f, 5.0f, 20.0f);

            //apply acceleration to velocity
            currentVelocityPerSecond += acceleration * Time.deltaTime;
            currentVelocityPerSecond = Mathf.Clamp(currentVelocityPerSecond, -maxSpeed, maxSpeed);
            velocityPerSecond *= currentVelocityPerSecond;
            //Debug.DrawLine(transform.position, transform.position + velocityPerSecond, Color.green); // Draw facing direction

            //Drift vector
            driftVector *= currentVelocityPerSecond;
            Debug.DrawLine(transform.position, transform.position + driftVector, Color.red); // draw drift force

            //TODO apply some steering behaviours?


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
            // Apply Drag
            velocityPerSecond = new Vector3(Mathf.Cos(currentDirectionDeg * Mathf.Deg2Rad), Mathf.Sin(currentDirectionDeg * Mathf.Deg2Rad));
            currentVelocityPerSecond = Mathf.Lerp(currentVelocityPerSecond, dragForce, Time.deltaTime); // lerp between current speed and drag force overtime
            velocityPerSecond *= currentVelocityPerSecond;
        }
        else if (isDeccelerating)
        {
            //adjust reversing values, can't reverse super quick
            AdjustForces(0.0f, 2.0f, 5.0f, 5.0f, 20.0f);
 
            // Do opposite of acceleration for DEcceleration
            velocityPerSecond = new Vector3(-Mathf.Cos(currentDirectionDeg * Mathf.Deg2Rad), -Mathf.Sin(currentDirectionDeg * Mathf.Deg2Rad));
            currentVelocityPerSecond -= acceleration * Time.deltaTime;
            currentVelocityPerSecond = Mathf.Clamp(currentVelocityPerSecond, -maxSpeed, maxSpeed - 10.0f);
            velocityPerSecond *= -currentVelocityPerSecond;


            #region Rigidbody Movement
            ////UNITY RIGIDBODY PHYSICS
            //Vector3 forceDirection = new Vector3(Mathf.Cos(currentDirectionDeg * Mathf.Deg2Rad), Mathf.Sin(currentDirectionDeg * Mathf.Deg2Rad));
            //forceDirection.Normalize();
            //Rigidbody rb = GetComponent<Rigidbody>();
            //rb.AddForce(-forceDirection * acceleration * Time.deltaTime, ForceMode.Force);
            #endregion
        }
        else if (!isDeccelerating && currentVelocityPerSecond < -0.5f) //Not decelleration, but still rolling
        {
            velocityPerSecond = new Vector3(Mathf.Cos(currentDirectionDeg * Mathf.Deg2Rad), Mathf.Sin(currentDirectionDeg * Mathf.Deg2Rad));
            currentVelocityPerSecond = Mathf.Lerp(currentVelocityPerSecond, dragForce, Time.deltaTime);
            velocityPerSecond *= currentVelocityPerSecond;
        }
        else
        {
            // Reset values
            currentVelocityPerSecond = 0;
            velocityPerSecond = Vector3.zero;
        }

        #endregion

        // add rotation & accel vectors to move
        transform.position += velocityPerSecond * Time.deltaTime;
    }

    void AdjustForces(float desiredVelocity, float newAccel, float newMaxSpeed, float originalAccel, float originalMaxSpeed)
    {
        //TODO !!!!Changing editor values!!!!!
        if (currentVelocityPerSecond < desiredVelocity)
        {
            maxSpeed = newMaxSpeed;
            acceleration = newAccel; // quick take offs
        }
        else if (currentVelocityPerSecond > desiredVelocity)
        {
            maxSpeed = originalMaxSpeed;
            acceleration = originalAccel;
        }
    }

    void HandleDriftLeftTurn(bool pressedDrift)
    {
        // Find perpendicular vector to current velocity travelling
        Vector3 driftForce = Vector2.Perpendicular(velocityPerSecond);

        ////TODO THIS NEEDS TO HAPPEN REGARDLESS OF TURNING
        //Vector3 dForce = new Vector3(Mathf.Cos((currentDirectionDeg + driftAngle) * Mathf.Deg2Rad) * 0.5f, Mathf.Sin((currentDirectionDeg + driftAngle) * Mathf.Deg2Rad) * 0.5f);
        //dForce *= (timeTurning * 5); //TODO this could times against another variable, only manipulated when left or right is pressed
        //Debug.DrawLine(transform.position, transform.position + dForce, Color.magenta);

        // add it to drift vector
        if (pressedDrift && currentVelocityPerSecond > 0.0f)
        {
            driftTime += Time.deltaTime; // build up drag period
            driftTime = Mathf.Clamp(driftTime, 0.0f, 1.0f); // clamp it
            driftVector += driftForce; // add drift force to our current drifting vector (rotation considered)
            transform.position -= driftVector * Time.deltaTime * driftAmount; // add to current vector negatively to push away from car
        }
        else if (!pressedDrift && driftTime > 0.0f) // if let go of 'drift' and still sliding
        {
            driftTime -= Time.deltaTime;

            //TODO Handle how transform is translated when drift is released... But how....
            //Vector3 driftDecay = driftForce;
            //driftDecay.Normalize();
            //driftDecay *= 1.5f;
            //
            ////get direction adjustments
            //Vector3 vectorAdjust = ((driftDecay - velocityPerSecond).normalized * 1.5f) * Time.deltaTime;
            //
            //// Determine new velocity direction with drift decay added
            //Vector3 currentDrift = velocityPerSecond + vectorAdjust;
            //currentDrift.Normalize();
            //
            ////determine new velocity
            //velocityPerSecond = currentDrift * 1.5f;
            //velocityPerSecond.x = Mathf.Clamp(velocityPerSecond.x, -maxSpeed, maxSpeed);
            //velocityPerSecond.y = Mathf.Clamp(velocityPerSecond.y, -maxSpeed, maxSpeed);
            //transform.position += velocityPerSecond * Time.deltaTime;


            //driftVector += driftForce; // add drift force to our current drifting vector (rotation considered)
            //transform.position -= driftVector * Time.deltaTime * driftAmount;
            //currentVelocityPerSecond -= 3.0f;
            //driftVector *= currentVelocityPerSecond;
            //Vector3 driftDecay = -driftVector * Time.deltaTime * driftAmount;
            //transform.position += driftDecay; 
        }
        else
        {
            driftTime = 0.0f;
        }
    }

    void HandleDriftRightTurn(bool pressedDrift)
    {
        // create drift force
        Vector3 driftForce = Vector2.Perpendicular(-velocityPerSecond);

        ////TODO THIS NEEDS TO HAPPEN REGARDLESS OF TURNING
        //Vector3 dForce = new Vector3(Mathf.Cos((currentDirectionDeg - driftAngle) * Mathf.Deg2Rad) * 0.5f, Mathf.Sin((currentDirectionDeg - driftAngle) * Mathf.Deg2Rad) * 0.5f);
        //dForce *= (timeTurning * 5);
        //Debug.DrawLine(transform.position, transform.position + dForce, Color.magenta);

        // add it to drift vector
        if (pressedDrift && currentVelocityPerSecond > 0.0f)
        {
            driftTime += Time.deltaTime;
            driftTime = Mathf.Clamp(driftTime, 0.0f, 1.0f);
            driftVector += driftForce;
            transform.position -= driftVector * Time.deltaTime * driftAmount;
        }
        else if (!pressedDrift && driftTime > 0.0f)
        {
            driftTime -= Time.deltaTime;

            //TODO Handle how transform is translated when drift is released... 
            Vector3 driftDecay = -driftVector * Time.deltaTime * driftAmount;
            transform.position += driftDecay;
        }
        else
        {
            driftTime = 0.0f;
        }
    }
}
