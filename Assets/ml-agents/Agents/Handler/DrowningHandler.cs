using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DrowningHandler : MonoBehaviour
{
    public Transform nose;
    public bool isDrowning = false;
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
        if(!isDrowning && nose.position.y < 0)
        {
            isDrowning = true;
            timerSeconds = decreaseHealtVerySeconds;
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
        timerSeconds -= Time.deltaTime;
        if (decreaseHealtVerySeconds <= 0.0f)
        {
            timerSeconds = decreaseHealtVerySeconds;
            agent.health -= healthDecreaseForDrowning;
        }
    }
}
