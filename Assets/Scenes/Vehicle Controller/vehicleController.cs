using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using B83.Image.BMP;

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

    //public int flowForce;
    //public FlowFieldManager ffM1;
    //public FlowFieldManager ffM2;
    //public FlowFieldManager ffM3;
    //public FlowFieldManager ffM4;
    //public FlowFieldManager ffM5;
    //public FlowFieldManager ffM6;
    //public FlowFieldManager ffM7;
    //public FlowFieldManager ffM8;

    public FlowFieldDetector ffD;

    // Update is called once per frame
    void Update()
    {
        //// Load Textures with Bitmap files
        //BMPLoader loader = new BMPLoader();
        ////loader.ForceAlphaReadWhenPossible = true; // can be uncomment to read alpha
        //
        ////load the image data
        //BMPImage img = loader.LoadBMP("C:\\Users\\charl\\Pictures\\bmpee.bmp");
        //
        //// Convert the Color32 array into a Texture2D
        //Texture2D tex = img.ToTexture2D();
        //
        //print(tex);


        // Track facing direction for vectors
        driftVector = new Vector3(Mathf.Cos(currentDirectionDeg * Mathf.Deg2Rad) * 0.35f, Mathf.Sin(currentDirectionDeg * Mathf.Deg2Rad) * 0.35f); 
        velocityPerSecond = new Vector3(Mathf.Cos(currentDirectionDeg * Mathf.Deg2Rad), Mathf.Sin(currentDirectionDeg * Mathf.Deg2Rad));
        slideForceLeft = new Vector3(Mathf.Cos((currentDirectionDeg + driftAngle) * Mathf.Deg2Rad) * 0.5f, Mathf.Sin((currentDirectionDeg + driftAngle) * Mathf.Deg2Rad) * 0.5f);
        slideForceRight = new Vector3(Mathf.Cos((currentDirectionDeg - driftAngle) * Mathf.Deg2Rad) * 0.5f, Mathf.Sin((currentDirectionDeg - driftAngle) * Mathf.Deg2Rad) * 0.5f);

        
        //testing SMALLER draw lines for visual aesthetics in debugging...
        Vector3 velocityPerSecondTracker = new Vector3(Mathf.Cos(currentDirectionDeg * Mathf.Deg2Rad) * 0.5f, Mathf.Sin(currentDirectionDeg * Mathf.Deg2Rad) * 0.5f);
        velocityPerSecondTracker *= currentVelocityPerSecond;
        velocityPerSecondTracker *= 0.5f;
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
        minSpeedForTurn = Mathf.Clamp01(minSpeedForTurn); // clamp betwee 0 & 1

        //TODO CHANGE move rotation based on physics, a force in the direction we wish to turn

        // attach rotation to vector for steering
        Vector3 Rotation = new Vector3(0.0f, 0.0f, currentDirectionDeg - 90.0f);
        transform.eulerAngles = Rotation;


        // LEFT TURNING =====================================================================================
        if (turningLeft)
        {
            timeRightTurning -= Time.deltaTime * 2.0f; // Reset so right turn does NOT interfere

            //TODO Slide Force vector runs too rampant when just holding turn direction, need to lower it with velocity
            // Left Turn Slide
            timeLeftTurning += Time.deltaTime * 2.0f;
            timeLeftTurning = Mathf.Clamp(timeLeftTurning, 0.0f, 1.5f); // clamp it
            slideForceLeft *= (timeLeftTurning * 5) * minSpeedForTurn;
            velocityPerSecond -= slideForceLeft * Time.deltaTime * driftAmount; // add slide force to current force

            Debug.DrawLine(transform.position, transform.position + slideForceLeft, Color.magenta); // DUBUG

            if (isDeccelerating)
            {
                // allow turning no matter what
                currentDirectionDeg += rotationSpeed * Time.deltaTime;
            }
            else // apply rotation speed clamp
            {
                currentDirectionDeg += rotationSpeed * Time.deltaTime * minSpeedForTurn;
            }

            // drift left
            if (pressedDrift && currentVelocityPerSecond > 0.0f)
            {
                // immediately increase slide vector force, to simulate kickout & slide
                timeLeftTurning += Time.deltaTime * 5; // slideing timer
                timeLeftTurning = Mathf.Clamp(timeLeftTurning, 0.0f, 2.0f); // clamp it
                slideForceLeft *= (timeLeftTurning * 5) * minSpeedForTurn;
            }


        }
        else if (!turningLeft && !turningRight && timeLeftTurning > 0.0f)
        {
            // Left turn slide - Release
            timeLeftTurning -= Time.deltaTime * 2; // multiply speed of release by 2
            timeLeftTurning = Mathf.Clamp(timeLeftTurning, 0.0f, 1.5f);
            slideForceLeft *= (timeLeftTurning * 5) * minSpeedForTurn;
            velocityPerSecond -= slideForceLeft * Time.deltaTime * driftAmount;
            Debug.DrawLine(transform.position, transform.position + slideForceLeft, Color.magenta); // DEBUG
        }

        // RIGHT TURNING ======================================================================================
        if (turningRight)
        {
            timeLeftTurning -= Time.deltaTime * 2.0f;

            timeRightTurning += Time.deltaTime * 2; // slideing timer
            timeRightTurning = Mathf.Clamp(timeRightTurning, 0.0f, 1.5f); // clamp it
            slideForceRight *= (timeRightTurning * 5) * minSpeedForTurn; // Clamp slide force to velocity (Cant slide while NO velocity)
            Debug.DrawLine(transform.position, transform.position + slideForceRight, Color.magenta);

            velocityPerSecond -= slideForceRight * Time.deltaTime * driftAmount; // add slide force to current force

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

            // drift right
            if (pressedDrift && currentVelocityPerSecond > 0.0f)
            {
                // immediately increse slide vector force, to simulate kickout & slide
                timeRightTurning += Time.deltaTime * 5;
                timeRightTurning = Mathf.Clamp(timeRightTurning, 0.0f, 2.0f); // clamp it
                slideForceRight *= (timeRightTurning * 5) * minSpeedForTurn;
            }

        }
        else if (!turningLeft && !turningRight && timeRightTurning > 0.0f)
        {
            // Right slide release
            timeRightTurning -= Time.deltaTime * 2;
            timeRightTurning = Mathf.Clamp(timeRightTurning, 0.0f, 1.5f);
            slideForceRight *= (timeRightTurning * 5) * minSpeedForTurn;
            velocityPerSecond -= slideForceRight * Time.deltaTime * driftAmount;
            Debug.DrawLine(transform.position, transform.position + slideForceRight, Color.magenta);
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
            //Debug.DrawLine(transform.position, transform.position + driftVector, Color.red); // draw drift force

            #region Rigidbody Movement
            ////UNITY RIGIDBODY PHYSICS
            //Vector3 forceDirection = new Vector3(Mathf.Cos(currentDirectionDeg * Mathf.Deg2Rad), Mathf.Sin(currentDirectionDeg * Mathf.Deg2Rad));
            //forceDirection.Normalize();
            //Rigidbody rb = GetComponent<Rigidbody>();
            //rb.AddForce(forceDirection * acceleration * Time.deltaTime, ForceMode.Force);
            #endregion
        }
        else if (isDeccelerating)
        {
            //TODO Create a new vector to pull in the opposite direction, influencing the same vector seems inefficient

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
        else if (!isAccelerating && currentVelocityPerSecond > 0.5f) // if not holding GO & still rolling
        {
            // Apply Drag
            velocityPerSecond = new Vector3(Mathf.Cos(currentDirectionDeg * Mathf.Deg2Rad), Mathf.Sin(currentDirectionDeg * Mathf.Deg2Rad));
            currentVelocityPerSecond = Mathf.Lerp(currentVelocityPerSecond, dragForce, Time.deltaTime); // lerp between current speed and drag force overtime
            currentVelocityPerSecond = Mathf.Clamp(currentVelocityPerSecond, 0, 100);
            velocityPerSecond *= currentVelocityPerSecond;
        }
        //else if (!isDeccelerating && currentVelocityPerSecond < -0.5f) //Not decelleration, but still rolling
        //{
        //    velocityPerSecond = new Vector3(Mathf.Cos(currentDirectionDeg * Mathf.Deg2Rad), Mathf.Sin(currentDirectionDeg * Mathf.Deg2Rad));
        //    currentVelocityPerSecond = Mathf.Lerp(currentVelocityPerSecond, dragForce, Time.deltaTime);
        //    velocityPerSecond *= currentVelocityPerSecond;
        //}
        else
        {
            // Reset values
            currentVelocityPerSecond = 0;
            velocityPerSecond = Vector3.zero;
            driftVector = Vector3.zero;
        }

        #endregion

        //GET Force force from Flow Field and apply it to Car respective of EACH flowfield
        //FlowBoundry();

        //TODO FLOW FIELD ACTIVATION HERE
        // Invoke flow Field Forces onto velocity
        velocityPerSecond += ffD.FlowBoundry();
            
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

    //void FlowBoundry()
    //{
    //    if (transform.position.x > ffM1.gridStartX && transform.position.x < ffM1.width + ffM1.gridStartX && transform.position.y > ffM1.gridStartY && transform.position.y < ffM1.height + ffM1.gridStartY)
    //    {
    //        Vector3 force = ffM1.GetForceForPosition(transform.position);
    //        velocityPerSecond += force * flowForce;
    //    }
    //
    //    if (transform.position.x > ffM2.gridStartX && transform.position.x < ffM2.width + ffM2.gridStartX && transform.position.y > ffM2.gridStartY && transform.position.y < ffM2.height + ffM2.gridStartY)
    //    {
    //        Vector3 force = ffM2.GetForceForPosition(transform.position);
    //        velocityPerSecond += force * flowForce;
    //    }
    //
    //    if (transform.position.x > ffM3.gridStartX && transform.position.x < ffM3.width + ffM3.gridStartX && transform.position.y > ffM3.gridStartY && transform.position.y < ffM3.height + ffM3.gridStartY)
    //    {
    //        Vector3 force = ffM3.GetForceForPosition(transform.position);
    //        velocityPerSecond += force * flowForce;
    //    }
    //
    //    if (transform.position.x > ffM4.gridStartX && transform.position.x < ffM4.width + ffM4.gridStartX && transform.position.y > ffM4.gridStartY && transform.position.y < ffM4.height + ffM4.gridStartY)
    //    {
    //        Vector3 force = ffM4.GetForceForPosition(transform.position);
    //        velocityPerSecond += force * flowForce;
    //    }
    //
    //    if (transform.position.x > ffM5.gridStartX && transform.position.x < ffM5.width + ffM5.gridStartX && transform.position.y > ffM5.gridStartY && transform.position.y < ffM5.height + ffM5.gridStartY)
    //    {
    //        Vector3 force = ffM5.GetForceForPosition(transform.position);
    //        velocityPerSecond += force * flowForce;
    //    }
    //
    //    if (transform.position.x > ffM6.gridStartX && transform.position.x < ffM6.width + ffM6.gridStartX && transform.position.y > ffM6.gridStartY && transform.position.y < ffM6.height + ffM6.gridStartY)
    //    {
    //        Vector3 force = ffM6.GetForceForPosition(transform.position);
    //        velocityPerSecond += force * flowForce;
    //    }
    //
    //    if (transform.position.x > ffM7.gridStartX && transform.position.x < ffM7.width + ffM7.gridStartX && transform.position.y > ffM7.gridStartY && transform.position.y < ffM7.height + ffM7.gridStartY)
    //    {
    //        Vector3 force = ffM7.GetForceForPosition(transform.position);
    //        velocityPerSecond += force * flowForce;
    //    }
    //
    //    if (transform.position.x > ffM8.gridStartX && transform.position.x < ffM8.width + ffM8.gridStartX && transform.position.y > ffM8.gridStartY && transform.position.y < ffM8.height + ffM8.gridStartY)
    //    {
    //        Vector3 force = ffM8.GetForceForPosition(transform.position);
    //        velocityPerSecond += force * flowForce;
    //    }
    //}

    #region Old Drift Functions

    ////TODO These Drift functions are now a bit broken with the added turning Slide
    //void HandleDriftLeftTurn(bool pressedDrift)
    //{
    //    //TODO Just make the other drift vector Max, for instant slide
    //    // Find perpendicular vector to current velocity travelling
    //    Vector3 driftForce = Vector2.Perpendicular(velocityPerSecond);
    //
    //    // add it to drift vector
    //    if (pressedDrift && currentVelocityPerSecond > 0.0f)
    //    {
    //        /*Slide Vector addition attempt*/
    //        //TODO Tried add small amount to slide vector
    //        driftTime += Time.deltaTime; // build up drag period
    //        driftTime = Mathf.Clamp(driftTime, 0.0f, 1.0f); // clamp it
    //        slideForceLeft *= driftTime * driftAmount;
    //        transform.position -= slideForceLeft * Time.deltaTime;
    //
    //        /*new drift Vector addition attempt*/
    //        //TODO Tried adding new vector and add it, though slide & drift vectors both added to velocity is too much
    //        //driftForce *= driftTime;// * (currentVelocityPerSecond / 8);
    //        //driftVector += driftForce; // add drift force to our current drifting vector (rotation considered)
    //        ////driftVector.x = Mathf.Clamp(driftVector.x, 0.0f, 5.0f);
    //        //transform.position -= driftForce * Time.deltaTime;// * driftAmount; // add to current vector negatively to push away from car
    //    }
    //    else if (!pressedDrift && driftTime > 0.0f) // if let go of 'drift' and still sliding
    //    {
    //        /*Slide Vector addition attempt*/
    //        driftTime -= Time.deltaTime;
    //        driftTime = Mathf.Clamp(driftTime, 0.0f, 1.0f); // clamp it
    //        slideForceLeft *= driftTime * driftAmount;
    //        transform.position -= slideForceLeft * Time.deltaTime;
    //
    //        /*new drift Vector addition attempt*/
    //        //driftForce *= driftTime;// * (currentVelocityPerSecond / 8); 
    //        //driftVector += driftForce; // add drift force to our current drifting vector (rotation considered)
    //        //transform.position -= driftForce * Time.deltaTime;// * driftAmount;
    //
    //    }
    //    else
    //    {
    //        driftTime = 0.0f;
    //    }
    //}
    //
    //void HandleDriftRightTurn(bool pressedDrift)
    //{
    //    // create drift force
    //    Vector3 driftForce = Vector2.Perpendicular(-velocityPerSecond);
    //
    //    // add it to drift vector
    //    if (pressedDrift && currentVelocityPerSecond > 0.0f)
    //    {
    //        driftTime += Time.deltaTime;
    //        driftTime = Mathf.Clamp(driftTime, 0.0f, 1.0f);
    //        driftVector += driftForce;
    //        transform.position -= driftVector * Time.deltaTime * driftAmount;
    //    }
    //    else if (!pressedDrift && driftTime > 0.0f)
    //    {
    //        driftTime -= Time.deltaTime;
    //
    //        //TODO Handle how transform is translated when drift is released... 
    //        Vector3 driftDecay = -driftVector * Time.deltaTime * driftAmount;
    //        transform.position += driftDecay;
    //    }
    //    else
    //    {
    //        driftTime -= Time.deltaTime;
    //        driftTime = Mathf.Clamp(driftTime, 0.0f, 1.0f);
    //    }
    //}

    #endregion

}