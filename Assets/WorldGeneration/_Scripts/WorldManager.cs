using System;
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
    public class WorldManager /*: MonoBehaviour*/
    {
        // public
        public static WorldManager Instance;
        public float[,] Map;
        public bool WallGenerated;

        //private
        private GameObject _world;
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
                _world = new GameObject("World");

                _terrainGenerated = GenerateTerrain(resolution, maxTerrainHeight, waterLevel, generalSettings,
                    noiseSettings, noiseWithClamp, _world, ground, water);

                if (_terrainGenerated)
                {
                    _burrowsGenerated = assetManager.InitialSpawnBurrows(resolution, maxTerrainHeight, waterLevel,
                        generalSettings, Map, _world, burrows);
                    _plantsGenerated = assetManager.InitialSpawnPlants(resolution, maxTerrainHeight, waterLevel,
                        generalSettings, Map, _world, plants);
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
        public bool ReloadWorld(Vector2Int resolution, float maxTerrainHeight, float waterLevel,
            GeneralSettings generalSettings, NoiseSettings noiseSettings, NoiseWithClamp noiseWithClamp,
            GroundGenerator ground, WaterGenerator water, AssetManager assetManager, Plants plants,
            Burrows burrows)
        {
            if (_plantsGenerated)
            {
                foreach (var parent in assetManager.Plants.PlantParents) Object.Destroy(parent.Value.gameObject);
            }

            var rabbits = GameObject.FindGameObjectsWithTag("Rabbit");
            foreach (var rabbit in rabbits)
            {
                rabbit.transform.parent.gameObject.GetComponent<AgentRabbit>().DestroyAgent();
            }

            if (_burrowsGenerated)
            {
                foreach (var parent in assetManager.Burrows.BurrowParents)
                {
                    var burrow = parent.Key.assetPrefab.GetComponent<Burrow>();
                    // if (burrow.type == AgentType.RABBIT)
                    // {
                    //     foreach (GameObject rabbit in burrow.inhabitants)
                    //     {
                    //         rabbit.GetComponent<AgentRabbit>().DestroyAgent();
                    //         // Object.Destroy(rabbit);
                    //     }
                    // }

                    if (burrow.inhabitants.Count == 0)
                    {
                        Object.Destroy(parent.Value.gameObject);
                    }
                }
            }
            
            Object.Destroy(_world);

            GenerateInitialWorld(resolution, maxTerrainHeight, waterLevel, generalSettings,
                noiseSettings, noiseWithClamp, ground, water, assetManager, plants, burrows);

            return true;
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
        /// <returns></returns>
        private bool GenerateTerrain(Vector2Int resolution, float maxTerrainHeight, float waterLevel,
            GeneralSettings generalSettings, NoiseSettings noiseSettings, NoiseWithClamp noiseWithClamp,
            GameObject world, GroundGenerator ground, WaterGenerator water)
        {
            var groundGenerator = Object.Instantiate(ground, world.transform);
            var groundGen = GroundGenerator.Instance;
            var groundGenerated = groundGen.GenerateGround(resolution, maxTerrainHeight, generalSettings, noiseSettings,
                noiseWithClamp);

            if (groundGenerated)
            {
                try
                {
                    WallGenerated = groundGen.GenerateWall(resolution, maxTerrainHeight, generalSettings);

                    var waterGenerator = Object.Instantiate(water, world.transform);
                    var waterGen = WaterGenerator.Instance;
                    var waterGenerated =
                        waterGen.GenerateWater(resolution, maxTerrainHeight, waterLevel, generalSettings);

                    return true;
                }
                catch
                {
                    return false;
                }
            }
            else
            {
                Object.Destroy(groundGenerator);
                Debug.Log("False in groundGenerated");
                return false;
            }
        }
    }
}