using UnityEngine;

namespace ml_agents.Agents.Handler
{
    public class DrowningHandler : MonoBehaviour
    {
        public Transform nose;
        public bool isDrowning = false;
        public float drowningHeight = 0;
        [Range(1, 10)] public float decreaseHealtVerySeconds = 3;
        [Range(1, 10)] public int healthDecreaseForDrowning = 4;

        CustomAgent agent;

        float timerSeconds;

        // Start is called before the first frame update
        void Start()
        {
            agent = gameObject.GetComponent<CustomAgent>();
        }

        // Update is called once per frame
        void Update()
        {
            if(!isDrowning && nose.position.y < drowningHeight)
            {
                isDrowning = true;
                timerSeconds = decreaseHealtVerySeconds;
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
            timerSeconds -= Time.deltaTime;
            if (timerSeconds <= 0.0f)
            {
                agent.RemoveHealth(healthDecreaseForDrowning);
                timerSeconds = decreaseHealtVerySeconds;
            }
        }
    }
}