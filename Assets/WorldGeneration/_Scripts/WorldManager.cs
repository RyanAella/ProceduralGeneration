using System;
using System.Collections.Generic;
using ml_agents.Agents.rabbit;
using UnityEngine;
using WorldGeneration._Scripts.Helper;
using WorldGeneration._Scripts.ScriptableObjects;
using WorldGeneration._Scripts.Spawning;
using WorldGeneration._Scripts.Spawning.TerrainAssets;
using WorldGeneration._Scripts.TerrainGeneration;
using Object = UnityEngine.Object;

namespace WorldGeneration._Scripts
{
    [Serializable]
    public class WorldManager /*: MonoBehaviour*/
    {
        // public
        public static WorldManager Instance;
        public float[,] Map;
        public bool WallGenerated;
        public List<GameObject> burrowList;
        public List<GameObject> rabbitList;

        //private
        public GameObject World;
        private Transform _parent;


        private bool _waterGenerated;
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

        public static WorldManager ResetInstance()
        {
            Instance = new WorldManager();
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
        /// <param name="plants"></param>
        /// <param name="burrows"></param>
        /// <returns></returns>
        public bool GenerateInitialWorld(Vector2Int resolution, float maxTerrainHeight, float waterLevel,
            GeneralSettings generalSettings, NoiseSettings noiseSettings, NoiseWithClamp noiseWithClamp,
            GroundGenerator ground, WaterGenerator water, AssetManager assetManager, Plants plants,
            Burrows burrows)
        {
            // try
            // {

            burrowList = new List<GameObject>();
            rabbitList = new List<GameObject>();
            
            World = new GameObject("World");

            _terrainGenerated = GenerateTerrain(resolution, maxTerrainHeight, waterLevel, generalSettings,
                noiseSettings, noiseWithClamp, World, ground, water);

            if (_terrainGenerated)
            {
                _burrowsGenerated = assetManager.InitialSpawnBurrows(resolution, maxTerrainHeight, waterLevel,
                    generalSettings, Map, World, burrows);
                _plantsGenerated = assetManager.InitialSpawnPlants(resolution, maxTerrainHeight, waterLevel,
                    generalSettings, Map, World, plants);
                return true;
            }
            else
            {
                return false;
            }


            // }
            // catch
            // {
            //     return false;
            // }
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
        /// <param name="plants"></param>
        /// <param name="burrows"></param>
        /// <returns></returns>
        // public bool ReloadWorld(Vector2Int resolution, float maxTerrainHeight, float waterLevel,
        //     GeneralSettings generalSettings, NoiseSettings noiseSettings, NoiseWithClamp noiseWithClamp,
        //     GroundGenerator ground, WaterGenerator water, AssetManager assetManager, Plants plants,
        //     Burrows burrows)
        // {
        //     if (_plantsGenerated)
        //     {
        //         foreach (var parent in assetManager.Plants.PlantParents) Object.Destroy(parent.Value.gameObject);
        //     }
        //
        //     var rabbits = GameObject.FindGameObjectsWithTag("Rabbit");
        //     foreach (var rabbit in rabbits)
        //     {
        //         rabbit.transform.parent.gameObject.GetComponent<AgentRabbit>().DestroyAgent();
        //     }
        //
        //     if (_burrowsGenerated)
        //     {
        //         foreach (var parent in assetManager.Burrows.BurrowParents)
        //         {
        //             var burrow = parent.Key.assetPrefab.GetComponent<Burrow>();
        //
        //
        //             if (burrow.inhabitants.Count == 0)
        //             {
        //                 Object.Destroy(parent.Value.gameObject);
        //             }
        //         }
        //     }
        //     
        //     Object.Destroy(_world);
        //
        //     GenerateInitialWorld(resolution, maxTerrainHeight, waterLevel, generalSettings,
        //         noiseSettings, noiseWithClamp, ground, water, assetManager, plants, burrows);
        //
        //     return true;
        // }

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
        /// <returns></returns>
        private bool GenerateTerrain(Vector2Int resolution, float maxTerrainHeight, float waterLevel,
            GeneralSettings generalSettings, NoiseSettings noiseSettings, NoiseWithClamp noiseWithClamp,
            GameObject world, GroundGenerator ground, WaterGenerator water)
        {
            var terrainGenerated = false;

            GroundGenerator groundGenerator = Object.Instantiate(ground);
            var groundGen = GroundGenerator.Instance;
            groundGenerator.transform.parent = world.transform;

            var groundGenerated = groundGenerator.GenerateGround(resolution, maxTerrainHeight, generalSettings,
                noiseSettings, noiseWithClamp, out Map);

            if (groundGenerated)
            {
                // try
                // {
                WallGenerated = groundGenerator.GenerateWall(resolution, maxTerrainHeight, generalSettings);

                WaterGenerator waterGenerator = Object.Instantiate(water);
                waterGenerator.transform.parent = world.transform;

                var waterGen = WaterGenerator.Instance;
                var waterGenerated =
                    waterGen.GenerateWater(resolution, maxTerrainHeight, waterLevel, generalSettings);

                return true;
            }
            // catch
            // {
            //     return false;
            // }
            // }
            else
            {
                Object.Destroy(groundGenerator);
                Debug.Log("False in groundGenerated");
                return false;
            }
        }
    }
}