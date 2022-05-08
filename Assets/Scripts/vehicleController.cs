using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using B83.Image.BMP;

public class vehicleController : MonoBehaviour
{
    /* A lot of these varibles can be hidden when testing is done */
    
    [Header("FORCES % VELOCITY")]
    public float maxSpeed = 1.0f;
    public float acceleration = 3.0f;
    public float deceleration = 0.0f;
    public float rotationSpeed = 90.0f;
    public float dragForce = -3.0f;
    //Velocity
    public Vector3 velocityPerSecond = Vector3.zero;
    Vector3 decelrationForce = Vector3.zero;
    public float currentVelocityPerSecond = 0.0f;
    public float currentDirectionDeg = 90.0f;
    public float currentNegativeVelocityPerSecond = 0.0f;

    //Drift & Slide
    [Header("DRIFT & SLIDE")]
    public Vector3 driftVector = Vector3.zero;
    Vector3 slideForceLeft = Vector3.zero;
    Vector3 slideForceRight = Vector3.zero;
    public float driftAmount = 1.0f;
    public float driftAngle = 90.0f;
    public float timeLeftTurning = 0.0f;
    public float timeRightTurning = 0.0f;
    float minSpeedForTurn = 0.0f;

    //Booleans
    public bool isAccelerating;
    public bool isDeccelerating;
    public bool turningLeft;
    public bool turningRight;
    public bool accelerateSoundGate;
    bool pressedDrift;

    [Header("TRACK REFERENCES")]
    public GameObject startFinishLine;
    public GameObject lapCountReset;
    public bool readyToCrossFinishLine;
    public int lap = 0;
    public FlowFieldDetector ffD;
    public GameManager gm;
    public audioManager audioM;

    private void Start()
    {
        readyToCrossFinishLine = false;
        accelerateSoundGate = true;
    }

    void Update()
    {
        TrackLap();

        // Track facing direction for vectors
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

        // Attach turning rotation to speed, if going too slow, no turn
        minSpeedForTurn = currentVelocityPerSecond / 8;
        minSpeedForTurn = Mathf.Clamp01(minSpeedForTurn); // clamp betwee 0 & 1

        // attach rotation to vector for steering
        Vector3 Rotation = new Vector3(0.0f, 0.0f, currentDirectionDeg - 90.0f);
        transform.eulerAngles = Rotation; // This is what steers or perp

        if (gm.isGameRunning)
        {
            #region Acceleration
            if (isAccelerating)
            {
                if(accelerateSoundGate)
                {
                    //play sounds
                    audioM.PlayAudio(audioM.accelerationSource, audioM.accelerationSound, 0.15f);
                    accelerateSoundGate = false;
                }

                audioM.PlayAudio(audioM.drivingSource, audioM.drivingSound, 0.25f);

                //apply acceleration to velocity
                currentVelocityPerSecond += acceleration * Time.deltaTime;
                currentVelocityPerSecond = Mathf.Clamp(currentVelocityPerSecond, -maxSpeed, maxSpeed);
                velocityPerSecond = HandleTurningAndSlide(velocityPerSecond);
                velocityPerSecond *= currentVelocityPerSecond;

            }
            else if (!isAccelerating && currentVelocityPerSecond > 0.5f) // if not holding GO & still rolling
            {
                accelerateSoundGate = true;

                //play engine sounds
               audioM.PlayAudio(audioM.drivingSource, audioM.drivingSound, 0.25f);

                // Apply Drag
                velocityPerSecond = new Vector3(Mathf.Cos(currentDirectionDeg * Mathf.Deg2Rad), Mathf.Sin(currentDirectionDeg * Mathf.Deg2Rad));
                currentVelocityPerSecond = Mathf.Lerp(currentVelocityPerSecond, 0, Time.deltaTime * dragForce); // lerp between current speed and drag force overtime
                currentVelocityPerSecond = Mathf.Clamp(currentVelocityPerSecond, 0, 100);
                velocityPerSecond = HandleTurningAndSlide(velocityPerSecond);
                velocityPerSecond *= currentVelocityPerSecond;
            }
            else
            {
                // Reset values
                currentVelocityPerSecond = 0;
                velocityPerSecond = Vector3.zero;
                //driftVector = Vector3.zero;
            }

        #endregion

            #region Deceleration 
            if (isDeccelerating)
            {
                //TODO OMG THIS WORKS. what the fuck
                currentVelocityPerSecond -= 4 * Time.deltaTime;
                
                // assign deceleration force
                decelrationForce = new Vector3(Mathf.Cos(currentDirectionDeg * Mathf.Deg2Rad), Mathf.Sin(currentDirectionDeg * Mathf.Deg2Rad));
                decelrationForce *= -deceleration;

                Debug.DrawLine(transform.position, transform.position + decelrationForce, Color.cyan);

                //allow for turning whilst reversing
                if (turningLeft)
                {
                    // allow turning no matter what
                    //TODO THIS is too quick... how to slow it down??
                    currentDirectionDeg += rotationSpeed * Time.deltaTime;
                }
                if(turningRight)
                {
                    currentDirectionDeg -= rotationSpeed * Time.deltaTime;
                }
    
                if (currentVelocityPerSecond <= 0)
                {
                    deceleration *= 3;
                    deceleration = Mathf.Clamp(deceleration, 0.0f, 10.0f);
                }
                else
                {
                    deceleration = 6;
                }

                velocityPerSecond += decelrationForce;
            }
            else // account for decceleration force whilst NOT deccelerating
            {
                velocityPerSecond += decelrationForce * Time.deltaTime;
            }

        #endregion

            // Invoke flow Field Forces onto velocity
            velocityPerSecond += ffD.FlowBoundry();

            // add rotation & accel vectors to move
            transform.position += velocityPerSecond * Time.deltaTime;
        }

    }

