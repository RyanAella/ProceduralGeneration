using System;
using UnityEngine;
using WorldGeneration._Scripts.Helper;
using WorldGeneration._Scripts.ScriptableObjects;
using WorldGeneration._Scripts.TerrainGeneration;

namespace WorldGeneration._Scripts
{
    [Serializable]
    public class TerrainManager
    {
        // private
        private static TerrainManager _instance;
        private GroundGenerator _groundGenerator;
        private WaterGenerator _waterGenerator;
        private NoiseGenerator _noiseGenerator;

        public static TerrainManager GetInstance()
        {
            if (_instance == null)
            {
                _instance = new TerrainManager();
                return _instance;
            }

            return _instance;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="maxTerrainHeight"></param>
        /// <param name="generalSettings"></param>
        /// <param name="noiseSettings"></param>
        /// <param name="wallPrefab"></param>
        /// <param name="resolution"></param>
        /// <param name="noiseWithClamp"></param>
        public void GenerateTerrain(Vector2Int resolution, float maxTerrainHeight, float waterLevel, GeneralSettings generalSettings, NoiseSettings noiseSettings, GameObject wallPrefab, NoiseWithClamp noiseWithClamp)
        {
            _noiseGenerator = new NoiseGenerator();

            _groundGenerator = GroundGenerator.Instance;
            _waterGenerator = WaterGenerator.Instance;
            
            _groundGenerator.GenerateGround(resolution, maxTerrainHeight, generalSettings, noiseSettings, noiseWithClamp);
            _groundGenerator.GenerateWall(wallPrefab, resolution, maxTerrainHeight, generalSettings);

            _waterGenerator = WaterGenerator.Instance;
            _waterGenerator.GenerateWater(resolution, maxTerrainHeight, waterLevel, generalSettings);
        }
    }
}