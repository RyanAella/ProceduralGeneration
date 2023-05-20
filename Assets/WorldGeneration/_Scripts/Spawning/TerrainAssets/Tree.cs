using InGameTime;
using UnityEngine;
using WorldGeneration._Scripts.ScriptableObjects;

namespace WorldGeneration._Scripts.Spawning.TerrainAssets
{
    public class Tree : MonoBehaviour
    {
        // [SerializeField]
        public PlantSettings settings;

        // private
        private InGameDate _birthDate;
        private InGameDate _dayOfDeath;
        private TimeManager _timer;

        private void Awake()
        {
            var size = gameObject.GetComponent<Renderer>().bounds.size;
            settings.radius = size.x >= size.z ? size.x : size.z;
        }

        private void Start()
        {
            _timer = TimeManager.Instance;
            _birthDate = _timer.GetCurrentDate();
            settings.lifespan = new InGameDate().CalcDate(settings.lifespan);
            _dayOfDeath = new InGameDate().CalcDate(_birthDate.AddDates(settings.lifespan));
        }

        private void Update()
        {
            if (_dayOfDeath.Equals(_timer.GetCurrentDate())) Dying();
        }

        private void Dying()
        {
            settings.assets.Remove(gameObject);
            Destroy(gameObject);
        }
    }
}