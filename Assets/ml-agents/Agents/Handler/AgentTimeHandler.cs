using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InGameTime;
using ml_agents.Agents.Handler;

public class AgentTimeHandler : MonoBehaviour
{
    CustomAgent agent;
    InteractionHandler interaction;

    [Space(5)]
    [Header("Age")]
    public int ageInMonths = 0;
    public int ageWhenAdult = 4;

    [Header("Reduction Per Month")]
    public int healthReductionPerMonth = 20; // 4 carrots per month / 3min

    // Start is called before the first frame update
    void Start()
    {
        agent = gameObject.GetComponent<CustomAgent>();
        interaction = gameObject.GetComponent<InteractionHandler>();

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

        if (!agent.isAdult)
        {
            CheckAgeRestrictions();
        }
        agent.health -= healthReductionPerMonth;
    }

    void CheckHealthCondition()
    {
        if (agent.health < agent.unhealthyThreshhold)
        {
            agent.AddReward(-(100 - agent.health) / agent.lowHealthPenaltyDivider);
        }

        if (agent.hunger >= 100 ||agent.thirst >= 100)
        {
            agent.Kill();
        }
    }

    void CheckAgeRestrictions()
    {
        if (ageInMonths >= ageWhenAdult)
        {
            agent.isAdult = true;
        }
    }
}
