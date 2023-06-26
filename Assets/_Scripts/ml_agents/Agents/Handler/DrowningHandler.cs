using UnityEngine;

namespace _Scripts.ml_agents.Agents.Handler
{
    public class DrowningHandler : MonoBehaviour
    {
        public Transform nose;
        public bool isDrowning;
        public float drowningHeight;
        [Range(1, 10)] public float decreaseHealthVerySeconds = 3;
        [Range(1, 10)] public int healthDecreaseForDrowning = 4;

        private CustomAgent _agent;

        private float _timerSeconds;

        // Start is called before the first frame update
        void Start()
        {
            _agent = gameObject.GetComponent<CustomAgent>();
        }

        // Update is called once per frame
        void Update()
        {
            if (!isDrowning && nose.position.y < drowningHeight && !_agent.isInBurrow)
            {
                isDrowning = true;
                _timerSeconds = decreaseHealthVerySeconds;
            }

            if (isDrowning)
            {
                if (nose.position.y >= drowningHeight)
                {
                    isDrowning = false;
                }

                HandleBlockTimer();
            }
        }

        private void HandleBlockTimer()
        {
            _timerSeconds -= Time.deltaTime;
            if (_timerSeconds <= 0.0f)
            {
                _agent.RemoveHealth(healthDecreaseForDrowning);
                _timerSeconds = decreaseHealthVerySeconds;
            }
        }
    }
}