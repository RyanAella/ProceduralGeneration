using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundDetection : MonoBehaviour
{
    public CustomAgent agent;
    public LayerMask layerMask;
    [Range(1, 10)] public float soundRangeMultiplier = 1;
    float soundRange = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        soundRange = agent.movement.walkSpeed * soundRangeMultiplier;

        RaycastHit[] hits = Physics.SphereCastAll(transform.position, soundRange, transform.forward, 0, layerMask);
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.gameObject.GetComponent<CustomAgent>() != null)
            {
                hit.collider.gameObject.GetComponent<CustomAgent>().NotificationOnMovement(agent);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, soundRange);
    }

    float Remap(float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }
}
