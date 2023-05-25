using System;
using System.Collections.Generic;
using UnityEngine;
using WorldGeneration._Scripts.Helper;
using WorldGeneration._Scripts.ScriptableObjects;
using WorldGeneration._Scripts.Spawning;
using WorldGeneration._Scripts.TerrainGeneration;
using Object = UnityEngine.Object;

namespace WorldGeneration._Scripts
{
    [Serializable]
    public class WorldManager
    {
        // public
        public static WorldManager Instance;
        public List<GameObject> burrowList = new();
        public List<GameObject> rabbitList = new();
        public List<GameObject> foxList = new();
        public GameObject world;
        private bool _burrowsGenerated;
        private bool _plantsGenerated;

        //private
        private bool _terrainGenerated;
        public float[,] Map;

        public static WorldManager GetInstance()
        {
            if (Instance == null)
            {
                Instance = new WorldManager();
                return Instance;
            }

            return Instance;
        }

        public static WorldManager ResetInstance()
        {
            Instance = new WorldManager();
            return Instance;
        }

        /// <summary>
        /// </summary>
        /// <param name="resolution"></param>
        /// <param name="maxTerrainHeight"></param>
        /// <param name="waterLevel"></param>
        /// <param name="generalSettings"></param>
        /// <param name="noiseSettings"></param>
        /// <param name="noiseWithClamp"></param>
        /// <param name="ground"></param>
        /// <param name="water"></param>
        /// <param name="assetManager"></param>
        /// <param name="plants"></param>
        /// <param name="burrows"></param>
        /// <returns></returns>
        public bool GenerateInitialWorld(Vector2Int resolution, float maxTerrainHeight, float waterLevel,
            GeneralSettings generalSettings, NoiseSettings noiseSettings, NoiseWithClamp noiseWithClamp,
            GroundGenerator ground, WaterGenerator water, AssetManager assetManager, Plants plants,
            Burrows burrows)
        {
            world = new GameObject("World");

            _terrainGenerated = GenerateTerrain(resolution, maxTerrainHeight, waterLevel, generalSettings,
                noiseSettings, noiseWithClamp, ground, water);

            if (_terrainGenerated)
            {
                _burrowsGenerated = assetManager.InitialSpawnBurrows(resolution, maxTerrainHeight, waterLevel,
                    generalSettings, Map, world, burrows);
                _plantsGenerated = assetManager.InitialSpawnPlants(resolution, maxTerrainHeight, waterLevel,
                    generalSettings, Map, world, plants);
                return _burrowsGenerated && _plantsGenerated;
            }

            return _terrainGenerated;
        }

        /// <summary>
        /// </summary>
        /// <param name="resolution"></param>
        /// <param name="maxTerrainHeight"></param>
        /// <param name="waterLevel"></param>
        /// <param name="generalSettings"></param>
        /// <param name="noiseSettings"></param>
        /// <param name="noiseWithClamp"></param>
        /// <param name="ground"></param>
        /// <param name="water"></param>
        /// <returns></returns>
        private bool GenerateTerrain(Vector2Int resolution, float maxTerrainHeight, float waterLevel,
            GeneralSettings generalSettings, NoiseSettings noiseSettings, NoiseWithClamp noiseWithClamp,
            GroundGenerator ground, WaterGenerator water)
        {
            var groundGenerator = Object.Instantiate(ground);
            groundGenerator.transform.parent = world.transform;

            var groundGenerated = groundGenerator.GenerateGround(resolution, maxTerrainHeight, generalSettings,
                noiseSettings, noiseWithClamp, out Map);

            if (groundGenerated)
            {
                groundGenerator.GenerateWall(resolution, maxTerrainHeight, generalSettings);

                var waterGenerator = Object.Instantiate(water);
                waterGenerator.transform.parent = world.transform;

                var waterGen = WaterGenerator.Instance;
                waterGen.GenerateWater(resolution, maxTerrainHeight, waterLevel, generalSettings);

                return true;
            }

            Object.Destroy(groundGenerator);
            Debug.Log("False in groundGenerated");
            return false;
        }
    }
}