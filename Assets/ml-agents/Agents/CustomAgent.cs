using InGameTime;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using UnityEngine;

public enum AgentType { RABBIT,FOX}

public abstract class CustomAgent : Agent
{
    public AgentType type;
    [Space(5)]
    [Header("Status")]
    public bool canMove = true;
    public int ageInMonths = 0;
    public bool isAdult = false;
    [Range(0, 100)] public int health = 100;
    [Range(0, 100)] public int stamina = 100;
    [Range(0, 100)] public int thirst = 0;
    [Range(0, 100)] public int hunger = 0;

    [Space(5)]
    public bool isEating = false;
    public bool isDrinking = false;
    [Space(5)]
    public bool canEat = false;
    public bool canDrink = false;

    [Space(10)]
    [Header("Properties")]
    public bool canBreed = false;
    public bool hasBreeded = false;


    public bool hasDen;
    public bool canBuildDen;
    public bool isDenBuildableHere;
    [Space(5)]
    [Range(0, 2)] public int protectionLevel;


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
    [Header("Settings")]
    public int ageWhenAdult = 4;

    [Space(10)]
    [Header("Sound")]
    public float lastSoundDistance = -1;
    public Vector3 lastSoundDirection = new Vector3(0, 0, 0);
    public bool soundOnlyFromEnemy = false;


    [Space(10)]
    [Header("External Data")]
    public CharacterController controller;
    public Movement movement;

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(health);
        sensor.AddObservation(stamina);
        sensor.AddObservation(canEat);
        sensor.AddObservation(canDrink);
        sensor.AddObservation(thirst);
        sensor.AddObservation(hunger);
        sensor.AddObservation(canMove);
        sensor.AddObservation(canBreed);
        sensor.AddObservation(hasBreeded);
        sensor.AddObservation(hasDen);
        sensor.AddObservation(canBuildDen);
        sensor.AddObservation(isDenBuildableHere);
        sensor.AddObservation(protectionLevel);

        //velocity of agent in any direction
        sensor.AddObservation(new Vector3(controller.velocity.x, controller.velocity.y, controller.velocity.z));

        resetTemporaryObservations();
    }

    private void resetTemporaryObservations()
    {
        lastSoundDistance = -1;
        lastSoundDirection = new Vector3(0,0,0);
    }

    // Start is called before the first frame update
    void Start()
    {
        //weeklyEvent.AddListener(WeekOver);
        //monthlyEvent.AddListener(MonthOver);
        TimeManager.OnWeekChanged += WeekOver;
        TimeManager.OnMonthChanged += MonthOver;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(new Vector3(controller.velocity.x, controller.velocity.y, controller.velocity.z));
    }

    void WeekOver()
    {
        health -= 2;
        CheckHealthCondition();
    }

    void MonthOver()
    {
        ageInMonths += 1;

        if (!isAdult)
        {
            CheckAgeRestrictions();
        }
    }

    void CheckHealthCondition()
    {
        if (health < 50)
        {
            if (health <= 0)
            {
                Death();
            }
            else
            {
                AddReward(-(100 - health) / lowHealthPenaltyDivider);
            }

        }
    }

    void CheckAgeRestrictions()
    {
        if (ageInMonths >= ageWhenAdult)
        {
            isAdult = true;
            canBreed = true;
            canBuildDen = true;
        }
    }

    void Kill()
    {

    }

    void Death()
    {
        AddReward(deathPenalty);
    }

    public abstract void Eat();

    public abstract void Breed();

    public void NotificationOnMovement(CustomAgent loudAgent)
    {
        if (this != loudAgent)
        {
            if (!soundOnlyFromEnemy)
            {
                lastSoundDistance = Vector3.Distance(transform.position, loudAgent.transform.position);
                lastSoundDirection = loudAgent.transform.position - transform.position;
                Debug.Log("Sound!");
            } else if (type != loudAgent.type)
            {
                lastSoundDistance = Vector3.Distance(transform.position, loudAgent.transform.position);
                lastSoundDirection = loudAgent.transform.position - transform.position;
                Debug.Log("Enemy sound!");
            }
            
        } 
    }
}
