using UnityEngine;
using WorldGeneration._Scripts.ScriptableObjects;

namespace WorldGeneration._Scripts.TerrainGeneration
{
    /// <summary>
    /// This class generates the water map.
    /// </summary>
    public class WaterGenerator : MonoBehaviour
    {
        // private
        public static WaterGenerator Instance;

        private float[,] _map;
        private Mesh _mesh;
        private MeshRenderer _meshRenderer;

        private void Awake()
        {
            if (Instance == null)
            {
                transform.parent = null;
                // DontDestroyOnLoad(gameObject);
                Instance = this;
            }
            else if (Instance != this)
            {
                Destroy(Instance);
                Instance = this;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resolution"></param>
        /// <param name="maxTerrainHeight"></param>
        /// <param name="waterLevel"></param>
        /// <param name="generalSettings"></param>
        public bool GenerateWater(Vector2Int resolution, float maxTerrainHeight, float waterLevel, GeneralSettings generalSettings)
        {
            _meshRenderer = GetComponent<MeshRenderer>();
            _meshRenderer.enabled = true;

            _mesh = new Mesh()
            {
                name = "Water Mesh"
            };

            gameObject.tag = "Water";
            gameObject.layer = LayerMask.NameToLayer("Water");

            _map = new float[resolution.x, resolution.y];

            MeshGenerator.GenerateMesh(_mesh, _map, maxTerrainHeight, generalSettings);

            GetComponent<MeshFilter>().mesh = _mesh;

            var pos = transform.position;
            transform.position = new Vector3(pos.x, maxTerrainHeight * waterLevel, pos.z);
            
            return true;
        }
    }
}