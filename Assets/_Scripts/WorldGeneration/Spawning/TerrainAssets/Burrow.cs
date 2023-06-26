using System;
using System.Collections.Generic;
using _Scripts.InGameTime;
using _Scripts.ml_agents.Agents;
using _Scripts.WorldGeneration.Helper;
using _Scripts.WorldGeneration.ScriptableObjects;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Scripts.WorldGeneration.Spawning.TerrainAssets
{
    public class Burrow : MonoBehaviour
    {
        [SerializeField] private BurrowSettings settings;
        public AgentType type;
        public LayerMask layerMask;

        public bool isInitialBurrow;
        public List<GameObject> inhabitants;
        public bool isOccupied;
        public GameObject rabbitPrefab;
        public GameObject foxPrefab;

        public List<GameObject> spawnPoints;

        private InGameDate _birthDate;

        private bool _canDie;
        private InGameDate _dayOfDeath;
        private int _lastSpawnPoint;
        private TimeManager _timer;

        private void Awake()
        {
            // Get the size and calculate the radius
            var size = gameObject.transform.GetChild(1).transform.GetChild(5).GetComponent<MeshRenderer>().bounds.size;
            settings.radius = size.x >= size.z ? size.x + 1f : size.z + 1f;

            inhabitants.Clear();
        }

        private void Start()
        {
            _timer = TimeManager.Instance;
            WorldManager.GetInstance();

            // Get the dates and check if they are in the correct form
            _birthDate = TimeManager.GetCurrentDate();
            settings.lifespan = new InGameDate().CalcDate(settings.lifespan);
            _dayOfDeath = new InGameDate().CalcDate(_birthDate.AddDates(settings.lifespan));

            _canDie = false;

            PositionCheck();

            if (inhabitants.Count >= 2)
                isOccupied = true;
            else if (inhabitants.Count < 2) isOccupied = false;

            TimeManager.OnDayChanged += CanDie;
        }

        private void Update()
        {
            if (inhabitants.Count >= 2)
                isOccupied = true;
            else if (inhabitants.Count < 2) isOccupied = false;
        }

        /// <summary>
        ///     Checks if the current date equals the day of death.
        ///     If it's equal or canDie is true, the burrow can be destroyed.
        /// </summary>
        private void CanDie()
        {
            if (_dayOfDeath.Equals(TimeManager.GetCurrentDate()) || _canDie) Dying();
        }

        /// <summary>
        ///     If the burrow is not occupied, it can be deleted and destroyed.
        ///     If the burrow is occupied, canDie is true.
        /// </summary>
        private void Dying()
        {
            if (!isOccupied)
            {
                if (settings.assets.Contains(gameObject)) settings.assets.Remove(gameObject);
                if (Checker.BurrowList.Contains(gameObject)) Checker.BurrowList.Remove(gameObject);
                if (gameObject != null) Destroy(gameObject);
            }

            _canDie = true;
        }

        /// <summary>
        ///     If agent type is equal to the burrow type it can enter the burrow.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="customAgent"></param>
        /// <param name="controller"></param>
        public void Enter(GameObject obj, CustomAgent customAgent, CharacterController controller)
        {
            // If the type matches and the burrow is not occupied the agent becomes a child of the burrow,
            // it gets teleported into the burrow keep,
            // isInBurrow is true and it gets added to the burrows inhabitants list
            if (customAgent.type != type) return;

            if (isOccupied) return;

            obj.transform.SetParent(transform);

            Teleport(obj, controller, new Vector3(
                transform.GetChild(1).position.x, transform.GetChild(1).position.y,
                transform.GetChild(1).position.z));

            customAgent.isInBurrow = true;

            inhabitants.Add(obj);
        }

        /// <summary>
        ///     The agent can leave the burrow.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="customAgent"></param>
        /// <param name="controller"></param>
        public void Leave(GameObject obj, CustomAgent customAgent, CharacterController controller)
        {
            // Every time an agent wants to leave the burrow another spawn point gets selected
            var prngSpawnPoints = (_lastSpawnPoint + 1) % spawnPoints.Count;

            // The agent gets removed from the burrows inhabitants list,
            // isInBurrow is false, the agent gets teleported to the selected spawn point,
            // it no longer is a child of the burrow.
            inhabitants.Remove(obj);

            customAgent.isInBurrow = false;

            var pos = spawnPoints[prngSpawnPoints].transform.position;
            pos.y -= 1;

            Teleport(obj, controller, pos);

            obj.transform.parent = null;

            // Check if the burrow is now unoccupied
            if (inhabitants.Count < 2) isOccupied = false;

            // The current spawn point now is the last spawn point
            _lastSpawnPoint = prngSpawnPoints;
        }

        /// <summary>
        ///     Teleport the given gameObject to a new position.
        /// </summary>
        /// <param name="agent">The agent gameObject which gets teleported</param>
        /// <param name="characterController">The CharacterController of the GameObject</param>
        /// <param name="position">The new position</param>
        private static void Teleport(GameObject agent, CharacterController characterController, Vector3 position)
        {
            // To teleport the GameObject, the CharacterController must be disabled
            characterController.enabled = false;
            agent.transform.position = position;
            characterController.enabled = true;
        }

        /// <summary>
        ///     If the agent is in the burrow it can breed.
        /// </summary>
        public void Breed(CustomAgent customAgent)
        {
            // The position of the burrow keep
            var pos = new Vector3(
                transform.GetChild(1).position.x, transform.GetChild(1).position.y,
                transform.GetChild(1).position.z);

            if (customAgent.type == AgentType.Rabbit)
            {
                // Instantiate a new rabbit
                var newRabbit = Instantiate(rabbitPrefab, pos,
                    Quaternion.Euler(new Vector3(0, Random.Range(0, 360), 0)));

                newRabbit.transform.SetParent(transform);

                inhabitants.Add(newRabbit);

                newRabbit.TryGetComponent(out CustomAgent agent);
                agent.isInBurrow = true;
            }
            else if (customAgent.type == AgentType.Fox)
            {
                // Instantiate a new fox
                var newFox = Instantiate(foxPrefab, pos,
                    Quaternion.Euler(new Vector3(0, Random.Range(0, 360), 0)));

                newFox.transform.SetParent(transform);

                inhabitants.Add(newFox);

                newFox.TryGetComponent(out CustomAgent agent);
                agent.isInBurrow = true;
            }
        }

        /// <summary>
        ///     Check the position of the burrow.
        /// </summary>
        private void PositionCheck()
        {
            var child0 = transform.GetChild(0).GetChild(0).transform.position;
            var child1 = transform.GetChild(0).GetChild(1).transform.position;
            var child2 = transform.GetChild(0).GetChild(2).transform.position;
            var child3 = transform.GetChild(0).GetChild(3).transform.position;
            var ray0 = new Ray(child0, -transform.GetChild(0).transform.up);
            var ray1 = new Ray(child1, -transform.GetChild(1).transform.up);
            var ray2 = new Ray(child2, -transform.GetChild(2).transform.up);
            var ray3 = new Ray(child3, -transform.GetChild(3).transform.up);

            Vector3 newVector;
            if (Physics.Raycast(ray0, out var hit0, 10f, layerMask))
            {
                newVector = hit0.normal.normalized;
                transform.GetChild(0).up = newVector;
            }

            if (Physics.Raycast(ray1, out var hit1, 10f, layerMask))
            {
                newVector = hit1.normal.normalized;
                transform.GetChild(0).up = newVector;
            }

            if (Physics.Raycast(ray2, out var hit2, 10f, layerMask))
            {
                newVector = hit2.normal.normalized;
                transform.GetChild(0).up = newVector;
            }

            if (Physics.Raycast(ray3, out var hit3, 10f, layerMask))
            {
                newVector = hit3.normal.normalized;
                transform.GetChild(0).up = newVector;
            }

            var max = Math.Max(hit0.distance, Math.Max(hit1.distance, Math.Max(hit2.distance, hit3.distance)));
            transform.GetChild(0).position = new Vector3(transform.GetChild(0).position.x,
                transform.GetChild(0).position.y - max, transform.GetChild(0).position.z);
        }

        /// <summary>
        ///     Instantiate the initial inhabitants.
        /// </summary>
        public void InstantiateInhabitants()
        {
            if (!isInitialBurrow) return;

            var position = transform.GetChild(1).position;

            // Instantiate inhabitants
            for (var j = 0; j < settings.initialInhabitants; j++)
            {
                GameObject inhabitant;
                switch (type)
                {
                    case AgentType.Rabbit:
                        inhabitant = Instantiate(rabbitPrefab, position,
                            Quaternion.Euler(new Vector3(0, Random.Range(0, 360), 0)),
                            transform);

                        inhabitants.Add(inhabitant);

                        inhabitant.GetComponent<CustomAgent>().isInBurrow = true;
                        break;
                    case AgentType.Fox:
                        inhabitant = Instantiate(foxPrefab, position,
                            Quaternion.Euler(new Vector3(0, Random.Range(0, 360), 0)),
                            transform);

                        inhabitants.Add(inhabitant);

                        inhabitant.GetComponent<CustomAgent>().isInBurrow = true;
                        break;
                }
            }
        }
    }
}