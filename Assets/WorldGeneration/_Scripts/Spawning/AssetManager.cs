using System;
using System.Collections.Generic;
using System.Linq;
using ml_agents.Agents;
using ml_agents.Agents.Handler;
using Unity.VisualScripting;
using UnityEngine;
using WorldGeneration._Scripts.Helper;
using WorldGeneration._Scripts.ScriptableObjects;
using WorldGeneration._Scripts.Spawning.TerrainAssets;
using Object = UnityEngine.Object;
using Random = System.Random;

namespace WorldGeneration._Scripts.Spawning
{
    [Serializable]
    public class AssetManager
    {
        public static AssetManager Instance;
        public LayerMask layerMask;
        public Plants Plants;
        public Burrows Burrows;
        public List<BurrowSettings> initialBurrowSettings = new List<BurrowSettings>();

        // private
        private Vector2Int _resolution;
        private float _terrainHeight;
        private float _waterLevel;
        private GeneralSettings _settings;
        private float[,] _map;

        private Random _prng = new();

        private List<GameObject> _plantPrefabs;

        private WorldManager _worldManager = WorldManager.GetInstance();

        public static AssetManager GetInstance()
        {
            if (Instance == null)
            {
                Instance = new AssetManager();
                return Instance;
            }

            return Instance;
        }

        public static AssetManager ResetInstance()
        {
            Instance = new AssetManager();
            return Instance;
        }

