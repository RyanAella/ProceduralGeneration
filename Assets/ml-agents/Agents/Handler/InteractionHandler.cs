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

        [Space(10)] [Header("Burrow")] public bool canBuildBurrow;
        public bool isBurrowBuildableHere;
        public bool isStandingBeforeBurrow = false;

        [Space(10)] [Header("Rewards")] public float rewardForBreeding = 1f;
        public float penaltyForTryingToDoSomethingWithoutCorrectConditions = 0.01f;

        CustomAgent _agent;

        // Start is called before the first frame update
        void Start()
        {
            _agent = gameObject.GetComponent<CustomAgent>();
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
                _agent.isEating = true;
                gameObject.GetComponent<Carrot>().Eat(gameObject);
            }
        }

        public void Drink()
        {
            if (canDrink)
            {
                _agent.isDrinking = true;
                _agent.BlockMovementForSeconds(0.5f);

                if (_agent.thirst > 0)
                {
                    _agent.thirst--;
                }
            }
        }

        public void Breed()
        {
            if (!CheckAllConditionsForBreeding())
                return;

            _agent.hasBreeded = true;
            transform.parent.GetComponent<Burrow>().Breed(gameObject);
        }

        public void EnterBurrow()
        {
            if (CheckAllConditionsForEnteringBurrow())
            {
                gameObject.transform.parent.GetComponent<Burrow>().Enter(gameObject);
            }
        }

        public void LeaveBurrow()
        {
            if (_agent.isInBurrow)
            {
                gameObject.transform.parent.GetComponent<Burrow>().Leave(gameObject);
            }
        }

        public void BuildBurrow()
        {
            canBuildBurrow = CheckAllConditionsForBuildingBurrow();
            if (!canBuildBurrow)
                return;

            AssetManager.GetInstance().BuildBurrow(gameObject);
        }

        private bool CheckAllConditionsForBuildingBurrow()
        {
            return isBurrowBuildableHere && !_agent.isInBurrow && !_agent.hasBurrow;
        }

        private bool CheckAllConditionsForEnteringBurrow()
        {
            if (isStandingBeforeBurrow)
            {
                return gameObject.transform.parent.GetComponent<Burrow>().inhabitants.Count < 2;
            }

            return false;
        }

        private bool CheckAllConditionsForBreeding()
        {
            if (_agent.isInBurrow && _agent.isAdult && !_agent.hasBreeded)
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
            if (_agent.isAdult && !_agent.hasBreeded)
            {
                //instant block so other agent can not ask
                _agent.hasBreeded = true;
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