using _Scripts.ml_agents.Agents.Handler;
using _Scripts.WorldGeneration.Helper;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;
using Random = System.Random;

public enum AgentType
{
    Rabbit,
    Fox
}

namespace _Scripts.ml_agents.Agents
{
    public abstract class CustomAgent : Agent
    {
        public int id;
        public AgentType type;

        [Space(5)] 
        [Header("Status")] 
        public bool canInteract = true;
        public bool isAdult;
        public bool hearsEnemy;
        public bool dead;

        [Space(3)] 
        public bool isInBurrow;

        [Space(3)] 
        [Range(0, 100)] public float health = 100;
        [Range(0, 100)] public float stamina = 100;
        [Range(-30, 100)] public int thirst;
        [Range(-30, 100)] public int hunger;

        [Space(5)] public int thresholdHungerThirstForRecovery = 50;

        [Space(10)] 
        [Header("Properties")] 
        public float healthRecoveryAmount = 3;
        public float staminaRecoveryAmount = 5;
        public int nutritionValue = 8;
        public bool hasBred;
        public bool hasBurrowBuild;

        [Space(10)] 
        [Header("Locations")] 
        public Vector3 lastBurrow = new(0, 0, 0);

        [Space(10)] 
        [Header("Penalties")] 
        public float deathPenalty = 5;

        [Space(10)] 
        [Header("External Data")] 
        public CharacterController controller;
        public Movement movement;
        public ProtectionHandler protectionHandler;
        public InteractionHandler interaction;
        public DrowningHandler drowningHandler;
        public SoundDetectionHandler soundDetectionHandler;

        private float _blockMovementSeconds;
        private bool _blockTimerActive;

        private readonly Random _random = new();

        /**
         * preparing agent
         * setting unique id for agent type
         * adding random hunger & thirst so first month also needs interactions
         */
        protected override void Awake()
        {
            base.Awake();
            hunger = _random.Next(15, 81);
            thirst = _random.Next(15, 81);

            if (type == AgentType.Rabbit)
            {
                id = Checker.RabbitID;
                Checker.RabbitID++;

                Checker.RabbitCounter++;
            }
            else if (type == AgentType.Fox)
            {
                id = Checker.FoxID;
                Checker.FoxID++;

                Checker.FoxCounter++;
            }
        }

        /*
         * EndEpisode needed if gameObject will be destroyed
         * OnDestroy throws errors on ending game via Unity Editor
         * ml-Agents not optimized for creating / deleting agents dynamically
         */
        private void OnDestroy()
        {
            EndEpisode();
        }

        public override void CollectObservations(VectorSensor sensor)
        {
            sensor.AddObservation(canInteract);

            sensor.AddObservation(health);
            sensor.AddObservation(stamina);

            sensor.AddObservation(hunger);
            sensor.AddObservation(interaction.canEat);

            sensor.AddObservation(thirst);
            sensor.AddObservation(interaction.canDrink);
            sensor.AddObservation(drowningHandler.isDrowning);

            sensor.AddObservation(interaction.CanBreed());

            sensor.AddObservation(interaction.canBuildBurrow);
            sensor.AddObservation(interaction.isBurrowBuildableHere);

            sensor.AddObservation(protectionHandler.someoneInGroupHearsEnemy);

            sensor.AddObservation(soundDetectionHandler.lastEnemySoundDistance);
            sensor.AddObservation(soundDetectionHandler.lastEnemySoundDirection);

            //velocity of agent in any direction
            sensor.AddObservation(new Vector3(controller.velocity.x, controller.velocity.y, controller.velocity.z));
            sensor.AddObservation(movement.head.transform.rotation);
        }

        public override void OnActionReceived(ActionBuffers actionBuffers)
        {
            if (!dead)
            {
                if (_blockTimerActive) HandleBlockTimer();

                Recover();
                Move(
                    Mathf.Clamp(actionBuffers.ContinuousActions[0], 0, 1),
                    Mathf.Clamp(actionBuffers.ContinuousActions[1], -1, 1),
                    Mathf.Clamp(actionBuffers.ContinuousActions[2], -1, 1),
                    Mathf.Clamp(actionBuffers.ContinuousActions[3], -1, 1)
                    );

                if (canInteract)
                {
                    Interact(
                        actionBuffers.DiscreteActions[0],
                        actionBuffers.DiscreteActions[1],
                        actionBuffers.DiscreteActions[2],
                        actionBuffers.DiscreteActions[3],
                        actionBuffers.DiscreteActions[4],
                        actionBuffers.DiscreteActions[5]);
                }

                AddPenalties();
                CheckIsDead();
            }
        }

        /**
         * adding penalties based on
         * health state below 80
         * stamina below 50
         * hunger or thirst over 50
         * overeating / overdrinking == hunger / thirst below -10
         */
        private void AddPenalties()
        {
            if (health <= 80) 
            {
                AddReward(-((100 - health) / 80) * Time.deltaTime);
            }

            if (stamina <= 50) 
            { 
                AddReward(-((100 - stamina) / 100) * Time.deltaTime); 
            }

            if (hunger >= 50 || thirst >= 50)
            {
                if (hunger >= thirst)
                    AddReward(-(hunger / 75) * Time.deltaTime);
                else
                    AddReward(-(thirst / 75) * Time.deltaTime);
            }

            if (hunger <= -10) AddReward(hunger / 20 * Time.deltaTime);

            if (thirst <= -10) AddReward(thirst / 20 * Time.deltaTime);
        }

