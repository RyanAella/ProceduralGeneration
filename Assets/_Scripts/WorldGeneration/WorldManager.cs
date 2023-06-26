/*
* Copyright (c) mmq
*/

using _Scripts.Cameras.FirstPerson;
using _Scripts.Cameras.FlyCam;
using _Scripts.WorldGeneration.Helper;
using _Scripts.WorldGeneration.ScriptableObjects;
using _Scripts.WorldGeneration.Spawning;
using _Scripts.WorldGeneration.Spawning.TerrainAssets;
using _Scripts.WorldGeneration.TerrainGeneration;
using UnityEngine;

namespace _Scripts.WorldGeneration
{
    public class WorldManager
    {
        #region Variables

        // public
        public GameObject World;
        public float[,] Map;

        //private
        private static WorldManager _instance;
        private AssetManager _assetManager;
        private bool _terrainGenerated;
        private bool _burrowsGenerated;
        private bool _plantsGenerated;
        private GameObject _firstPersonController;
        private GameObject _flyCam;

        #endregion

        #region Unity Methods

        public static WorldManager GetInstance()
        {
            if (_instance == null)
            {
                _instance = new WorldManager();
                return _instance;
            }

            return _instance;
        }

        public static WorldManager ResetInstance()
        {
            _instance = new WorldManager();
            return _instance;
        }

        /// <summary>
        ///     Generate the initial world.
        /// </summary>
        /// <param name="resolution">The x and y resolution as Vector2</param>
        /// <param name="maxTerrainHeight">The maximum terrain height as float</param>
        /// <param name="waterLevel">The height of the water level as a percentage of the maximum terrain height in float</param>
        /// <param name="mountainArea">The height of the mountain area as a percentage of the maximum terrain height in float</param>
        /// <param name="generalSettings">The general settings of the simulation</param>
        /// <param name="noiseSettings">The noise setting needed for the mesh generation</param>
        /// <param name="noiseWithClamp">The noise clamp needed for the mesh generation</param>
        /// <param name="ground">The GroundGenerator</param>
        /// <param name="water">The WaterGenerator</param>
        /// <param name="assetManager">The AssetManager</param>
        /// <param name="plants">All the information about the plants</param>
        /// <param name="burrows">All the information about the burrows</param>
        /// <param name="firstPerson">The first person controller</param>
        /// <param name="flyCam">The flyCam</param>
        /// <param name="mainCamera">The main camera in the scene</param>
        /// <returns>true - if all was successful</returns>
        public bool GenerateInitialWorld(Vector2Int resolution, float maxTerrainHeight, float waterLevel,
            float mountainArea, GeneralSettings generalSettings, NoiseSettings noiseSettings,
            NoiseWithClamp noiseWithClamp,
            GroundGenerator ground, WaterGenerator water, AssetManager assetManager, Plants plants,
            Burrows burrows, GameObject firstPerson, FlyCam flyCam, Camera mainCamera)
        {
            World = new GameObject("World");

            // Generate the terrain meshes
            _terrainGenerated = GenerateTerrain(resolution, maxTerrainHeight, waterLevel, generalSettings,
                noiseSettings, noiseWithClamp, ground, water);

            if (!_terrainGenerated) return _terrainGenerated;

            // Spawn the initial burrows...
            _burrowsGenerated = assetManager.InitialSpawnBurrows(resolution, maxTerrainHeight, waterLevel,
                generalSettings, Map, World, burrows);

            // ...then instantiate the inhabitants of the burrows
            if (_burrowsGenerated)
                foreach (var burrow in Checker.BurrowList)
                    burrow.GetComponent<Burrow>().InstantiateInhabitants();
            else
                return _burrowsGenerated;

            // Spawn the plants
            _plantsGenerated = assetManager.InitialSpawnPlants(resolution, maxTerrainHeight, waterLevel, mountainArea,
                generalSettings, Map, World, plants);

            CreateWatcher(resolution, maxTerrainHeight, generalSettings, firstPerson, flyCam, mainCamera);

            return _plantsGenerated /*&& _playerCameraGenerated*/;
        }

