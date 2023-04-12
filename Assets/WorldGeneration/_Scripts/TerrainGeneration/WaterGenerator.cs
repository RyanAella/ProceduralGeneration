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
                DontDestroyOnLoad(gameObject);
                Instance = this;
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resolution"></param>
        /// <param name="maxTerrainHeight"></param>
        /// <param name="generalSettings"></param>
        public void GenerateWater(Vector2Int resolution, float maxTerrainHeight, GeneralSettings generalSettings)
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

            // // System.Random prng = new System.Random();
            // float prng = Random.Range(0.0f, 1.0f);
            //
            // int width = generalSettings.resolution.x;
            // int height = generalSettings.resolution.y;
            //
            // for (int x = 0; x < width; x++)
            // {
            //     for (int y = 0; y < height; y++)
            //     {
            //         map[x, y] = 0.2f;//Mathf.Lerp(0.0f, 1.0f, (float) prng);
            //     }
            // }

            MeshGenerator.GenerateMesh(_mesh, _map, maxTerrainHeight, generalSettings);

            GetComponent<MeshFilter>().sharedMesh = _mesh;

            transform.position = new Vector3(transform.position.x, maxTerrainHeight / 3.0f,
                transform.position.z);
        }
    }
}