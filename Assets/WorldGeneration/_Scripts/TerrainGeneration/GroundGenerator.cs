using UnityEngine;
using WorldGeneration._Scripts.Helper;
using WorldGeneration._Scripts.ScriptableObjects;

namespace WorldGeneration._Scripts.TerrainGeneration
{
    /// <summary>
    ///     This class controls the generation of the ground and water maps and meshes.
    /// </summary>
    [RequireComponent(typeof(MeshFilter))]
    public class GroundGenerator : MonoBehaviour
    {
        // [SerializeField]
        [SerializeField] private Gradient gradient;

        // public
        public static GroundGenerator Instance;
        
        // private
        private Vector2[] _boundaries;

        private float[,] _map;
        
        private Mesh _mesh;
        private MeshRenderer _meshRenderer;

        // ...
        private bool _running;

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
        /// <param name="maxTerrainHeight"></param>
        /// <param name="generalSettings"></param>
        /// <param name="noiseWithClamp"></param>
        /// <param name="noiseSettings"></param>
        /// <param name="resolution"></param>
        public void GenerateGround(Vector2Int resolution, float maxTerrainHeight, GeneralSettings generalSettings,
            NoiseSettings noiseSettings, NoiseWithClamp noiseWithClamp)
        {
            _meshRenderer = GetComponent<MeshRenderer>();
            _meshRenderer.enabled = true;

            _mesh = new Mesh
            {
                name = "Ground Mesh"
            };

            gameObject.tag = "Ground";
            gameObject.layer = LayerMask.NameToLayer("Ground");

            GetComponent<MeshFilter>().sharedMesh = _mesh;

            _map = new float[resolution.x, resolution.y];

            for (var x = 0; x < resolution.x; x++)
            {
                for (var y = 0; y < resolution.y; y++)
                {
                    float sampleX = x - resolution.x / 2;
                    float sampleY = y - resolution.y / 2;

                    // _map[x, y] = _noiseGenerator.GenerateNoiseValue(sampleX, sampleY);
                    _map[x, y] = noiseWithClamp.NoiseGenerator.GenerateNoiseValueWithFbm(noiseSettings, sampleX, sampleY);

                    noiseWithClamp.ValueClamp.Compare(_map[x, y]);
                }
            }

            // Get each point back into bounds
                for (var x = 0; x < resolution.x; x++)
                for (var y = 0; y < resolution.y; y++)
                    _map[x, y] = noiseWithClamp.ValueClamp.ClampValue(_map[x, y]);

                MeshGenerator.GenerateMesh(_mesh, _map, maxTerrainHeight, generalSettings);
                ColorGenerator.AssignColor(gradient, _mesh, maxTerrainHeight);
            }
        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="wallPrefab"></param>
        /// <param name="maxTerrainHeight"></param>
        /// <param name="generalSettings"></param>
        /// <param name="resolution"></param>
        public void GenerateWall(GameObject wallPrefab, Vector2Int resolution, float maxTerrainHeight, GeneralSettings generalSettings)
        {
            // Generate the left wall
            var tempTransform = transform;
            var tempTransformPos = tempTransform.position;
            // var resolution = resolution;
            var squareSize = generalSettings.squareSize;

            var yScale = (generalSettings.maxBorderHeight + maxTerrainHeight) * 2;

            var xPos = tempTransformPos.x - (float)resolution.x / 2 * squareSize + squareSize / 2 -
                       wallPrefab.transform.localScale.x / 2;
            var zPos = tempTransformPos.z;
            var position = new Vector3(xPos, 0, zPos);
            var newObject = Instantiate(wallPrefab, position, Quaternion.identity, tempTransform); // generate left
            newObject.name = "left_wall";
            var objScale = newObject.transform.localScale;
            newObject.transform.localScale =
                new Vector3(objScale.x, yScale, (resolution.y - 1) * squareSize + objScale.z * 2);


            // Generate the right wall
            position = new Vector3(-xPos, 0, zPos);
            newObject = Instantiate(wallPrefab, position, Quaternion.identity, tempTransform); // generate right
            newObject.name = "right_wall";
            objScale = newObject.transform.localScale;
            newObject.transform.localScale =
                new Vector3(objScale.x, yScale, (resolution.y - 1) * squareSize + objScale.z * 2);

            // Generate the upper wall
            xPos = tempTransformPos.x;
            zPos = tempTransformPos.z - (float)resolution.y / 2 * squareSize + squareSize / 2 -
                   wallPrefab.transform.localScale.z / 2;
            position = new Vector3(xPos, 0, -zPos);
            newObject = Instantiate(wallPrefab, position, Quaternion.identity, tempTransform); // generate up
            newObject.name = "upper_wall";
            objScale = newObject.transform.localScale;
            newObject.transform.localScale = new Vector3((resolution.x - 1) * squareSize, yScale, objScale.y);

            // Generate the lower wall
            position = new Vector3(xPos, 0, zPos);
            newObject = Instantiate(wallPrefab, position, Quaternion.identity, tempTransform); // generate down
            newObject.name = "lower_wall";
            objScale = newObject.transform.localScale;
            newObject.transform.localScale = new Vector3((resolution.x - 1) * squareSize, yScale, objScale.y);
        }
    }
}