using UnityEngine;

namespace ml_agents.Agents.rabbit
{
    public class AgentRabbit : CustomAgent
    {
        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.L) && isInBurrow)
            {
                Interact(0, 0, 0, 1, 0, 0);
            }
            else if (Input.GetKeyDown(KeyCode.E) && !isInBurrow)
            {
                Interact(0, 0, 1, 0, 0, 0);
            }
        }
    }
}