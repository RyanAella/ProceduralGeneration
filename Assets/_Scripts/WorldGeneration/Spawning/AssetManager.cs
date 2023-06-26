/*
* Copyright (c) mmq
*/

using System;
using System.Collections.Generic;
using _Scripts.ml_agents.Agents;
using _Scripts.ml_agents.Agents.Handler;
using _Scripts.WorldGeneration.Helper;
using _Scripts.WorldGeneration.ScriptableObjects;
using _Scripts.WorldGeneration.Spawning.TerrainAssets;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = System.Random;

namespace _Scripts.WorldGeneration.Spawning
{
    [Serializable]
    public class AssetManager
    {
        #region Variables

        //public
        public static AssetManager Instance;
        public Plants Plants;
        public Burrows Burrows;

        // private
        private Vector2Int _resolution;
        private float _terrainHeight;
        private float _waterLevel;
        private float _mountainArea;
        private GeneralSettings _settings;
        private float[,] _map;
        private List<BurrowSettings> _initialBurrowSettings = new();

        private Random _prng = new();

        #endregion

        #region Unit Methods

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

        #region Plants

        /// <summary>
        ///     Spawn the initial plants.
        /// </summary>
        /// <param name="resolution">The x and y resolution as Vector2</param>
        /// <param name="maxTerrainHeight">The maximum terrain height as float</param>
        /// <param name="waterLevel">The height of the water level as a percentage of the maximum terrain height in float</param>
        /// <param name="mountainArea">The height of the mountain area as a percentage of the maximum terrain height in float</param>
        /// <param name="generalSettings">The general settings of the simulation</param>
        /// <param name="map">The heightmap</param>
        /// <param name="world">The parent gameObject</param>
        /// <param name="plants">All the information about the plants</param>
        /// <returns>true - if all was successful</returns>
        public bool InitialSpawnPlants(Vector2Int resolution, float maxTerrainHeight, float waterLevel,
            float mountainArea,
            GeneralSettings generalSettings, float[,] map, GameObject world, Plants plants)
        {
            // Save for later use
            _resolution = resolution;
            _terrainHeight = maxTerrainHeight;
            _waterLevel = waterLevel;
            _mountainArea = mountainArea;
            _settings = generalSettings;
            _map = map;

            Plants.PlantsList = new List<PlantSettings>();
            Plants.PlantParents = new Dictionary<PlantSettings, Transform>();
            Plants = plants;

            if (Plants.PlantsList.Count == 0) return true;

            Plants.PlantParents = new Dictionary<PlantSettings, Transform>();

            foreach (var settings in Plants.PlantsList)
            {
                // Instantiate and add the parent object
                Plants.PlantParents.Add(settings, Object.Instantiate(settings.parent, world.transform));
                settings.assets = new List<GameObject>();

                for (var i = 0; i < settings.maxNumber; i++)
                {
                    var posFound = false;

                    var prngPrefab = _prng.Next(settings.assetPrefab.Count);

                    // Ends if posFound == true or counter == numberOfPlacementAttempts
                    var counter = 0;
                    while (!posFound)
                    {
                        if (counter < settings.numberOfPlacementAttempts)
                        {
                            var pos = GetPosition(map);

                            posFound = CheckPosition(settings, pos, prngPrefab);
                        }
                        else
                        {
                            break;
                        }

                        counter++;
                    }
                }
            }

            return true;
        }

        /// <summary>
        ///     Spawn plants if the current number is less than the number given in the settings.
        /// </summary>
        /// <param name="map">The heightmap</param>
        /// <param name="plants">All the information about the plants</param>
        public void SpawnPlants(float[,] map, Plants plants)
        {
            // Check for carrots
            foreach (var plant in plants.PlantsList)
            {
                if (!plant.name.Contains("Carrot")) continue;

                // If not enough objects
                var count = plant.maxNumber - plant.assets.Count;

                for (var i = 0; i < count; i++)
                {
                    var posFound = false;

                    var prngPrefab = _prng.Next(plant.assetPrefab.Count);

                    // Ends if posFound == true or counter == numberOfPlacementAttempts
                    var counter = 0;
                    while (!posFound)
                    {
                        if (counter < plant.numberOfPlacementAttempts)
                        {
                            var pos = GetPosition(map);
                            pos.y -= 0.5f;

                            posFound = CheckPosition(plant, pos, prngPrefab);
                        }
                        else
                        {
                            break;
                        }

                        counter++;
                    }
                }
            }
        }

