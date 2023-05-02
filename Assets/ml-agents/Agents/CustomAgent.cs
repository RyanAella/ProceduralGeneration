using InGameTime;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public enum AgentType { RABBIT,FOX}

public abstract class CustomAgent : Agent
{
    public AgentType type;
    [Space(5)]
    [Header("Status")]
    public bool canMove = true;
    public bool isAdult = false;

    [Space(3)]
    public bool isInDen = false;

    [Space(3)]
    [Range(0, 100)] public int health = 100;
    [Range(0, 100)] public int stamina = 100;
    [Range(0, 100)] public int thirst = 0;
    [Range(0, 100)] public int hunger = 0;

    [Space(5)]
    public bool isEating = false;
    public bool isDrinking = false;

    [Space(10)]
    [Header("Properties")]
    public bool hasBreeded = false;
    public bool hasDen = false;
    public bool canBreedWithCurrentAge = false;
    public bool canBuildDenWithCurrentAge = false;

    [Space(10)]
    [Header("Locations")]
    public Den homeLocation;
    public Den nearestDenLocation;


    [Space(10)]
    [Header("Rewards")]
    public float eatReward;


    [Space(10)]
    [Header("Penalties")]
    public float deathPenalty = 5;
    [Range(10, 60)] public int lowHealthPenaltyDivider = 30;

    [Space(10)]
    [Header("Sound")]



    [Space(10)]
    [Header("External Data")]
    public CharacterController controller;
    public Movement movement;
    public ProtectionHandler protectionHandler;
    public InteractionHandler interaction;
    public DrowningHandler drowningHandler;
    public SoundDetectionHandler soundDetectionHandler;

    float blockMovementSeconds = 0;
    bool blockTimerActive = false;

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(canMove);

        sensor.AddObservation(health);
        sensor.AddObservation(stamina);

        sensor.AddObservation(hunger);
        sensor.AddObservation(interaction.canEat);
        sensor.AddObservation(isEating);

        sensor.AddObservation(thirst);
        sensor.AddObservation(interaction.canDrink);
        sensor.AddObservation(isDrinking);
        sensor.AddObservation(drowningHandler.isDrowning);

        sensor.AddObservation(interaction.canBreed);
        sensor.AddObservation(hasBreeded);

        sensor.AddObservation(hasDen);
        sensor.AddObservation(interaction.canBuildDen);
        sensor.AddObservation(interaction.isDenBuildableHere);

        sensor.AddObservation(protectionHandler.protectionLevel);

        //sound
        sensor.AddObservation(soundDetectionHandler.lastSoundDistance);
        sensor.AddObservation(soundDetectionHandler.lastSoundDirection);

        //velocity of agent in any direction
        sensor.AddObservation(new Vector3(controller.velocity.x, controller.velocity.y, controller.velocity.z));

        ResetTemporaryObservations();
    }

    private void ResetTemporaryObservations()
    {
        soundDetectionHandler.lastSoundDistance = -1;
        soundDetectionHandler.lastSoundDirection = new Vector3(0,0,0);
    }

    // Update is called once per frame
    void Update()
    {
        if(blockTimerActive)
        {
            HandleBlockTimer();
        }
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        if(canMove)
        {
            Interact(
                actionBuffers.DiscreteActions[0],
                actionBuffers.DiscreteActions[0],
                actionBuffers.DiscreteActions[0],
                actionBuffers.DiscreteActions[0],
                actionBuffers.DiscreteActions[0]);

            Move(
                Mathf.Clamp(actionBuffers.ContinuousActions[0], 0, 1),
                Mathf.Clamp(actionBuffers.ContinuousActions[1], -1, 1),
                Mathf.Clamp(actionBuffers.ContinuousActions[2], -1, 1),
                Mathf.Clamp(actionBuffers.ContinuousActions[3], -1, 1));
        }
    }

    private void Interact(int v1, int v2, int v3, int v4, int v5)
    {
        Debug.Log("I----------------");
        Debug.Log(v1);
        Debug.Log(v2);
        Debug.Log(v3);
        Debug.Log(v4);
        Debug.Log(v5);
        bool wantToEat = v1 != 0;
        bool wantToDrink = v2 != 0;
        bool wantToEnterDen = v3 != 0;
        bool wantToBuildDen = v4 != 0;
        bool wantToBreed = v5 != 0;

        //check drinking
        if(wantToDrink && interaction.canDrink)
        {
            Drink();
        }

        //check eating
        if (wantToEat& interaction.canEat)
        {
            Eat();
        }

        //check EnterDen
        //check BuildDen
        //check Breed
    }

    private void Move(float walkspeed, float rotation, float headRotationX, float headRotationY)
    {
        movement.walkSpeed = walkspeed;
        movement.rotation = rotation;
        movement.headRotationX = headRotationX;
        movement.headRotationY = headRotationY;
    }

    public override void OnEpisodeBegin()
    {
        //setRandomPosition();
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActions = actionsOut.DiscreteActions; 
        

        var continuousActionsOut = actionsOut.ContinuousActions;
        //continuousActionsOut[0] = Input.GetAxis("Vertical");
        //continuousActionsOut[1] = Input.GetAxis("Horizontal");
    }

    //later replaced because food is setting the time for BlockMovementForSeconds
    public void Eat()
    {
        isEating = true;
        BlockMovementForSeconds(4);

        if (hunger > 0)
        {
            thirst -= 5;
        }
    }

    public void Drink()
    {
        isDrinking = true;
        BlockMovementForSeconds(0.5f);

        if (thirst > 0)
        {
            thirst--;
        }
    }

    public void Kill()
    {
        AddReward(deathPenalty);
        EndEpisode();
        Destroy(this);
    }

    public void Death()
    {
        AddReward(deathPenalty);
    }

    public void BlockMovementForSeconds(float sec)
    {
        canMove = false;
        blockMovementSeconds = sec;
        blockTimerActive = true;
    }

    private void HandleBlockTimer()
    {
        blockMovementSeconds -= Time.deltaTime;
        if (blockMovementSeconds <= 0.0f)
        {
            blockTimerActive = false;

            isEating = false;
            isDrinking = false;
            
            canMove = true;
        }
    }
}
