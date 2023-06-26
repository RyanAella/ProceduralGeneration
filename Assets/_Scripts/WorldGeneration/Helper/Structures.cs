using System;
using System.Collections.Generic;
using _Scripts.WorldGeneration.ScriptableObjects;
using _Scripts.WorldGeneration.TerrainGeneration;
using UnityEngine;

namespace _Scripts.WorldGeneration.Helper
{
    /// <summary>
    /// This struct holds the NoiseGenerator and ValueClamp.
    /// </summary>
    public struct NoiseWithClamp
    {
        public NoiseGenerator NoiseGenerator;
        public ValueClamp ValueClamp;
    }

    /// <summary>
    /// This struct holds a dictionary for the plant parent transforms and a list for the plants.
    /// </summary>
    public struct Plants
    {
        public Dictionary<PlantSettings, Transform> PlantParents;
        public List<PlantSettings> PlantsList;
    }

    /// <summary>
    /// This struct holds a dictionary for the burrow parent transforms, a list for the initial burrows and the normal burrows.
    /// </summary>
    public struct Burrows
    {
        public Dictionary<BurrowSettings, Transform> BurrowParents;
        public List<BurrowSettings> BurrowsList; // initial burrows
        public BurrowSettings Burrow;
    }
}