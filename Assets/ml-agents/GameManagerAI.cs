using System;
using System.Collections.Generic;
using ml_agents.Agents.rabbit;
using UnityEngine;

namespace ml_agents
{
    public class GameManagerAI : MonoBehaviour
    {
        public List<GameObject> rabbits = new List<GameObject>();

        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        void ResetScene()
        {
            foreach (GameObject rabbit in rabbits)
            {
                rabbit.GetComponent<AgentRabbit>().Kill();
            }

            GenerateEnvironment();
        }

        private void GenerateEnvironment()
        {
            throw new NotImplementedException();
        }
    }
}