        private void CreateWatcher(Vector2Int resolution, float maxTerrainHeight, GeneralSettings generalSettings,
            GameObject firstPerson, FlyCam flyCam, Camera mainCamera)
        {
            switch (generalSettings.watchMode)
            {
                case WatchMode.Default:
                {
                    mainCamera.enabled = true;

                    // If there is a first person controller destroy it
                    if (_flyCam != null) Object.Destroy(Object.FindObjectOfType<FlyCam>().gameObject);

                    // If there is a first person controller destroy it
                    if (_firstPersonController != null) Object.Destroy(Object.FindObjectOfType<FirstPersonController>().gameObject);
                    
                    break;
                }
                case WatchMode.FlyCam:
                {
                    mainCamera.enabled = false;
                    
                    // If there is a first person controller destroy it
                    if (_firstPersonController != null) Object.Destroy(Object.FindObjectOfType<FirstPersonController>().gameObject);

                    // Activate the flyCam
                    _flyCam = Object.Instantiate(flyCam.gameObject, new Vector3(0, maxTerrainHeight + 25f, 0),
                        Quaternion.identity);
                    
                    break;
                }
                case WatchMode.FirstPerson:
                {
                    mainCamera.enabled = false;

                    // If there is a first person controller destroy it
                    if (_flyCam != null) Object.Destroy(Object.FindObjectOfType<FlyCam>().gameObject);

                    // Instantiate the first person controller
                    _firstPersonController = Object.Instantiate(firstPerson,
                        GeneratorFunctions.GetSurfacePointFromWorldCoordinate(0, 0, resolution, maxTerrainHeight, Map,
                            generalSettings), Quaternion.identity);
                    var pos = _firstPersonController.transform.position;
                    pos.y += 2.5f;
                    _firstPersonController.transform.position = pos;
                    
                    break;
                }
            }
        }

        /// <summary>
        ///     Generate the Terrain meshes.
        /// </summary>
        /// <param name="resolution">The x and y resolution as Vector2</param>
        /// <param name="maxTerrainHeight">The maximum terrain height as float</param>
        /// <param name="waterLevel">The height of the water level as a percentage of the maximum terrain height in float</param>
        /// <param name="generalSettings">The general settings of the simulation</param>
        /// <param name="noiseSettings">The noise setting needed for the mesh generation</param>
        /// <param name="noiseWithClamp">The noise clamp needed for the mesh generation</param>
        /// <param name="ground">The GroundGenerator</param>
        /// <param name="water">The WaterGenerator</param>
        /// <returns>true - if all was successful</returns>
        private bool GenerateTerrain(Vector2Int resolution, float maxTerrainHeight, float waterLevel,
            GeneralSettings generalSettings, NoiseSettings noiseSettings, NoiseWithClamp noiseWithClamp,
            GroundGenerator ground, WaterGenerator water)
        {
            var groundGenerator = Object.Instantiate(ground);
            groundGenerator.transform.parent = World.transform;

            // Generate the ground mesh
            var groundGenerated = groundGenerator.GenerateGround(resolution, maxTerrainHeight, generalSettings,
                noiseSettings, noiseWithClamp, out Map);

            if (groundGenerated)
            {
                // Generate the surrounding walls
                groundGenerator.GenerateWall(resolution, maxTerrainHeight, generalSettings);

                // Generate the death floor beneath the ground
                groundGenerator.GenerateDeathFloor(resolution, generalSettings, World);

                var waterGenerator = Object.Instantiate(water);
                waterGenerator.transform.parent = World.transform;

                var waterGen = WaterGenerator.Instance;
                // Generate the water
                waterGen.GenerateWater(resolution, maxTerrainHeight, waterLevel, generalSettings);

                return true;
            }

            Object.Destroy(groundGenerator);
            Debug.Log("False in groundGenerated");
            return false;
        }

        /// <summary>
        ///     Respawn the plants.
        /// </summary>
        /// <param name="plants">All the information about the plants</param>
        public void RespawnPlants(Plants plants)
        {
            _assetManager = AssetManager.GetInstance();
            _assetManager.SpawnPlants(Map, plants);
        }

        #endregion
    }
}