    // A function to track the lap of player.
    // checks if they have hit a certain point then increments
    // their lap count when they cross the starting line
    void TrackLap()
    {
        bool lapCountReset = transform.position.x > (this.lapCountReset.transform.position.x - this.lapCountReset.transform.localScale.x) &&
                             transform.position.x < this.lapCountReset.transform.localScale.x + this.lapCountReset.transform.position.x &&
                             transform.position.y > (this.lapCountReset.transform.position.y - this.lapCountReset.transform.localScale.y) &&
                             transform.position.y < this.lapCountReset.transform.localScale.y + this.lapCountReset.transform.position.y;

        bool isTouchingLapLine = transform.position.x > (startFinishLine.transform.position.x - startFinishLine.transform.localScale.x) &&
                                 transform.position.x < startFinishLine.transform.localScale.x + startFinishLine.transform.position.x &&
                                 transform.position.y > (startFinishLine.transform.position.y - startFinishLine.transform.localScale.y) &&
                                 transform.position.y < startFinishLine.transform.localScale.y + startFinishLine.transform.position.y;


        if (lapCountReset)
            readyToCrossFinishLine = true;


        if (isTouchingLapLine && readyToCrossFinishLine)
        {
            readyToCrossFinishLine = false; // bool so it only happens once
            print("touching the FINISH LINE WOOOOOO!");
            lap++;
        }
    }

    Vector3 HandleTurningAndSlide(Vector3 velocityPerSecond)
    {
        // LEFT TURNING =====================================================================================
        #region Left Turning Physics
        if (turningLeft)
        {
            timeRightTurning -= Time.deltaTime * 2.0f; // Reset so right turn does NOT interfere


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

            //play skid sounds
            if(timeLeftTurning > 1.0f)
            audioM.PlayAudio(audioM.skidSource, audioM.skidSound1, 0.35f);

            // drift left
            if (pressedDrift && currentVelocityPerSecond > 0.0f)
            {
                // immediately increase slide vector force, to simulate kickout & slide
                timeLeftTurning += Time.deltaTime * 5; // slideing timer
                timeLeftTurning = Mathf.Clamp(timeLeftTurning, 0.0f, 2.0f); // clamp ity
                slideForceLeft *= (timeLeftTurning * 8) * minSpeedForTurn;
                Debug.DrawLine(transform.position, transform.position + driftVector, Color.red); // draw drift force

                //play skid sound
                audioM.PlayAudio(audioM.driftSource, audioM.skidSound2, 0.25f);

                //display Drift logo
                gm.Drift.SetActive(true);
            }
            else
            {
                gm.Drift.SetActive(false);
            }    


        }
        else if (!turningLeft && !turningRight && timeLeftTurning > 0.0f)
        {
            // Left turn slide - Release
            timeLeftTurning -= Time.deltaTime * 2; // multiply speed of release by 2
            timeLeftTurning = Mathf.Clamp(timeLeftTurning, 0.0f, 1.5f);
            slideForceLeft *= (timeLeftTurning * 5) * minSpeedForTurn;
            velocityPerSecond -= slideForceLeft * Time.deltaTime * driftAmount;
        }
        #endregion

        // RIGHT TURNING ======================================================================================
        #region Right Turning Physics
        if (turningRight)
        {
            timeLeftTurning -= Time.deltaTime * 2.0f;
            timeLeftTurning = Mathf.Clamp(timeLeftTurning, 0.0f, 2.0f);

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
                slideForceRight *= (timeRightTurning * 8) * minSpeedForTurn;

                //play skid sound
                audioM.PlayAudio(audioM.driftSource, audioM.skidSound2, 0.25f);

                //display drift logo
                gm.Drift.SetActive(true);
            }
            else
            {
                gm.Drift.SetActive(false);
            }

        }
        else if (!turningLeft && !turningRight && timeRightTurning > 0.0f)
        {
            // Right slide release
            timeRightTurning -= Time.deltaTime * 2.0f;
            timeRightTurning = Mathf.Clamp(timeRightTurning, 0.0f, 1.5f);
            slideForceRight *= (timeRightTurning * 5) * minSpeedForTurn;
            velocityPerSecond -= slideForceRight * Time.deltaTime * driftAmount;
            Debug.DrawLine(transform.position, transform.position + slideForceRight, Color.magenta);
        }
        #endregion

        return velocityPerSecond;
    } 
}