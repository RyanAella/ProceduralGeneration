using System;
using InGameTime;
using UnityEngine;
using WorldGeneration._Scripts.ScriptableObjects;

namespace WorldGeneration._Scripts.Spawning.TerrainAssets
{
    public class Carrot : MonoBehaviour, Interactables
    {
        // [SerializeField]
        public AssetSettings settings;
        
        // private
        private InGameDate _birthDate;
        private InGameDate _dayOfDeath;
        private TimeManager _timer;
        private bool _getsEaten;

        private void Start()
        {
            _timer = TimeManager.Instance;
            _birthDate = _timer.GetCurrentDate();
            settings.lifespan = new InGameDate().CalcDate(settings.lifespan);
            _dayOfDeath = new InGameDate().CalcDate(_birthDate.AddDates(settings.lifespan));
        }

        private void Update()
        {
            if (_dayOfDeath.Equals(_timer.GetCurrentDate()))
            {
                Debug.Log("Carrot dies");
                settings.assets.Remove(gameObject);
                Destroy(gameObject);
            }
        }

        public bool Interact(GameObject interactingObject)
        {
            if (interactingObject.GetComponent<CustomAgent>().type == AgentType.FOX)
            {
                _getsEaten = false;
            }
            else if (interactingObject.GetComponent<CustomAgent>().type == AgentType.RABBIT)
            {
                interactingObject.GetComponent<CustomAgent>().canMove = false;
                
                Debug.Log("Carrot dies");
                settings.assets.Remove(gameObject);
                Destroy(gameObject, 2f);

                _getsEaten = true;
            }

            return _getsEaten;
        }
    }
}