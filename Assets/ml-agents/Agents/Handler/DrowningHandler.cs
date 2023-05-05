using UnityEngine;

namespace ml_agents.Agents.Handler
{
    public class DrowningHandler : MonoBehaviour
    {
        public Transform nose;
        public bool isDrowning = false;
        [Range(1, 10)] public float decreaseHealtVerySeconds = 3;
        [Range(1, 10)] public int healthDecreaseForDrowning = 4;

        CustomAgent _agent;

        float _timerSeconds;

        // Start is called before the first frame update
        void Start()
        {
            _agent = gameObject.GetComponent<CustomAgent>();
        }

        // Update is called once per frame
        void Update()
        {
            if(!isDrowning && nose.position.y < 0)
            {
                isDrowning = true;
                _timerSeconds = decreaseHealtVerySeconds;
            }

            if (isDrowning)
            {
                if(nose.position.y >= 0)
                {
                    isDrowning = false;
                }
                HandleBlockTimer();
            }
        }

        private void HandleBlockTimer()
        {
            _timerSeconds -= Time.deltaTime;
            if (decreaseHealtVerySeconds <= 0.0f)
            {
                _timerSeconds = decreaseHealtVerySeconds;
                _agent.health -= healthDecreaseForDrowning;
            }
        }
    }
}
