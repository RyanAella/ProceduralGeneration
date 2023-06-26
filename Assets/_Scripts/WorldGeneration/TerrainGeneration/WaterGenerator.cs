/*
* Copyright (c) mmq
*/

using _Scripts.WorldGeneration.ScriptableObjects;
using UnityEngine;

namespace _Scripts.WorldGeneration.TerrainGeneration
{
    /// <summary>
    /// This class generates the water map.
    /// </summary>
    [RequireComponent(typeof(MeshCollider))]
    public class WaterGenerator : MonoBehaviour
    {
        #region Variables
        
        // public
        public static WaterGenerator Instance;

        // private
        private float[,] _map;
        private Mesh _mesh;
        private MeshRenderer _meshRenderer;
        private MeshCollider _meshCollider;
        
        #endregion

        #region Unity Methods
        private void Awake()
        {
            if (Instance == null)
            {
                transform.parent = null;
                Instance = this;
            }
            else if (Instance != this)
            {
                Destroy(Instance);
                Instance = this;
            }
        }

        /// <summary>
        /// Generate the water.
        /// </summary>
        /// <param name="resolution">The x and y resolution as Vector2</param>
        /// <param name="maxTerrainHeight">The maximum terrain height as float</param>
        /// <param name="waterLevel">The height of the water level as a percentage of the maximum terrain height in float</param>
        /// <param name="generalSettings">The general settings of the simulation</param>
        public void GenerateWater(Vector2Int resolution, float maxTerrainHeight, float waterLevel,
            GeneralSettings generalSettings)
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

            // Generate the water mesh
            MeshGenerator.GenerateMesh(_mesh, _map, maxTerrainHeight, generalSettings);

            GetComponent<MeshFilter>().mesh = _mesh;
            GetComponent<MeshCollider>().sharedMesh = _mesh;

            // Put it at the right position
            var pos = transform.position;
            transform.position = new Vector3(pos.x, maxTerrainHeight * waterLevel, pos.z);
        }
        
        #endregion
    }
}