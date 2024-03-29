using _Scripts.InGameTime;
using UnityEngine;

namespace _Scripts.ml_agents.Agents.Handler
{
    public class AgentTimeHandler : MonoBehaviour
    {
        [Space(5)] 
        [Header("Age")] 
        public int ageInMonths;
        public int ageWhenAdult = 3;

        [Header("Reduction Per Month")] 
        public int hungerIncreasePerMonth = 20;
        public int thirstIncreasePerMonth = 20;

        [Header("Rewards")] 
        public int rewardSurvivingMonth = 2;

        private CustomAgent _agent;

        private void Start()
        {
            _agent = gameObject.GetComponent<CustomAgent>();
            gameObject.GetComponent<InteractionHandler>();

            TimeManager.OnMonthChanged += MonthOver;
            TimeManager.OnYearChanged += YearOver;
        }

        void MonthOver()
        {
            ageInMonths += 1;

            _agent.AddReward(rewardSurvivingMonth);

            _agent.hunger += hungerIncreasePerMonth;
            _agent.thirst += thirstIncreasePerMonth;

            if (!_agent.isAdult)
            {
                CheckAgeRestrictions();
            }
        }

        void YearOver()
        {
            //rabbit can build a new burrow every year
            _agent.hasBurrowBuild = false;
        }

        void CheckAgeRestrictions()
        {
            if (ageInMonths >= ageWhenAdult)
            {
                _agent.isAdult = true;
            }
        }
    }
}