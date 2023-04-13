using UnityEngine;
using WorldGeneration._Scripts.ScriptableObjects;
using WorldGeneration._Scripts.TerrainGeneration;

namespace WorldGeneration._Scripts.Helper
{
    public static class GeneratorFunctions
    {
        /// <summary>
        ///     Get the height of a Vector2 on the map.
        /// </summary>
        /// <param name="x">The x coordinate</param>
        /// <param name="y">The y coordinate</param>
        /// <param name="resolution"></param>
        /// <param name="maxTerrainHeight"></param>
        /// <param name="generalSettings">The general settings of the simulation</param>
        /// <param name="noiseSettings"></param>
        /// <param name="noiseWithClamp"></param>
        /// <returns></returns>
        public static Vector3 GetSurfacePoint(float x, float z, Vector2Int resolution, float maxTerrainHeight,
            float[,] map, GeneralSettings generalSettings
            /*NoiseSettings noiseSettings, NoiseWithClamp noiseWithClamp*/)
        {
            // var xPos = x - (float)resolution.x / 2;
            // var zPos = z - (float)resolution.y / 2;
            // var yPos = noiseWithClamp.NoiseGenerator.GenerateNoiseValueWithFbm(noiseSettings, xPos, zPos);
            //
            // // xPos = x * generalSettings.squareSize + generalSettings.squareSize / 2.0f;
            // // zPos = zPos * generalSettings.squareSize + generalSettings.squareSize / 2.0f;
            // yPos = noiseWithClamp.ValueClamp.ClampValue(yPos) /* * maxTerrainHeight*/;
            //
            // var mapWidth = resolution.x * generalSettings.squareSize;
            // var mapHeight = resolution.y * generalSettings.squareSize;
            //
            // var pos = new Vector3(-mapWidth / 2 + x * generalSettings.squareSize + generalSettings.squareSize / 2,
            //     yPos * maxTerrainHeight,
            //     -mapHeight / 2 + z * generalSettings.squareSize + generalSettings.squareSize / 2);

            int xLow, xHigh, zLow, zHigh;

            xLow = Mathf.FloorToInt(x);
            xHigh = Mathf.CeilToInt(x);
            zLow = Mathf.FloorToInt(z);
            zHigh = Mathf.CeilToInt(z);
            
            // Debug.Log(xLow + ", " + zLow);

            float val00, val01, val10, val11, xRange, zRange;
            val00 = map[xLow, zLow];
            val01 = map[xLow, zHigh];
            val10 = map[xHigh, zLow];
            val11 = map[xHigh, zHigh];

            // xRange = xHigh - xLow;
            // zRange = zHigh - zLow;
            xRange = 1;
            zRange = 1;

            // values for:
            //      - r1 val00 -> val10,
            //      - r2 val01 -> val11
            float r1, r2;
            r1 = (xHigh - x) * val00 + (x - xLow) * val10;
            r2 = (xHigh - x) * val01 + (x - xLow) * val11;

            float yPos = (zHigh - z) * r1 + (z - zLow) * r2;

            var mapWidth = resolution.x * generalSettings.squareSize;
            var mapHeight = resolution.y * generalSettings.squareSize;

            float xPos = -mapWidth / 2 + x * generalSettings.squareSize + generalSettings.squareSize / 2;
            float zPos = -mapHeight / 2 + z * generalSettings.squareSize + generalSettings.squareSize / 2;
            yPos *= maxTerrainHeight;
            
            return new Vector3(xPos, yPos, zPos);
        }

        /// <summary>
        /// </summary>
        /// <param name="generalSettings"></param>
        /// <param name="noiseSettings"></param>
        /// <param name="noiseGenerator"></param>
        /// <param name="clamp"></param>
        /// <returns></returns>
        public static Vector2[] GetCornerPoints(GeneralSettings generalSettings, NoiseSettings noiseSettings,
            NoiseWithClamp noiseWithClamp)
        {
            // Fertig bauen, Achtung GeneralSettings no longer working
            var corners = new Vector2[4];

            // lower left corner
            corners[0] =
                new Vector2(0, 0); //GetSurfacePoint(0,0, generalSettings, noiseSettings, noiseGenerator, clamp);

            // // upper left corner
            // var z = generalSettings.resolution.y;
            // corners[1] = GetSurfacePoint(0,z, generalSettings, noiseSettings, noiseGenerator, clamp);
            //
            // // upper right corner
            // var x = generalSettings.resolution.x;
            // corners[2] = GetSurfacePoint(x,0, generalSettings, noiseSettings, noiseGenerator, clamp);
            //
            // // upper right corner
            // x = generalSettings.resolution.x;
            // z = generalSettings.resolution.y;
            // corners[3] = GetSurfacePoint(x,z, generalSettings, noiseSettings, noiseGenerator, clamp);

            return corners;
        }
    }

    public class ValueClamp
    {
        // setting min and max value for comparison
        private float _maxValue = float.MinValue; // initialize maxValue to the smallest possible float value, 

        private float
            _minValue = float
                .MaxValue; // initialize minValue to the biggest possible float value, -> both will be changed

        /// <summary>
        ///     Compares a value with existing min and max values.
        /// </summary>
        /// <param name="value">The value that needs to bee compared</param>
        public void Compare(float value)
        {
            // Get the new min and max values
            if (value > _maxValue)
                _maxValue = value;
            else if (value < _minValue) _minValue = value;
        }

        /// <summary>
        ///     Clamps a value between existing min and max values.
        /// </summary>
        /// <param name="value">The value that needs to be clamped</param>
        /// <returns></returns>
        public float ClampValue(float value)
        {
            return Mathf.InverseLerp(_minValue, _maxValue, value);
        }
    }
}