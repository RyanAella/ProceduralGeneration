using ml_agents.Custom_Attributes_for_editor;
using Unity.IO.LowLevel.Unsafe;
using UnityEditor.VersionControl;
using UnityEngine;
using WorldGeneration._Scripts.Spawning;
using WorldGeneration._Scripts.Spawning.TerrainAssets;

namespace ml_agents.Agents.Handler
{
    public class InteractionHandler : MonoBehaviour
    {
        public Transform mouth;
        [Range(0.1f, 10)] public float interactionRange = 0.5f;
        public LayerMask interactableLayer;

        [Space(10)] [Header("Eating")] public bool canEat = false;
        [TagSelector] public string foodTag;
        public GameObject detectedFood;

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
            RaycastHit[] hits = Physics.RaycastAll(ray, interactionRange, interactableLayer);

            if (hits.Length > 0)
            {
                GameObject firstHit = hits[0].collider.gameObject;

                canEat = firstHit.CompareTag(foodTag);

                if (agent.type == AgentType.Fox)
                {
                    firstHit = firstHit.transform.parent.gameObject;
                }

                if (canEat)
                {
                    detectedFood = firstHit;

                    canDrink = false;
                    isStandingBeforeBurrow = false;
                    detectedBurrow = null;
                }

                if (firstHit.layer == LayerMask.NameToLayer("Water"))
                {
                    canDrink = true;

                    canEat = false;
                    isStandingBeforeBurrow = false;
                    detectedBurrow = null;
                }
                

                isStandingBeforeBurrow = firstHit.CompareTag("Burrow");
                if (isStandingBeforeBurrow)
                {
                    detectedBurrow = firstHit;
                    canDrink = false;
                    canEat = false;
                }
            } else
            {
                canEat = false;
                canDrink = false;
                isStandingBeforeBurrow = false;
                detectedBurrow = null;
            }

            //here check if location sit possible
            isBurrowBuildableHere = AssetManager.GetInstance().CheckLocation(transform.position, agent.type) 
                                    && !agent.isInBurrow && !agent.hasBurrowBuild;
        }

        public void Eat()
        {
            if (canEat)
            {
                Debug.Log(agent.type + " - " + "ate succesful");
                agent.isEating = true;

                CustomAgent toEatAgent = detectedFood.GetComponent<CustomAgent>();

                if ( toEatAgent != null)
                {
                    toEatAgent.GotAttacked(agent);
                } else
                {
                    detectedFood.GetComponent<Carrot>().Eat(gameObject);
                }
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
                Debug.Log(agent.type + " - " + "drunk succesful");
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
                Debug.Log(agent.type + " - " + "breeded succesful");
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
                detectedBurrow.transform.parent.TryGetComponent<Burrow>(out var burrow);
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

                //save position of last visited burrow
                agent.lastBurrow = transform.parent.position;
                burrow.Leave(gameObject, agent, agent.controller);
            }
            else
            {
                agent.AddReward(penaltyForTryingToDoSomethingWithoutCorrectConditions);
            }
        }

        public void BuildBurrow()
        {
            if (isBurrowBuildableHere)
            {
                Debug.Log(agent.type + " - " + "builded burrow succesful");
                AssetManager.GetInstance().BuildBurrow(gameObject, this, agent);
            }
            else
            {
                agent.AddReward(penaltyForTryingToDoSomethingWithoutCorrectConditions);
            }
        }

        private bool CheckAllConditionsForEnteringBurrow()
        {
            if (isStandingBeforeBurrow)
            {
                detectedBurrow.transform.parent.TryGetComponent<Burrow>(out var burrow);
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
            return false;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(mouth.position, mouth.TransformDirection(Vector3.forward) * interactionRange);
        }
    }
}