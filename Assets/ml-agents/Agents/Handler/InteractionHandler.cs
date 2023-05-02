using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionHandler : MonoBehaviour
{
    //PROBLEM: Water is l
    public Transform mouth;
    [Range(0.1f, 1)] public float interactionRange = 0.5f;
    public LayerMask interactableLayers;

    [Space(10)]
    [Header("Eating")]
    public LayerMask eatableLayer;
    public bool canEat = false;
   
    [Space(10)]
    [Header("Drinking")]
    public bool canDrink = false;

    [Space(10)]
    [Header("Den")]
    public bool canBreed = false;
    public bool canBuildDen;
    public bool isDenBuildableHere;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = new Ray(mouth.position, mouth.TransformDirection(Vector3.forward));
        RaycastHit[] hits = Physics.RaycastAll(ray);
        if (hits.Length > 0)
        {
            if (hits[0].distance < interactionRange && hits[0].collider.gameObject.layer == LayerMask.NameToLayer("Water") )
            {
                canDrink = true;
            } else
            {
                canDrink = false;
            }

            //TODO: EATABLE?
            //Other interactions?
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(mouth.position, mouth.TransformDirection(Vector3.forward) * interactionRange);
    }
}
