using InGameTime;
using UnityEngine;
using WorldGeneration._Scripts.ScriptableObjects;

namespace WorldGeneration._Scripts.Spawning.TerrainAssets
{
    public class Carrot : MonoBehaviour
    {
        [SerializeField] private PlantSettings settings;
        private InGameDate _birthDate;
        private TimeManager _timer;

        private void Start()
        {
            _timer = TimeManager.Instance;
            _birthDate = _timer.GetCurrentDate();
            settings.lifespan = new InGameDate().CalcDate(settings.lifespan);
        }

        private void Update()
        {
            if (_birthDate.AddDates(settings.lifespan).Equals(_timer.GetCurrentDate())) Destroy(gameObject);
        }
    }
}