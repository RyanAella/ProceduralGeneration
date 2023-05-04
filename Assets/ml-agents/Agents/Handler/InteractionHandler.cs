using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InteractionHandler : MonoBehaviour
{
    //PROBLEM: Water is l
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
    public bool canBreed = false;
    public bool canBuildDen;
    public bool isDenBuildableHere;
    public bool isStandingBeforeDen = false;

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

            //here check if location sit possible
            // isDenBuildableHere = ???
        }
    }

    //later replaced because food is setting the time for BlockMovementForSeconds
    public void Eat()
    {
        if (canEat)
        {
            agent.isEating = true;
            agent.BlockMovementForSeconds(4);

            if (agent.hunger > 0)
            {
                agent.thirst -= 5;
            }
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

        //breed
    }
    public void EnterBurrow()
    {
        if(checkAllConditionsForEnteringBurrow())
        {
            //enterBurrow
        }
    }

    public void LeaveBurrow()
    {
        if (agent.isInDen)
        {
            //leavBurrow
        }
    }

    public void BuildBurrow()
    {
        canBuildDen = checkAllConditionsForBuildingDen();
        if (!canBuildDen)
            return;

        //build
    }

    private bool checkAllConditionsForBuildingDen()
    {
        return isDenBuildableHere && canBreed && !agent.isInDen && !agent.hasDen;
    }

    private bool checkAllConditionsForBreeding()
    {
        if (agent.isInDen && agent.isInDen && agent.isAdult && !agent.hasBreeded)
        {
            //TODO: check if other rabbit partner is in burrow and if it can also breed
            bool hasPartner = false;
            return hasPartner;
        }
        return false;
    }

    private bool checkAllConditionsForEnteringBurrow()
    {
        if (isStandingBeforeDen)
        {
            //return canEnterBurrow / is empty?
            return true;
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
