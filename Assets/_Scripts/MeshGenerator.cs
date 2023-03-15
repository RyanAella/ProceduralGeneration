using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace _Scripts
{
    // Simple Mesh Generator
    // Generates a plane mesh
    // Based on Marching Squares Algorithm
    public class MeshGenerator
    {
        private SquareGrid squareGrid;
        private List<Vector3> vertices;
        private List<int> triangles;

        private Color[] colors;

        public void GenerateMesh(Mesh mesh, Gradient gradient, float[,] map, float maxHeight, float squareSize)
        {
            squareGrid = new SquareGrid(map, maxHeight, squareSize);

            vertices = new List<Vector3>();
            triangles = new List<int>();

            for (int x = 0; x < squareGrid.squares.GetLength(0); x++)
            {
                for (int y = 0; y < squareGrid.squares.GetLength(1); y++)
                {
                    TriangulateSquare(squareGrid.squares[x, y]);
                }
            }

            AssignColor(gradient, vertices, maxHeight);

            // Mesh mesh = new Mesh();
            mesh.Clear();

            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.colors = colors;

            mesh.RecalculateNormals();
        }

        void TriangulateSquare(Square square)
        {
            MeshFromPoints(square.topLeft, square.topRight, square.bottomRight, square.bottomLeft);

            // switch (square.configuration)
            // {
            //     // case 0:
            //     //     break;
            //     //
            //     // // 1 point active
            //     // case 1: 
            //     //     MeshFromPoints(square.centreBottom, square.bottomLeft, square.centreLeft);
            //     //     break;
            //     // case 2: 
            //     //     MeshFromPoints(square.centreRight, square.bottomRight, square.centreBottom);
            //     //     break;
            //     // case 4: 
            //     //     MeshFromPoints(square.centreTop, square.topRight, square.centreRight);
            //     //     break;
            //     // case 8: 
            //     //     MeshFromPoints(square.topLeft, square.centreTop, square.centreLeft);
            //     //     break;
            //     //
            //     // // 2 points active
            //     // case 3:
            //     //     MeshFromPoints(square.centreRight, square.bottomRight, square.bottomLeft, square.centreLeft);
            //     //     break;
            //     // case 6:
            //     //     MeshFromPoints(square.centreTop, square.topRight, square.bottomRight, square.centreBottom);
            //     //     break;
            //     // case 9:
            //     //     MeshFromPoints(square.topLeft, square.centreTop, square.centreBottom, square.bottomLeft);
            //     //     break;
            //     // case 12:
            //     //     MeshFromPoints(square.topLeft, square.topRight, square.centreRight, square.centreLeft);
            //     //     break;
            //     // case 5:
            //     //     MeshFromPoints(square.centreTop, square.topRight, square.centreRight, square.centreBottom, square.bottomLeft, square.centreLeft);
            //     //     break;
            //     // case 10:
            //     //     MeshFromPoints(square.topLeft, square.centreTop, square.centreRight, square.bottomRight, square.centreBottom, square.centreLeft);
            //     //     break;
            //     //
            //     // // 3 points active
            //     // case 7:
            //     //     MeshFromPoints(square.centreTop, square.topRight, square.bottomRight, square.bottomLeft, square.centreLeft);
            //     //     break;
            //     // case 11:
            //     //     MeshFromPoints(square.topLeft, square.centreTop, square.centreRight, square.bottomRight, square.bottomLeft);
            //     //     break;
            //     // case 13:
            //     //     MeshFromPoints(square.topLeft, square.topRight, square.centreRight, square.centreBottom, square.bottomLeft);
            //     //     break;
            //     // case 14:
            //     //     MeshFromPoints(square.topLeft, square.topRight, square.bottomRight, square.centreBottom, square.centreLeft);
            //     //     break;
            //     
            //     // 4 points active
            //     case 15:
            //         MeshFromPoints(square.topLeft, square.topRight, square.bottomRight, square.bottomLeft);
            //         break;
            //     default:
            //         Debug.LogWarning("Found Square Configuration other than 15: " + square.configuration);
            //         break;
            // }
        }

        void MeshFromPoints(params Node[] points)
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

            // if (points.Length >= 5)
            // {
            //     CreateTriangle(points[0], points[3], points[4]);
            // }
            //
            // if (points.Length >= 6)
            // {
            //     CreateTriangle(points[0], points[4], points[5]);
            // }
        }

        // Assign point as vertex
        void AssignVertices(Node[] points)
        {
            for (int i = 0; i < points.Length; i++)
            {
                if (points[i].vertexIndex == -1) // if node index unknown (index = -1), add to vertices and add index
                {
                    points[i].vertexIndex = vertices.Count;
                    vertices.Add(points[i].position); // add vertex to vertices list, automatically increases vertices count
                }
            }
        }

        // Create triangle from nodes
        void CreateTriangle(Node a, Node b, Node c)
        {
            triangles.Add(a.vertexIndex);
            triangles.Add(b.vertexIndex);
            triangles.Add(c.vertexIndex);
        }

        // Assign the color to the vertex
        void AssignColor(Gradient gradient, List<Vector3> vertices, float maxHeight)
        {
            float min = maxHeight;
            float max = 0;
            colors = new Color[vertices.Count];

            for (int i = 0; i < vertices.Count; i++)
            {
                // Debug.Log("vertices[i].y: " + vertices[i].y);
                float height = Mathf.InverseLerp(0.0f, maxHeight, vertices[i].y);
                colors[i] = gradient.Evaluate(height);

                if (vertices[i].y < min)
                {
                    min = vertices[i].y;
                    // Debug.Log(minHeight);
                }
                if (vertices[i].y > max)
                {
                    max = vertices[i].y;
                    // Debug.Log(minHeight);
                }
            }
            Debug.Log("minHeight: " + min);
            Debug.Log("maxHeight: " + max);
        }

        private class SquareGrid
        {
            public Square[,] squares;

            public SquareGrid(float[,] map, float maxHeight, float squareSize)
            {
                int nodeCountX = map.GetLength(0);
                int nodeCountY = map.GetLength(1);
                float mapWidth = nodeCountX * squareSize;
                float mapHeigth = nodeCountY * squareSize;

                Node[,] nodes = new Node[nodeCountX, nodeCountY];

                // create control nodes
                for (int x = 0; x < nodeCountX; x++)
                {
                    for (int z = 0; z < nodeCountY; z++)
                    {
                        Vector3 pos = new Vector3(-mapWidth / 2 + x * squareSize + squareSize / 2, 
                            (float)map[x, z] * maxHeight, -mapHeigth / 2 + z * squareSize + squareSize / 2);
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

        private class Square
        {
            public Node topLeft, topRight, bottomRight, bottomLeft;
            // public Node centreTop, centreRight, centreBottom, centreLeft;
            // public int configuration;

            public Square(Node _topLeft, Node _topRight, Node _bottomRight, Node _bottomLeft)
            {
                topLeft = _topLeft;
                topRight = _topRight;
                bottomRight = _bottomRight;
                bottomLeft = _bottomLeft;

                // centreTop = topLeft.right;
                // centreRight = bottomRight.above;
                // centreBottom = bottomLeft.right;
                // centreLeft = bottomLeft.above;

                // if (topLeft.active)
                // {
                //     configuration += 8;
                // }
                //
                // if (topRight.active)
                // {
                //     configuration += 4;
                // }
                //
                // if (bottomRight.active)
                // {
                //     configuration += 2;
                // }
                //
                // if (bottomLeft.active)
                // {
                //     configuration += 1;
                // }
            }
        }

        private class Node
        {
            public Vector3 position;
            public int vertexIndex = -1;

            public Node(Vector3 _pos)
            {
                position = _pos;
            }
        }

        // public class ControlNode : Node
        // {
        //     public bool active;
        //     public Node above, right;
        //
        //     public ControlNode(Vector3 _pos, bool _active, float squareSize) : base(_pos)
        //     {
        //         // active = _active;
        //         // above = new Node(position + Vector3.forward * squareSize / 2f);
        //         // right = new Node(position + Vector3.right * squareSize / 2f);
        //     }
        // }
    }
}