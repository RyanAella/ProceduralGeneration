using System.Collections.Generic;
using UnityEngine;
using WorldGeneration._Scripts.ScriptableObjects;
using WorldGeneration._Scripts.TerrainGeneration;

namespace WorldGeneration._Scripts.Helper
{
    public struct NoiseWithClamp
    {
        public NoiseGenerator NoiseGenerator;
        public ValueClamp ValueClamp;
    }

    public struct Plants
    {
        public Dictionary<AssetSettings, Transform> PlantParents;
        public List<AssetSettings> PlantsList;
    }
    
    public struct Burrows
    {
        public Dictionary<BurrowSettings, Transform> BurrowParents;
        public List<BurrowSettings> BurrowsList;
        public BurrowSettings Burrow;
    }
}
