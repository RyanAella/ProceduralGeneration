using UnityEngine;

namespace _Scripts.ml_agents.Agents.Fox
{
    public class AgentFox : CustomAgent
    {
        void Start()
        {
            interaction.foodTag = "Rabbit";
        }

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