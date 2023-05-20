using System;
using UnityEngine;
using WorldGeneration._Scripts.Spawning.TerrainAssets;

namespace ml_agents.Agents.Handler
{
    public class ProtectionHandler : MonoBehaviour
    {
        public bool someoneInGroupHearsEnemy;
        public LayerMask layerMask;
        [Range(1, 50)] public float detectionRange = 5;

        [Space(5)]
        [Range(0, 3)] public int protectionLevel;

        CustomAgent _agent;
    
        string _toSearchForTag;

        // Start is called before the first frame update
        void Start()
        {
            _agent = gameObject.GetComponent<CustomAgent>();

            switch(_agent.type)
            {
                case AgentType.Rabbit:
                    _toSearchForTag = "Rabbit";
                    break;
                case AgentType.Fox:
                    _toSearchForTag = "Fox";
                    break;
            }
        }

        // Update is called once per frame
        void Update()
        {
            protectionLevel = GetGroupProtectionLevel() + Convert.ToInt16(_agent.isInBurrow);
        }

        private int GetGroupProtectionLevel()
        {
            int targetsInRange = 0;

            RaycastHit[] hits = Physics.SphereCastAll(transform.position, detectionRange, transform.forward, 0, layerMask);

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

                    targetsInRange++;
                }
            }

            someoneInGroupHearsEnemy = anybodyHearAnEnemy;

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
}
