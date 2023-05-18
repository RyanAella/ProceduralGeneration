using System;
using System.Collections;
using UnityEngine;

namespace InGameTime
{
    /// <summary>
    ///     Struct <c>InGameDate</c> is representation of the inGame time in dd:ww:mm:yyyy
    /// </summary>
    [Serializable]
    public struct InGameDate
    {
        public int day;
        public int week;
        public int month;
        public int year;

        public InGameDate(int day, int week, int month, int year)
        {
            this.day = day;
            this.week = week;
            this.month = month;
            this.year = year;
        }

        /// <summary>
        /// </summary>
        /// <param name="format">Possible formats are: " ", ":", ";", "," and "/" as default.</param>
        /// <returns>a string representation of the InGameDate</returns>
        public string PrintToString(string format)
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

        /// <summary>
        ///     Add a date to the current InGame time.
        /// </summary>
        /// <param name="date">InGameDate that has to be added to the current date.</param>
        /// <returns>A new InGameDate</returns>
        public InGameDate AddDates(InGameDate date)
        {
            return new InGameDate
            {
                day = day + date.day,
                week = week + date.week,
                month = month + date.month,
                year = year + date.year
            };
        }

        /// <summary>
        ///     Subtract a date from the current InGame time.
        /// </summary>
        /// <param name="date">InGameDate that has to be subtracted from the current date.</param>
        /// <returns>A new InGameDate</returns>
        public InGameDate SubtractDates(InGameDate date)
        {
            return new InGameDate
            {
                day = day - date.day,
                week = week - date.week,
                month = month - date.month,
                year = year - date.year
            };
        }

        /// <summary>
        ///     Calculates the correct date.
        /// </summary>
        /// <param name="date">InGameDate that has to be calculated.</param>
        /// <returns>date - The correct date</returns>
        public InGameDate CalcDate(InGameDate date)
        {
            if (date.day >= 7)
            {
                date.week += date.day / 7;
                date.day %= 7;
            }

            if (date.week >= 4)
            {
                date.month += date.week / 4;
                date.week %= 4;
            }

            if (date.month >= 12)
            {
                date.year += date.month / 12;
                date.month %= 12;
            }

            return date;
        }
        
        public InGameDate CalcDays(InGameDate date)
        {
            if (date.year > 0)
            {
                date.month += date.year * 12;
                date.year = 0;
            }
            if (date.month > 0)
            {
                date.week += date.month * 4;
                date.month = 0;
            }
            if (date.week > 0)
            {
                date.day += date.week * 7;
                date.week = 0;
            }

            return date;
        }

        // /// <summary>
        // /// If days >= 7 the date needs to be recalculated.
        // /// </summary>
        // /// <param name="days">The number of days that need to be recalculated.</param>
        // /// <returns>A new InGameDate</returns>
        // public static InGameDate DaysToDate(int days)
        // {
        //     int weeks = days / 7;
        //     int months = weeks / 4;
        //
        //     return new InGameDate
        //     {
        //         day = days % 7,
        //         week = weeks % 4,
        //         month = months % 12,
        //         year = months / 12,
        //     };
        // }
        //
        // /// <summary>
        // /// If weeks >= 4 the date needs to be recalculated.
        // /// </summary>
        // /// <param name="weeks">The number of weeks that need to be recalculated.</param>
        // /// <returns>A new InGameDate</returns>
        // public static InGameDate WeeksToDate(int weeks)
        // {
        //     int months = weeks / 4;
        //
        //     return new InGameDate
        //     {
        //         week = weeks % 4,
        //         month = months % 12,
        //         year = months / 12,
        //     };
        // }
        //
        // /// <summary>
        // /// If months >= 12 the date needs to be recalculated.
        // /// </summary>
        // /// <param name="months">The number of days that need to be recalculated.</param>
        // /// <returns>A new InGameDate</returns>
        // public static InGameDate MonthsToDate(int months)
        // {
        //     return new InGameDate
        //     {
        //         month = months % 12,
        //         year = months / 12,
        //     };
        // }
    }

