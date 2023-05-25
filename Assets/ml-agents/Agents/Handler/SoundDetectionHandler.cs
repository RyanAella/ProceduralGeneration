using UnityEngine;

namespace ml_agents.Agents.Handler
{
    public class SoundDetectionHandler : MonoBehaviour
    {
        public float lastSoundDistance = -1;
        public Vector3 lastSoundDirection = new(0, 0, 0);

        public float lastEnemySoundDistance = -1;
        public Vector3 lastEnemySoundDirection = new(0, 0, 0);

        public AgentType enemy;

        [Range(1, 50)] public float soundRangeMultiplier = 1;
        public LayerMask layerMask;

        private CustomAgent _agent;
        private float _soundRange;
        private bool enemySoundHasChanged;
        private ProtectionHandler protectionHandler;


        private bool soundHasChanged;


        // Start is called before the first frame update
        private void Start()
        {
            _agent = gameObject.GetComponent<CustomAgent>();
            protectionHandler = _agent.protectionHandler;
        }

        // Update is called once per frame
        private void Update()
        {
            _soundRange = _agent.movement.walkSpeed * soundRangeMultiplier;

            var hits = Physics.SphereCastAll(transform.position, _soundRange, transform.forward, 0, layerMask);
            foreach (var hit in hits)
                if (hit.collider.gameObject.GetComponent<CustomAgent>() != null)
                    hit.collider.gameObject.GetComponent<SoundDetectionHandler>().CheckSoundOrigin(_agent);
            RemoveOldSounds();
            _agent.hearsEnemy = lastEnemySoundDistance != -1;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _soundRange);
        }

        private void RemoveOldSounds()
        {
            if (!soundHasChanged)
            {
                lastSoundDistance = -1;
                lastSoundDirection = new Vector3(0, 0, 0);
            }

            if (!enemySoundHasChanged)
            {
                lastEnemySoundDistance = -1;
                lastEnemySoundDirection = new Vector3(0, 0, 0);
            }

            soundHasChanged = false;
            enemySoundHasChanged = false;
        }

        public void CheckSoundOrigin(CustomAgent loudAgent)
        {
            if (!loudAgent)
            {
                Debug.Log("null agent found");
                return;
            }

            if (_agent != loudAgent)
            {
                if (_agent.type == loudAgent.type)
                {
                    lastSoundDistance = Vector3.Distance(transform.position, loudAgent.transform.position);
                    lastSoundDirection = loudAgent.transform.position - transform.position;
                    //Debug.Log("Sound!");
                    soundHasChanged = true;
                }

                if (loudAgent.type == enemy)
                {
                    lastEnemySoundDistance = Vector3.Distance(transform.position, loudAgent.transform.position);
                    lastEnemySoundDirection = loudAgent.transform.position - transform.position;
                    //Debug.Log("Enemy sound!");
                    enemySoundHasChanged = true;
                }
            }
        }

        private float Remap(float value, float from1, float to1, float from2, float to2)
        {
            return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
        }
    }
}