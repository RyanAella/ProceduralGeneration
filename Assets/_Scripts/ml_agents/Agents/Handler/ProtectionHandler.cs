using System;
using UnityEngine;

namespace _Scripts.ml_agents.Agents.Handler
{
    public class ProtectionHandler : MonoBehaviour
    {
        public bool someoneInGroupHearsEnemy;
        public LayerMask layerMask;
        [Range(1, 50)] public float detectionRange = 5;

        private CustomAgent _agent;

        private string _toSearchForTag;

        // Start is called before the first frame update
        void Start()
        {
            _agent = gameObject.GetComponent<CustomAgent>();
            _toSearchForTag = _agent.type.ToString();
        }

        // Update is called once per frame
        void Update()
        {
            CheckSomeoneInGroupHearsEnemy();
        }

        private void CheckSomeoneInGroupHearsEnemy()
        {
            RaycastHit[] hits =
                Physics.SphereCastAll(transform.position, detectionRange, transform.forward, 0, layerMask);

            bool anybodyHearAnEnemy = false;

            foreach (RaycastHit hit in hits)
            {
                if (hit.transform.gameObject != gameObject && hit.transform.gameObject.CompareTag(_toSearchForTag))
                {
                    CustomAgent agentInRange = hit.transform.gameObject.GetComponent<CustomAgent>();
                    if (!anybodyHearAnEnemy && agentInRange != null)
                    {
                        anybodyHearAnEnemy = agentInRange.hearsEnemy;
                    }
                }
            }
            someoneInGroupHearsEnemy = anybodyHearAnEnemy;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, detectionRange);
        }
    }
}