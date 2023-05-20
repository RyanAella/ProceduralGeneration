using System;
using System.Collections.Generic;
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
        public LayerMask layerMask;

        public List<GameObject> inhabitants;
        public bool isOccupied;
        public GameObject rabbitPrefab;
        public GameObject foxPrefab;

        public List<GameObject> spawnPoints;

        private InGameDate _birthDate;
        private InGameDate _dayOfDeath;
        private TimeManager _timer;
        private WorldManager _worldManager;

        private bool _canDie;
        private int _lastSpawnPoint;

        private void Awake()
        {
            var size = gameObject.transform.GetChild(0).GetComponent<MeshRenderer>().bounds.size;
            settings.radius = size.x >= size.z ? size.x + 1f : size.z + 1f;
            inhabitants.Clear();
        }

        private void Start()
        {
            _timer = TimeManager.Instance;
            _worldManager = WorldManager.GetInstance();
            
            _birthDate = _timer.GetCurrentDate();
            settings.lifespan = new InGameDate().CalcDate(settings.lifespan);
            _dayOfDeath = new InGameDate().CalcDate(_birthDate.AddDates(settings.lifespan));

            _canDie = false;

            if (inhabitants.Count < 2)
                isOccupied = false;
            else
                isOccupied = false;

            Check();
        }

        private void Update()
        {
            if (_dayOfDeath.Equals(_timer.GetCurrentDate()) || _canDie) Dying();

            if (inhabitants.Count >= 2)
                isOccupied = true;
            else if (inhabitants.Count < 2) isOccupied = false;
        }

        private void Dying()
        {
            if (!isOccupied)
            {
                settings.assets.Remove(gameObject);
                _worldManager.burrowList.Remove(gameObject);
                Destroy(gameObject);
            }

            _canDie = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="customAgent"></param>
        /// <param name="controller"></param>
        public void Enter(GameObject obj, CustomAgent customAgent, CharacterController controller)
        {
            if (customAgent.type == AgentType.Rabbit)
            {
                if (!isOccupied)
                {
                    obj.transform.SetParent(transform);

                    Teleport(obj, controller, new Vector3(
                        transform.GetChild(1).position.x, transform.GetChild(1).position.y,
                        transform.GetChild(1).position.z));

                    customAgent.isInBurrow = true;

                    inhabitants.Add(obj);

                    // Debug.Log("Enter Burrow");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="customAgent"></param>
        /// <param name="controller"></param>
        public void Leave(GameObject obj, CustomAgent customAgent, CharacterController controller)
        {
            var prngSpawnPoints = (_lastSpawnPoint + 1) % spawnPoints.Count;

            if (customAgent.type == AgentType.Rabbit)
            {
                inhabitants.Remove(obj);

                customAgent.isInBurrow = false;

                // var posFound = false;
                // while (!posFound)
                // {
                // var prngSpawnPoints = _prng.Next(spawnPoints.Count);
                // if (prngSpawnPoints != lastSpawnPoint)
                // {
                
                var pos = spawnPoints[prngSpawnPoints].transform.position;
                pos.y -= 1;

                Teleport(obj, controller, pos);

                obj.transform.parent = null;

                if (inhabitants.Count < 2) isOccupied = false;

                _lastSpawnPoint = prngSpawnPoints;
                
                //         posFound = true;
                //     }
                // }
            }
        }

        private void Teleport(GameObject obj, CharacterController controller, Vector3 pos)
        {
            controller.enabled = false;
            // yield return new WaitForSeconds(1f);
            obj.transform.position = pos;
            // yield return new WaitForSeconds(1f);
            controller.enabled = true;
        }

        public void Breed()
        {
            var pos = new Vector3(
                transform.GetChild(1).position.x, transform.GetChild(1).position.y,
                transform.GetChild(1).position.z);

            var newRabbit = Instantiate(rabbitPrefab, pos,
                Quaternion.Euler(new Vector3(0, UnityEngine.Random.Range(0, 360), 0)));

            newRabbit.transform.SetParent(transform);

            inhabitants.Add(newRabbit);

            newRabbit.TryGetComponent(out CustomAgent agent);
            agent.isInBurrow = true;
        }

        // private void OnDrawGizmos()
        // {
        //     Gizmos.color = Color.gray;
        //     Gizmos.DrawWireSphere(transform.position, settings.radius);
        // }

        private void Check()
        {
            var child0 = transform.GetChild(0).GetChild(0).transform.position;
            var child1 = transform.GetChild(0).GetChild(1).transform.position;
            var child2 = transform.GetChild(0).GetChild(2).transform.position;
            var child3 = transform.GetChild(0).GetChild(3).transform.position;
            Ray ray0 = new Ray(child0, -transform.GetChild(0).transform.up);
            Ray ray1 = new Ray(child1, -transform.GetChild(1).transform.up);
            Ray ray2 = new Ray(child2, -transform.GetChild(2).transform.up);
            Ray ray3 = new Ray(child3, -transform.GetChild(3).transform.up);

            Vector3 newVector;
            if (Physics.Raycast(ray0, out var hit0, 10f, layerMask))
            {
                newVector = (hit0.normal).normalized;
                transform.GetChild(0).up = newVector;
            }

            if (Physics.Raycast(ray1, out var hit1, 10f, layerMask))
            {
                newVector = (hit1.normal).normalized;
                transform.GetChild(0).up = newVector;
            }

            if (Physics.Raycast(ray2, out var hit2, 10f, layerMask))
            {
                newVector = (hit2.normal).normalized;
                transform.GetChild(0).up = newVector;
            }

            if (Physics.Raycast(ray3, out var hit3, 10f, layerMask))
            {
                newVector = (hit3.normal).normalized;
                transform.GetChild(0).up = newVector;
            }

            var max = Math.Max(hit0.distance, Math.Max(hit1.distance, Math.Max(hit2.distance, hit3.distance)));
            transform.GetChild(0).position = new Vector3(transform.GetChild(0).position.x,
                transform.GetChild(0).position.y - max, transform.GetChild(0).position.z);
        }
    }
}