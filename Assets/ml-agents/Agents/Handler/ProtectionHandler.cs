using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProtectionHandler : MonoBehaviour
{
    public LayerMask layerMask;
    [Range(1, 5)] public float detectionRange = 5;

    [Space(5)]
    [Range(0, 3)] public int protectionLevel;

    CustomAgent agent;
    
    string toSearchForTag;

    // Start is called before the first frame update
    void Start()
    {
        agent = gameObject.GetComponent<CustomAgent>();

        switch(agent.type)
        {
            case AgentType.RABBIT:
                toSearchForTag = "Rabbit";
                break;
            case AgentType.FOX:
                toSearchForTag = "Fox";
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        protectionLevel = GetGroupProtectionLevel() + Convert.ToInt16(agent.isInDen);
    }

    private int GetGroupProtectionLevel()
    {
        int targetsInRange = 0;

        RaycastHit[] hits = Physics.SphereCastAll(transform.position, detectionRange, transform.forward, 0, layerMask);
        foreach (RaycastHit hit in hits)
        {
            if (hit.transform.gameObject.CompareTag(toSearchForTag))
            {
                targetsInRange++;
            }
        }

        targetsInRange -= 1;

        if (targetsInRange <= 0)
        {
            return 0;
        } else if (targetsInRange <=2)
        {
            return 1; 
        } else
        {
            return 2;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
