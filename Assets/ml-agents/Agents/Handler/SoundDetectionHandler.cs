using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundDetectionHandler : MonoBehaviour
{
    public float lastSoundDistance = -1;
    public Vector3 lastSoundDirection = new Vector3(0, 0, 0);
    public bool soundOnlyFromEnemy = false;
    [Range(1, 10)] public float soundRangeMultiplier = 1;
    float soundRange = 0;

    CustomAgent agent;
    public LayerMask layerMask;

    // Start is called before the first frame update
    void Start()
    {
        agent = gameObject.GetComponent<CustomAgent>();
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
                hit.collider.gameObject.GetComponent<SoundDetectionHandler>().CheckSoundOrigin(agent);
            }
        }
    }

    public void CheckSoundOrigin(CustomAgent loudAgent)
    {
        if (agent != loudAgent)
        {
            if (!soundOnlyFromEnemy)
            {
                lastSoundDistance = Vector3.Distance(transform.position, loudAgent.transform.position);
                lastSoundDirection = loudAgent.transform.position - transform.position;
                Debug.Log("Sound!");
            }
            else if (agent.type != loudAgent.type)
            {
                lastSoundDistance = Vector3.Distance(transform.position, loudAgent.transform.position);
                lastSoundDirection = loudAgent.transform.position - transform.position;
                Debug.Log("Enemy sound!");
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
