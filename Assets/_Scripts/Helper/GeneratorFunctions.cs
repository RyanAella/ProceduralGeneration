using _Scripts.Generator;
using _Scripts.ScriptableObjects;
using UnityEngine;

namespace _Scripts.Helper
{
    public class GeneratorFunctions : MonoBehaviour
    {
        /// <summary>
        /// Get the height of a Vector2 on the map.
        /// </summary>
        /// <param name="x">The x coordinate</param>
        /// <param name="y">The y coordinate</param>
        /// <param name="generalSettings">The general settings of the simulation</param>
        /// <param name="noiseGenerator">A reference to the NoiseGenerator class</param>
        /// <param name="clamp">A reference to the ValueClamp class</param>
        /// <returns></returns>
        public static Vector3 GetSurfacePoint(float x, float y, GeneralSettings generalSettings, NoiseGenerator noiseGenerator, ValueClamp clamp)
        {
            float xPos = x - generalSettings.resolution.x / 2.0f;
            float zPos = y - generalSettings.resolution.y / 2.0f;
            float yPos = noiseGenerator.GenerateNoiseValueWithFbm(xPos, zPos);

            xPos = xPos * generalSettings.squareSize + generalSettings.squareSize / 2.0f;
            zPos = zPos * generalSettings.squareSize + generalSettings.squareSize / 2.0f;
            yPos = clamp.ClampValue(yPos) * generalSettings.maxTerrainHeight;

            return new Vector3(xPos, yPos, zPos);
        }
    }
    
    public class ValueClamp
    {
        // setting min and max value for comparison
        private float _maxValue = float.MinValue; // initialize maxValue to the smallest possible float value, 
        private float _minValue = float.MaxValue; // initialize minValue to the biggest possible float value, -> both will be changed

        /// <summary>
        /// Compares a value with existing min and max values.
        /// </summary>
        /// <param name="value">The value that needs to bee compared</param>
        public void Compare(float value)
        {
            // Get the new min and max values
            if (value > _maxValue)
            {
                _maxValue = value;
            }
            else if (value < _minValue)
            {
                _minValue = value;
            }
        }

        /// <summary>
        /// Clamps a value between existing min and max values.
        /// </summary>
        /// <param name="value">The value that needs to be clamped</param>
        /// <returns></returns>
        public float ClampValue(float value)
        {
            return Mathf.InverseLerp(_minValue, _maxValue, value);
        }
    }
}
