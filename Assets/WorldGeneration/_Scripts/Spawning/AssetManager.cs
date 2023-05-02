using System;
using System.Collections.Generic;
using UnityEngine;
using WorldGeneration._Scripts.Helper;
using WorldGeneration._Scripts.ScriptableObjects;
using WorldGeneration._Scripts.Spawning.TerrainAssets;
using Object = UnityEngine.Object;
using Random = System.Random;

namespace WorldGeneration._Scripts.Spawning
{
    [Serializable]
    public class AssetManager
    {
        public static AssetManager Instance;

        private Random _prng = new();
        private List<GameObject> assetPrefabs;
        private List<BurrowSettings> burrows;

        // private
        private List<AssetSettings> plants;

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
        /// <param name="waterLevel"></param>
        /// <param name="generalSettings"></param>
        /// <param name="map"></param>
        /// <param name="settings"></param>
        /// <param name="parent"></param>
        public void InitialSpawnPlants(Vector2Int resolution, float maxTerrainHeight, float waterLevel,
            GeneralSettings generalSettings, float[,] map, AssetSettings settings, Transform parent)
        {
            plants = new List<AssetSettings>();

            assetPrefabs = settings.assetPrefab;

            for (var i = 0; i < settings.maxNumber; i++)
            {
                var posFound = false;
                float xPos, zPos;
                Vector3 pos;

                var prngPrefab = _prng.Next(assetPrefabs.Count);

                while (!posFound)
                {
                    xPos = UnityEngine.Random.Range(-generalSettings.squareSize * (resolution.x - 1) / 2,
                        generalSettings.squareSize * (resolution.x - 1) / 2);
                    zPos = UnityEngine.Random.Range(-generalSettings.squareSize * (resolution.y - 1) / 2,
                        generalSettings.squareSize * (resolution.y - 1) / 2);

                    // zPos = Random.Range(0.0f, 1.0f) * (resolution.y - 1);
                    // Debug.Log("xPos: " + xPos);
                    // Debug.Log("zPos: "+ zPos);

                    pos = GeneratorFunctions.GetSurfacePointFromWorldCoordinate(xPos, zPos, resolution,
                        maxTerrainHeight, map, generalSettings);

                    if (settings.nearWater)
                    {
                        var max = maxTerrainHeight * waterLevel;
                        var min = maxTerrainHeight * waterLevel - 5f;
                        if (min < pos.y && pos.y < max)
                        {
                            var asset = Object.Instantiate(settings.assetPrefab[prngPrefab], pos,
                                Quaternion.Euler(new Vector3(0, UnityEngine.Random.Range(0, 360), 0)));
                            asset.transform.SetParent(parent);

                            settings.assets.Add(asset);

                            posFound = true;
                        }
                    }
                    else
                    {
                        if (pos.y > maxTerrainHeight * waterLevel /*&& pos.y < maxTerrainHeight * 0.8f*/)
                        {
                            var asset = Object.Instantiate(settings.assetPrefab[prngPrefab], pos,
                                Quaternion.Euler(new Vector3(0, UnityEngine.Random.Range(0, 360), 0)));
                            asset.transform.SetParent(parent);

                            settings.assets.Add(asset);

                            posFound = true;
                        }
                    }
                }
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
        /// </summary>
        /// <param name="resolution"></param>
        /// <param name="maxTerrainHeight"></param>
        /// <param name="waterLevel"></param>
        /// <param name="generalSettings"></param>
        /// <param name="map"></param>
        /// <param name="settings"></param>
        /// <param name="parent"></param>
        /// <param name="count"></param>
        public void SpawnPlants(Vector2Int resolution, float maxTerrainHeight, float waterLevel,
            GeneralSettings generalSettings,
            float[,] map, AssetSettings settings, Transform parent, int count)
        {
            assetPrefabs = settings.assetPrefab;

            for (var i = 0; i < count; i++)
            {
                var posFound = false;
                float xPos, zPos;
                Vector3 pos;

                var prngPrefab = _prng.Next(assetPrefabs.Count);

                while (!posFound)
                {
                    xPos = UnityEngine.Random.Range(-generalSettings.squareSize * (resolution.x - 1) / 2,
                        generalSettings.squareSize * (resolution.x - 1) / 2);
                    zPos = UnityEngine.Random.Range(-generalSettings.squareSize * (resolution.y - 1) / 2,
                        generalSettings.squareSize * (resolution.y - 1) / 2);

                    pos = GeneratorFunctions.GetSurfacePointFromWorldCoordinate(xPos, zPos, resolution,
                        maxTerrainHeight, map, generalSettings);

                    if (settings.nearWater)
                    {
                        var max = maxTerrainHeight * waterLevel;
                        var min = maxTerrainHeight * waterLevel - 5f;
                        if (min < pos.y && pos.y < max)
                        {
                            var asset = Object.Instantiate(settings.assetPrefab[prngPrefab], pos,
                                Quaternion.Euler(new Vector3(0, UnityEngine.Random.Range(0, 360), 0)));
                            asset.transform.SetParent(parent);

                            settings.assets.Add(asset);

                            posFound = true;
                        }
                    }
                    else
                    {
                        if (pos.y > maxTerrainHeight * waterLevel /*&& pos.y < maxTerrainHeight * 0.8f*/)
                        {
                            var asset = Object.Instantiate(settings.assetPrefab[prngPrefab], pos,
                                Quaternion.Euler(new Vector3(0, UnityEngine.Random.Range(0, 360), 0)));
                            asset.transform.SetParent(parent);

                            settings.assets.Add(asset);

                            posFound = true;
                        }
                    }
                }
            }
        }


        /// <summary>
        /// </summary>
        /// <param name="resolution"></param>
        /// <param name="maxTerrainHeight"></param>
        /// <param name="waterLevel"></param>
        /// <param name="generalSettings"></param>
        /// <param name="map"></param>
        /// <param name="settings"></param>
        /// <param name="parent"></param>
        public void InitialSpawnBurrows(Vector2Int resolution, float maxTerrainHeight, float waterLevel,
            GeneralSettings generalSettings, float[,] map, BurrowSettings settings, Transform parent)
        {
            burrows = new List<BurrowSettings>();

            for (var i = 0; i < 1; i++)
            {
                var posFound = false;
                float xPos, zPos;
                Vector3 pos;

                while (!posFound)
                {
                    xPos = UnityEngine.Random.Range(-generalSettings.squareSize * (resolution.x - 1) / 2,
                        generalSettings.squareSize * (resolution.x - 1) / 2);
                    zPos = UnityEngine.Random.Range(-generalSettings.squareSize * (resolution.y - 1) / 2,
                        generalSettings.squareSize * (resolution.y - 1) / 2);

                    pos = GeneratorFunctions.GetSurfacePointFromWorldCoordinate(xPos, zPos, resolution,
                        maxTerrainHeight, map, generalSettings);


                    if (pos.y > maxTerrainHeight * waterLevel /*&& pos.y < maxTerrainHeight * 0.8f*/)
                    {
                        var asset = Object.Instantiate(settings.assetPrefab, pos,
                            Quaternion.Euler(new Vector3(0, UnityEngine.Random.Range(0, 360), 0)));
                        asset.transform.SetParent(parent);

                        settings.assets.Add(asset);
                        
                        var rabbits = settings.assetPrefab.GetComponent<RabbitBurrow>().inhabitants;
                        foreach (var rabbit in rabbits)
                        {
                            var rabbitPos = new Vector3(pos.x, pos.y + rabbit.transform.localScale.y / 2, pos.z);
                            Object.Instantiate(rabbit, rabbitPos,
                                Quaternion.Euler(new Vector3(0, UnityEngine.Random.Range(0, 360), 0)));
                        }

                        posFound = true;
                    }
                }
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="resolution"></param>
        /// <param name="maxTerrainHeight"></param>
        /// <param name="waterLevel"></param>
        /// <param name="generalSettings"></param>
        /// <param name="map"></param>
        /// <param name="settings"></param>
        /// <param name="parent"></param>
        /// <param name="count"></param>
        public void SpawnBurrows(Vector2Int resolution, float maxTerrainHeight, float waterLevel,
            GeneralSettings generalSettings,
            float[,] map, BurrowSettings settings, Transform parent, int count)
        {
            for (var i = 0; i < count; i++)
            {
                var posFound = false;
                float xPos, zPos;
                Vector3 pos;

                while (!posFound)
                {
                    xPos = UnityEngine.Random.Range(-generalSettings.squareSize * (resolution.x - 1) / 2,
                        generalSettings.squareSize * (resolution.x - 1) / 2);
                    zPos = UnityEngine.Random.Range(-generalSettings.squareSize * (resolution.y - 1) / 2,
                        generalSettings.squareSize * (resolution.y - 1) / 2);

                    pos = GeneratorFunctions.GetSurfacePointFromWorldCoordinate(xPos, zPos, resolution,
                        maxTerrainHeight, map, generalSettings);


                    if (pos.y > maxTerrainHeight * waterLevel /*&& pos.y < maxTerrainHeight * 0.8f*/)
                    {
                        var asset = Object.Instantiate(settings.assetPrefab, pos,
                            Quaternion.Euler(new Vector3(0, UnityEngine.Random.Range(0, 360), 0)));
                        asset.transform.SetParent(parent);

                        settings.assets.Add(asset);

                        posFound = true;
                    }
                }
            }
        }
    }
}