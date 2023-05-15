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

        // private
        private Vector2Int _resolution;
        private float _terrainHeight;
        private float _waterLevel;
        private GeneralSettings _settings;
        private float[,] _map;

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
                                // var prefab = settings.assetPrefab[prngPrefab];

                                var max = _terrainHeight * _waterLevel;
                                var min = _terrainHeight * _waterLevel - 5f;

                                if (min < pos.y && pos.y < max)
                                {
                                    if (CheckSurrounding(pos, settings.radius))
                                    {
                                        var asset = Object.Instantiate(settings.assetPrefab[prngPrefab], pos,
                                            Quaternion.Euler(new Vector3(0, UnityEngine.Random.Range(0, 360), 0)));
                                        asset.transform.SetParent(Plants.PlantParents[settings]);

                                        settings.assets.Add(asset);

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
                                        var asset = Object.Instantiate(settings.assetPrefab[prngPrefab], pos,
                                            Quaternion.Euler(new Vector3(0, UnityEngine.Random.Range(0, 360), 0)));
                                        asset.transform.SetParent(Plants.PlantParents[settings]);

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

        private Vector3 GetPosition(float[,] map)
        {
            var xPos = UnityEngine.Random.Range(-_settings.squareSize * (_resolution.x - 1) / 2,
                _settings.squareSize * (_resolution.x - 1) / 2);
            var zPos = UnityEngine.Random.Range(-_settings.squareSize * (_resolution.y - 1) / 2,
                _settings.squareSize * (_resolution.y - 1) / 2);

            var pos = GeneratorFunctions.GetSurfacePointFromWorldCoordinate(xPos, zPos, _resolution,
                _terrainHeight, map, _settings);
            return pos;
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

                        while (!posFound)
                        {
                            var pos = GetPosition(map);

                            if (pos.y > (_terrainHeight * _waterLevel + 5f))
                            {
                                if (CheckSurrounding(pos, burrow.radius))
                                {
                                    var burrowAsset = Object.Instantiate(burrow.assetPrefab, pos,
                                        Quaternion.Euler(new Vector3(0, 0, 0)));

                                    burrowAsset.transform.SetParent(Burrows.BurrowParents[burrow]);

                                    burrow.assets.Add(burrowAsset);

                                    GameObject prefab = null;

                                    for (int j = 0; j < 2; j++)
                                    {
                                        if (burrow.assetPrefab.GetComponent<Burrow>().type == AgentType.Rabbit)
                                        {
                                            prefab = burrow.assetPrefab.GetComponent<Burrow>().rabbitPrefab;

                                            var position = burrowAsset.transform.GetChild(1).GetChild(5).position;

                                            var gameObject = Object.Instantiate(prefab, position,
                                                Quaternion.Euler(new Vector3(0, UnityEngine.Random.Range(0, 360), 0)),
                                                burrowAsset.transform);

                                            burrowAsset.GetComponent<Burrow>().inhabitants.Add(gameObject);

                                            gameObject.GetComponent<CustomAgent>().isInBurrow = true;
                                        }
                                        // else if (burrow.assetPrefab.GetComponent<Burrow>().type == AgentType.Fox)
                                        // {
                                        //     prefab = burrow.assetPrefab.GetComponent<Burrow>().foxPrefab;
                                        // }

                                        // var position = burrowAsset.transform.GetChild(1).GetChild(5).position;
                                        //
                                        // var gameObject = Object.Instantiate(prefab, position,
                                        //     Quaternion.Euler(new Vector3(0, UnityEngine.Random.Range(0, 360), 0)),
                                        //     burrowAsset.transform);
                                        //
                                        // burrowAsset.GetComponent<Burrow>().inhabitants.Add(gameObject);
                                        //
                                        // Debug.Log(burrowAsset.GetComponent<Burrow>().inhabitants.Count);
                                        // gameObject.GetComponent<CustomAgent>().isInBurrow = true;
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
                            asset.transform.SetParent(Plants.PlantParents[settings]);

                            settings.assets.Add(asset);

                            posFound = true;
                            // }
                        }
                    }
                    else
                    {
                        if (pos.y > _terrainHeight * _waterLevel)
                        {
                            if (CheckSurrounding(pos, settings.radius))
                            {
                                var asset = Object.Instantiate(settings.assetPrefab[prngPrefab], pos,
                                    Quaternion.Euler(new Vector3(0, UnityEngine.Random.Range(0, 360), 0)));

                                if (Plants.PlantParents.TryGetValue(settings, out var parent))
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
            var radius = Burrows.Burrow.radius;
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

            if (interactingObject.GetComponent<InteractionHandler>().isBurrowBuildableHere)
            {
                var settings = Burrows.BurrowsList[2].assetPrefab;

                var asset = Object.Instantiate(settings, interactingObject.transform.position, Quaternion.identity);

                asset.GetComponent<Burrow>().type = interactingObject.GetComponent<CustomAgent>().type;

                asset.transform.SetParent(Burrows.BurrowParents[Burrows.BurrowsList[2]]);

                Burrows.BurrowsList[2].assets.Add(asset);
            }
        }
    }
}