using System;
using UnityEngine;
using WorldGeneration._Scripts.Helper;
using WorldGeneration._Scripts.ScriptableObjects;
using WorldGeneration._Scripts.TerrainGeneration;
using Random = UnityEngine.Random;

namespace WorldGeneration._Scripts.Spawning
{
    [Serializable]
    public class AssetManager
    {
        // private
        private static AssetManager _instance;
        private CarrotSpawner _carrotSpawner;

        public static AssetManager GetInstance()
        {
            if (_instance == null)
            {
                _instance = new AssetManager();
                return _instance;
            }

            return _instance;
        }

        /// <summary>
        /// </summary>
        /// <param name="maxTerrainHeight"></param>
        /// <param name="generalSettings"></param>
        /// <param name="noiseSettings"></param>
        /// <param name="noiseWithClamp"></param>
        /// <param name="demoCarrot"></param>
        /// <param name="resolution"></param>
        /// <param name="corners"></param>
        /// <param name="maxNumberOfCarrots"></param>
        public void SpawnAssets(Vector2Int resolution, float maxTerrainHeight, float waterLevel, GeneralSettings generalSettings,
            NoiseSettings noiseSettings, NoiseWithClamp noiseWithClamp, GameObject demoCarrot,
            Vector2[] corners, int maxNumberOfCarrots)
        {
            _carrotSpawner = new CarrotSpawner();

            for (int i = 0; i <= maxNumberOfCarrots; i++)
            {
                var xPos = Random.Range(0.0f, resolution.x);
                var yPos = Random.Range(0.0f, resolution.y);
                var pos = GeneratorFunctions.GetSurfacePoint(xPos, yPos, resolution, maxTerrainHeight, generalSettings,
                    noiseSettings, noiseWithClamp);
                
                // while (pos.y < maxTerrainHeight * waterLevel || pos.y > maxTerrainHeight * 0.8f)
                // {
                //     xPos = Random.Range(0.0f, resolution.x);
                //     yPos = Random.Range(0.0f, resolution.y);
                //     pos = GeneratorFunctions.GetSurfacePoint(xPos, yPos, resolution, maxTerrainHeight, generalSettings,
                //         noiseSettings, noiseWithClamp);
                // }

                _carrotSpawner.Spawn(demoCarrot, pos);
                
                
                
                // _carrotSpawner.Spawn(demoCarrot,
                //     GeneratorFunctions.GetSurfacePoint(0.0f, 0.0f, resolution, maxTerrainHeight, generalSettings,
                //         noiseSettings, noiseWithClamp));
                // _carrotSpawner.Spawn(demoCarrot,
                //     GeneratorFunctions.GetSurfacePoint(resolution.x - 1, resolution.y - 1, resolution, maxTerrainHeight,
                //         generalSettings, noiseSettings, noiseWithClamp));
            }
        }
    }
}