    /// <summary>
    ///     Class <c>TimeManager</c> (Singleton) manages the InGame time.
    ///     1 day = 6 sec.
    ///     1 week = 42 sec.
    ///     1 month = 168 sec.
    ///     1 year = 2016 sec.
    ///     To use it, you have to call BeginTimer().
    ///     It can be terminated with EndTimer().
    /// </summary>
    public class TimeManager : MonoBehaviour
    {
        public static TimeManager Instance;

        public static Action OnDayChanged;
        public static Action OnWeekChanged;
        public static Action OnMonthChanged;
        public static Action OnYearChanged;

        // fixed time intervals for one second
        [SerializeField] private float timeInterval = 0.5f;

        private readonly float _daysToRealTime = 6.0f; // 6s are one day

        private InGameDate _inGameDate;

        // private float timeScale = 1.0f;
        private float _lastTimeScale;
        // private float _weekToRealTime = 42.0f; // 42s are one week
        // private float _monthToRealTime = 168.0f; // 168s are one month
        // private float _yearsToRealTime = 2016.0f; // 2016s are one year

        private float _timer;

        private bool _timerGoing;

        private void Awake()
        {
            if (Instance == null)
            {
                // DontDestroyOnLoad(gameObject);
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
                year = 0
            };
            _timer = _daysToRealTime;

            // UnityEngine.Time.timeScale = 2f;
            _lastTimeScale = Time.timeScale;
            _timerGoing = false;
        }

        /** 
         * The countdown that manages the InGame time.
         */
        private IEnumerator UpdateTime()
        {
            // multiply timer with 1 / timeInterval
            _timer *= 1 / timeInterval;

            while (_timerGoing)
            {
                // wait for timeInterval -> two ticks per second
                yield return new WaitForSeconds(timeInterval);
                _timer--;

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
                }

                yield return null;
            }
        }

        /// <summary>
        ///     Start The InGame Timer.
        /// </summary>
        public void BeginTimer()
        {
            _timerGoing = true;
            StartCoroutine(UpdateTime());
        }

        /// <summary>
        ///     Stop the InGame Timer.
        /// </summary>
        public void EndTimer()
        {
            _timerGoing = false;
        }

        /// <summary>
        ///     Pause the Game.
        /// </summary>
        public void Stop()
        {
            // Debug.Log("Pause Game");
            EndTimer();
            _lastTimeScale = Time.timeScale;
            Time.timeScale = 0.0f;
        }

        /// <summary>
        ///     Resume the InGameTimer.
        /// </summary>
        public void Resume()
        {
            // Debug.Log("Resume Game");
            BeginTimer();
            Time.timeScale = _lastTimeScale;
        }
        
        /// <summary>
        ///     Restart the InGameTimer.
        /// </summary>
        public void ResetTimer()
        {
            // Debug.Log("Reset Timer");
            _inGameDate = new InGameDate
            {
                day = 0,
                week = 0,
                month = 0,
                year = 0
            };
            _timer = _daysToRealTime;

            // UnityEngine.Time.timeScale = 2f;
            _lastTimeScale = Time.timeScale;
            
            _timerGoing = false;
        }

        /// <summary>
        ///     Set the time scale.
        /// </summary>
        /// <param name="timeScale"></param>
        public void SetTimeScale(float timeScale)
        {
            _lastTimeScale = Time.timeScale;
            Time.timeScale = timeScale;
            // Debug.Log("timeScale: " + UnityEngine.Time.timeScale);
        }

        /// <summary>
        ///     Get the current InGame date.
        /// </summary>
        /// <returns>The current InGameDate</returns>
        public InGameDate GetCurrentDate()
        {
            return _inGameDate;
        }

        /// <summary>
        ///     The time an InGame day references in real life in seconds.
        /// </summary>
        /// <returns>The time in seconds</returns>
        public float GetDaysToRealtime()
        {
            return _daysToRealTime;
        }
    }
}