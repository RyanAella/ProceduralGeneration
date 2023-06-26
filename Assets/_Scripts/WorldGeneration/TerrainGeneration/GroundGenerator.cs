/*
* Copyright (c) mmq
*/

using _Scripts.WorldGeneration.Helper;
using _Scripts.WorldGeneration.ScriptableObjects;
using UnityEngine;

namespace _Scripts.WorldGeneration.TerrainGeneration
{
    /// <summary>
    /// This class controls the generation of the ground and water maps and meshes.
    /// </summary>
    [RequireComponent(typeof(MeshFilter))]
    public class GroundGenerator : MonoBehaviour
    {
        #region Variables
        
        // [SerializeField]
        [SerializeField] private GameObject wallPrefab;
        [SerializeField] private GameObject deathFloorPrefab;
        [SerializeField] private Gradient gradient;

        // private
        private static GroundGenerator _instance;
        
        private Vector2[] _boundaries;

        private Mesh _mesh;
        private MeshRenderer _meshRenderer;

        private GameObject _leftWall, _rightWall, _upperWall, _lowerWall;
        private GameObject _deathFloor;

        private bool _running;
        
        #endregion
        
        #region Unity Methods

        private void Awake()
        {
            if (_instance == null)
            {
                transform.parent = null;
                _instance = this;
            }
            else if (_instance != this)
            {
                Destroy(_instance);
                _instance = this;
            }
        }

        /// <summary>
        /// Generate the ground.
        /// </summary>
        /// <param name="resolution">The x and y resolution as Vector2</param>
        /// <param name="maxTerrainHeight">The maximum terrain height as float</param>
        /// <param name="generalSettings">The general settings of the simulation</param>
        /// <param name="noiseSettings">The noise setting needed for the mesh generation</param>
        /// <param name="noiseWithClamp">The noise clamp needed for the mesh generation</param>
        /// <param name="map">The heightmap</param>
        /// <returns>true - if all was successful</returns>
        public bool GenerateGround(Vector2Int resolution, float maxTerrainHeight, GeneralSettings generalSettings,
            NoiseSettings noiseSettings, NoiseWithClamp noiseWithClamp, out float[,] map)
        {
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

            map = new float[resolution.x, resolution.y];

            // for each coordinate get the height value
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

            // Generate the mesh and assign the colour to it
            MeshGenerator.GenerateMesh(_mesh, map, maxTerrainHeight, generalSettings);
            ColorGenerator.AssignColor(gradient, _mesh, maxTerrainHeight);

            return true;
        }


        /// <summary>
        /// Generate the surrounding walls.
        /// </summary>
        /// <param name="maxTerrainHeight">The maximum terrain height as float</param>
        /// <param name="generalSettings">The general settings of the simulation</param>
        /// <param name="resolution">The x and y resolution as Vector2</param>
        public void GenerateWall(Vector2Int resolution, float maxTerrainHeight, GeneralSettings generalSettings)
        {
            var wallScale = wallPrefab.transform.localScale;
            
            // Generate the left wall
            var tempTransform = transform;
            var tempTransformPos = tempTransform.position;
            var squareSize = generalSettings.squareSize;

            var yScale = (generalSettings.maxBorderHeight + maxTerrainHeight) * 2;

            var xPos = tempTransformPos.x - (float)resolution.x / 2 * squareSize + squareSize / 2 - wallScale.x / 2;
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
            zPos = tempTransformPos.z - (float)resolution.y / 2 * squareSize + squareSize / 2 - wallScale.z / 2;
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
        }

        /// <summary>
        /// Generate the death floor beneath the terrain.
        /// </summary>
        /// <param name="resolution">The x and y resolution as Vector2</param>
        /// <param name="generalSettings">The general settings of the simulation</param>
        /// <param name="world">The parent gameObject</param>
        public void GenerateDeathFloor(Vector2Int resolution, GeneralSettings generalSettings, GameObject world)
        {
            var tempTransform = transform;
            var squareSize = generalSettings.squareSize;

            var position = new Vector3(0, -50f, 0);

            _deathFloor = Instantiate(deathFloorPrefab, position, Quaternion.identity, tempTransform);

            _deathFloor.name = "DeathFloor";

            _deathFloor.transform.localScale =
                new Vector3((resolution.x - 1) * squareSize, 1, (resolution.y - 1) * squareSize);
        }
        
        #endregion
    }
}