using _Scripts.WorldGeneration.ScriptableObjects;
using UnityEngine;

namespace _Scripts.WorldGeneration.Helper
{
    public static class GeneratorFunctions
    {
        /// <summary>
        ///     Get the height / y coordinate from a Vector2 on the map.
        /// </summary>
        /// <param name="x">The logical x coordinate</param>
        /// <param name="z">The logical z coordinate</param>
        /// <param name="resolution"></param>
        /// <param name="maxTerrainHeight"></param>
        /// <param name="map"></param>
        /// <param name="generalSettings">The general settings of the simulation</param>
        /// <returns>Vector3 in world coordinates</returns>
        private static Vector3 GetSurfacePoint(float x, float z, Vector2Int resolution, float maxTerrainHeight,
            float[,] map, GeneralSettings generalSettings)
        {
            var xLow = Mathf.FloorToInt(x);
            var xHigh = Mathf.CeilToInt(x);
            var zLow = Mathf.FloorToInt(z);
            var zHigh = Mathf.CeilToInt(z);

            // Debug.Log(xLow + ", " + zLow);
            var val00 = map[xLow, zLow];
            var val01 = map[xLow, zHigh];
            var val10 = map[xHigh, zLow];
            var val11 /*, xRange, zRange*/ = map[xHigh, zHigh];
            // xRange = xHigh - xLow;
            // zRange = zHigh - zLow;
            // xRange = 1;
            // zRange = 1;
            // values for:
            //      - r1 val00 -> val10,
            //      - r2 val01 -> val11
            var r1 = (xHigh - x) * val00 + (x - xLow) * val10;
            var r2 = (xHigh - x) * val01 + (x - xLow) * val11;

            var yPos = (zHigh - z) * r1 + (z - zLow) * r2;

            var mapWidth = resolution.x * generalSettings.squareSize;
            var mapHeight = resolution.y * generalSettings.squareSize;

            var xPos = -mapWidth / 2 + x * generalSettings.squareSize + generalSettings.squareSize / 2;
            var zPos = -mapHeight / 2 + z * generalSettings.squareSize + generalSettings.squareSize / 2;
            yPos *= maxTerrainHeight;

            return new Vector3(xPos, yPos, zPos);
        }

        /// <summary>
        ///     Get the height / y coordinate from a Vector2 from world coordinates.
        /// </summary>
        /// <param name="x">The world x coordinate</param>
        /// <param name="z">The world z coordinate</param>
        /// <param name="resolution"></param>
        /// <param name="maxTerrainHeight"></param>
        /// <param name="map"></param>
        /// <param name="generalSettings"></param>
        /// <returns>Vector3 in world coordinates</returns>
        public static Vector3 GetSurfacePointFromWorldCoordinate(float x, float z, Vector2Int resolution,
            float maxTerrainHeight, float[,] map, GeneralSettings generalSettings)
        {
            var mapWidth = resolution.x * generalSettings.squareSize;

            var xPosL = (x + mapWidth / 2 - generalSettings.squareSize / 2) / generalSettings.squareSize;
            var zPosL = (z + mapWidth / 2 - generalSettings.squareSize / 2) / generalSettings.squareSize;

            return GetSurfacePoint(xPosL, zPosL, resolution, maxTerrainHeight, map, generalSettings);
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