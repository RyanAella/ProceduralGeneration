using System.Collections.Generic;
using InGameTime;
using UnityEngine;
using WorldGeneration._Scripts.ScriptableObjects;

namespace WorldGeneration._Scripts.Spawning.TerrainAssets
{
    public class RabbitBurrow : MonoBehaviour
    {
        [SerializeField] private BurrowSettings settings;
        private InGameDate _birthDate;
        private TimeManager _timer;
        private bool _canEnter;
        private List<GameObject> _inhabitants;

        private void Start()
        {
            _timer = TimeManager.Instance;
            _birthDate = _timer.GetCurrentDate();
            settings.lifespan = new InGameDate().CalcDate(settings.lifespan);
            settings.isOccupied = false;
        }

        private void Update()
        {
            if (_birthDate.AddDates(settings.lifespan).Equals(_timer.GetCurrentDate())) Destroy(gameObject);
        }
        
        public bool Interact(GameObject interactingObject)
        {
            if (interactingObject.GetComponent<CustomAgent>().type == AgentType.FOX)
            {
                _canEnter = false;
            }
            else if (interactingObject.GetComponent<CustomAgent>().type == AgentType.RABBIT)
            {
                if (_inhabitants.Count < 2)
                {
                    interactingObject.GetComponent<CustomAgent>().canMove = false;
                    _inhabitants.Add(interactingObject);

                    _canEnter = true;
                }
                else
                {
                    _canEnter = false;
                }
                
            }

            return _canEnter;
        }
    }
}