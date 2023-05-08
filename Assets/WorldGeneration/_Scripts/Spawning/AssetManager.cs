using System;
using System.Collections.Generic;
using System.Linq;
using ml_agents.Agents;
using ml_agents.Agents.Handler;
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

        // private
        private Vector2Int _resolution;
        private float _terrainHeight;
        private float _waterLevel;
        private GeneralSettings _settings;
        private float[,] _map;

        private Food _food;
        private Plants _plants;
        private Burrows _burrows;

        private Random _prng = new();

        private List<GameObject> _plantPrefabs;
        private GameObject _burrowPrefab;

        public static AssetManager GetInstance()
        {
            if (Instance == null)
            {
                Instance = new AssetManager();
                return Instance;
            }

            return Instance;
        }

        public bool InitialSpawnFood(Vector2Int res, float maxTerrainHeight, float waterLevel,
            GeneralSettings generalSettings, float[,] map, GameObject world, Food food)
        {
            _resolution = res;
            _terrainHeight = maxTerrainHeight;
            _waterLevel = waterLevel;
            _settings = generalSettings;
            _map = map;

            _food = food;

            if (_food.FoodList.Count != 0)
            {
                _food.FoodParents = new Dictionary<PlantSettings, Transform>();
                foreach (var settings in _food.FoodList)
                {
                    _food.FoodParents.Add(settings, Object.Instantiate(settings.parent, world.transform));
                    settings.assets = new List<GameObject>();

                    for (var i = 0; i < settings.minNumber; i++)
                    {
                        var posFound = false;

                        var prngPrefab = _prng.Next(settings.assetPrefab.Count);

                        while (!posFound)
                        {
                            var xPos = UnityEngine.Random.Range(-_settings.squareSize * (_resolution.x - 1) / 2,
                                _settings.squareSize * (_resolution.x - 1) / 2);
                            var zPos = UnityEngine.Random.Range(-_settings.squareSize * (_resolution.y - 1) / 2,
                                _settings.squareSize * (_resolution.y - 1) / 2);

                            var pos = GeneratorFunctions.GetSurfacePointFromWorldCoordinate(xPos, zPos, _resolution,
                                _terrainHeight, map, _settings);

                            if (settings.nearWater)
                            {
                                var prefab = settings.assetPrefab[prngPrefab];

                                var max = _terrainHeight * _waterLevel;
                                var min = _terrainHeight * _waterLevel - 5f;

                                if (min < pos.y && pos.y < max)
                                {
                                    if (CheckSurrounding(pos, settings.radius) == true)
                                    {
                                        var asset = Object.Instantiate(prefab, pos,
                                            Quaternion.Euler(new Vector3(0, UnityEngine.Random.Range(0, 360), 0)));
                                        asset.transform.SetParent(_food.FoodParents[settings]);

                                        settings.assets.Add(asset);

                                        posFound = true;
                                    }
                                }
                            }
                            else
                            {
                                if (pos.y > _terrainHeight * _waterLevel)
                                {
                                    if (CheckSurrounding(pos, settings.radius) == true)
                                    {
                                        var asset = Object.Instantiate(settings.assetPrefab[prngPrefab], pos,
                                            Quaternion.Euler(new Vector3(0, UnityEngine.Random.Range(0, 360), 0)));
                                        asset.transform.SetParent(_food.FoodParents[settings]);

                                        settings.assets.Add(asset);

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
        /// <param name="plants"></param>
        public bool InitialSpawnPlants(Vector2Int res, float maxTerrainHeight, float waterLevel,
            GeneralSettings generalSettings, float[,] map, GameObject world, Plants plants)
        {
            _resolution = res;
            _terrainHeight = maxTerrainHeight;
            _waterLevel = waterLevel;
            _settings = generalSettings;
            _map = map;

            _plants = plants;

            if (_plants.PlantsList.Count != 0)
            {
                _plants.PlantParents = new Dictionary<PlantSettings, Transform>();
                foreach (var settings in _plants.PlantsList)
                {
                    _plantPrefabs = settings.assetPrefab;

                    _plants.PlantParents.Add(settings, Object.Instantiate(settings.parent, world.transform));
                    settings.assets = new List<GameObject>();

                    for (var i = 0; i < settings.minNumber; i++)
                    {
                        var posFound = false;

                        var prngPrefab = _prng.Next(_plantPrefabs.Count);

                        while (!posFound)
                        {
                            var xPos = UnityEngine.Random.Range(-_settings.squareSize * (_resolution.x - 1) / 2,
                                _settings.squareSize * (_resolution.x - 1) / 2);
                            var zPos = UnityEngine.Random.Range(-_settings.squareSize * (_resolution.y - 1) / 2,
                                _settings.squareSize * (_resolution.y - 1) / 2);

                            var pos = GeneratorFunctions.GetSurfacePointFromWorldCoordinate(xPos, zPos, _resolution,
                                _terrainHeight, map, _settings);

                            if (settings.nearWater)
                            {
                                var prefab = settings.assetPrefab[prngPrefab];

                                var max = _terrainHeight * _waterLevel;
                                var min = _terrainHeight * _waterLevel - 5f;

                                if (min < pos.y && pos.y < max)
                                {
                                    if (CheckSurrounding(pos, settings.radius))
                                    {
                                        var asset = Object.Instantiate(prefab, pos,
                                            Quaternion.Euler(new Vector3(0, UnityEngine.Random.Range(0, 360), 0)));
                                        asset.transform.SetParent(_plants.PlantParents[settings]);

                                        settings.assets.Add(asset);

                                        posFound = true;
                                    }
                                }
                            }
                            else
                            {
                                if (pos.y > _terrainHeight * _waterLevel)
                                {
                                    if (CheckSurrounding(pos, settings.radius) == true)
                                    {
                                        var asset = Object.Instantiate(settings.assetPrefab[prngPrefab], pos,
                                            Quaternion.Euler(new Vector3(0, UnityEngine.Random.Range(0, 360), 0)));
                                        asset.transform.SetParent(_plants.PlantParents[settings]);

                                        settings.assets.Add(asset);

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

        public void SpawnPlants()
        {
            // _plants.PlantParents = new Dictionary<PlantSettings, Transform>();

            foreach (var plant in _plants.PlantsList.Where(plant => plant.assets.Count < plant.minNumber))
            {
                _plantPrefabs = plant.assetPrefab;

                var count = plant.minNumber - plant.assets.Count;

                for (var i = 0; i < count; i++)
                {
                    var posFound = false;

                    var prngPrefab = _prng.Next(_plantPrefabs.Count);

                    while (!posFound)
                    {
                        var xPos = UnityEngine.Random.Range(-_settings.squareSize * (_resolution.x - 1) / 2,
                            _settings.squareSize * (_resolution.x - 1) / 2);
                        var zPos = UnityEngine.Random.Range(-_settings.squareSize * (_resolution.y - 1) / 2,
                            _settings.squareSize * (_resolution.y - 1) / 2);

                        var pos = GeneratorFunctions.GetSurfacePointFromWorldCoordinate(xPos, zPos, _resolution,
                            _terrainHeight, _map, _settings);

                        if (plant.nearWater)
                        {
                            var max = _terrainHeight * _waterLevel;
                            var min = _terrainHeight * _waterLevel - 5f;

                            if (min < pos.y && pos.y < max)
                            {
                                if (CheckSurrounding(pos, plant.radius) == true)
                                {
                                    var asset = Object.Instantiate(plant.assetPrefab[prngPrefab], pos,
                                        Quaternion.Euler(new Vector3(0, UnityEngine.Random.Range(0, 360), 0)));
                                    asset.transform.SetParent(_plants.PlantParents[plant]);

                                    plant.assets.Add(asset);

                                    posFound = true;
                                }
                            }
                        }
                        else
                        {
                            if (pos.y > _terrainHeight * _waterLevel)
                            {
                                if (CheckSurrounding(pos, plant.radius) == true)
                                {
                                    var asset = Object.Instantiate(plant.assetPrefab[prngPrefab], pos,
                                        Quaternion.Euler(new Vector3(0, UnityEngine.Random.Range(0, 360), 0)));

                                    if (_plants.PlantParents.TryGetValue(plant, out var parent))
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
                    var xPos = UnityEngine.Random.Range(-_settings.squareSize * (_resolution.x - 1) / 2,
                        _settings.squareSize * (_resolution.x - 1) / 2);
                    var zPos = UnityEngine.Random.Range(-_settings.squareSize * (_resolution.y - 1) / 2,
                        _settings.squareSize * (_resolution.y - 1) / 2);

                    var pos = GeneratorFunctions.GetSurfacePointFromWorldCoordinate(xPos, zPos, _resolution,
                        _terrainHeight, _map, _settings);

                    if (settings.nearWater)
                    {
                        var max = _terrainHeight * _waterLevel;
                        var min = _terrainHeight * _waterLevel - 5f;

                        if (min < pos.y && pos.y < max)
                        {
                            // if (CheckSurrounding(pos, settings.radius) == true)
                            // {
                                var asset = Object.Instantiate(settings.assetPrefab[prngPrefab], pos,
                                    Quaternion.Euler(new Vector3(0, UnityEngine.Random.Range(0, 360), 0)));
                                asset.transform.SetParent(_plants.PlantParents[settings]);

                                settings.assets.Add(asset);

                                posFound = true;
                            // }
                        }
                    }
                    else
                    {
                        if (pos.y > _terrainHeight * _waterLevel)
                        {
                            if (CheckSurrounding(pos, settings.radius) == true)
                            {
                                var asset = Object.Instantiate(settings.assetPrefab[prngPrefab], pos,
                                    Quaternion.Euler(new Vector3(0, UnityEngine.Random.Range(0, 360), 0)));

                                if (_plants.PlantParents.TryGetValue(settings, out var parent))
                                {
                                    asset.transform.SetParent(parent);
                                }

                                settings.assets.Add(asset);

                                posFound = true;
                            }
                        }
                    }
                }
            }
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

            _burrows = burrows;

            if (_burrows.BurrowsList.Count != 0)
            {
                _burrows.BurrowParents = new Dictionary<BurrowSettings, Transform>();

                foreach (var burrow in burrows.BurrowsList)
                {
                    _burrows.BurrowParents.Add(burrow, Object.Instantiate(burrow.parent[0], world.transform));
                    burrow.assets = new List<GameObject>();

                    for (var i = 0; i < burrow.minNumber; i++)
                    {
                        var posFound = false;

                        while (!posFound)
                        {
                            var xPos = UnityEngine.Random.Range(-_settings.squareSize * (_resolution.x - 1) / 2,
                                _settings.squareSize * (_resolution.x - 1) / 2);
                            var zPos = UnityEngine.Random.Range(-_settings.squareSize * (_resolution.y - 1) / 2,
                                _settings.squareSize * (_resolution.y - 1) / 2);

                            var pos = GeneratorFunctions.GetSurfacePointFromWorldCoordinate(xPos, zPos, _resolution,
                                _terrainHeight, _map, _settings);


                            if (pos.y > (_terrainHeight * _waterLevel + 5f))
                            {
                                if (CheckSurrounding(pos, burrow.radius) == true)
                                {
                                    var asset = Object.Instantiate(burrow.assetPrefab, pos,
                                        Quaternion.Euler(new Vector3(0, 0, 0)));

                                    asset.transform.SetParent(_burrows.BurrowParents[burrow]);

                                    burrow.assets.Add(asset);

                                    var rabbits = burrow.assetPrefab.GetComponent<Burrow>().inhabitants;

                                    foreach (var rabbit in rabbits)
                                    {
                                        var rabbitPos = asset.transform.GetChild(1).position;
                                        Object.Instantiate(rabbit, rabbitPos,
                                            Quaternion.Euler(new Vector3(0, UnityEngine.Random.Range(0, 360), 0)),
                                            asset.transform).GetComponent<CustomAgent>().isInDen = true;
                                    }

                                    posFound = true;
                                }
                            }
                        }
                    }
                }
            }

            return true;
        }

        private bool CheckSurrounding(Vector3 position, float radius)
        {
            if (Physics.CheckSphere(position, radius))
            {
                return false;
            }
            
            return true;
        }

        public bool CheckLocation(Vector3 position)
        {
            var radius = _burrows.Burrow.radius;
            if (Physics.CheckSphere(position, radius))
            {
                return false;
            }
            
            return true;
        }

        /// <summary>
        /// </summary>
        /// <param name="interactingObject"></param>
        public void BuildBurrow(GameObject interactingObject)
        {
            // Transform vom Hasen
            // wenn pos frei, dann Burrow spawnen
            // wenn Burrow gespawnt, Hasen in Bau teleportieren

            if (interactingObject.GetComponent<InteractionHandler>().isDenBuildableHere)
            {
                var settings = _burrows.BurrowsList[2].assetPrefab;

                var asset = Object.Instantiate(settings, interactingObject.transform.position, Quaternion.identity);

                asset.GetComponent<Burrow>().type = interactingObject.GetComponent<CustomAgent>().type;

                asset.transform.SetParent(_burrows.BurrowParents[_burrows.BurrowsList[2]]);

                _burrows.BurrowsList[2].assets.Add(asset);
            }
        }
    }
}