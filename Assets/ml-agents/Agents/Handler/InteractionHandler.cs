using ml_agents.Custom_Attributes_for_editor;
using UnityEditor.VersionControl;
using UnityEngine;
using WorldGeneration._Scripts.Spawning;
using WorldGeneration._Scripts.Spawning.TerrainAssets;

namespace ml_agents.Agents.Handler
{
    public class InteractionHandler : MonoBehaviour
    {
        public Transform mouth;
        [Range(0.1f, 1)] public float interactionRange = 0.5f;
        public LayerMask interactableLayer;

        [Space(10)] [Header("Eating")] public bool canEat = false;
        [TagSelector] public string foodTag;

        [Space(10)] [Header("Drinking")] public bool canDrink = false;
        public int thirstDecreasePerDrink = 1;

        [Space(10)] [Header("Burrow")] public bool canBuildBurrow;
        public bool isBurrowBuildableHere;
        public bool isStandingBeforeBurrow = false;
        public GameObject detectedBurrow;

        [Space(10)] [Header("Rewards")] public float rewardForBreeding = 1f;
        public float penaltyForTryingToDoSomethingWithoutCorrectConditions = -0.01f;

        CustomAgent agent;

        // Start is called before the first frame update
        void Start()
        {
            agent = gameObject.GetComponent<CustomAgent>();
        }

        // Update is called once per frame
        void Update()
        {
            Ray ray = new Ray(mouth.position, mouth.TransformDirection(Vector3.forward));
            RaycastHit[] hits = Physics.RaycastAll(ray);
            if (hits.Length > 0 && hits[0].distance < interactionRange)
            {
                if (hits[0].collider.gameObject.layer == interactableLayer)
                {
                    isStandingBeforeBurrow = hits[0].collider.gameObject.CompareTag("Burrow");
                    if (isStandingBeforeBurrow)
                    {
                        detectedBurrow = hits[0].collider.gameObject;
                    }
                    canEat = hits[0].collider.gameObject.CompareTag(foodTag);
                }
                else if (hits[0].collider.gameObject.layer == LayerMask.NameToLayer("Water"))
                {
                    canDrink = true;
                }
                else
                {
                    canDrink = false;
                }
            }

            //here check if location sit possible
            isBurrowBuildableHere = AssetManager.GetInstance().CheckLocation(transform.position);
        }

        public void Eat()
        {
            if (canEat)
            {
                agent.isEating = true;
                gameObject.GetComponent<Carrot>().Eat(gameObject);
            }
            else
            {
                agent.AddReward(penaltyForTryingToDoSomethingWithoutCorrectConditions);
            }
        }

        public void Drink()
        {
            if (canDrink)
            {
                agent.isDrinking = true;
                agent.BlockMovementForSeconds(0.5f);

                if (agent.thirst > 0)
                {
                    agent.thirst -= thirstDecreasePerDrink;
                }
            }
            else
            {
                agent.AddReward(penaltyForTryingToDoSomethingWithoutCorrectConditions);
            }
        }

        public void Breed()
        {
            if (CheckAllConditionsForBreeding())
            {
                agent.hasBreeded = true;
                agent.AddReward(rewardForBreeding);
                transform.parent.TryGetComponent<Burrow>(out Burrow burrow);
                burrow.Breed();
            }
            else
            {
                agent.AddReward(penaltyForTryingToDoSomethingWithoutCorrectConditions);
            }
        }

        public void EnterBurrow()
        {
            if (CheckAllConditionsForEnteringBurrow())
            {
                detectedBurrow.TryGetComponent<Burrow>(out var burrow);
                burrow.Enter(gameObject, agent, agent.controller);
            }
            else
            {
                agent.AddReward(penaltyForTryingToDoSomethingWithoutCorrectConditions);
            }
        }

        public void LeaveBurrow()
        {
            if (agent.isInBurrow)
            {
                transform.parent.TryGetComponent<Burrow>(out var burrow);
                burrow.Leave(gameObject, agent, agent.controller);
            }
            else
            {
                agent.AddReward(penaltyForTryingToDoSomethingWithoutCorrectConditions);
            }
        }

        public void BuildBurrow()
        {
            if (CheckAllConditionsForBuildingBurrow())
            {
                AssetManager.GetInstance().BuildBurrow(gameObject, this, agent);
            }
            else
            {
                agent.AddReward(penaltyForTryingToDoSomethingWithoutCorrectConditions);
            }
        }

        private bool CheckAllConditionsForBuildingBurrow()
        {
            return isBurrowBuildableHere && !agent.isInBurrow && !agent.hasBurrowBuild;
        }

        private bool CheckAllConditionsForEnteringBurrow()
        {
            if (isStandingBeforeBurrow)
            {
                detectedBurrow.TryGetComponent<Burrow>(out var burrow);
                return burrow.inhabitants.Count < 2;
            }

            return false;
        }

        private bool CheckAllConditionsForBreeding()
        {
            if (agent.isInBurrow && agent.isAdult && !agent.hasBreeded)
            {
                if (gameObject.transform.parent.GetComponent<Burrow>().inhabitants.Count == 2)
                {
                    foreach (GameObject agentGameObject in gameObject.transform.parent.GetComponent<Burrow>()
                                 .inhabitants)
                    {
                        if (agentGameObject != this.gameObject)
                        {
                            return agentGameObject.GetComponent<InteractionHandler>().CanIBreed();
                        }
                    }
                }
            }

            return false;
        }

        public bool CanIBreed()
        {
            if (agent.isAdult && !agent.hasBreeded)
            {
                //instant block so other agent can not ask
                agent.hasBreeded = true;
                return true;
            }

            ;
            return false;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(mouth.position, mouth.TransformDirection(Vector3.forward) * interactionRange);
        }
    }
}