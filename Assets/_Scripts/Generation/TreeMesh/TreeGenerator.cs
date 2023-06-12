using System.Collections.Generic;
using UnityEngine;

namespace Assets._Scripts.Generation.TreeMesh
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class TreeGenerator : MonoBehaviour
    {
        public GameObject tree;
        // public float firstBranchHeight = 0.2f;
        // public float reductionRate = 0.1f;
        // public int recursiveAspect = 3; // how often the trunk splits up
        public float floorHeight = 1f; // height form vertices on level 1 to vertices on level 2
        [Range(3, 10)] public int baseEdges = 6; // number of edges for the first polygon
        [Range(1, 10)] public int floorCount = 2; // how many floors the tree shall be high
        public float trunkThickness = 5f; // radius

        public Material trunkMaterial;


        // Start is called before the first frame update
        void Start()
        {
            // GenerateTree(baseEdges, trunkThickness);
            Generate();
        }

        void Generate()
        {
            tree = new GameObject("tree");
            Mesh mesh = new Mesh();

            // Vector3[] vertices = new Vector3[baseEdges * floorCount];
            // Vector2[] uv = new Vector2[vertices.Length];
            // int[] triangles = new int[6 * baseEdges * 2];
            List<Vector3> vertices = new List<Vector3>();
            List<int> triangles = new List<int>();

            // angle for each segment in radians
            float angle = 2 * Mathf.PI / baseEdges;
            float angularOffset = 0f;

            List<Vector3[]> arrays = new List<Vector3[]>();
            Vector3[] array1 = new Vector3[baseEdges];
            Vector3[] array2 = new Vector3[baseEdges];

            arrays.Add(array1);
            arrays.Add(array2);

            float y = 0f;
            for (int j = 0; j < floorCount; j++)
            {
                for (int i = 0; i < baseEdges; i++)
                {
                    var x = Mathf.Cos(i * angle + angularOffset) * trunkThickness;
                    var z = Mathf.Sin(i * angle + angularOffset) * trunkThickness;
                    // vertices[3 * j + i] = new Vector3(x, y, z);
                    vertices.Add(new Vector3(x, y, z));
                    // arrays[j][i] = new Vector3(x, y, z);
                }
                y += floorHeight;
            }


            // for (int j = 0; j < floorCount - 1; j++)
            // {
            //     vertices.Add(arrays[0][(0 + j) % baseEdges]);
            //     vertices.Add(arrays[0][(2 + j) % baseEdges]);
            //     vertices.Add(arrays[0 + 1][(2 + j) % baseEdges]);
            //     // vertices.Add(arrays[0 + 1][(2 + j) % baseEdges]);
            //     vertices.Add(arrays[0 + 1][(0 + j) % baseEdges]);
            //     // vertices.Add(arrays[0][(0 + j) % baseEdges]);
            // }

            int k = 0;
            // foreach (var array in arrays)
            // {
            foreach (var ver in vertices)
            {
                GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                sphere.transform.position = ver;
                sphere.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                sphere.name = k.ToString();
                k++;
            }
            // }


            // for (int j = 0; j <= floorCount - 2; j++)
            // {
            //     for (int i = 0; i < baseEdges; i++) // baseEdges * 2 = number of triangles for two floors
            //     {
            //         // triangles[i] = j % baseEdges;
            //         // triangles[i + 1] = j % baseEdges + 1;
            //         // triangles[i + 2] = j % baseEdges + 1 + baseEdges;
            //         // triangles[i + 3] = j % baseEdges + 1 + baseEdges;
            //         // triangles[i + 4] = j % baseEdges + baseEdges;
            //         // triangles[i + 5] = j % baseEdges;
            //         // }
            //         // triangles[6 * i + 0] = (0 + i) % baseEdges;
            //         // triangles[6 * i + 1] = (1 + i) % baseEdges;
            //         // triangles[6 * i + 2] = (4 + i) % (baseEdges * 2);
            //         // triangles[6 * i + 3] = (4 + i) % (baseEdges * 2);
            //         // triangles[6 * i + 4] = (3 + i) % (baseEdges * 2);
            //         // triangles[6 * i + 5] = (0 + i) % baseEdges;
            //         
            //         triangles.Add((j * baseEdges) + (0 + i) % baseEdges);
            //         triangles.Add((j * baseEdges) + /*(2 + i) % baseEdges)*/ (baseEdges + 1) % baseEdges + (int)(baseEdges * 0.5) + i);
            //         triangles.Add((j * baseEdges) + (5 % baseEdges + i) % baseEdges + baseEdges);
            //         triangles.Add((j * baseEdges) + (5 % baseEdges + i) % baseEdges + baseEdges);
            //         triangles.Add((j * baseEdges) + (baseEdges % baseEdges + i) + baseEdges);
            //         triangles.Add((j * baseEdges) + (0 + i) % baseEdges);
            //         
            //         // triangles[i + 6] = 1;
            //         // triangles[i + 7] = 2;
            //         // triangles[i + 8] = 5;
            //
            //         // 0 1 [4]  triangle from floor 0 up to 1
            //         // [4 3] 0  triangle from floor 1 down to 0
            //         // vertices.Add(arrays[0][i]);
            //     }
            // }

            // for (int j = 0; j <= floorCount - 2; j++)
            // {
            //     for (int i = 0; i < baseEdges; i++) // baseEdges * 2 = number of triangles for two floors
            //     {
            //         triangles.Add((j * baseEdges) + (0 + i) % baseEdges);
            //         // triangles.Add((j * baseEdges) + (2 + i) % baseEdges);
            //         triangles.Add((j * baseEdges) + (baseEdges - 1 + i) % baseEdges);
            //         // triangles.Add((j * baseEdges) + (5 % baseEdges + i) % baseEdges + baseEdges);
            //         // triangles.Add((j * baseEdges) + (5 % baseEdges + i) % baseEdges + baseEdges);
            //         triangles.Add((j * baseEdges) + (baseEdges - 1 + i) % baseEdges + baseEdges);
            //         triangles.Add((j * baseEdges) + (baseEdges - 1 + i) % baseEdges + baseEdges);
            //         // triangles.Add((j * baseEdges) + (3 % baseEdges + i) + baseEdges);
            //         triangles.Add((j * baseEdges) + (baseEdges - 1 + i) % baseEdges + 1);
            //         triangles.Add((j * baseEdges) + (0 + i) % baseEdges);
            //     }
            // }
            
            for (int j = 0; j <= floorCount - 2; j++)
            {
                for (int i = 0; i < baseEdges; i++) // baseEdges * 2 = number of triangles for two floors
                {
                    triangles.Add((j * baseEdges) + (0 + i) % baseEdges);
                    triangles.Add((j * baseEdges) + (baseEdges - 1 + i) % baseEdges);
                    triangles.Add((j * baseEdges) + (baseEdges - 1 + i) % baseEdges + baseEdges);
                    triangles.Add((j * baseEdges) + (baseEdges - 1 + i) % baseEdges + baseEdges);
                    triangles.Add((j * baseEdges) + (baseEdges - 1 + i) /*% baseEdges*/ + 1);
                    triangles.Add((j * baseEdges) + (0 + i) % baseEdges);
                }
            }


            mesh.vertices = vertices.ToArray();
            // mesh.uv = uv;
            mesh.triangles = triangles.ToArray();

            tree.AddComponent<MeshFilter>().mesh = mesh;
            tree.AddComponent<MeshRenderer>().materials = new Material[1] { trunkMaterial };
        }
        //
        // private void GenerateTree(int baseEdges, float thickness)
        // {
        //     tree = new GameObject("tree");
        //
        //     int basePolygon = Mathf.Max(3, baseEdges);
        //     Vector3[] startVertices = new Vector3[basePolygon];
        //     Vector2[] startUv = new Vector2[basePolygon];
        //
        //     float angularStep = 2f * Mathf.PI / (float)basePolygon; // Kreisumfang / Anzahl Punkte
        //     
        //     for (int j = 0; j < basePolygon; ++j)
        //     {
        //         Vector3 pos = new Vector3(Mathf.Cos(j * angularStep), 0f, Mathf.Sin(j * angularStep));
        //         startVertices[j] = thickness * (pos);
        //         startUv[j] = new Vector2(j * angularStep, startVertices[j].y);
        //     }
        //
        //     // Mesh mesh = GenBranch(tree, basePolygon, startVertices, startUv, getCloseValue(_trunkThickness), getCloseValue(_floorHeight), getCloseValue(_floorNumber), new Vector3(), Vector3.up, 0f, getCloseValue(_distorsionCone), getCloseValue(_roughness), getCloseValue(_branchDensity), getCloseValue(_recursionLevel));
        //
        //     Vector3 startingPos = new Vector3();
        //     
        //     Mesh mesh = GenBranch(tree, basePolygon, startVertices, startUv, startingPos);
        //     
        //     mesh.RecalculateNormals();
        //
        //     tree.AddComponent<MeshFilter>().mesh = mesh;
        //     tree.AddComponent<MeshRenderer>().materials = new Material[1] { trunkMaterial };
        // }
        //
        // private Mesh GenBranch(GameObject tree, int basePolygon, Vector3[] startVertices, Vector2[] startUv, Vector3 startingPos)
        // {
        //     Mesh mesh = new Mesh();
        //     
        //     float angularOffset = 0f;
        //     
        //     Vector3[] vertices = new Vector3[basePolygon * floorCount];
        //     Vector2[] uv = new Vector2[vertices.Length];
        //     int[] triangles;
        //     
        //     float angularStep = 2f * Mathf.PI / (float)basePolygon; // Kreisumfang / Anzahl Punkte
        //     Vector3 first = new Vector3(Mathf.Cos(angularOffset), 0f, Mathf.Sin(angularOffset));
        //     first = ChangeCoordinates(first, Vector3.up, Vector3.up);
        //     first += startingPos;
        //     
        //     for (int i = 0; i < startVertices.Length; ++i)
        //     {
        //         vertices[i] = startVertices[i];
        //         uv[i] = startUv[i];
        //     }
        //
        //     // for (int i = 0; i < floorCount; i++)
        //     // {
        //     //     int y = i;
        //     //     for (int j = 0; j < basePolygon; j++)
        //     //     {
        //     //         // Vector3 pos = new Vector3(Mathf.Cos(j * angularOffset), y, Mathf.Sin(j * angularOffset));
        //     //         // startVertices[j] = thickness * (pos);
        //     //         // startUv[j] = new Vector2(j * angularOffset, startVertices[j].y);
        //     //         vertices[i] = new Vector3(Mathf.Sin(i * angularStep), y, Mathf.Cos(i * angularStep)) * thickness;
        //     //         uv[i] = new Vector2(1 + Mathf.Sin(i * angularStep), 1 + Mathf.Cos(i * angularStep)) * 0.5f;
        //     //     }
        //     // }
        //
        //     // Vector3 firstVertex = new Vector3(Mathf.Cos(0), 0f, Mathf.Sin(0)) * thickness;
        //
        //     // for (int i = 0; i < baseEdges; i++)
        //     // {
        //     //     vertices[i] = new Vector3(Mathf.Sin(i * angularStep), 0, Mathf.Cos(i * angularStep)) * thickness;
        //     //     uv[i] = new Vector2(1 + Mathf.Sin(i * angularStep), 1 + Mathf.Cos(i * angularStep)) * 0.5f;
        //     // }
        //
        //     // triangles
        //     triangles = new int[6 * (vertices.Length - basePolygon)]; // 3 indices per triangle; 2 triangles per side 
        //     
        //     Vector3 growDirection = Vector3.up;
        //     Vector3 lastCenter = new Vector3(0, 0, 0);
        //     
        //     for (int i = 1; i < vertices.Length / basePolygon; ++i)
        //     {
        //         Vector3 center = lastCenter + floorHeight * growDirection;
        //         lastCenter = center;
        //
        //         for (int j = 0; j < basePolygon; j++)
        //         {
        //             Vector3 pos = new Vector3(Mathf.Cos(j * angularStep + angularOffset), 0f, Mathf.Sin(j * angularStep + angularOffset));
        //             pos = ChangeCoordinates(pos, new Vector3(0f, 1f, 0f), growDirection);
        //             vertices[i * basePolygon + j] = pos + center;
        //             uv[i * basePolygon + j] = new Vector2( j*angularStep, vertices[i * basePolygon + j].y );
        //         
        //             triangles[6 * ((i - 1) * basePolygon + j)]     = (i - 1) * basePolygon + j;
        //             triangles[6 * ((i - 1) * basePolygon + j) + 1] = (i) * basePolygon + j;
        //             triangles[6 * ((i - 1) * basePolygon + j) + 2] = (i - 1) * basePolygon + (j + 1) % basePolygon;
        //             triangles[6 * ((i - 1) * basePolygon + j) + 3] = (i - 1) * basePolygon + (j + 1) % basePolygon;
        //             triangles[6 * ((i - 1) * basePolygon + j) + 4] = (i) * basePolygon + j;
        //             triangles[6 * ((i - 1) * basePolygon + j) + 5] = (i) * basePolygon + (j + 1) % basePolygon;
        //         }
        //     }
        //
        //     mesh.vertices = vertices;
        //     mesh.uv = uv;
        //     mesh.triangles = triangles;
        //
        //     return mesh;
        // }
        //
        // Vector3 ChangeCoordinates(Vector3 input, Vector3 inputNormal, Vector3 newNormal)
        // {
        //     float angle = Vector3.Angle(inputNormal, newNormal);
        //     Vector3 axis = Vector3.Cross(inputNormal, newNormal);
        //     Quaternion rot = Quaternion.AngleAxis(angle, axis);
        //     return rot * input;
        // }
    }
}