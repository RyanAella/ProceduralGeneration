using System;
using System.Collections.Generic;
using UnityEngine;
using WorldGeneration._Scripts.Helper;
using WorldGeneration._Scripts.ScriptableObjects;
using Random = UnityEngine.Random;

namespace WorldGeneration._Scripts.Spawning
{
    [Serializable]
    public class AssetManager
    {
        public static AssetManager Instance;
        
        // private
        private List<AssetSettings> assets;

        public static AssetManager GetInstance()
        {
            if (Instance == null)
            {
                Instance = new AssetManager();
                return Instance;
            }
        
            return Instance;
        }

        /// <summary>
        /// </summary>
        /// <param name="resolution"></param>
        /// <param name="maxTerrainHeight"></param>
        /// <param name="generalSettings"></param>
        /// <param name="map"></param>
        /// <param name="settings"></param>
        /// <param name="parent"></param>
        public void InitialSpawnAssets(Vector2Int resolution, float maxTerrainHeight, GeneralSettings generalSettings,
            float[,] map, AssetSettings settings, Transform parent)
        {
            assets = new List<AssetSettings>();

            for (var i = 0; i < settings.maxNumber; i++)
            {
                // var posFound = false;
                float xPos, zPos;
                Vector3 pos;

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
                    map, generalSettings);
                // pos = GeneratorFunctions.GetSurfacePoint(xPos, zPos, resolution, maxTerrainHeight, map,
                //     generalSettings);
                // if (pos.y > maxTerrainHeight * waterLevel && pos.y < maxTerrainHeight * 0.8f)
                // {
                var asset = UnityEngine.Object.Instantiate(settings.assetPrefab, pos, Quaternion.Euler(new Vector3(0, Random.Range(0, 360), 0)));
                asset.transform.SetParent(parent);

                settings.assets.Add(asset);
                assets.Add(settings);

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

            // for (var i = 0; i < grassSettings.maxNumber; i++)
            // {
            //     // var posFound = false;
            //     float xPos, zPos;
            //     Vector3 pos;
            //
            //     xPos = Random.Range(-generalSettings.squareSize * (resolution.x - 1) / 2,
            //         generalSettings.squareSize * (resolution.x - 1) / 2);
            //     zPos = Random.Range(-generalSettings.squareSize * (resolution.y - 1) / 2,
            //         generalSettings.squareSize * (resolution.y - 1) / 2);
            //
            //     // xPos = Random.Range(
            //     //     -generalSettings.squareSize * resolution.x / 2 /* + generalSettings.squareSize / 2*/,
            //     //     -generalSettings.squareSize * resolution.x / 2 + generalSettings.squareSize * resolution.x /*+
            //     //     generalSettings.squareSize / 2*/);
            //     // zPos = Random.Range(
            //     //     -generalSettings.squareSize * resolution.y / 2 /* + generalSettings.squareSize / 2*/,
            //     //     -generalSettings.squareSize * resolution.y / 2 + generalSettings.squareSize * resolution.y /*+
            //     //     generalSettings.squareSize / 2*/);
            //
            //     pos = GeneratorFunctions.GetSurfacePointFromWorldCoordinate(xPos, zPos, resolution, maxTerrainHeight,
            //         map, generalSettings);
            //
            //     var plant = _assetSpawner.Spawn(grassSettings.plantPrefab, pos);
            //     plant.transform.SetParent(grassSettings.parent);
            // }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resolution"></param>
        /// <param name="maxTerrainHeight"></param>
        /// <param name="generalSettings"></param>
        /// <param name="map"></param>
        /// <param name="settings"></param>
        /// <param name="parent"></param>
        /// <param name="count"></param>
        public void SpawnAssets(Vector2Int resolution, float maxTerrainHeight, GeneralSettings generalSettings,
            float[,] map, AssetSettings settings, Transform parent, int count)
        {
                for (var i = 0; i < count; i++)
                {
                    // var posFound = false;
                    float xPos, zPos;
                    Vector3 pos;
                    
                    xPos = Random.Range(-generalSettings.squareSize * (resolution.x - 1) / 2,
                        generalSettings.squareSize * (resolution.x - 1) / 2);
                    zPos = Random.Range(-generalSettings.squareSize * (resolution.y - 1) / 2,
                        generalSettings.squareSize * (resolution.y - 1) / 2);

                    pos = GeneratorFunctions.GetSurfacePointFromWorldCoordinate(xPos, zPos, resolution,
                        maxTerrainHeight, map, generalSettings);

                    var carrot = UnityEngine.Object.Instantiate(settings.assetPrefab, pos, Quaternion.Euler(new Vector3(0, Random.Range(0, 360), 0)));
                    carrot.transform.SetParent(parent);

                    settings.assets.Add(carrot);
                }
        }
    }
}