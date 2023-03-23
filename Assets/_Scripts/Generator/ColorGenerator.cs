using System.Collections.Generic;
using UnityEngine;

namespace _Scripts.Generator
{
    public static class ColorGenerator
    {
        // Assign the color to the vertex
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
    }
}
