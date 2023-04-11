using System;
using System.Collections;
using UnityEngine;

namespace _Scripts.Time
{
    /// <summary>
    /// Struct <c>InGameDate</c> is representation of the inGame time in dd:ww:mm:yyyy
    /// </summary>
    [Serializable]
    public struct InGameDate
    {
        public int day;
        public int week;
        public int month;
        public int year;

        /** 
         * <param name="format"> Possible formats are: " ", ":", ";", "," and "/" as default.</param>
         * <returns> a string representation of the InGameDate</returns>
         */
        public String ToString(String format)
        {
            switch (format)
            {
                case " ":
                    return day.ToString("00") + " " + week.ToString("00") + " " + month.ToString("00") + " " +
                           year.ToString("0000");
                case ":":
                    return day.ToString("00") + ":" + week.ToString("00") + ":" + month.ToString("00") + ":" +
                           year.ToString("0000");
                case ";":
                    return day.ToString("00") + ";" + week.ToString("00") + ";" + month.ToString("00") + ";" +
                           year.ToString("0000");
                case ",":
                    return day.ToString("00") + "," + week.ToString("00") + "," + month.ToString("00") + "," +
                           year.ToString("0000");
                default:
                    return day.ToString("00") + "/" + week.ToString("00") + "/" + month.ToString("00") + "/" +
                           year.ToString("0000");
            }
        }

        /** 
         * Add a date to the current InGame time.
         * <param name="date">InGameDate that has to be added to the current date.</param>
         * <returns> A new InGameDate </returns>
         */
        public InGameDate AddDates(InGameDate date)
        {
            return new InGameDate
            {
                day = day + date.day,
                week = week + date.week,
                month = month + date.month,
                year = year + date.year,
            };
        }

        /** 
         * Calculates the correct date.
         * <param name="date">InGameDate that has to be calculated.</param>
         * <returns> date - The correct date </returns>
         */
        public InGameDate CalcDate(InGameDate date)
        {
            if (date.day >= 7)
            {
                date = DaysToDate(date.day);
            }
            else if (date.week >= 4)
            {
                date = WeeksToDate(date.week);
            }
            else if (date.month >= 12)
            {
                date = MonthsToDate(date.month);
            }

            return date;
        }

        /** 
         * If days >= 7 the date needs to be recalculated.
         * <param name="days">The number of days that need to be recalculated.</param>
         * <returns> A new InGameDate </returns>
         */
        public static InGameDate DaysToDate(int days)
        {
            int weeks = days / 7;
            int months = weeks / 4;

            return new InGameDate
            {
                day = days % 7,
                week = weeks % 4,
                month = months % 12,
                year = months / 12,
            };
        }

        /** 
         * If weeks >= 4 the date needs to be recalculated.
         * <param name="weeks">The number of weeks that need to be recalculated.</param>
         * <returns> A new InGameDate </returns>
         */
        public static InGameDate WeeksToDate(int weeks)
        {
            int months = weeks / 4;

            return new InGameDate
            {
                week = weeks % 4,
                month = months % 12,
                year = months / 12,
            };
        }

        /** 
         * If months >= 12 the date needs to be recalculated.
         * <param name="months">The number of days that need to be recalculated.</param>
         * <returns> A new InGameDate </returns>
         */
        public static InGameDate MonthsToDate(int months)
        {
            return new InGameDate
            {
                month = months % 12,
                year = months / 12,
            };
        }
    }

    /// <summary>
    /// Class <c>TimeManager</c> (Singleton) manages the InGame time.
    /// 1 day = 6 sec.
    /// 1 week = 42 sec.
    /// 1 month = 168 sec.
    /// 1 year = 2016 sec.
    /// </summary>
    public class TimeManager : MonoBehaviour
    {
        public static TimeManager Instance;
        
        // fixed time intervals for one second
        [SerializeField] private float timeInterval = 0.5f;
        
        public static Action OnDayChanged;
        public static Action OnWeekChanged;
        public static Action OnMonthChanged;
        public static Action OnYearChanged;

        private float _daysToRealTime = 6.0f; // 6s are one day
        // private float _weekToRealTime = 42.0f; // 42s are one week
        // private float _monthToRealTime = 168.0f; // 168s are one month
        // private float _yearsToRealTime = 2016.0f; // 2016s are one year
        
        private float _timer;

        // private float timeScale = 1.0f;
        private float _lastTimeScale;

        private InGameDate _inGameDate;

        private void Awake()
        {
            if (Instance == null)
            {
                DontDestroyOnLoad(gameObject);
                Instance = this;
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            _inGameDate = new InGameDate
            {
                day = 0,
                week = 0,
                month = 0,
                year = 0,
            };
            _timer = _daysToRealTime;

            // UnityEngine.Time.timeScale = 2f;
            _lastTimeScale = UnityEngine.Time.timeScale;
            StartCoroutine(Countdown());
        }

        /** 
         * The countdown that manages the InGame time.
         */
        private IEnumerator Countdown()
        {
            _timer *= 1 / timeInterval;
            // Debug.Log("Timer Value at Start of coroutine: " + _timer);

            while (true)
            {
                yield return new WaitForSeconds(timeInterval);
                _timer--;
                // Debug.Log("Timer Value: " + _timer);

                if (_timer <= 0)
                {
                    _inGameDate.day++;
                    OnDayChanged?.Invoke();

                    if (_inGameDate.day >= 7)
                    {
                        _inGameDate.week++;
                        _inGameDate.day = 0;
                        OnWeekChanged?.Invoke();

                        if (_inGameDate.week >= 4)
                        {
                            _inGameDate.month++;
                            _inGameDate.week = 0;
                            OnMonthChanged?.Invoke();

                            if (_inGameDate.month >= 12)
                            {
                                _inGameDate.year++;
                                _inGameDate.month = 0;
                                OnYearChanged?.Invoke();
                            }
                        }
                    }

                    // reset day timer
                    _timer = _daysToRealTime * (1 / timeInterval);
                    // Debug.Log("Timer Value after increment of days: " + _timer);
                }

                // Debug.Log(_inGameDate.ToString(":"));
            }
        }

        public void Stop()
        {
            // Debug.Log("Pause Game");
            _lastTimeScale = UnityEngine.Time.timeScale;
            UnityEngine.Time.timeScale = 0.0f;
        }

        public void Resume()
        {
            // Debug.Log("Resume Game");
            UnityEngine.Time.timeScale = _lastTimeScale;
        }

        public void SetTimeScale(float timeScale)
        {
            _lastTimeScale = UnityEngine.Time.timeScale;
            UnityEngine.Time.timeScale = timeScale;
            // Debug.Log("timeScale: " + UnityEngine.Time.timeScale);
        }

        /** 
         * <returns> The current InGameDate </returns>
         */
        public InGameDate GetCurrentDate()
        {
            return _inGameDate;
        }

        /** 
         * <returns> The time an InGame day references in real life in seconds </returns>
         */
        public float GetDaysToRealtime()
        {
            return _daysToRealTime;
        }
    }
}