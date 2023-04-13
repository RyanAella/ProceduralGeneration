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
        /// 
        /// </summary>
        /// <param name="resolution"></param>
        /// <param name="maxTerrainHeight"></param>
        /// <param name="waterLevel"></param>
        /// <param name="generalSettings"></param>
        /// <param name="noiseSettings"></param>
        /// <param name="map"></param>
        /// <param name="noiseWithClamp"></param>
        /// <param name="demoCarrot"></param>
        /// <param name="corners"></param>
        /// <param name="maxNumberOfCarrots"></param>
        public void SpawnAssets(Vector2Int resolution, float maxTerrainHeight, float waterLevel,
            GeneralSettings generalSettings,
            NoiseSettings noiseSettings, float[,] map, NoiseWithClamp noiseWithClamp, GameObject demoCarrot,
            Vector2[] corners, int maxNumberOfCarrots)
        {
            _carrotSpawner = new CarrotSpawner();

            for (var i = 0; i <= maxNumberOfCarrots; i++)
            {
                var posFound = false;
                float xPos, zPos;
                Vector3 pos;

                // xPos = 3.346446854546165441312716954234876589f;
                // zPos = 6.023497893456012363497379756109274570f;
                //
                // pos = GeneratorFunctions.GetSurfacePoint(xPos, zPos, resolution, maxTerrainHeight, generalSettings,
                //     noiseSettings, noiseWithClamp);
                //
                // Debug.Log(pos.ToString());


                while (!posFound)
                {
                    xPos = Random.Range(0.0f, 1.0f) * (resolution.x - 1);
                    zPos = Random.Range(0.0f, 1.0f) * (resolution.y - 1);
                    // Debug.Log("xPos: " + xPos);
                    // Debug.Log("zPos: "+ zPos);
                    
                    pos = GeneratorFunctions.GetSurfacePoint(xPos, zPos, resolution, maxTerrainHeight, map, generalSettings
                        /*noiseSettings, noiseWithClamp*/);
                    if (pos.y > maxTerrainHeight * waterLevel && pos.y < maxTerrainHeight * 0.8f)
                    {
                        _carrotSpawner.Spawn(demoCarrot, pos);
                        posFound = true;
                    }
                }


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