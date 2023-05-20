using InGameTime;
using ml_agents.Agents;
using UnityEngine;
using WorldGeneration._Scripts.ScriptableObjects;
using Random = UnityEngine.Random;

namespace WorldGeneration._Scripts.Spawning.TerrainAssets
{
    public class Carrot : MonoBehaviour
    {
        // [SerializeField]
        public PlantSettings settings;
        public float timeNeededForEating = 2f;
        public int nutritionValue = 5;

        // private
        private InGameDate _birthDate;
        private InGameDate _dayOfDeath;
        private InGameDate _fertilityAge;
        private bool _getsEaten;
        private TimeManager _timer;

        private void Awake()
        {
            var size = gameObject.transform.GetChild(0).GetComponent<MeshRenderer>().bounds.size;
            settings.radius = size.x >= size.z ? size.x + 1f : size.z + 1f;
        }

        private void Start()
        {
            _timer = TimeManager.Instance;
            _birthDate = _timer.GetCurrentDate();
            settings.lifespan = new InGameDate().CalcDate(settings.lifespan);
            _dayOfDeath = new InGameDate().CalcDate(_birthDate.AddDates(settings.lifespan));

            var days = new InGameDate().CalcDays(settings.lifespan);
            days.day = (int)((settings.percentageAge / 100) * days.day);
            _fertilityAge = new InGameDate().CalcDate(days);
        }

        private void Update()
        {
            if (_birthDate.AddDates(_fertilityAge).Equals(_timer.GetCurrentDate())) Reproduce();

            if (_dayOfDeath.Equals(_timer.GetCurrentDate())) Dying();
        }

        private void Reproduce()
        {
            var assetManager = AssetManager.GetInstance();

            if (Random.Range(0,100) == (int)settings.reproductionChance)
            {
                assetManager.Spawn(transform, settings);
            }
        }

        private void Dying()
        {
            settings.assets.Remove(gameObject);
            Destroy(gameObject);
        }

        public void Eat(GameObject interactingObject)
        {
            interactingObject.GetComponent<CustomAgent>().BlockMovementForSeconds(timeNeededForEating);

            // Move the object upward in world space 1 unit/second.
            transform.Translate(Vector3.up * Time.deltaTime, Space.Self);

            if (interactingObject.GetComponent<CustomAgent>().hunger >= nutritionValue)
            {
                interactingObject.GetComponent<CustomAgent>().hunger -= nutritionValue;
            }
            else
            {
                interactingObject.GetComponent<CustomAgent>().hunger = 0;
            }

            settings.assets.Remove(gameObject);
            Destroy(gameObject, 2f);
        }
        
        // private void OnDrawGizmos()
        // {
        //     Gizmos.color = Color.yellow;
        //     Gizmos.DrawWireSphere(transform.position, settings.radius);
        // }
    }
}