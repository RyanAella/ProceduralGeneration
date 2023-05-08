using InGameTime;
using UnityEngine;
using WorldGeneration._Scripts.ScriptableObjects;

namespace WorldGeneration._Scripts.Spawning.TerrainAssets
{
    public class Reed : MonoBehaviour
    {
        // [SerializeField]
        public PlantSettings settings;

        private void Awake()
        {
            var size = gameObject.transform.GetComponent<MeshRenderer>().bounds.size;
            settings.radius = size.x >= size.z ? size.x : size.z;
        }

        private void Start()
        {
        }

        private void Update()
        {
        }

        // private void OnDrawGizmos()
        // {
        //     Gizmos.color = Color.red;
        //     Gizmos.DrawWireSphere(transform.position, settings.radius);
        // }
    }
}