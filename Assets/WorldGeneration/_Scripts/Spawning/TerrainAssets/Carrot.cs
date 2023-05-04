using InGameTime;
using UnityEngine;
using WorldGeneration._Scripts.ScriptableObjects;

namespace WorldGeneration._Scripts.Spawning.TerrainAssets
{
    public class Carrot : MonoBehaviour
    {
        // [SerializeField]
        public AssetSettings settings;

        // private
        private InGameDate _birthDate;
        private InGameDate _dayOfDeath;
        private bool _getsEaten;
        private TimeManager _timer;

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
            // Debug.Log("Carrot dies");
            settings.assets.Remove(gameObject);
            Destroy(gameObject);
        }

        public void Eat(GameObject interactingObject)
        {
            if (interactingObject.GetComponent<CustomAgent>().type == AgentType.RABBIT)
            {
                interactingObject.GetComponent<CustomAgent>().canMove = false;

                // Debug.Log("Carrot dies");

                // Move the object upward in world space 1 unit/second.
                transform.Translate(Vector3.up * Time.deltaTime, Space.Self);

                settings.assets.Remove(gameObject);
                Destroy(gameObject, 2f);
            }
        }
    }
}