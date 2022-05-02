using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class goblin_ai : MonoBehaviour
{
    public enum State
    {
        IDLE,
        CHASING,
        FLEEING,
        SEEKING,
        STUNNED,
        DEAD
    };
    
    [Header("Current State")]
    public State state = State.IDLE;

    [Header("Actor variables")]
    Vector3 velocity_per_second = Vector3.zero;
    public float health = 50.0f;
    public bool idling;
    public bool seeking;
    public Transform lava;
    public Transform spikeBall;
    public Transform Player;
    public Transform wanderCircle;

    [Header("Chase Variables")]
    public float speed = 1.0f;
    public float steering_speed = 1.0f;
    
    [Header("Wander Variables")]
    Vector3 wander_direction = Vector3.zero;
    public float wander_turning_clamp = 1.0f;
    public float time_until_random_changes = 0.0f;
    public float time_between_random_change = 3.0f;
    bool beganSearching = true;
    public float timeStandingStill = 3.0f;
    public float timeSeeking = 10.0f;
    bool resetSeekTimer;

    [Header("Stunned Variables")]
    public bool actorCanMove;
    public float stunnedFor;
    public float stunTime;
    public float spikeDamage;

    float boundry_X = 4.5f;
    float boundry_Y = 4.5f;

    void Start()
    {
        actorCanMove = true;
        resetSeekTimer = false;
        idling = true;
        seeking = false;
        OnEnter(state);
    }

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
        print("Goblin Exiting: " + state.ToString() + " to: " + entering.ToString());
    }

    void OnEnter(State exiting)
    {
        print("Goblin Entering: " + state.ToString() + " from: " + exiting.ToString());

        switch (state)
        {
            case State.STUNNED:
                {
                    SpriteRenderer sprite = GetComponent<SpriteRenderer>();
                    sprite.color = Color.yellow;
                    break;
                }
            case State.IDLE:
                {
                    SpriteRenderer sprite = GetComponent<SpriteRenderer>();
                    sprite.color = Color.green;
                    break;
                }
            case State.SEEKING:
                {
                    SpriteRenderer sprite = GetComponent<SpriteRenderer>();
                    sprite.color = Color.green;
                    break;
                }
            case State.CHASING:
                {
                    SpriteRenderer sprite = GetComponent<SpriteRenderer>();
                    sprite.color = Color.magenta;
                    break;
                }
            case State.FLEEING:
                {
                    SpriteRenderer sprite = GetComponent<SpriteRenderer>();
                    sprite.color = Color.grey;
                    break;
                }
            case State.DEAD:
                {
                    SpriteRenderer sprite = GetComponent<SpriteRenderer>();
                    sprite.color = Color.red;
                    break;
                }
        }
    }

    // Plays once per frame
    void OnUpdate()
    {
        // track which state is active in console
        print("Current State: " + state.ToString());

        switch (state)
        {
            case State.IDLE:
                {
                    // handle enviro tasks
                    HandleSpikeBall();
                    HandleInLava();
                    HandleSeekTimer();


                    //watch for player distance
                    Vector3 ActorToPlayer = Player.transform.position - this.transform.position;
                    float distanceToPlayer = ActorToPlayer.magnitude;
                    
                    // Set State BOOLS
                    bool ActorIsDead = health <= 0.0f;
                    bool lowHealth = health <= 10.0f;
                    bool detectedPlayer = distanceToPlayer < 3.0f;
                    bool stoodStillTooLong = timeStandingStill <= 0.0f;
                    idling = true;

                    //handle state transitions
                    if (ActorIsDead)
                    {
                        TransitionTo(State.DEAD);
                    }
                    else if (lowHealth && detectedPlayer)
                    {
                        TransitionTo(State.FLEEING);
                    }
                    else if (detectedPlayer)
                    {
                        TransitionTo(State.CHASING);
                    }
                    else if (stoodStillTooLong)
                    {
                        idling = false;
                        timeStandingStill = Random.Range(4.0f, 8.0f);
                        TransitionTo(State.SEEKING);
                    }

                    break;
                }
            case State.CHASING:
                {
                    // handle enviro tasks
                    HandleInLava();
                    HandleSpikeBall();

                    // find vector of self to player
                    Vector3 actorToPlayer = Player.transform.position - this.transform.position;
                    float distanceToPlayer = actorToPlayer.magnitude;
                    actorToPlayer.Normalize(); //normalize for direction

                    // Set state bools
                    idling = false;
                    seeking = false;
                    bool ActorIsDead = health <= 0.0f;
                    bool isChasing = distanceToPlayer < 3.0f;
                    bool lostPlayer = distanceToPlayer > 3.0f;
                    bool lowHealth = health <= 10.0f;

                    // Handle state actions & transitions
                    if (ActorIsDead)
                    {
                        TransitionTo(State.DEAD);
                    }
                    else if (lowHealth)
                    {
                        TransitionTo(State.FLEEING);
                    }
                    else if (isChasing)
                    {
                        speed = 2.0f;
                        steering_speed = 2.0f;

                        //Set wander circle to player
                        wanderCircle.transform.position = Player.transform.position;

                        // if close, add intercept
                        if (distanceToPlayer < 2.0f)
                        {
                            
                            steering_speed = 3.0f; // getting serious

                            /*Intercept*/
                            {
                                //Get reference to player & track its trajectory(speed & location)/interception time
                                player_ai playerScript = Player.GetComponent<player_ai>();
                                Vector3 speed_difference = playerScript.move_per_frame - velocity_per_second;
                                float interceptTime = distanceToPlayer / speed_difference.magnitude;
                                Vector3 predicted_position = Player.position + playerScript.move_per_frame * interceptTime;

                                // create vector to add to current
                                Vector3 desired_position = predicted_position - transform.position;
                                desired_position.Normalize();
                                desired_position *= speed;

                                // Compute Steering Vector For This Frame (steering vector to add to direction to smoothly turn actor)
                                Vector3 steering_vector_ps = (desired_position - velocity_per_second).normalized * steering_speed;
                                Vector3 steering_vector_pf = steering_vector_ps * Time.deltaTime;

                                // Determine New Velocity Direction with Added Steering (add steering vector to current vector)
                                Vector3 steering_and_current = velocity_per_second + steering_vector_pf;
                                steering_and_current.Normalize(); // get direction for added vector

                                // Determine New Velocity With Speed
                                velocity_per_second = steering_and_current * speed;
                            }
                            transform.position += velocity_per_second * Time.deltaTime;
                        }

                        // Chase!
                        HandleSteering(); 
                    }
                    else if (lostPlayer)
                    {
                        speed = 1.0f;
                        steering_speed = 1.0f;
                        TransitionTo(State.SEEKING);
                    }
                
                    break;
                }
            case State.FLEEING:
                {
                    // handle enviro
                    HandleSpikeBall();
                    HandleInLava();
                    HandleBoundries();

                    // find direction vector to player
                    Vector3 actorToPlayer = Player.transform.position - this.transform.position;
                    float distanceToPlayer = actorToPlayer.magnitude;
                    actorToPlayer.Normalize();

                    // Set current state bools
                    bool ActorIsDead = health <= 0.0f;
                    bool isFleeing = distanceToPlayer <= 4.0f;
                    bool lostPlayer = distanceToPlayer >= 4.0f;

                    // Handle state actions & transitions
                    if (ActorIsDead)
                    {
                        TransitionTo(State.DEAD);
                    }
                    else if (isFleeing)
                    {
                        bool isOutOfBounds = (this.transform.position.x >= boundry_X ||
                                              this.transform.position.x <= -boundry_X ||
                                              this.transform.position.y >= boundry_Y ||
                                              this.transform.position.y <= -boundry_Y);

                        if (isOutOfBounds)
                        {
                            steering_speed = 20.0f;
                            //come back in border
                            if (wanderCircle.position.x > boundry_X) wanderCircle.position = new Vector3(boundry_X - 1, wanderCircle.position.y, wanderCircle.position.z); // RIGHT WALL
                            if (wanderCircle.position.x < -boundry_X) wanderCircle.position = new Vector3(-boundry_X + 1, wanderCircle.position.y, wanderCircle.position.z); // LEFT WALL
                            if (wanderCircle.position.y > boundry_Y) wanderCircle.position = new Vector3(wanderCircle.position.x, boundry_Y - 1, wanderCircle.position.z); // TOP WALL
                            if (wanderCircle.position.y < -boundry_Y) wanderCircle.position = new Vector3(wanderCircle.position.x, -boundry_Y + 1, wanderCircle.position.z); // BOTTOM WALL

                        }
                        else //run away from player
                        { 
                            wanderCircle.transform.position =  this.transform.position - actorToPlayer;
                        }

                        // turn quickly
                        steering_speed = 10.0f;
                        speed += 2.0f * Time.deltaTime;

                        // move fluently
                        HandleSteering();
                    }
                    else if (lostPlayer)
                    {
                        steering_speed = 1.0f;
                        speed = 1.0f;
                        TransitionTo(State.SEEKING);
                    }
                    break;
                }
            case State.SEEKING:
                {
                    //handle enviro tasks
                    HandleSeekTimer();
                    HandleSpikeBall();
                    HandleInLava();
                    HandleBoundries();

                    // find vector of self to player
                    Vector3 actorToPlayer = Player.transform.position - this.transform.position;
                    float distanceToPlayer = actorToPlayer.magnitude;

                    // set current state boools
                    seeking = true;
                    bool foundPlayer = distanceToPlayer < 3.0f; 
                    bool ActorIsDead = health <= 0.0f;
                    bool seekedTooLong = timeSeeking <= 0.0f;
                    
                    // Varibales to help avoid Lava
                    Vector3 thisToLava = lava.transform.position - wanderCircle.transform.position;
                    float distanceBetween = thisToLava.magnitude;
                    float touchingLava = (lava.transform.localScale.x / 2) + (wanderCircle.transform.localScale.x / 2) - distanceBetween;

                    // Handle state actions & transitions
                    if (ActorIsDead)
                    {
                        TransitionTo(State.DEAD);
                    }
                    else if (beganSearching)
                    {
                        // create random vector direction
                        float x_Dir = Random.Range(-1.0f, 1.0f);
                        float y_Dir = Random.Range(-1.0f, 1.0f);
                        Vector3 randomWanderDirection = new Vector3(x_Dir, y_Dir, 0.0f);
                        randomWanderDirection.Normalize();

                        //reposition wander circle in front of wanderer to new desired direction
                        wanderCircle.transform.position = transform.position + randomWanderDirection * 2.5f;

                        beganSearching = false;
                    }
                    else if (touchingLava > -0.2f)
                    {
                        // determine change in wander circle x location, dependent on side of lava circle is
                        float x_change = 0.0f;
                        if (this.transform.position.x < 0) x_change = -1;
                        if (this.transform.position.x > 0) x_change = 1; 

                        // move Wander Circle away from lava
                        wanderCircle.position = new Vector3(wanderCircle.position.x + x_change, wanderCircle.position.y);
                        HandleSteering();
                    }
                    else if (foundPlayer)
                    {
                        beganSearching = true; // reset for next cycle
                        TransitionTo(State.CHASING);
                    }
                    else if (seekedTooLong)
                    {
                        seeking = false;
                        timeSeeking = Random.Range(15.0f, 36.0f); 
                        TransitionTo(State.IDLE);
                    }
                    else // continue SEEKING
                    {
                        time_until_random_changes -= Time.deltaTime;
                        bool readyToChangeDirection = time_until_random_changes <= 0.0f;
                        if (readyToChangeDirection)
                        {
                            //Get some random values for slight direction change, using clamp
                            float x_change = Random.Range(wander_direction.x - wander_turning_clamp, wander_direction.x + wander_turning_clamp);
                            float y_change = Random.Range(wander_direction.y - wander_turning_clamp, wander_direction.y + wander_turning_clamp);
                            Vector3 direction_adjustment = new Vector3(wander_direction.x + x_change, wander_direction.y + y_change); // add values to current direction vector into new vector
                            direction_adjustment.Normalize(); // normalize for direction

                            // set wander circle at a new location based on its current (plus or minus a few degrees), to give natural wander effect
                            wanderCircle.transform.position = transform.position + direction_adjustment * 1.2f; // multiply to position circle ahead of wanderer

                            time_between_random_change = Random.Range(1, 4);
                            time_until_random_changes = time_between_random_change;

                        }

                        // Steer!
                        HandleSteering();
                    }
                    break;
                }
            case State.STUNNED:
                {
                    bool isStunned = stunnedFor > 0.0f;

                    if (isStunned)
                    {
                        actorCanMove = false;
                        health -= spikeDamage * Time.deltaTime; // deduct health

                        // find vector from thing to spike ball & adjust things position after contact
                        Vector3 thisToSpikeBall = spikeBall.transform.position - transform.position;
                        float distanceBetween = thisToSpikeBall.magnitude;
                        float intersection_depth = (this.transform.localScale.x / 2) + (spikeBall.transform.localScale.x / 2) - (distanceBetween - 0.2f);
                        this.transform.position -= thisToSpikeBall.normalized * intersection_depth; // move player so hit trigger is NOT always triggered
                    }
                    else
                    {
                        TransitionTo(State.IDLE);
                        actorCanMove = true;
                    }
                    break;
                }
        }
    }

    // Function to drive objects fluidly in a the direction of their Wander Circles
    void HandleSteering()
    {
        // Get desired wander direction from NEW position of wander circle
        wander_direction = wanderCircle.position - transform.position; // get direction to go
        wander_direction.Normalize(); // normalize it
        wander_direction *= speed; // add speed

        // Compute Steering Vector For This Frame (steering vector to add to direction to smoothly turn actor)
        Vector3 steering_vector = (wander_direction - velocity_per_second).normalized * steering_speed;
        Vector3 steering_vector_pf = steering_vector * Time.deltaTime;

        // Determine New Velocity Direction with Added Steering (add steering vector to current vector)
        Vector3 steering_and_current = velocity_per_second + steering_vector_pf;
        steering_and_current.Normalize(); // get direction for added vector

        // Determine New Velocity With Speed
        velocity_per_second = steering_and_current * speed;

        transform.position += velocity_per_second * Time.deltaTime;
    }

    // Function to handle the boundries of playable area and what entities do when they cross them
    void HandleBoundries()
    {

        // check if wander cirlce is out of bounds                       // bring it back within border
        if (wanderCircle.position.x > boundry_X) wanderCircle.position = new Vector3(boundry_X - 1, wanderCircle.position.y, wanderCircle.position.z); // RIGHT WALL
        if (wanderCircle.position.x < -boundry_X) wanderCircle.position = new Vector3(-boundry_X + 1, wanderCircle.position.y, wanderCircle.position.z); // LEFT WALL
        if (wanderCircle.position.y > boundry_Y) wanderCircle.position = new Vector3(wanderCircle.position.x, boundry_Y - 1, wanderCircle.position.z); // TOP WALL
        if (wanderCircle.position.y < -boundry_Y) wanderCircle.position = new Vector3(wanderCircle.position.x, -boundry_Y + 1, wanderCircle.position.z); // BOTTOM WALL

        bool isOutOfBounds = this.transform.position.x >= boundry_X ||
                             this.transform.position.x <= -boundry_X ||
                             this.transform.position.y >= boundry_Y ||
                             this.transform.position.y <= -boundry_Y;

        //Adjust steering speed when goblin is off screen
        if (isOutOfBounds)
        {
            steering_speed = 5.0f;

            // to ensure they don't stop off screen
            timeSeeking = 10.0f; 
            timeStandingStill = 0.0f;
        }
        else
        {
            steering_speed = 1.0f;
        }
    }

    // Function which handles when timers will switch & reset dependent on state enemies are in
    void HandleSeekTimer()
    {
        if (idling)
        {
            if (resetSeekTimer)
            {
                resetSeekTimer = false;
            }

            if (timeStandingStill >= 0.0f)
            {
                timeStandingStill -= Time.deltaTime;
            }
            else if (timeStandingStill < 0.0f)
            {
                resetSeekTimer = true;
            }
        }
        else if (seeking)
        {
            if (resetSeekTimer)
            {
                resetSeekTimer = false;
            }

            if (timeSeeking >= 0.0f)
            {
                timeSeeking -= Time.deltaTime;
            }
            else if (timeSeeking <= 0.0f)
            {
                resetSeekTimer = true;
            }
        }

    }

    // Function to handle what happens when objects collide with lava object
    void HandleInLava()
    {
        Vector3 thisToLava = lava.transform.position - transform.position;
        bool isInLava = thisToLava.magnitude < (this.transform.localScale.x / 2) + (lava.transform.localScale.x / 2);
        if (isInLava)
        {
            print("MY GOBLIN SKIN IT BURNINS!");
            health -= 10.0f * Time.deltaTime;
        }
    }

    // Function that manages distance between objects and spike ball & stunned state transitions
    void HandleSpikeBall()
    {
        Vector3 thisToSpikeBall = spikeBall.transform.position - transform.position;
        float distanceBetween = thisToSpikeBall.magnitude;
        bool hasHitSpikeBall = distanceBetween < (this.transform.localScale.x / 2) + (spikeBall.transform.localScale.x / 2);
        if (hasHitSpikeBall)
        {
            print("GOBLIN OUCH");
            TransitionTo(State.STUNNED);

            stunnedFor = stunTime; // set time to wait
        }
    }

    void Update()
    {
        OnUpdate();

        stunnedFor -= Time.deltaTime; // begin countdown of time
        if (stunnedFor < 0.0f) stunnedFor = 0.0f;
    }
}
