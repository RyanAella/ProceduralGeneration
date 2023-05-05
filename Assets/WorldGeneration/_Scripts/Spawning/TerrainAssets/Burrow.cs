using System.Collections.Generic;
using System.Reflection.Emit;
using InGameTime;
using ml_agents.Agents;
using UnityEngine;
using WorldGeneration._Scripts.ScriptableObjects;

namespace WorldGeneration._Scripts.Spawning.TerrainAssets
{
    public class Burrow : MonoBehaviour
    {
        [SerializeField] private BurrowSettings settings;
        public AgentType type;

        public List<GameObject> inhabitants;
        public bool isOccupied;
        public GameObject rabbitPrefab;
        public GameObject foxPrefab;

        private InGameDate _birthDate;
        private InGameDate _dayOfDeath;
        private TimeManager _timer;

        private bool canDie = false;

        private void Start()
        {
            _timer = TimeManager.Instance;
            _birthDate = _timer.GetCurrentDate();
            settings.lifespan = new InGameDate().CalcDate(settings.lifespan);
            _dayOfDeath = new InGameDate().CalcDate(_birthDate.AddDates(settings.lifespan));
            
            canDie = false;

            if (inhabitants.Count < 2)
            {
                isOccupied = false;
            }
            else
            {
                isOccupied = false;

            }

            
        }

        private void Update()
        {
            if (_dayOfDeath.Equals(_timer.GetCurrentDate()) || canDie)
            {
                Dying();
            }

            if (inhabitants.Count >= 2) isOccupied = true;
        }

        private void Dying()
        {
            if (!isOccupied)
            {
                // Debug.Log("Burrow dies");
                settings.assets.Remove(gameObject);
                Destroy(gameObject);
            }

            canDie = true;
        }


        public void Enter(GameObject rabbit)
        {
            if (rabbit.GetComponent<CustomAgent>().type == AgentType.RABBIT)
                if (!isOccupied /*_inhabitants.Count < 2*/)
                {
                    rabbit.GetComponent<CustomAgent>().transform.position = new Vector3(
                        transform.GetChild(1).position.x, transform.GetChild(1).position.y,
                        transform.GetChild(1).position.z);
                    
                    rabbit.transform.SetParent(transform);

                    inhabitants.Add(rabbit);
                    
                    rabbit.GetComponent<CustomAgent>().isInDen = true;
                }
        }

        public void Leave(GameObject rabbit)
        {
            if (rabbit.GetComponent<CustomAgent>().type == AgentType.RABBIT)
            {
                rabbit.GetComponent<CustomAgent>().transform.position = new Vector3(
                    transform.GetChild(0).position.x, transform.GetChild(0).position.y,
                    transform.GetChild(0).position.z);
                
                // rabbit.transform.parent = null;

                inhabitants.Remove(rabbit);
                
                // rabbit.GetComponent<CustomAgent>().isInDen = false;
            }
        }

        public void Breed(GameObject rabbit)
        {
            var pos = new Vector3(
                transform.GetChild(1).position.x, transform.GetChild(1).position.y,
                transform.GetChild(1).position.z);
            
            var newRabbit = Instantiate(rabbitPrefab, pos, Quaternion.Euler(new Vector3(0, Random.Range(0, 360), 0)));
            
            newRabbit.transform.SetParent(transform);
            
            inhabitants.Add(rabbit);
            
            rabbit.GetComponent<CustomAgent>().isInDen = true;
        }
    }
}