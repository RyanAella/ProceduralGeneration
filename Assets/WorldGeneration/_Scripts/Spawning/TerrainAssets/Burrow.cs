using System.Collections.Generic;
using System.Reflection.Emit;
using InGameTime;
using ml_agents.Agents;
using ml_agents.Agents.rabbit;
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

        private void Awake()
        {
            var size = gameObject.transform.GetChild(0).GetComponent<MeshRenderer>().bounds.size;
            settings.radius = size.x >= size.z ? size.x + 1f : size.z + 1f;
            inhabitants.Clear();
        }

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

            if (inhabitants.Count >= 2)
            {
                isOccupied = true;
            }
            else if (inhabitants.Count < 2)
            {
                isOccupied = false;
            }
        }

        private void Dying()
        {
            if (!isOccupied)
            {
                Debug.Log("Burrow dies");
                settings.assets.Remove(gameObject);
                Destroy(gameObject);
            }

            canDie = true;
        }


        public void Enter(GameObject rabbit)
        {
            if (rabbit.GetComponent<CustomAgent>().type == AgentType.Rabbit)
            {
                Debug.Log("Enter Burrow");
                if (!isOccupied)
                {
                    rabbit.transform.SetParent(transform);

                    rabbit.GetComponent<CharacterController>().enabled = false;

                    rabbit.GetComponent<CustomAgent>().transform.position = new Vector3(
                        transform.GetChild(1).position.x, transform.GetChild(1).position.y,
                        transform.GetChild(1).position.z);

                    rabbit.GetComponent<CharacterController>().enabled = true;

                    rabbit.GetComponent<CustomAgent>().isInBurrow = true;

                    inhabitants.Add(rabbit);
                }
            }
        }

        public void Leave(GameObject rabbit)
        {
            if (rabbit.GetComponent<CustomAgent>().type == AgentType.Rabbit)
            {
                inhabitants.Remove(rabbit);

                rabbit.GetComponent<CustomAgent>().isInBurrow = false;

                var pos = rabbit.transform.position;

                rabbit.GetComponent<CharacterController>().enabled = false;

                rabbit.transform.position = new Vector3(pos.x - 5, transform.position.y + 5, pos.z - 5);

                rabbit.GetComponent<CharacterController>().enabled = true;

                rabbit.transform.parent = null;
                
                if (inhabitants.Count < 2)
                {
                    
                    isOccupied = false;
                }
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

            rabbit.GetComponent<CustomAgent>().isInBurrow = true;
        }

        // private void OnDrawGizmos()
        // {
        //     Gizmos.color = Color.gray;
        //     Gizmos.DrawWireSphere(transform.position, settings.radius);
        // }
    }
}