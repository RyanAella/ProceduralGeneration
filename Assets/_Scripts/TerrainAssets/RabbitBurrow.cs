using _Scripts.ScriptableObjects;
using _Scripts.Time;
using UnityEngine;

namespace _Scripts.Assets
{
    public class RabbitBurrow : MonoBehaviour
    {
        private TimeManager _timer;
        private InGameDate _birthDate;

        // private bool run = true;

        [SerializeField] private BurrowSettings settings;
        
        void Start()
        {
            _timer = TimeManager.Instance;
            _birthDate = _timer.GetCurrentDate();
            settings.lifespan = new InGameDate().CalcDate(settings.lifespan);
            settings.isOccupied = true;
            // Debug.Log("RabbitBurrow: " + _birthDate.AddDates(settings.lifespan).ToString(":"));
        }

        void Update()
        {
            // if (run)
            // {
            //     Debug.Log(birthDate.ToString(":"));
            //     run = false;
            // }
        
            if (_birthDate.AddDates(settings.lifespan).Equals(_timer.GetCurrentDate()))
            {
                // Debug.Log(timer.GetCurrentDate().ToString(":"));
                // Debug.Log("Carrot Dies");
                Destroy(gameObject);
            }
        }
    }
}
