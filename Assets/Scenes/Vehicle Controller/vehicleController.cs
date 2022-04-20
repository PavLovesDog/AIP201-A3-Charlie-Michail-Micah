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

    //Drift & Slide
    public Vector3 driftVector = Vector3.zero;
    Vector3 slideForceLeft = Vector3.zero;
    Vector3 slideForceRight = Vector3.zero;
    public float driftAmount = 1.0f;
    public float driftAngle = 90.0f;
    public float driftTime = 0.0f;
    public float timeLeftTurning = 0.0f;
    public float timeRightTurning = 0.0f;
    
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
        slideForceLeft = new Vector3(Mathf.Cos((currentDirectionDeg + driftAngle) * Mathf.Deg2Rad) * 0.5f, Mathf.Sin((currentDirectionDeg + driftAngle) * Mathf.Deg2Rad) * 0.5f);
        slideForceRight = new Vector3(Mathf.Cos((currentDirectionDeg - driftAngle) * Mathf.Deg2Rad) * 0.5f, Mathf.Sin((currentDirectionDeg - driftAngle) * Mathf.Deg2Rad) * 0.5f);

        
        //testing SMALLER draw lines for visual aesthetics in debugging...
        Vector3 velocityPerSecondTracker = new Vector3(Mathf.Cos(currentDirectionDeg * Mathf.Deg2Rad) * 0.5f, Mathf.Sin(currentDirectionDeg * Mathf.Deg2Rad) * 0.5f);
        velocityPerSecondTracker *= currentVelocityPerSecond;
        Debug.DrawLine(transform.position, transform.position + velocityPerSecondTracker, Color.green);

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


        // LEFT TURNING =====================================================================================
        if (turningLeft)
        {
            timeRightTurning = 0.0f; // Reset so right turn does NOT interfere

            // Left Turn Slide
            timeLeftTurning += Time.deltaTime;
            timeLeftTurning = Mathf.Clamp(timeLeftTurning, 0.0f, 1.5f); // clamp it
            slideForceLeft *= (timeLeftTurning * 5) * minSpeedForTurn;
            Debug.DrawLine(transform.position, transform.position + slideForceLeft, Color.magenta); // DUBUG

            transform.position -= slideForceLeft * Time.deltaTime * driftAmount;

            if (isDeccelerating)
            {
                // allow turning no matter what
                currentDirectionDeg += rotationSpeed * Time.deltaTime;
            }
            else // apply rotation speed clamp
            {
                currentDirectionDeg += rotationSpeed * Time.deltaTime * minSpeedForTurn;
            }


        }
        else if (!turningLeft && !turningRight && timeLeftTurning > 0.0f)
        {
            // Left turn slide - Release
            timeLeftTurning -= Time.deltaTime * 2; // multiply speed of release by 2
            timeLeftTurning = Mathf.Clamp(timeLeftTurning, 0.0f, 1.5f);
            slideForceLeft *= (timeLeftTurning * 5) * minSpeedForTurn;
            transform.position -= slideForceLeft * Time.deltaTime * driftAmount;
            Debug.DrawLine(transform.position, transform.position + slideForceLeft, Color.magenta); // DEBUG
        }

        // RIGHT TURNING ======================================================================================
        if (turningRight)
        {
            timeLeftTurning = 0.0f;
            timeRightTurning += Time.deltaTime; // slideing timer
            timeRightTurning = Mathf.Clamp(timeRightTurning, 0.0f, 1.5f); // clamp it
            slideForceRight *= (timeRightTurning * 5) * minSpeedForTurn; // Clamp slide force to velocity (Cant slide while NO velocity)
            Debug.DrawLine(transform.position, transform.position + slideForceRight, Color.magenta);

            transform.position -= slideForceRight * Time.deltaTime * driftAmount; // add slide force to current transform

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

        }
        else if (!turningLeft && !turningRight && timeRightTurning > 0.0f)
        {
            timeRightTurning -= Time.deltaTime * 2;
            timeRightTurning = Mathf.Clamp(timeRightTurning, 0.0f, 1.5f);
            slideForceRight *= (timeRightTurning * 5) * minSpeedForTurn;
            transform.position -= slideForceRight * Time.deltaTime * driftAmount;
            Debug.DrawLine(transform.position, transform.position + slideForceRight, Color.magenta);
        }

        /* below functions do not function correctly with new slide vectors above */
        #region Left Turn Drift

        HandleDriftLeftTurn(pressedDrift);

        #endregion

        #region Right Turn Drift

        HandleDriftRightTurn(pressedDrift);

        #endregion

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
            driftVector = Vector3.zero;
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

    //TODO These Drift functions are now a bit broken with the added turning Slide
    void HandleDriftLeftTurn(bool pressedDrift)
    {
        // Find perpendicular vector to current velocity travelling
        Vector3 driftForce = Vector2.Perpendicular(velocityPerSecond);

        // add it to drift vector
        if (pressedDrift && currentVelocityPerSecond > 0.0f)
        {
            /*Slide Vector addition attempt*/
            //TODO Tried add small amount to slide vector
            driftTime += Time.deltaTime; // build up drag period
            driftTime = Mathf.Clamp(driftTime, 0.0f, 1.0f); // clamp it
            slideForceLeft *= driftTime * driftAmount;
            transform.position -= slideForceLeft * Time.deltaTime;

            /*new drift Vector addition attempt*/
            //TODO Tried adding new vector and add it, though slide & drift vectors both added to velocity is too much
            //driftForce *= driftTime;// * (currentVelocityPerSecond / 8);
            //driftVector += driftForce; // add drift force to our current drifting vector (rotation considered)
            ////driftVector.x = Mathf.Clamp(driftVector.x, 0.0f, 5.0f);
            //transform.position -= driftForce * Time.deltaTime;// * driftAmount; // add to current vector negatively to push away from car
        }
        else if (!pressedDrift && driftTime > 0.0f) // if let go of 'drift' and still sliding
        {
            /*Slide Vector addition attempt*/
            driftTime -= Time.deltaTime;
            driftTime = Mathf.Clamp(driftTime, 0.0f, 1.0f); // clamp it
            slideForceLeft *= driftTime * driftAmount;
            transform.position -= slideForceLeft * Time.deltaTime;

            /*new drift Vector addition attempt*/
            //driftForce *= driftTime;// * (currentVelocityPerSecond / 8); 
            //driftVector += driftForce; // add drift force to our current drifting vector (rotation considered)
            //transform.position -= driftForce * Time.deltaTime;// * driftAmount;

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
            driftTime -= Time.deltaTime;
            driftTime = Mathf.Clamp(driftTime, 0.0f, 1.0f);
        }
    }
}