        /// <summary>
        /// </summary>
        /// <param name="res"></param>
        /// <param name="maxTerrainHeight"></param>
        /// <param name="waterLevel"></param>
        /// <param name="generalSettings"></param>
        /// <param name="map"></param>
        /// <param name="world"></param>
        /// <param name="plants"></param>
        public bool InitialSpawnPlants(Vector2Int res, float maxTerrainHeight, float waterLevel,
            GeneralSettings generalSettings, float[,] map, GameObject world, Plants plants)
        {
            _resolution = res;
            _terrainHeight = maxTerrainHeight;
            _waterLevel = waterLevel;
            _settings = generalSettings;
            _map = map;

            Plants.PlantsList = new List<PlantSettings>();
            Plants.PlantParents = new Dictionary<PlantSettings, Transform>();
            Plants = plants;

            if (Plants.PlantsList.Count != 0)
            {
                Plants.PlantParents = new Dictionary<PlantSettings, Transform>();

                foreach (var settings in Plants.PlantsList)
                {
                    Plants.PlantParents.Add(settings, Object.Instantiate(settings.parent, world.transform));
                    settings.assets = new List<GameObject>();

                    for (var i = 0; i < settings.minNumber; i++)
                    {
                        var posFound = false;

                        var prngPrefab = _prng.Next(settings.assetPrefab.Count);

                        while (!posFound)
                        {
                            var pos = GetPosition(map);

                            if (settings.nearWater)
                            {
                                var max = _terrainHeight * _waterLevel;
                                var min = _terrainHeight * _waterLevel - 5f;

                                if (min < pos.y && pos.y < max)
                                {
                                    if (CheckSurrounding(pos, settings.radius))
                                    {
                                        InstantiatePlant(settings, prngPrefab, pos);

                                        posFound = true;
                                    }
                                }
                            }
                            else
                            {
                                if (pos.y > _terrainHeight * _waterLevel)
                                {
                                    if (CheckSurrounding(pos, settings.radius))
                                    {
                                        InstantiatePlant(settings, prngPrefab, pos);

                                        posFound = true;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// </summary>
        /// <param name="res"></param>
        /// <param name="maxTerrainHeight"></param>
        /// <param name="waterLevel"></param>
        /// <param name="generalSettings"></param>
        /// <param name="map"></param>
        /// <param name="world"></param>
        /// <param name="burrows"></param>
        public bool InitialSpawnBurrows(Vector2Int res, float maxTerrainHeight, float waterLevel,
            GeneralSettings generalSettings, float[,] map, GameObject world, Burrows burrows)
        {
            _resolution = res;
            _terrainHeight = maxTerrainHeight;
            _waterLevel = waterLevel;
            _settings = generalSettings;
            _map = map;

            // Burrows.BurrowsList = new List<BurrowSettings>();
            // Burrows.BurrowParents = new Dictionary<BurrowSettings, Transform>();

            initialBurrowSettings = burrows.BurrowsList;

            Burrows = burrows;

            if (Burrows.BurrowsList.Count != 0)
            {
                Burrows.BurrowParents = new Dictionary<BurrowSettings, Transform>();

                foreach (var burrow in burrows.BurrowsList)
                {
                    Burrows.BurrowParents.Add(burrow, Object.Instantiate(burrow.parent[0], world.transform));
                    burrow.assets = new List<GameObject>();

                    for (var i = 0; i < burrow.minNumber; i++)
                    {
                        var posFound = false;
                        GameObject burrowAsset = null;

                        while (!posFound)
                        {
                            var pos = GetPosition(map);

                            // Check position
                            if (pos.y > (_terrainHeight * _waterLevel + 5f))
                            {
                                if (CheckSurrounding(pos, burrow.radius))
                                {
                                    burrowAsset = InstantiateBurrow(burrow, pos);

                                    posFound = true;
                                }
                            }
                        }

                        InstantiateInhabitants(burrow, burrowAsset);
                    }
                }
            }

            return true;
        }


        public void SpawnPlants()
        {
            foreach (var plant in Plants.PlantsList.Where(plant => plant.assets.Count < plant.minNumber))
            {
                _plantPrefabs = plant.assetPrefab;

                var count = plant.minNumber - plant.assets.Count;

                for (var i = 0; i < count; i++)
                {
                    var posFound = false;

                    var prngPrefab = _prng.Next(_plantPrefabs.Count);

                    while (!posFound)
                    {
                        var pos = GetPosition(_map);

                        if (plant.nearWater)
                        {
                            var max = _terrainHeight * _waterLevel;
                            var min = _terrainHeight * _waterLevel - 5f;

                            if (min < pos.y && pos.y < max)
                            {
                                if (CheckSurrounding(pos, plant.radius))
                                {
                                    var asset = Object.Instantiate(plant.assetPrefab[prngPrefab], pos,
                                        Quaternion.Euler(new Vector3(0, UnityEngine.Random.Range(0, 360), 0)));
                                    asset.transform.SetParent(Plants.PlantParents[plant]);

                                    plant.assets.Add(asset);

                                    posFound = true;
                                }
                            }
                        }
                        else
                        {
                            if (pos.y > _terrainHeight * _waterLevel)
                            {
                                if (CheckSurrounding(pos, plant.radius))
                                {
                                    var asset = Object.Instantiate(plant.assetPrefab[prngPrefab], pos,
                                        Quaternion.Euler(new Vector3(0, UnityEngine.Random.Range(0, 360), 0)));

                                    if (Plants.PlantParents.TryGetValue(plant, out var parent))
                                    {
                                        asset.transform.SetParent(parent);
                                    }

                                    plant.assets.Add(asset);

                                    posFound = true;
                                }
                            }
                        }
                    }
                }
            }
        }

        public void Spawn(Transform mother, PlantSettings settings)
        {
            _plantPrefabs = settings.assetPrefab;

            var count = UnityEngine.Random.Range(1, 3);

            for (var i = 0; i < count; i++)
            {
                var posFound = false;

                var prngPrefab = _prng.Next(_plantPrefabs.Count);

                while (!posFound)
                {
                    var pos = GetPosition(_map);

                    if (settings.nearWater)
                    {
                        var max = _terrainHeight * _waterLevel;
                        var min = _terrainHeight * _waterLevel - 5f;

                        if (min < pos.y && pos.y < max)
                        {
                            if (CheckSurrounding(pos, settings.radius))
                            {
                                InstantiatePlant(settings, prngPrefab, pos);

                                posFound = true;
                            }
                        }
                    }
                    else
                    {
                        if (pos.y > _terrainHeight * _waterLevel)
                        {
                            if (CheckSurrounding(pos, settings.radius))
                            {
                                InstantiatePlant(settings, prngPrefab, pos);

                                posFound = true;
                            }
                        }
                    }
                }
            }
        }

        private GameObject InstantiateBurrow(BurrowSettings burrow, Vector3 pos)
        {
            var burrowAsset = Object.Instantiate(burrow.assetPrefab, pos,
                Quaternion.Euler(new Vector3(0, 0, 0)));

            burrowAsset.transform.SetParent(Burrows.BurrowParents[burrow]);
            _worldManager.burrowList.Add(burrowAsset);

            burrow.assets.Add(burrowAsset);
            return burrowAsset;
        }

        private void InstantiateInhabitants(BurrowSettings burrow, GameObject burrowAsset)
        {
            GameObject prefab;
            GameObject gameObject;
            var position = burrowAsset.transform.GetChild(1).GetChild(5).position;

            // Instantiate inhabitants
            for (int j = 0; j < burrow.initialInhabitants; j++)
            {
                if (burrow.assetPrefab.GetComponent<Burrow>().type == AgentType.Rabbit)
                {
                    prefab = burrow.assetPrefab.GetComponent<Burrow>().rabbitPrefab;

                    // var position = burrowAsset.transform.GetChild(1).GetChild(5).position;

                    gameObject = Object.Instantiate(prefab, position,
                        Quaternion.Euler(new Vector3(0, UnityEngine.Random.Range(0, 360), 0)),
                        burrowAsset.transform);

                    _worldManager.rabbitList.Add(gameObject);
                    burrowAsset.GetComponent<Burrow>().inhabitants.Add(gameObject);

                    gameObject.GetComponent<CustomAgent>().isInBurrow = true;
                }
                else if (burrow.assetPrefab.GetComponent<Burrow>().type == AgentType.Fox)
                {
                    prefab = burrow.assetPrefab.GetComponent<Burrow>().foxPrefab;

                    gameObject = Object.Instantiate(prefab, position,
                        Quaternion.Euler(new Vector3(0, UnityEngine.Random.Range(0, 360), 0)),
                        burrowAsset.transform);

                    _worldManager.foxList.Add(gameObject);
                    burrowAsset.GetComponent<Burrow>().inhabitants.Add(gameObject);

                    gameObject.GetComponent<CustomAgent>().isInBurrow = true;
                }
            }
        }

        private void InstantiatePlant(PlantSettings settings, int prngPrefab, Vector3 pos)
        {
            var asset = Object.Instantiate(settings.assetPrefab[prngPrefab], pos,
                Quaternion.Euler(new Vector3(0, UnityEngine.Random.Range(0, 360), 0)));
            asset.transform.SetParent(Plants.PlantParents[settings]);

            settings.assets.Add(asset);
        }

        private Vector3 GetPosition(float[,] map)
        {
            var xPos = UnityEngine.Random.Range(-_settings.squareSize * (_resolution.x - 2) / 2,
                _settings.squareSize * (_resolution.x - 2) / 2);
            var zPos = UnityEngine.Random.Range(-_settings.squareSize * (_resolution.y - 2) / 2,
                _settings.squareSize * (_resolution.y - 2) / 2);

            var pos = GeneratorFunctions.GetSurfacePointFromWorldCoordinate(xPos, zPos, _resolution,
                _terrainHeight, map, _settings);
            return pos;
        }

        private bool CheckSurrounding(Vector3 position, float radius)
        {
            if (Physics.CheckSphere(position, radius))
            {
                return false;
            }

            return true;
        }

        public bool CheckLocation(Vector3 position, AgentType type)
        {
            foreach (var burrow in initialBurrowSettings)
            {
                burrow.assetPrefab.TryGetComponent<Burrow>(out var _burrow);
                
                if (type == _burrow.type)
                {
                    var radius = burrow.radius;
                    if (Physics.CheckSphere(position, radius))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// </summary>
        /// <param name="interactingObject"></param>
        /// <param name="handler"></param>
        /// <param name="agent"></param>
        public void BuildBurrow(GameObject interactingObject, InteractionHandler handler, CustomAgent agent)
        {
            if (handler.isBurrowBuildableHere)
            {
                var settings = Burrows.BurrowsList[2].assetPrefab;

                var burrowAsset =
                    Object.Instantiate(settings, interactingObject.transform.position, Quaternion.identity);

                burrowAsset.TryGetComponent(out Burrow burrow);
                burrow.type = agent.type;

                if (burrow.type == AgentType.Rabbit)
                {
                    burrow.transform.GetChild(0).localScale = new Vector3(1, 0.4f, 1);
                    burrow.transform.GetChild(1).localScale = new Vector3(3, 3, 3);
                }
                else if (burrow.type == AgentType.Fox)
                {
                    burrow.transform.GetChild(0).localScale = new Vector3(2, 0.8f, 2);
                    burrow.transform.GetChild(1).localScale = new Vector3(5, 5, 5);
                }

                burrowAsset.transform.SetParent(Burrows.BurrowParents[Burrows.BurrowsList[2]]);

                Burrows.BurrowsList[2].assets.Add(burrowAsset);

                _worldManager.burrowList.Add(burrowAsset);
            }
        }
    }
}