        public override void Heuristic(in ActionBuffers actionsOut)
        {
            //var discreteActions = actionsOut.DiscreteActions;
            //var continuousActionsOut = actionsOut.ContinuousActions;
            //continuousActionsOut[0] = Input.GetAxis("Vertical");
            //continuousActionsOut[1] = Input.GetAxis("Horizontal");
        }

        /**
         * interactions from NN (0 or 1) mapped to bool
         * calling method of interaction, based on bool
         */
        public void Interact(int v0, int v1, int v2, int v3, int v4, int v5)
        {
            var wantToEat = v0 != 0;
            var wantToDrink = v1 != 0;
            var wantToEnterBurrow = v2 != 0;
            var wantToLeaveBurrow = v3 != 0;
            var wantToBuildBurrow = v4 != 0;
            var wantToBreed = v5 != 0;

            if (canInteract)
            {
                if (wantToDrink)
                    interaction.Drink();

                if (wantToEat)
                    interaction.Eat();

                if (wantToBuildBurrow)
                    interaction.BuildBurrow();

                if (wantToBreed)
                    interaction.Breed();

                if (wantToEnterBurrow)
                    interaction.EnterBurrow();

                if (wantToLeaveBurrow)
                    interaction.LeaveBurrow();
            }
        }

        /**
         * Moving body & head based on NN floats
         */
        private void Move(float walkSpeed, float rotation, float headRotationX, float headRotationY)
        {
            movement.walkSpeed = walkSpeed;
            movement.rotation = rotation;
            movement.headRotationX = headRotationX;
            movement.headRotationY = headRotationY;
        }

        /**
         * recovering stamina  & thirst if agent ist not moving much
         * health condition: stamina over 50
         * stamina condition: not moving much
         */
        private void Recover()
        {
            if (!IsHungryOrThirsty())
            {
                if (stamina >= 50)
                {
                    if (health + healthRecoveryAmount * Time.deltaTime > 100)
                        health = 100;
                    else
                        health += healthRecoveryAmount * Time.deltaTime;
                }

                if (IsCloseToNotMoving())
                {
                    if (stamina + staminaRecoveryAmount * Time.deltaTime > 100)
                        stamina = 100;
                    else
                        stamina += staminaRecoveryAmount * Time.deltaTime;
                }
            }
        }

        private bool IsHungryOrThirsty()
        {
            return hunger >= thresholdHungerThirstForRecovery || thirst >= thresholdHungerThirstForRecovery;
        }

        /**
         * called when this agent got attacked
         * @param attackingAgent agent that attacked this agent
         * removing health this agent
         * reducing hunger attackingAgent
         */
        public void GotAttacked(CustomAgent attackingAgent)
        {
            RemoveHealth(nutritionValue);

            attackingAgent.BlockInteractionsForSeconds(2);

            if ((attackingAgent.hunger - nutritionValue) >= -30)
            {
                attackingAgent.hunger -= nutritionValue;
            }else
            {
                attackingAgent.hunger = -30;
            }  
        }

        /**
         * check if the agents movement is low, so it can recover health & stamina
         */
        private bool IsCloseToNotMoving()
        {
            return movement.walkSpeed <= 0.13f && Mathf.Abs(movement.rotation) <= 0.15f;
        }

        /**
         * starting a timer (updated via Update())
         * for blocking agent interactions for seconds
         * @param sec seconds for blocking interactions
         */
        public void BlockInteractionsForSeconds(float sec)
        {
            canInteract = false;
            movement.walkSpeed = 0;
            movement.headRotationX = 0;
            movement.headRotationY = 0;
            _blockMovementSeconds = sec;
            _blockTimerActive = true;
        }

        /**
         * checking vor seconds of BlockInteractionsForSeconds() are over
         * enabling interactions again
         * disabling timer if seconds are over
         */
        private void HandleBlockTimer()
        {
            _blockMovementSeconds -= Time.deltaTime;
            if (_blockMovementSeconds <= 0.0f)
            {
                _blockTimerActive = false;
                canInteract = true;
            }
        }

        /**
         * public methode for overall removing stamina
         * if stamina reaches 0, health will be used instead
         */
        public void ReduceStamina(float reduction)
        {
            //if stamina < 0 reduction will take place in health
            if (stamina - reduction < 0.0f)
            {
                reduction -= stamina;
                stamina = 0;
                RemoveHealth(reduction);
            }
            else
            {
                stamina -= reduction;
            }

            
        }

       /**
        * public methode for overall removing health
        * if stamina reaches 0, health will be used instead
        */
        public void RemoveHealth(float reduction)
        {
            if (health - reduction < 0.0f)
            {
                reduction -= health;
                health = 0;
            }
            else
            {
                health -= reduction;
            }
        }

        private void CheckIsDead()
        {
            if (health <= 0 || hunger >= 100 || thirst >= 100) {
                Kill();
            } 
        }

       /**
        * killing agent, includes 
        * ending episode of agent
        * destroying the gameObject
        * 
        * reducing counter of existing agents if thi type
        */
        private void Kill()
        {
            if (dead) return;
            dead = true;
            canInteract = false;

            Debug.Log("<color=" + interaction.debugColor + ">" + type + "</color>" + " - " +
                      "<color=#fe019a> got killed </color> [" + id + "]");

            AddReward(deathPenalty);

            if (type == AgentType.Rabbit)
                Checker.RabbitCounter--;
            else if (type == AgentType.Fox) Checker.FoxCounter--;
            Destroy(gameObject);
        }
    }
}