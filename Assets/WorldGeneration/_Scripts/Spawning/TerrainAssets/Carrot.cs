using Time;
using UnityEngine;
using WorldGeneration._Scripts.ScriptableObjects;

namespace WorldGeneration._Scripts.Spawning.TerrainAssets
{
    public class Carrot : MonoBehaviour
    {
        [SerializeField] private PlantSettings settings;
        
        private TimeManager _timer;
        private InGameDate _birthDate;

        void Start()
        {
            _timer = TimeManager.Instance;
            _birthDate = _timer.GetCurrentDate();
            settings.lifespan = new InGameDate().CalcDate(settings.lifespan);
        }

        void Update()
        {
            if (_birthDate.AddDates(settings.lifespan).Equals(_timer.GetCurrentDate()))
            {
                Debug.Log("Carrot Dies");
                Destroy(gameObject);
            }
        }
    }
}
