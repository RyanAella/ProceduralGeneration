using System;
using UnityEngine;

namespace _Scripts.Time
{
    public class TimeManager : MonoBehaviour
    {
        // public static Action OnMonthChanged;
        // public static Action OnYearChanged;
        
        public int Week { get; private set; }
        public int Month { get; private set; }
        public int Year { get; private set; }

        private float _weekToRealTime = 15.0f; // 15s are one week
        // private float _monthToRealTime = 60.0f; // 60s are one month
        private float _timer;
        // private float timeScale = 1.0f;
        private float _lastTimeScale;
        
        public static TimeManager Instance;

        private void Awake()
        {
            if (Instance == null)
            {
                DontDestroyOnLoad(gameObject);
                Instance = this;
            } else if (Instance != this)
            {
                Destroy(gameObject);
            }
        }
        
        void Start()
        {
            Week = 1;
            Month = 0;
            Year = 0;
            _timer = _weekToRealTime;
        }

        // Update is called once per frame
        private void Update()
        {
            _timer -= UnityEngine.Time.deltaTime;

            if (_timer <= 0)
            {
                Week++;
                // OnMonthChanged?.Invoke();
                
                if (Week >= 4)
                {
                    Month++;
                    Week = 0;
                    // OnYearChanged?.Invoke();
                    
                    if (Month >= 12)
                    {
                        Year++;
                        Month = 0;
                    }
                }

                _timer = _weekToRealTime;
            }
        }

        public void Stop()
        {
            _lastTimeScale = UnityEngine.Time.timeScale;
            UnityEngine.Time.timeScale = 0.0f;
        }

        public void Resume()
        {
            UnityEngine.Time.timeScale = _lastTimeScale;
            // lastTimeScale = 0.0f;
        }

        public void SetTimeScale(float timeScale)
        {
            _lastTimeScale = UnityEngine.Time.timeScale;
            UnityEngine.Time.timeScale = timeScale;
        }
    }
}
