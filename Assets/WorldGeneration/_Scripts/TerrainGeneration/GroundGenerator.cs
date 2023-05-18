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
        // public
        public static GroundGenerator Instance;

        // [SerializeField]
        [SerializeField] private GameObject wallPrefab;
        [SerializeField] private Gradient gradient;

        // private
        private Vector2[] _boundaries;

        private Mesh _mesh;
        private MeshRenderer _meshRenderer;

        private GameObject _leftWall, _rightWall, _upperWall, _lowerWall;

        // ...
        private bool _running;

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
        /// <param name="generalSettings"></param>
        /// <param name="noiseSettings"></param>
        /// <param name="noiseWithClamp"></param>
        /// <param name="map"></param>
        /// <returns></returns>
        public bool GenerateGround(Vector2Int resolution, float maxTerrainHeight, GeneralSettings generalSettings,
            NoiseSettings noiseSettings, NoiseWithClamp noiseWithClamp, out float[,] map)
        {
            // try
            // {
                _meshRenderer = GetComponent<MeshRenderer>();
                _meshRenderer.enabled = true;

                _mesh = new Mesh
                {
                    name = "Ground Mesh"
                };

                gameObject.tag = "Ground";
                gameObject.layer = LayerMask.NameToLayer("Ground");

                GetComponent<MeshFilter>().mesh = _mesh;
                GetComponent<MeshCollider>().sharedMesh = null;
                GetComponent<MeshCollider>().sharedMesh = _mesh;

                // noiseWithClamp.ValueClamp = new ValueClamp();
                // var worldManager = WorldManager.GetInstance();
                map = new float[resolution.x, resolution.y];

                for (var x = 0; x < resolution.x; x++)
                {
                    for (var y = 0; y < resolution.y; y++)
                    {
                        var sampleX = x - (float)resolution.x / 2;
                        var sampleY = y - (float)resolution.y / 2;

                        map[x, y] = noiseWithClamp.NoiseGenerator.GenerateNoiseValueWithFbm(noiseSettings, sampleX,
                            sampleY);

                        noiseWithClamp.ValueClamp.Compare(map[x, y]);
                    }
                }

                // Get each point back into bounds
                for (var x = 0; x < resolution.x; x++)
                {
                    for (var y = 0; y < resolution.y; y++)
                    {
                        map[x, y] = noiseWithClamp.ValueClamp.ClampValue(map[x, y]);
                    }
                }

                MeshGenerator.GenerateMesh(_mesh, map, maxTerrainHeight, generalSettings);
                ColorGenerator.AssignColor(gradient, _mesh, maxTerrainHeight);

                return true;
            // }
            // catch
            // {
            //     return false;
            // }
   
        }


        /// <summary>
        /// </summary>
        /// <param name="maxTerrainHeight"></param>
        /// <param name="generalSettings"></param>
        /// <param name="resolution"></param>
        public bool GenerateWall(Vector2Int resolution, float maxTerrainHeight,
            GeneralSettings generalSettings)
        {
            // if (WorldManager.GetInstance().WallGenerated)
            // {
            //     Destroy(_leftWall);
            //     Destroy(_rightWall);
            //     Destroy(_upperWall);
            //     Destroy(_lowerWall);
            // }

            // Generate the left wall
            var tempTransform = transform;
            var tempTransformPos = tempTransform.position;
            var squareSize = generalSettings.squareSize;

            var yScale = (generalSettings.maxBorderHeight + maxTerrainHeight) * 2;

            var xPos = tempTransformPos.x - (float)resolution.x / 2 * squareSize + squareSize / 2 -
                       wallPrefab.transform.localScale.x / 2;
            var zPos = tempTransformPos.z;
            var position = new Vector3(xPos, 0, zPos);
            _leftWall = Instantiate(wallPrefab, position, Quaternion.identity, tempTransform); // generate left
            _leftWall.name = "left_wall";
            var objScale = _leftWall.transform.localScale;
            _leftWall.transform.localScale =
                new Vector3(objScale.x, yScale, (resolution.y - 1) * squareSize + objScale.z * 2);


            // Generate the right wall
            position = new Vector3(-xPos, 0, zPos);
            _rightWall = Instantiate(wallPrefab, position, Quaternion.identity, tempTransform); // generate right
            _rightWall.name = "right_wall";
            objScale = _rightWall.transform.localScale;
            _rightWall.transform.localScale =
                new Vector3(objScale.x, yScale, (resolution.y - 1) * squareSize + objScale.z * 2);

            // Generate the upper wall
            xPos = tempTransformPos.x;
            zPos = tempTransformPos.z - (float)resolution.y / 2 * squareSize + squareSize / 2 -
                   wallPrefab.transform.localScale.z / 2;
            position = new Vector3(xPos, 0, -zPos);
            _upperWall = Instantiate(wallPrefab, position, Quaternion.identity, tempTransform); // generate up
            _upperWall.name = "upper_wall";
            objScale = _upperWall.transform.localScale;
            _upperWall.transform.localScale = new Vector3((resolution.x - 1) * squareSize, yScale, objScale.y);

            // Generate the lower wall
            position = new Vector3(xPos, 0, zPos);
            _lowerWall = Instantiate(wallPrefab, position, Quaternion.identity, tempTransform); // generate down
            _lowerWall.name = "lower_wall";
            objScale = _lowerWall.transform.localScale;
            _lowerWall.transform.localScale = new Vector3((resolution.x - 1) * squareSize, yScale, objScale.y);

            return true;
        }
    }
}