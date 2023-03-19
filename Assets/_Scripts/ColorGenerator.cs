using System.Collections.Generic;
using UnityEngine;

namespace _Scripts
{
    public class ColorGenerator
    {
        // private List<Vector3> vertices;

        // Assign the color to the vertex
        public void AssignColor(Gradient gradient, Mesh mesh, float maxHeight)
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