        /// <summary>
        ///     Spawn plants if the mother plant can respawn.
        /// </summary>
        /// <param name="settings">The settings of the plant which needs to be spawned</param>
        /// <param name="parent">The parent gameObject</param>
        public void Spawn(PlantSettings settings, Transform parent)
        {
            // The number of plants to spawn
            var spawnCounter = UnityEngine.Random.Range(1, 3);
            for (var i = 0; i < spawnCounter; i++)
            {
                var posFound = false;

                var prngPrefab = _prng.Next(settings.assetPrefab.Count);

                // Ends if posFound == true or counter == numberOfPlacementAttempts
                var counter = 0;
                while (!posFound)
                {
                    if (counter < settings.numberOfPlacementAttempts)
                        posFound = CheckPosition(settings, GetPosition(_map), prngPrefab, parent);
                    else
                        break;

                    counter++;
                }
            }
        }

        /// <summary>
        ///     Instantiate the plant.
        /// </summary>
        /// <param name="settings">The settings of the plant which needs to be spawned</param>
        /// <param name="prngPrefab">The plant prefab</param>
        /// <param name="pos">The position at which the plant has to be spawned</param>
        /// <param name="parent">optional parameter</param>
        private void InstantiatePlant(PlantSettings settings, int prngPrefab, Vector3 pos, Transform parent = null)
        {
            var asset = Object.Instantiate(settings.assetPrefab[prngPrefab], pos,
                Quaternion.Euler(new Vector3(0, 0, 0)));

            asset.transform.SetParent(parent == null ? Plants.PlantParents[settings] : parent);

            settings.assets.Add(asset);
        }

        #endregion

        #region Burrows

        /// <summary>
        ///     Spawn the initial burrows.
        /// </summary>
        /// <param name="resolution">The x and y resolution as Vector2</param>
        /// <param name="maxTerrainHeight">The maximum terrain height as float</param>
        /// <param name="waterLevel">The height of the water level as a percentage of the maximum terrain height in float</param>
        /// <param name="generalSettings">The general settings of the simulation</param>
        /// <param name="map">The heightmap</param>
        /// <param name="world">The parent gameObject</param>
        /// <param name="burrows">All the information about the burrows</param>
        public bool InitialSpawnBurrows(Vector2Int resolution, float maxTerrainHeight, float waterLevel,
            GeneralSettings generalSettings, float[,] map, GameObject world, Burrows burrows)
        {
            // save for later use
            _resolution = resolution;
            _terrainHeight = maxTerrainHeight;
            _waterLevel = waterLevel;
            _settings = generalSettings;
            _map = map;

            _initialBurrowSettings = burrows.BurrowsList;

            Burrows = burrows;

            if (Burrows.BurrowsList.Count == 0) return true;

            Burrows.BurrowParents = new Dictionary<BurrowSettings, Transform>();

            foreach (var burrow in burrows.BurrowsList)
            {
                // Instantiate and add the parent object
                Burrows.BurrowParents.Add(burrow, Object.Instantiate(burrow.parent[0], world.transform));
                burrow.assets = new List<GameObject>();

                for (var i = 0; i < burrow.maxNumber; i++)
                {
                    var posFound = false;

                    // Ends if posFound == true or counter == numberOfPlacementAttempts
                    var counter = 0;
                    while (!posFound)
                    {
                        if (counter < burrow.numberOfPlacementAttempts)
                        {
                            var pos = GetPosition(map);

                            // Check position
                            if (pos.y > _terrainHeight * _waterLevel + 5f && pos.y < _terrainHeight * 0.75f)
                                if (CheckSurrounding(pos, burrow.radius))
                                {
                                    InstantiateBurrow(burrow, burrow.burrowPrefabs[0], pos);

                                    posFound = true;
                                }
                        }
                        else
                        {
                            break;
                        }

                        counter++;
                    }
                }
            }

            return true;
        }

        /// <summary>
        ///     Instantiate a burrow.
        /// </summary>
        /// <param name="burrow"></param>
        /// <param name="burrowPrefab"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        private GameObject InstantiateBurrow(BurrowSettings burrow, GameObject burrowPrefab, Vector3 pos)
        {
            var burrowAsset = Object.Instantiate(burrowPrefab, pos, Quaternion.Euler(new Vector3(0, 360, 0)));

            // Check if water plane goes through burrow child
            if (pos.y >= _terrainHeight * _waterLevel + 2.4f)
            {
                var diff = pos.y - _terrainHeight * _waterLevel;

                var position = burrowAsset.transform.GetChild(1).position;
                position.y -= diff;
                burrowAsset.transform.GetChild(1).position = position;
            }

            burrowAsset.transform.SetParent(Burrows.BurrowParents[burrow]);
            Checker.BurrowList.Add(burrowAsset);

            burrow.assets.Add(burrowAsset);
            return burrowAsset;
        }

