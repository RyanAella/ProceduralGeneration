using System.Collections.Generic;
using _Scripts.ScriptableObjects;
using UnityEngine;

namespace _Scripts.Generator
{
    /// <summary>
    /// Class <c>MeshGenerator</c> generates the ground mesh based on the Marching Squares Algorithm.
    /// </summary>
    public static class MeshGenerator
    {
        private static SquareGrid _squareGrid;
        private static List<Vector3> _vertices;
        private static List<int> _triangles;
        private static List<Vector2> _uvs;

        /// <summary>
        /// This method generates the mesh.
        /// </summary>
        /// <param name="mesh">The final mesh</param>
        /// <param name="map">The heightmap</param>
        /// <param name="generalSettings">The general settings of the simulation</param>
        public static void GenerateMesh(Mesh mesh, float[,] map, GeneralSettings generalSettings)
        {
            _squareGrid = new SquareGrid(map, generalSettings.maxTerrainHeight, generalSettings.squareSize);
            _vertices = new List<Vector3>();
            _triangles = new List<int>();
            _uvs = new List<Vector2>();

            // Square calculation
            for (int x = 0; x < _squareGrid.squares.GetLength(0); x++)
            {
                for (int y = 0; y < _squareGrid.squares.GetLength(1); y++)
                {
                    TriangulateSquare(_squareGrid.squares[x, y]);
                }
            }

            int resolutionX = map.GetLength(0);
            int resolutionY = map.GetLength(1);

            // UVs
            for (int x = 0; x < resolutionX; x++)
            {
                for (int y = 0; y < resolutionY; y++)
                {
                    _uvs.Add(new Vector2((float)x / resolutionX, (float)y / resolutionY));
                }
            }

            mesh.Clear();

            mesh.vertices = _vertices.ToArray();
            mesh.triangles = _triangles.ToArray();
            mesh.uv = _uvs.ToArray();

            mesh.RecalculateNormals();
        }

        /// <summary>
        /// This method triangulates the squares.
        /// </summary>
        /// <param name="square">The square which needs to be triangulated.</param>
        private static void TriangulateSquare(Square square)
        {
            MeshFromPoints(square.TopLeft, square.TopRight, square.BottomRight, square.BottomLeft);
        }

        /// <summary>
        /// This method generates a mesh from points.
        /// </summary>
        /// <param name="points"></param>
        private static void MeshFromPoints(params Node[] points)
        {
            AssignVertices(points);

            if (points.Length >= 3)
            {
                CreateTriangle(points[0], points[1], points[2]);
            }

            if (points.Length >= 4)
            {
                CreateTriangle(points[0], points[2], points[3]);
            }
        }

        /// <summary>
        /// Assign point as vertex.
        /// </summary>
        /// <param name="points"></param>
        private static void AssignVertices(Node[] points)
        {
            foreach (var point in points)
            {
                if (point.VertexIndex == -1) // if node index unknown (index = -1), add to vertices and add index
                {
                    point.VertexIndex = _vertices.Count;
                    _vertices.Add(point
                        .Position); // add vertex to vertices list, automatically increases vertices count
                }
            }
        }

        /// <summary>
        /// Create triangle from nodes.
        /// </summary>
        /// <param name="a">Node a</param>
        /// <param name="b">Node b</param>
        /// <param name="c">Node c</param>
        private static void CreateTriangle(Node a, Node b, Node c)
        {
            _triangles.Add(a.VertexIndex);
            _triangles.Add(b.VertexIndex);
            _triangles.Add(c.VertexIndex);
        }

        /// <summary>
        /// This class generates a SquareGrid.
        /// </summary>
        private class SquareGrid
        {
            public readonly Square[,] squares;

            /// <param name="map"></param>
            /// <param name="maxHeight"></param>
            /// <param name="squareSize"></param>
            public SquareGrid(float[,] map, float maxHeight, float squareSize)
            {
                int nodeCountX = map.GetLength(0);
                int nodeCountY = map.GetLength(1);
                float mapWidth = nodeCountX * squareSize;
                float mapHeight = nodeCountY * squareSize;

                Node[,] nodes = new Node[nodeCountX, nodeCountY];

                // create control nodes
                for (int x = 0; x < nodeCountX; x++)
                {
                    for (int z = 0; z < nodeCountY; z++)
                    {
                        Vector3 pos = new Vector3(-mapWidth / 2 + x * squareSize + squareSize / 2,
                            map[x, z] * maxHeight, -mapHeight / 2 + z * squareSize + squareSize / 2);
                        nodes[x, z] = new Node(pos);
                    }
                }

                // create squares from nodes
                squares = new Square[nodeCountX - 1, nodeCountY - 1];
                for (int x = 0; x < nodeCountX - 1; x++)
                {
                    for (int y = 0; y < nodeCountY - 1; y++)
                    {
                        squares[x, y] = new Square(nodes[x, y + 1], nodes[x + 1, y + 1],
                            nodes[x + 1, y], nodes[x, y]);
                    }
                }
            }
        }

        /// <summary>
        /// This class generates a Square.
        /// </summary>
        private class Square
        {
            public Node TopLeft, TopRight, BottomRight, BottomLeft;

            public Square(Node topLeft, Node topRight, Node bottomRight, Node bottomLeft)
            {
                TopLeft = topLeft;
                TopRight = topRight;
                BottomRight = bottomRight;
                BottomLeft = bottomLeft;
            }
        }

        private class Node
        {
            public Vector3 Position;
            public int VertexIndex = -1;

            public Node(Vector3 pos)
            {
                Position = pos;
            }
        }
    }
}