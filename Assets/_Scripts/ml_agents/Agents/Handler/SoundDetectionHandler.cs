using UnityEngine;

namespace _Scripts.ml_agents.Agents.Handler
{
    public class SoundDetectionHandler : MonoBehaviour
    {
        public float lastEnemySoundDistance = -1;
        public Vector3 lastEnemySoundDirection = new(0, 0, 0);

        public AgentType enemy;

        [Range(1, 50)] public float soundRangeMultiplier = 1;
        public LayerMask layerMask;

        private CustomAgent _agent;
        private float _soundRange;
        private bool _enemySoundHasChanged;

        private void Start()
        {
            _agent = gameObject.GetComponent<CustomAgent>();
        }

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
            if (!_enemySoundHasChanged)
            {
                lastEnemySoundDistance = -1;
                lastEnemySoundDirection = new Vector3(0, 0, 0);
            }
            _enemySoundHasChanged = false;
        }
        private void CheckSoundOrigin(CustomAgent loudAgent)
        {
            //check if loud agent is not itself & is an enemy
            if (_agent != loudAgent && loudAgent.type == enemy)
            {
                lastEnemySoundDistance = Vector3.Distance(transform.position, loudAgent.transform.position);
                lastEnemySoundDirection = loudAgent.transform.position - transform.position;
                _enemySoundHasChanged = true;
            }
        }
    }
}