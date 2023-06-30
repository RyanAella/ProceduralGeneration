using _Scripts.InGameTime;
using TMPro;
using UnityEngine;

namespace _Scripts.UI
{
    public class Clock : MonoBehaviour
    {
        public TextMeshProUGUI dateText;
        public TimeManager timeManager;

        private void OnEnable()
        {
            TimeManager.OnDayChanged += UpdateDate;
        }

        private void OnDisable()
        {
            TimeManager.OnDayChanged -= UpdateDate;
        }

        private void UpdateDate()
        {
            dateText.text = TimeManager.PrintCurrentDate("/");
        }
    }
}