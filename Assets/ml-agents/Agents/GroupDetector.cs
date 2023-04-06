using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroupDetector : MonoBehaviour
{
    public LayerMask layerMask;
    [TagSelector] public string toSearchForTag;
    [Range(1, 5)] public float detectionRange = 5;
    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        int targetsInRange = 0;

        RaycastHit[] hits = Physics.SphereCastAll(transform.position, detectionRange, transform.forward, 0, layerMask);
        foreach (RaycastHit hit in hits) {
            if (hit.transform.gameObject.CompareTag(toSearchForTag)) {
                targetsInRange++;
            }
        }
        targetsInRange -= 1;
        //Debug.Log("Close Rabbits: " + targetsInRange);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
