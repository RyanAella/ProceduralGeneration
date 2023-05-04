using UnityEngine;
using WorldGeneration._Scripts.Spawning.TerrainAssets;

public class InteractionHandler : MonoBehaviour
{
    public Transform mouth;
    [Range(0.1f, 1)] public float interactionRange = 0.5f;
    public LayerMask interactableLayer;

    [Space(10)]
    [Header("Eating")]
    public bool canEat = false;
    [TagSelector] public string foodTag;
   
    [Space(10)]
    [Header("Drinking")]
    public bool canDrink = false;

    [Space(10)]
    [Header("Den")]
    public bool canBuildDen;
    public bool isDenBuildableHere;
    public bool isStandingBeforeDen = false;

    [Space(10)]
    [Header("Rewards")]
    public float rewardForBreeding = 1f;
    public float penaltyForTryingToDoSomethingWithoutCorrectConditions = 0.01f;

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
                isStandingBeforeDen = hits[0].collider.gameObject.tag == "Burrow";
                canEat = hits[0].collider.gameObject.tag == foodTag;
            }
            else if (hits[0].collider.gameObject.layer == LayerMask.NameToLayer("Water") )
            {
                canDrink = true;
            } else
            {
                canDrink = false;
            }
        }

        //here check if location sit possible
        // isDenBuildableHere = ???
    }
    public void Eat()
    {
        if (canEat)
        {
            agent.isEating = true;
            //gameobject.getComponent<Carrot>().Eat(this.gameobject);
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
                agent.thirst--;
            }
        }
    }

    public void Breed()
    {
        if (!checkAllConditionsForBreeding())
            return;

        agent.hasBreeded = true;
        //gameObject.transform.parent.GetComponent<RabbitBurrow>.Breed();
    }

    public void EnterBurrow()
    {
        if(checkAllConditionsForEnteringBurrow())
        {
            gameObject.transform.parent.GetComponent<RabbitBurrow>().Enter(this.gameObject);
        }
    }

    public void LeaveBurrow()
    {
        if (agent.isInDen)
        {
            gameObject.transform.parent.GetComponent<RabbitBurrow>().Leave(this.gameObject);
        }
    }

    public void BuildBurrow()
    {
        canBuildDen = checkAllConditionsForBuildingDen();
        if (!canBuildDen)
            return;

        //AssetManager.BuildBurrow(this.gameobject.transform);
    }

    private bool checkAllConditionsForBuildingDen()
    {
        return isDenBuildableHere && !agent.isInDen && !agent.hasDen;
    }

    private bool checkAllConditionsForEnteringBurrow()
    {
        if (isStandingBeforeDen)
        {
            return gameObject.transform.parent.GetComponent<RabbitBurrow>().inhabitants.Count < 2;
        }
        return false;
    }

    private bool checkAllConditionsForBreeding()
    {
        if (agent.isInDen && agent.isAdult && !agent.hasBreeded)
        {
            if (gameObject.transform.parent.GetComponent<RabbitBurrow>().inhabitants.Count == 2)
            {
                foreach (GameObject agentGameObject in gameObject.transform.parent.GetComponent<RabbitBurrow>().inhabitants)
                {
                    if(agentGameObject != this.gameObject) {
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
        };
        return false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(mouth.position, mouth.TransformDirection(Vector3.forward) * interactionRange);
    }
}
