using System;
using UnityEngine;
using WorldGeneration._Scripts.Helper;
using WorldGeneration._Scripts.ScriptableObjects;
using WorldGeneration._Scripts.TerrainGeneration;

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
        /// <param name="noiseGenerator"></param>
        /// <param name="demoCarrot"></param>
        /// <param name="resolution"></param>
        /// <param name="corners"></param>
        /// <param name="clamp"></param>
        public void SpawnAssets(Vector2Int resolution, float maxTerrainHeight, GeneralSettings generalSettings, NoiseSettings noiseSettings, NoiseGenerator noiseGenerator, GameObject demoCarrot,
            Vector2[] corners, ValueClamp clamp)
        {
            _carrotSpawner = new CarrotSpawner();

            // _carrotSpawner.Spawn(DemoCarrot, GeneratorFunctions.GetSurfacePoint(2.3f, 4.7f, generalSettings, noiseSettings, noiseGenerator, _clamp));
            // _carrotSpawner.Spawn(DemoCarrot, GeneratorFunctions.GetSurfacePoint(5.4f, 4.7f, generalSettings, noiseSettings, noiseGenerator, _clamp));
            _carrotSpawner.Spawn(demoCarrot, GeneratorFunctions.GetSurfacePoint(0.0f, 0.0f, resolution, maxTerrainHeight, generalSettings, noiseSettings, noiseGenerator,
                    clamp));
            // _carrotSpawner.Spawn(demoCarrot,
            //     GeneratorFunctions.GetSurfacePoint(8.0f, 8.0f, generalSettings, noiseSettings, _noiseGenerator,
            //         _clamp));
            _carrotSpawner.Spawn(demoCarrot,GeneratorFunctions.GetSurfacePoint(resolution.x - 1, resolution.y - 1, resolution, maxTerrainHeight, generalSettings, noiseSettings, noiseGenerator,
                    clamp));
            // foreach (var corner in corners) _carrotSpawner.Spawn(demoCarrot, GeneratorFunctions.GetSurfacePoint(corner.x, corner.y, resolution, maxTerrainHeight, generalSettings, noiseSettings, noiseGenerator, _clamp));
        }
    }
}