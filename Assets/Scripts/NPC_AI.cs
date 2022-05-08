using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_AI : MonoBehaviour
{
    public enum State
    {
        IDLE,
        TRACKING,
    };
    
    [Header("NPC VARIABLES")]
    public State state = State.IDLE;
    public float speed = 1.0f;
    public float steering_speed = 1.0f;
    Vector3 velocityPerSecond = Vector3.zero;


    [Header("DESTINATION VARIABLES")]
    public Transform Destination;
    public Transform directionObject;
    public List<Transform> destinations = new List<Transform>();
    public float currentDirectionDeg = 90.0f;
    Vector3 desiredDirection = Vector3.zero;

    [Header("TRACKING VARIABLES")]
    public bool onToNextLocation = false;
    public bool nearDestination = false;
    public bool gate = false;
    public int trackingIndex = 0;
    public float distanceToDestination = 0.0f;

    [Header("RACE VARIABLES")]
    public GameObject startFinishLine;
    public GameObject lapCountReset;
    bool readyToCrossFinishLine;
    public int lap = 0;

    [Header("SCRIPT REFERENCES")]
    public FlowFieldDetector ffD;
    public GameManager gmScript;
    public audioManager audioManager;

    void TransitionTo(State transitionTo)
    {
        //On Exit event
        OnExit(transitionTo);

        State currentState = state;
        state = transitionTo;

        // On Enter event
        OnEnter(currentState);
    }

    void OnExit(State entering)
    {
        //ON EXIT EVENT
    }

    void OnEnter(State exiting)
    {
        //ON ENTER EVENT
    }


    // Plays once per frame
    void OnUpdate()
    {
        // ============================================================================ ROTATION
  
        Vector3 newDirection = (Destination.position - transform.position);
        Debug.DrawRay(transform.position, newDirection, Color.red);

        transform.up = newDirection; // not smooth, but at least they look their destination!

        Vector3 ActorToDestination = Destination.transform.position - this.transform.position;
        distanceToDestination = ActorToDestination.magnitude;

        // ===================================================================================== AI PATH FOLLOWING
        nearDestination = distanceToDestination < 5.0f;
        bool repeatTrack = trackingIndex < 1;

        // double bool gate for update location
        if (nearDestination)
        {
           gate = true;
        }

        // UPDATE NEXT POSITION TO TRACK
        if(gate)
        {
            // Generate random choice to determine path taken
            int choice = Random.Range(0, 2); // 1 or 0

            if (distanceToDestination > 0.25f) // If we travel pass the destination
            {
                // if beginning of race is found, reset
                if(repeatTrack)
                    trackingIndex = destinations.Count - 1;

                if (choice == 1)
                {
                    if (trackingIndex > 0)
                        trackingIndex--;
                }
                else
                {
                    if (trackingIndex > 1)
                        trackingIndex -= 2;
                }

                gate = false;
            }
        }

        //trackingTimer += Time.deltaTime;
        Destination = destinations[trackingIndex]; // constantly update destination to top position i nlist

        switch (state)
        {
            case State.IDLE:
                {
                    //watch for player distance
                    ActorToDestination = Destination.transform.position - this.transform.position;
                    distanceToDestination = ActorToDestination.magnitude;
                    TransitionTo(State.TRACKING);
                    break;
                }
            case State.TRACKING:
                {
                    // find vector of self to destination
                    ActorToDestination = Destination.transform.position - this.transform.position;
                    distanceToDestination = ActorToDestination.magnitude;
                    ActorToDestination.Normalize(); //normalize for direction
                 
                    //Set desired direction object.
                    directionObject.transform.position = Destination.transform.position;

                    // check Game Manager
                    if(gmScript.isGameRunning)
                    {
                        // Chase!
                        HandleSteering();

                        velocityPerSecond += ffD.FlowBoundry();
                    }
                    break;
                }
        } 
    }

    // Function to drive objects fluidly in a the direction of their Wander Circles
    void HandleSteering()
    {
        // Get desired wander direction from NEW position of wander circle
        desiredDirection = directionObject.position - transform.position; // get direction to go
        desiredDirection.Normalize(); // normalize it
        desiredDirection *= speed; // add speed

        // Compute Steering Vector For This Frame (steering vector to add to direction to smoothly turn actor)
        Vector3 steering_vector = (desiredDirection - velocityPerSecond).normalized * steering_speed;
        Vector3 steering_vector_pf = steering_vector * Time.deltaTime;

        // Determine New Velocity Direction with Added Steering (add steering vector to current vector)
        Vector3 steering_and_current = velocityPerSecond + steering_vector_pf;
        steering_and_current.Normalize(); // get direction for added vector

        // Determine New Velocity With Speed
        velocityPerSecond = steering_and_current * speed;

        transform.position += velocityPerSecond * Time.deltaTime;
    }

    // A function to track the lap of each agent. 
    // checks if agents have hit a certain point then increments their lap count
    // when they cross the starting line
    void TrackLap()
    {
        // if actor is touching the object
        bool lapCountReset = transform.position.x > (this.lapCountReset.transform.position.x - this.lapCountReset.transform.localScale.x) &&
                             transform.position.x < this.lapCountReset.transform.localScale.x + this.lapCountReset.transform.position.x &&
                             transform.position.y > (this.lapCountReset.transform.position.y - this.lapCountReset.transform.localScale.y) &&
                             transform.position.y < this.lapCountReset.transform.localScale.y + this.lapCountReset.transform.position.y;

        bool isTouchingLapLine = transform.position.x > (startFinishLine.transform.position.x - startFinishLine.transform.localScale.x) &&
                                 transform.position.x < startFinishLine.transform.localScale.x + startFinishLine.transform.position.x &&
                                 transform.position.y > (startFinishLine.transform.position.y - startFinishLine.transform.localScale.y) &&
                                 transform.position.y < startFinishLine.transform.localScale.y + startFinishLine.transform.position.y;


        // if actor has reached certain part of map
        if (lapCountReset)
            readyToCrossFinishLine = true;

        // lap count!
        if (isTouchingLapLine && readyToCrossFinishLine)
        {
            readyToCrossFinishLine = false; // bool so it only happens once
            print("touching the FINISH LINE WOOOOOO!");
            lap++;
        }
    }

    void Update()
    {
        OnUpdate();

        TrackLap();
    }

    private void Start()
    {
        readyToCrossFinishLine = false;

        // Set up Destinations to follow
        GameObject[] D = GameObject.FindGameObjectsWithTag("Destination");
        foreach(GameObject ds in D)
        {
            destinations.Add(ds.transform);
        }

        trackingIndex = destinations.Count - 1; // set index amount
        Destination = destinations[trackingIndex]; // set first destination
    }
}
