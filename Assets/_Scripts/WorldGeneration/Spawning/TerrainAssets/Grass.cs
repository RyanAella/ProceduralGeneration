/*
* Copyright (c) mmq
*/

using _Scripts.WorldGeneration.ScriptableObjects;
using UnityEngine;

namespace _Scripts.WorldGeneration.Spawning.TerrainAssets
{
    public class Grass : MonoBehaviour
    {
        public PlantSettings settings;

        private void Awake()
        {
            // Get the size and calculate the radius
            var size = gameObject.transform.GetComponent<MeshRenderer>().bounds.size;
            settings.radius = size.x >= size.z ? size.x : size.z;
        }
    }
}