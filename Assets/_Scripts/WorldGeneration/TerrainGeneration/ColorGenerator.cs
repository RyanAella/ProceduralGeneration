/*
* Copyright (c) mmq
*/


using UnityEngine;

namespace _Scripts.WorldGeneration.TerrainGeneration
{
    public static class ColorGenerator
    {
        #region Unity Methods
        
        /// <summary>
        /// Assign the color to the vertex.
        /// </summary>
        /// <param name="gradient">The color gradient</param>
        /// <param name="mesh">The mesh</param>
        /// <param name="maxHeight">The maximum terrain height</param>
        public static void AssignColor(Gradient gradient, Mesh mesh, float maxHeight)
        {
            Color[] colors = new Color[mesh.vertices.Length];

            for (int i = 0; i < mesh.vertices.Length; i++)
            {
                float height = Mathf.InverseLerp(0.0f, maxHeight, mesh.vertices[i].y);
                colors[i] = gradient.Evaluate(height);
            }

            mesh.colors = colors;
        }
        
        #endregion
    }
}