using System.Collections.Generic;
using InGameTime;
using UnityEngine;
using WorldGeneration._Scripts.ScriptableObjects;

namespace WorldGeneration._Scripts.Spawning.TerrainAssets
{
    public class RabbitBurrow : MonoBehaviour
    {
        [SerializeField] private BurrowSettings settings;

        public List<GameObject> inhabitants;
        public bool isOccupied;

        private InGameDate _birthDate;
        private InGameDate _dayOfDeath;
        private TimeManager _timer;

        private void Start()
        {
            _timer = TimeManager.Instance;
            _birthDate = _timer.GetCurrentDate();
            settings.lifespan = new InGameDate().CalcDate(settings.lifespan);
            // Debug.Log("settings.lifespan: " + settings.lifespan.PrintToString("/"));
            _dayOfDeath = new InGameDate().CalcDate(_birthDate.AddDates(settings.lifespan));
            // Debug.Log("_dayOfDeath: " + _dayOfDeath.PrintToString("/"));
            isOccupied = false;
        }

        private void Update()
        {
            if (_dayOfDeath.Equals(_timer.GetCurrentDate())) Dying();
            if (inhabitants.Count >= 2) isOccupied = true;
        }

        private void Dying()
        {
            // Debug.Log("Burrow dies");
            settings.assets.Remove(gameObject);
            Destroy(gameObject);
        }

        public void Enter(GameObject interactingObject)
        {
            if (interactingObject.GetComponent<CustomAgent>().type == AgentType.RABBIT)
                if (!isOccupied /*_inhabitants.Count < 2*/)
                {
                    interactingObject.GetComponent<CustomAgent>().transform.position = new Vector3(
                        transform.GetChild(1).position.x, transform.GetChild(1).position.y,
                        transform.GetChild(1).position.z);
                    inhabitants.Add(interactingObject);
                }
        }

        public void Leave(GameObject interactingObject)
        {
            if (interactingObject.GetComponent<CustomAgent>().type == AgentType.RABBIT)
            {
                interactingObject.GetComponent<CustomAgent>().transform.position = new Vector3(
                    transform.GetChild(0).position.x, transform.GetChild(0).position.y,
                    transform.GetChild(0).position.z);

                inhabitants.Remove(interactingObject);
            }
        }
    }
}