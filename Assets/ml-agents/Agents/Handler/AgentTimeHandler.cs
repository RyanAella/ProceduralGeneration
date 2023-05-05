using InGameTime;
using UnityEngine;

namespace ml_agents.Agents.Handler
{
    public class AgentTimeHandler : MonoBehaviour
    {
        CustomAgent _agent;
        InteractionHandler _interaction;

        [Space(5)]
        [Header("Age")]
        public int ageInMonths = 0;
        public int ageWhenAdult = 4;

        [Header("Reduction Per Month")]
        public int healthReductionPerMonth = 20; // 4 carrots per month / 3min

        // Start is called before the first frame update
        void Start()
        {
            _agent = gameObject.GetComponent<CustomAgent>();
            _interaction = gameObject.GetComponent<InteractionHandler>();

            TimeManager.OnWeekChanged += WeekOver;
            TimeManager.OnMonthChanged += MonthOver;
        }

        void WeekOver()
        {
            CheckHealthCondition();
        }

        void MonthOver()
        {
            ageInMonths += 1;

            if (!_agent.isAdult)
            {
                CheckAgeRestrictions();
            }
            _agent.health -= healthReductionPerMonth;
        }

        void CheckHealthCondition()
        {
            if (_agent.health < 50)
            {
                if (_agent.health <= 0)
                {
                    _agent.Death();
                } else 
                {
                    _agent.AddReward(-(100 - _agent.health) / _agent.lowHealthPenaltyDivider);
                }
            }

            if (_agent.hunger >= 100 ||_agent.thirst >= 100)
            {
                _agent.Kill();
            }
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
