using UnityEngine;
using WorldGeneration._Scripts.Helper;
using WorldGeneration._Scripts.ScriptableObjects;
using WorldGeneration._Scripts.Spawning;
using WorldGeneration._Scripts.TerrainGeneration;

namespace WorldGeneration._Scripts
{
    public class WorldManager /*: MonoBehaviour*/
    {
        // public
        public static WorldManager Instance;

        //private
        private GameObject _world;
        private Transform _parent;

        private bool _terrainGenerated;
        private bool _foodGenerated;
        private bool _plantsGenerated;
        private bool _burrowsGenerated;

        public static WorldManager GetInstance()
        {
            if (Instance == null)
            {
                Instance = new WorldManager();
                return Instance;
            }

            return Instance;
        }

        /// <summary>
        /// 
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
        /// <param name="food"></param>
        /// <param name="plants"></param>
        /// <param name="burrows"></param>
        /// <param name="map"></param>
        /// <returns></returns>
        public bool GenerateInitialWorld(Vector2Int resolution, float maxTerrainHeight, float waterLevel,
            GeneralSettings generalSettings, NoiseSettings noiseSettings, NoiseWithClamp noiseWithClamp,
            GroundGenerator ground, WaterGenerator water, AssetManager assetManager, Food food, Plants plants, Burrows burrows,
            out float[,] map)
        {
            _world = new GameObject("World");

            _terrainGenerated = GenerateTerrain(resolution, maxTerrainHeight, waterLevel, generalSettings,
                noiseSettings, noiseWithClamp, _world, ground, water, out map);

            if (_terrainGenerated)
            {
                _foodGenerated = assetManager.InitialSpawnFood(resolution, maxTerrainHeight, waterLevel, generalSettings, map, _world, food);
                _burrowsGenerated = assetManager.InitialSpawnBurrows(resolution, maxTerrainHeight, waterLevel, generalSettings, map, _world, burrows);
                _plantsGenerated = assetManager.InitialSpawnPlants(resolution, maxTerrainHeight, waterLevel, generalSettings, map, _world, plants);
            }

            return true;
        }

        /*
        public bool ReloadWorld(Vector2Int resolution, float maxTerrainHeight, float waterLevel,
            GeneralSettings generalSettings, NoiseSettings noiseSettings, NoiseWithClamp noiseWithClamp,
            GroundGenerator ground, WaterGenerator water, AssetManager assetManager, out float[,] map)
        {
                Object.Destroy(_world);

                if (_plantsGenerated)
                {
                    foreach (var parent in assetManager.PlantParents) Object.Destroy(parent.Value.gameObject);
                }

                if (_burrowsGenerated)
                {
                    foreach (var parent in assetManager.BurrowParents)
                    {
                        foreach (var rabbit in parent.Key.assetPrefab.GetComponent<Burrow>().inhabitants)
                        {
                            rabbit.GetComponent<CustomAgent>().DestroyAgent();
                        }

                        if (parent.Key.assetPrefab.GetComponent<Burrow>().inhabitants.Count == 0)
                        {
                            Object.Destroy(parent.Value.gameObject);
                        }
                    }
                }

            GenerateInitialWorld(resolution, maxTerrainHeight, waterLevel, generalSettings,
                noiseSettings, noiseWithClamp, ground, water, assetManager, out map);

            return true;
        }
        */
        
        public void RespawnPlants(Plants plants, AssetManager assetManager)
        {
            foreach (var plant in plants.PlantsList) 
            {
                if (plant.assets.Count < plant.minNumber)
                {
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resolution"></param>
        /// <param name="maxTerrainHeight"></param>
        /// <param name="waterLevel"></param>
        /// <param name="generalSettings"></param>
        /// <param name="noiseSettings"></param>
        /// <param name="noiseWithClamp"></param>
        /// <param name="world"></param>
        /// <param name="ground"></param>
        /// <param name="water"></param>
        /// <param name="map"></param>
        /// <returns></returns>
        private bool GenerateTerrain(Vector2Int resolution, float maxTerrainHeight, float waterLevel,
            GeneralSettings generalSettings, NoiseSettings noiseSettings, NoiseWithClamp noiseWithClamp,
            GameObject world, GroundGenerator ground, WaterGenerator water, out float[,] map)
        {
            Object.Instantiate(ground, world.transform);
            var groundGen = GroundGenerator.Instance;
            groundGen.GenerateGround(resolution, maxTerrainHeight, generalSettings, noiseSettings, noiseWithClamp,
                out map);
            groundGen.GenerateWall(resolution, maxTerrainHeight, generalSettings);

            Object.Instantiate(water, world.transform);
            var waterGen = WaterGenerator.Instance;
            waterGen.GenerateWater(resolution, maxTerrainHeight, waterLevel, generalSettings);

            return true;
        }
    }
}