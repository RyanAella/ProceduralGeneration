using InGameTime;
using UnityEngine;
using WorldGeneration._Scripts.ScriptableObjects;

namespace WorldGeneration._Scripts.Spawning.TerrainAssets
{
    public class Reed : MonoBehaviour
    {
        public PlantSettings settings;

        private void Awake()
        {
            var size = gameObject.transform.GetComponent<MeshRenderer>().bounds.size;
            settings.radius = size.x >= size.z ? size.x : size.z;
        }
    }
}