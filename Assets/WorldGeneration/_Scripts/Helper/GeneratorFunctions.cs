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
        public static Vector3 GetSurfacePoint(float x, float y, Vector2Int resolution, float maxTerrainHeight, GeneralSettings generalSettings,
            NoiseSettings noiseSettings, NoiseWithClamp noiseWithClamp)
        {
            float xPos = x - (float) resolution.x / 2;
            float zPos = y - (float) resolution.y / 2;
            float yPos = noiseWithClamp.NoiseGenerator.GenerateNoiseValueWithFbm(noiseSettings, xPos, zPos);

            xPos = xPos * generalSettings.squareSize + generalSettings.squareSize / 2.0f;
            zPos = zPos * generalSettings.squareSize + generalSettings.squareSize / 2.0f;
            yPos = noiseWithClamp.ValueClamp.ClampValue(yPos) * maxTerrainHeight;

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
            corners[0] = new Vector2(0, 0); //GetSurfacePoint(0,0, generalSettings, noiseSettings, noiseGenerator, clamp);

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

        private float _minValue = float.MaxValue; // initialize minValue to the biggest possible float value, -> both will be changed

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