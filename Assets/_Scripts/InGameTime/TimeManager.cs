using System;
using System.Collections;
using UnityEngine;

namespace _Scripts.InGameTime
{
    /// <summary>
    ///     Struct <c>InGameDate</c> is representation of the inGame time in dd:ww:mm:yyyy
    /// </summary>
    [Serializable]
    public struct InGameDate
    {
        #region Variables
        
        public int day;
        public int month;
        public int year;
        
        #endregion

        public InGameDate(int day, int month, int year)
        {
            this.day = day;
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
                    return day.ToString("00") + " " /*+ week.ToString("00") + " "*/ + month.ToString("00") + " " +
                           year.ToString("0000");
                case ":":
                    return day.ToString("00") + ":" /*+ week.ToString("00") + ":"*/ + month.ToString("00") + ":" +
                           year.ToString("0000");
                case ";":
                    return day.ToString("00") + ";" /*+ week.ToString("00") + ";"*/ + month.ToString("00") + ";" +
                           year.ToString("0000");
                case ",":
                    return day.ToString("00") + "," /*+ week.ToString("00") + ","*/ + month.ToString("00") + "," +
                           year.ToString("0000");
                default:
                    return day.ToString("00") + "/" /*+ week.ToString("00") + "/"*/ + month.ToString("00") + "/" +
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
            if (date.day > 30)
            {
                date.month += date.day / 30;
                date.day %= 30;
            }

            if (date.month > 12)
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
                date.day += date.month * 30;
                date.month = 0;
            }

            return date;
        }
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
        #region Variables

        public static TimeManager Instance;
        public static event Action OnDayChanged;
        public static event Action OnMonthChanged;
        public static event Action OnYearChanged;

        private static InGameDate _inGameDate;
        
        // fixed time intervals for one second
        private readonly float _timeInterval = 0.5f;

        // private float timeScale = 1.0f;
        private float _lastTimeScale;

        private readonly float dayToRealTimeSeconds = 6.0f; // 6s are one day
        // private float _weekToRealTime = 42.0f; // 42s are one week
        // private float _monthToRealTime = 168.0f; // 168s are one month
        // private float _yearsToRealTime = 2016.0f; // 2016s are one year

        private float _timer;

        private bool _timerGoing;
        
        #endregion

        private void Awake()
        {
            if (Instance == null)
            {
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
                day = 1,
                month = 0,
                year = 0
            };
            _timer = dayToRealTimeSeconds;

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
            _timer *= 1 / _timeInterval;

            while (_timerGoing)
            {
                // wait for timeInterval -> two ticks per second
                yield return new WaitForSeconds(_timeInterval);
                _timer--;

                if (_timer <= 0)
                {
                    _inGameDate.day++;
                    OnDayChanged?.Invoke();

                    if (_inGameDate.day > 30)
                    {
                        _inGameDate.month++;
                        _inGameDate.day = 0;
                        OnMonthChanged?.Invoke();

                        if (_inGameDate.month > 12)
                        {
                            _inGameDate.year++;
                            _inGameDate.month = 0;
                            OnYearChanged?.Invoke();
                        }
                    }

                    // reset day timer
                    _timer = dayToRealTimeSeconds * (1 / _timeInterval);
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
        private void EndTimer()
        {
            _timerGoing = false;
        }

        /// <summary>
        ///     Pause the Game.
        /// </summary>
        public void Stop()
        {
            EndTimer();
            _lastTimeScale = Time.timeScale;
            Time.timeScale = 0.0f;
        }

        /// <summary>
        ///     Resume the InGameTimer.
        /// </summary>
        public void Resume()
        {
            BeginTimer();
            Time.timeScale = _lastTimeScale;
        }

        /// <summary>
        ///     Restart the InGameTimer.
        /// </summary>
        public void ResetTimer()
        {
            _inGameDate = new InGameDate
            {
                day = 1,
                // week = 0,
                month = 0,
                year = 0
            };
            _timer = dayToRealTimeSeconds;

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
        }

        /// <summary>
        ///     Get the current InGame date.
        /// </summary>
        /// <returns>The current InGameDate</returns>
        public static InGameDate GetCurrentDate()
        {
            return _inGameDate;
        }

        public static string PrintCurrentDate(string format)
        {
            return _inGameDate.PrintToString(format);
        }

        /// <summary>
        ///     The time an InGame day references in real life in seconds.
        /// </summary>
        /// <returns>The time in seconds</returns>
        public float GetDaysToRealtime()
        {
            return dayToRealTimeSeconds;
        }
    }
}