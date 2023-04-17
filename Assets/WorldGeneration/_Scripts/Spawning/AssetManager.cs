using System;
using UnityEngine;
using WorldGeneration._Scripts.Helper;
using WorldGeneration._Scripts.ScriptableObjects;
using Random = UnityEngine.Random;

namespace WorldGeneration._Scripts.Spawning
{
    [Serializable]
    public class AssetManager
    {
        // private
        private static AssetManager _instance;
        private AssetSpawner _assetSpawner;

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
        /// <param name="resolution"></param>
        /// <param name="maxTerrainHeight"></param>
        /// <param name="generalSettings"></param>
        /// <param name="map"></param>
        /// <param name="carrotSettings"></param>
        /// <param name="carrots"></param>
        /// <param name="grassSettings"></param>
        /// <param name="grass"></param>
        public void SpawnAssets(Vector2Int resolution, float maxTerrainHeight, GeneralSettings generalSettings,
            float[,] map, PlantSettings carrotSettings, Transform carrots, PlantSettings grassSettings, Transform grass)
        {
            _assetSpawner = new AssetSpawner();

            for (var i = 0; i < carrotSettings.maxNumber; i++)
            {
                // var posFound = false;
                float xPos, zPos;
                Vector3 pos;

                // xPos = 3.346446854546165441312716954234876589f;
                // zPos = 6.023497893456012363497379756109274570f;
                //
                // pos = GeneratorFunctions.GetSurfacePoint(xPos, zPos, resolution, maxTerrainHeight, generalSettings,
                //     noiseSettings, noiseWithClamp);
                //
                // Debug.Log(pos.ToString());

                // while (!posFound)
                // {
                // xPos = Random.Range(-generalSettings.squareSize * (resolution.x - 1) / 2,
                //     generalSettings.squareSize * (resolution.x - 1) / 2 + generalSettings.squareSize * (resolution.x - 1) / 2);
                // zPos = Random.Range(-generalSettings.squareSize * (resolution.y - 1) / 2,
                //     generalSettings.squareSize * (resolution.y - 1) / 2 + generalSettings.squareSize * (resolution.y - 1) / 2);
                
                xPos = Random.Range(-generalSettings.squareSize * (resolution.x - 1) / 2,
                    generalSettings.squareSize * (resolution.x - 1) / 2);
                zPos = Random.Range(-generalSettings.squareSize * (resolution.y - 1) / 2,
                    generalSettings.squareSize * (resolution.y - 1) / 2);

                // zPos = Random.Range(0.0f, 1.0f) * (resolution.y - 1);
                // Debug.Log("xPos: " + xPos);
                // Debug.Log("zPos: "+ zPos);

                pos = GeneratorFunctions.GetSurfacePointFromWorldCoordinate(xPos, zPos, resolution, maxTerrainHeight,
                    map,
                    generalSettings);
                // pos = GeneratorFunctions.GetSurfacePoint(xPos, zPos, resolution, maxTerrainHeight, map,
                //     generalSettings);
                // if (pos.y > maxTerrainHeight * waterLevel && pos.y < maxTerrainHeight * 0.8f)
                // {
                var carrot = _assetSpawner.Spawn(carrotSettings.plantPrefab, pos);
                carrot.transform.SetParent(carrots);

                carrotSettings.plants.Add(carrot);

                // posFound = true;
                // }
                // }

                // _assetSpawner.Spawn(demoCarrot,
                //     GeneratorFunctions.GetSurfacePoint(0.0f, 0.0f, resolution, maxTerrainHeight, generalSettings,
                //         noiseSettings, noiseWithClamp));
                // _assetSpawner.Spawn(demoCarrot,
                //     GeneratorFunctions.GetSurfacePoint(resolution.x - 1, resolution.y - 1, resolution, maxTerrainHeight,
                //         generalSettings, noiseSettings, noiseWithClamp));
            }

            for (var i = 0; i < grassSettings.maxNumber; i++)
            {
                // var posFound = false;
                float xPos, zPos;
                Vector3 pos;

                xPos = Random.Range(-generalSettings.squareSize * (resolution.x - 1) / 2,
                    generalSettings.squareSize * (resolution.x - 1) / 2);
                zPos = Random.Range(-generalSettings.squareSize * (resolution.y - 1) / 2,
                    generalSettings.squareSize * (resolution.y - 1) / 2);

                // xPos = Random.Range(
                //     -generalSettings.squareSize * resolution.x / 2 /* + generalSettings.squareSize / 2*/,
                //     -generalSettings.squareSize * resolution.x / 2 + generalSettings.squareSize * resolution.x /*+
                //     generalSettings.squareSize / 2*/);
                // zPos = Random.Range(
                //     -generalSettings.squareSize * resolution.y / 2 /* + generalSettings.squareSize / 2*/,
                //     -generalSettings.squareSize * resolution.y / 2 + generalSettings.squareSize * resolution.y /*+
                //     generalSettings.squareSize / 2*/);

                pos = GeneratorFunctions.GetSurfacePointFromWorldCoordinate(xPos, zPos, resolution, maxTerrainHeight,
                    map, generalSettings);

                var plant = _assetSpawner.Spawn(grassSettings.plantPrefab, pos);
                plant.transform.SetParent(grass);
            }
        }
    }
}