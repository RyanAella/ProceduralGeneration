using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InGameTime;

public class AgentTimeHandler : MonoBehaviour
{
    CustomAgent agent;
    InteractionHandler interaction;

    [Space(5)]
    [Header("Age")]
    public int ageInMonths = 0;
    public int ageWhenAdult = 4;

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
        agent.health -= 2;
        CheckHealthCondition();
    }

    void MonthOver()
    {
        ageInMonths += 1;

        if (!agent.isAdult)
        {
            CheckAgeRestrictions();
        }
    }

    void CheckHealthCondition()
    {
        if (agent.health < 50)
        {
            if (agent.health <= 0)
            {
                agent.Death();
            } else 
            {
                agent.AddReward(-(100 - agent.health) / agent.lowHealthPenaltyDivider);
            }
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
