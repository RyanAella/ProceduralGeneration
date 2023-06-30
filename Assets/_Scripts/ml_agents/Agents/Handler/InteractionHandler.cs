using _Scripts.ml_agents.Agents.Fox;
using _Scripts.ml_agents.Agents.Rabbit;
using _Scripts.WorldGeneration.Spawning;
using _Scripts.WorldGeneration.Spawning.TerrainAssets;
using UnityEngine;

namespace _Scripts.ml_agents.Agents.Handler
{
    public class InteractionHandler : MonoBehaviour
    {
        public Transform mouth;
        [Range(0.1f, 10)] public float interactionRange = 0.5f;
        public LayerMask interactableLayer;

        [Space(10)] [Header("Eating")] public bool canEat;
        public string foodTag;
        public GameObject detectedFood;

        [Space(10)] [Header("Drinking")] public bool canDrink;
        public int thirstDecreasePerDrink = 1;

        [Space(10)] [Header("Burrow")] public bool canBuildBurrow;
        public bool isBurrowBuildableHere;
        public bool isStandingBeforeBurrow;
        public GameObject detectedBurrow;

        [Space(10)] [Header("Rewards")] 
        public float rewardForBreeding = 1f;
        public float rewardForDoingSomethingCorrect = 0.5f;
        public float penaltyForTryingToDoSomethingWithoutCorrectConditions = -0.01f;

        [Space(10)]
        [Header("Extra")]
        public string debugColor;
        public CustomAgent agent;

        // Update is called once per frame
        void Update()
        {
            Ray ray = new Ray(mouth.position, mouth.TransformDirection(Vector3.forward));
            RaycastHit[] hits = Physics.RaycastAll(ray, interactionRange, interactableLayer);

            if (hits.Length > 0)
            {
                GameObject firstHit = hits[0].collider.gameObject;

                canEat = firstHit.CompareTag(foodTag);

                if (canEat && agent.type == AgentType.Fox)
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

            //here check if location is possible
            isBurrowBuildableHere = AssetManager.GetInstance().CheckLocation(transform.position, agent.type) 
                                    && !agent.isInBurrow && !agent.hasBurrowBuild && agent.isAdult;
        }

        public void Eat()
        {
            if (canEat)
            {
                switch (agent.type)
                {
                    case AgentType.Rabbit:
                        //must be checked because other agents could interact at same time or carrot dies
                        if (detectedFood != null)
                        {
                            detectedFood.TryGetComponent(out Carrot toEatCarrot);
                            toEatCarrot.Eat(agent);
                            Debug.Log("<color=" + debugColor + ">" + agent.type + "</color>" + " - " + "ate successful [" + agent.GetComponent<AgentRabbit>().id + "]");
                        } else
                        {
                            
                            Debug.Log("carrot already null [" + agent.GetComponent<AgentRabbit>().id + "]");
                        }
                        break;
                    case AgentType.Fox:
                        //check because rabbit could die in between
                        if(detectedFood != null)
                        {
                            detectedFood.TryGetComponent(out CustomAgent toEatAgent);
                            toEatAgent.GotAttacked(agent);
                            Debug.Log("<color=" + debugColor + ">" + agent.type + "</color>" + " - " + "ate successful [" + agent.GetComponent<AgentFox>().id + "]");
                        } else
                        {
                            Debug.LogWarning("<color=" + debugColor + ">" + agent.type + "</color>" + " - " + " rabbit to eat already destroyed [" + agent.GetComponent<AgentFox>().id + "]");
                        }
                        break;
                }
                if(agent.hunger >= 0)
                {
                    agent.AddReward(rewardForDoingSomethingCorrect*2.25f);
                }
            }
        }

        public void Drink()
        {
            if (canDrink)
            {
                Debug.Log("<color=" + debugColor + ">" + agent.type + "</color>" + " - " + "drunk successful [" + agent.id + "]");
                agent.BlockInteractionsForSeconds(1f);

                if (agent.thirst > -30)
                {
                    if ((agent.thirst - thirstDecreasePerDrink) <= -30)
                    {
                        agent.thirst -= thirstDecreasePerDrink;
                    }
                    else
                    {
                        agent.thirst = -30;
                    }
                }
                if (agent.thirst >= 0)
                {
                    agent.AddReward(rewardForDoingSomethingCorrect);
                }
            }
        }

        public void Breed()
        {
            if (CheckAllConditionsForBreeding())
            {
                Debug.Log("<color=" + debugColor + ">" + agent.type + "</color>" + " - " + "bred successful [" + agent.id + "]");
                agent.hasBred = true;
                agent.AddReward(rewardForBreeding);
                transform.parent.TryGetComponent(out Burrow burrow);
                burrow.Breed(agent);
                agent.BlockInteractionsForSeconds(20);
            }
        }

        public void EnterBurrow()
        {
            if (CheckAllConditionsForEnteringBurrow())
            {
                agent.BlockInteractionsForSeconds(3);
                //Debug.Log("<color=" + debugColor + ">" + agent.type + "</color>" + " - " + "entered burrow successful [" + agent.id + "]");
                detectedBurrow.transform.parent.TryGetComponent(out Burrow burrow);
                burrow.Enter(gameObject, agent, agent.controller);
            }
        }

        public void LeaveBurrow()
        {
            if (agent.isInBurrow)
            {
                agent.BlockInteractionsForSeconds(3);
                //Debug.Log("<color=" + debugColor + ">" + agent.type + "</color>" + " - " + "left burrow successful [" + agent.id + "]");
                transform.parent.TryGetComponent(out Burrow burrow);

                //save position of last visited burrow
                agent.lastBurrow = transform.parent.position;
                burrow.Leave(gameObject, agent, agent.controller);
            }
        }

        public void BuildBurrow()
        {
            if (isBurrowBuildableHere)
            {
                agent.BlockInteractionsForSeconds(5f);
                agent.hasBurrowBuild = true;
                Debug.Log("<color=" + debugColor + ">" + agent.type + "</color>" + " - " + "built burrow successful [" + agent.id + "]");
                AssetManager.GetInstance().BuildBurrow(gameObject, this, agent);
            }
        }

        private bool CheckAllConditionsForEnteringBurrow()
        {
            if (isStandingBeforeBurrow && !agent.isInBurrow)
            {
                detectedBurrow.transform.parent.TryGetComponent(out Burrow burrow);

                return burrow.inhabitants.Count < 2;
            }

            return false;
        }

        private bool CheckAllConditionsForBreeding()
        {
            if (CanBreed())
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

        public bool CanBreed()
        {
            return agent.isInBurrow && agent.isAdult && !agent.hasBred;
        }

        /**
         * called by partner in burrow who wants to breed
         * sets agent as has bred and returning true, 
         * if breeding is possible
         */
        private bool CanIBreed()
        {
            if (agent.isAdult && !agent.hasBred)
            {
                //instant block so other agent can not ask
                agent.hasBred = true;
                agent.BlockInteractionsForSeconds(20);
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