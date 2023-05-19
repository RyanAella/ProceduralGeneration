using UnityEngine;

namespace ml_agents.Agents.Handler
{
    public class SoundDetectionHandler : MonoBehaviour
    {
        public float lastSoundDistance = -1;
        public Vector3 lastSoundDirection = new Vector3(0, 0, 0);
        public bool soundOnlyFromEnemy = false;
        [Range(1, 50)] public float soundRangeMultiplier = 1;
        float _soundRange = 0;

        CustomAgent _agent;
        public LayerMask layerMask;

        // Start is called before the first frame update
        void Start()
        {
            _agent = gameObject.GetComponent<CustomAgent>();
        }

        // Update is called once per frame
        void Update()
        {
            _soundRange = _agent.movement.walkSpeed * soundRangeMultiplier;

            RaycastHit[] hits = Physics.SphereCastAll(transform.position, _soundRange, transform.forward, 0, layerMask);
            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.gameObject.GetComponent<CustomAgent>() != null)
                {
                    hit.collider.gameObject.GetComponent<SoundDetectionHandler>().CheckSoundOrigin(_agent);
                }
            }
        }

        public void CheckSoundOrigin(CustomAgent loudAgent)
        {
            if (_agent != loudAgent)
            {
                if (!soundOnlyFromEnemy)
                {
                    lastSoundDistance = Vector3.Distance(transform.position, loudAgent.transform.position);
                    lastSoundDirection = loudAgent.transform.position - transform.position;
                    Debug.Log("Sound!");
                }
                else if (_agent.type != loudAgent.type)
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
            Gizmos.DrawWireSphere(transform.position, _soundRange);
        }

        float Remap(float value, float from1, float to1, float from2, float to2)
        {
            return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
        }
    }
}