        /// <summary>
        ///     Build a burrow based on the agent type.
        /// </summary>
        /// <param name="interactingObject">The gameObject which wants to build a burrow</param>
        /// <param name="handler">The InteractionHandler of the gameObject which wants to build</param>
        /// <param name="agent">The agent od the gameObject which wants to build</param>
        public void BuildBurrow(GameObject interactingObject, InteractionHandler handler, CustomAgent agent)
        {
            if (!handler.isBurrowBuildableHere) return;

            GameObject burrowAssetPrefab;

            switch (agent.type)
            {
                case AgentType.Rabbit:
                {
                    burrowAssetPrefab = Burrows.Burrow.burrowPrefabs[0];
                    var burrowAsset = Object.Instantiate(burrowAssetPrefab, interactingObject.transform.position,
                        Quaternion.identity);

                    burrowAsset.TryGetComponent(out Burrow burrow);
                    burrow.type = agent.type;

                    burrowAsset.transform.SetParent(Burrows.BurrowParents[Burrows.BurrowsList[0]]);

                    Burrows.Burrow.assets.Add(burrowAsset);

                    Checker.BurrowList.Add(burrowAsset);
                    break;
                }
                case AgentType.Fox:
                {
                    burrowAssetPrefab = Burrows.Burrow.burrowPrefabs[1];
                    var burrowAsset = Object.Instantiate(burrowAssetPrefab, interactingObject.transform.position,
                        Quaternion.identity);

                    burrowAsset.TryGetComponent(out Burrow burrow);
                    burrow.type = agent.type;

                    burrowAsset.transform.SetParent(Burrows.BurrowParents[Burrows.BurrowsList[1]]);

                    Burrows.Burrow.assets.Add(burrowAsset);

                    Checker.BurrowList.Add(burrowAsset);
                    break;
                }
            }
        }

        #endregion

        #region Helper

        /// <summary>
        ///     Get the position on the mesh.
        /// </summary>
        /// <param name="map">The heightmap</param>
        /// <returns>The position as a Vector3</returns>
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

        /// <summary>
        ///     Check the give position.
        /// </summary>
        /// <param name="settings">The settings of the plant</param>
        /// <param name="pos">The position at which the plant needs to be spawned</param>
        /// <param name="prngPrefab">The plant prefab</param>
        /// <param name="parent">optional parameter</param>
        /// <returns>true - if all was successful</returns>
        private bool CheckPosition(PlantSettings settings, Vector3 pos, int prngPrefab, Transform parent = null)
        {
            var posFound = false;

            if (settings.nearWater)
            {
                var max = _terrainHeight * _waterLevel + 0.25f;
                var min = _terrainHeight * _waterLevel - 5.25f;

                if (min <= pos.y && pos.y <= max)
                    if (CheckSurrounding(pos, settings.radius))
                    {
                        InstantiatePlant(settings, prngPrefab, pos, parent);

                        posFound = true;
                    }
            }
            else
            {
                if (settings.inMountainArea)
                {
                    if (pos.y > _terrainHeight * _mountainArea)
                        if (CheckSurrounding(pos, settings.radius))
                        {
                            InstantiatePlant(settings, prngPrefab, pos);

                            posFound = true;
                        }
                }
                else
                {
                    if (pos.y > _terrainHeight * _waterLevel + 2.5f && pos.y < _terrainHeight * _mountainArea)
                    {
                        if (settings.name.Equals("GrassSettings"))
                        {
                            var hitColliders = Physics.OverlapSphere(pos, settings.radius, settings.layerMask);
                            for (var i = 0; i < hitColliders.Length; i++)
                            {
                                if (!CheckSurrounding(pos, settings.radius)) continue;

                                InstantiatePlant(settings, prngPrefab, pos);

                                posFound = true;
                            }
                        }

                        if (!CheckSurrounding(pos, settings.radius)) return posFound;

                        InstantiatePlant(settings, prngPrefab, pos);

                        posFound = true;
                    }
                }
            }

            return posFound;
        }

        /// <summary>
        ///     Check the surrounding of the current position.
        /// </summary>
        /// <param name="position">The position at which the surrounding needs to be checked</param>
        /// <param name="radius">The radius of the gameObject</param>
        /// <returns>true - if all was successful</returns>
        private bool CheckSurrounding(Vector3 position, float radius)
        {
            return !Physics.CheckSphere(position, radius);
        }

        /// <summary>
        ///     Check if the location is clear.
        /// </summary>
        /// <param name="position">The position at which the location needs to be checked</param>
        /// <param name="type">The agent which wants to check its location</param>
        /// <returns></returns>
        public bool CheckLocation(Vector3 position, AgentType type)
        {
            foreach (var initialBurrow in _initialBurrowSettings)
            {
                initialBurrow.burrowPrefabs[0].TryGetComponent(out Burrow burrow);

                if (type == burrow.type)
                {
                    var radius = initialBurrow.radius;
                    if (Physics.CheckSphere(position, radius)) return false;
                }
            }

            return true;
        }

        #endregion

        #endregion
    }
}