using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class MeshConnectedComponents
{
    public List<int> vertexComponentIds = new List<int>();
    public int numComponents = 0;
}

public class MeshBuildingData
{
    public List<Vector3> verts = new List<Vector3>();
    public List<Vector2> uvs = new List<Vector2>();
    public List<int> triangle_indices = new List<int>();
    public int next_id = 0;
}

public class IntersectionData
{
    public Vector3 pos = new Vector3();
    public Vector2 uv = new Vector2();
    public Dictionary<int, int> class_id_to_vertex_id = new Dictionary<int, int>();
}

public class CutterMesh : MonoBehaviour
{
    
    // Start is called before the first frame update
    void Start()
    {
        OneCut();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OneCut()
    {
        Mesh mymesh = GetComponent<MeshFilter>().mesh;
        Material myMat = GetComponent<MeshRenderer>().material;

        Vector3 planeCenter = new Vector3(0,0,0);
        Vector3 cuttingDir = new Vector3(1,0,2);
        cuttingDir.Normalize();

        // do the cut 
        var meshesAfterCut = CutMesh(mymesh, planeCenter, cuttingDir);

        // create game objects with new meshes

        int i = 0;
        foreach (var mesh in meshesAfterCut)
        {
			GameObject obj = new GameObject("Cut" + i.ToString());
			obj.transform.parent = transform;
			obj.transform.localPosition = new Vector3(0, 0.2f, 0);
			obj.transform.localEulerAngles = Vector3.zero;
			obj.transform.localScale = Vector3.one;
			MeshFilter meshFilter = obj.AddComponent<MeshFilter>();
			MeshRenderer renderer = obj.AddComponent<MeshRenderer>();
			meshFilter.mesh = mesh;
            renderer.material = myMat;
            i++;
        }
    }
    private void RandomCenterCut(Mesh mesh, Vector3 cuttingPlaneTangent)
    {
        // compute cog
        Vector3 cog = Vector3.zero;
        foreach (Vector3 v in mesh.vertices)
        {
            cog += v;
        }
        cog /= mesh.vertexCount;
        
        // random dir
    }

    public class AdjacencyList
    {
        private Dictionary<int, List<int>> adjList = new Dictionary<int, List<int>>();

        public void AddEdge(int startKey, int endKey)
        {
            if (!adjList.ContainsKey(startKey))
            {
                adjList[startKey]  = new List<int>();
            }
            if (!adjList.ContainsKey(endKey)) 
            {
                adjList[endKey] = new List<int>();
            }

            if (!adjList[startKey].Contains(endKey))
            {
                adjList[startKey].Add(endKey);
            }
			if (!adjList[endKey].Contains(startKey))
			{
				adjList[endKey].Add(startKey);
			}
		}

        public List<int> GetNeighbours(int key)
        {
            if (adjList.ContainsKey(key)) return adjList[key];
            else return null;
        }
    }

    private MeshConnectedComponents ComputeConnectedComponentsAfterCut(Mesh mesh, Vector3 cutting_point,
        Vector3 cutting_normal)
    {
        MeshConnectedComponents res = new MeshConnectedComponents();
        // todo: implement graph search

        AdjacencyList adjacencyList = new AdjacencyList();

        // über jedes Dreieck iterieren
        for (int i = 0; i < mesh.triangles.Length; i += 3)
        {
            Vector3 corner1 = mesh.vertices[mesh.triangles[i]] - cutting_point;
            Vector3 corner2 = mesh.vertices[mesh.triangles[i + 1]] - cutting_point;
            Vector3 corner3 = mesh.vertices[mesh.triangles[i + 2]] - cutting_point;

            float dot1 = Vector3.Dot(corner1, cutting_normal);
            float dot2 = Vector3.Dot(corner2, cutting_normal);
            float dot3 = Vector3.Dot(corner3, cutting_normal);
            float sign1 = Mathf.Sign(dot1);
            float sign2 = Mathf.Sign(dot2);
            float sign3 = Mathf.Sign(dot3);

            if (sign1 == sign2 && sign1 != sign3)
            {
                adjacencyList.AddEdge(mesh.triangles[i], mesh.triangles[i + 1]);
                continue;
            }

            if (sign1 == sign3 && sign1 != sign2)
            {
                adjacencyList.AddEdge(mesh.triangles[i], mesh.triangles[i + 2]);
                continue;
            }

            if (sign2 == sign3 && sign1 != sign2)
            {
                adjacencyList.AddEdge(mesh.triangles[i + 1], mesh.triangles[i + 2]);
                continue;
            }

            adjacencyList.AddEdge(mesh.triangles[i], mesh.triangles[i + 1]);
            adjacencyList.AddEdge(mesh.triangles[i + 1], mesh.triangles[i + 2]);
            adjacencyList.AddEdge(mesh.triangles[i], mesh.triangles[i + 2]);
        }

        // do BFS to find components
        int n = mesh.vertexCount;
		// Mark all the vertices as not visited
		bool[] visited = new bool[n];
        for (int i = 0; i < n; i++)
            visited[i] = false;

        // save vertexComponentindex
        int[] componentIndex = new int[n];
        int component = 0;

        for (int start_index = 0; start_index < n; start_index++)
        {
            if (visited[start_index]) { continue; }
			// start bfs

			// Create a queue for BFS
			Queue<int> queue = new Queue<int>();
			queue.Enqueue(start_index);

            while(queue.Any())
            {
				// Dequeue a vertex from queue
				// and print it
				int curr_index = queue.First();
				queue.Dequeue();

                componentIndex[curr_index] = component;
                visited[curr_index] = true;

                // Get all adjacent vertices of the
                // dequeued vertex s. If a adjacent
                // has not been visited, then mark it
                // visited and enqueue it

                foreach (var val in adjacencyList.GetNeighbours(curr_index))
                {
                    if (!visited[val])
                    {
                        queue.Enqueue(val);
                    }
                }
            }
            component++;
		}

        res.vertexComponentIds = componentIndex.ToList();
        res.numComponents = component;

        return res;
    }

    private List<Mesh> CutMesh(Mesh mesh, Vector3 cutting_point, Vector3 cutting_normal)
    {
        MeshConnectedComponents component_prediction = ComputeConnectedComponentsAfterCut(mesh, cutting_point, cutting_normal);

        Debug.Log(component_prediction.numComponents);

        List<MeshBuildingData> mesh_data = new List<MeshBuildingData>(component_prediction.numComponents);
        for (int i = 0; i < component_prediction.numComponents; i++)
        {
            mesh_data.Add(new MeshBuildingData());
        }

        // wtf why so bad c#
        List<int> old_to_new_ids = new List<int>();
        for (int i = 0; i < mesh.vertexCount; i++)
        {
            old_to_new_ids.Add(-1);
        }

        // store all cut vertices
        var intersection_dict = new Dictionary<Tuple<int, int>, IntersectionData>();

        for (int i = 0; i < mesh.triangles.Length; i += 3)
        {
            Vector3 corner1 = mesh.vertices[mesh.triangles[i]] - cutting_point;
            Vector3 corner2 = mesh.vertices[mesh.triangles[i + 1]] - cutting_point;
            Vector3 corner3 = mesh.vertices[mesh.triangles[i + 2]] - cutting_point;

            Vector3[] corners = { corner1, corner2, corner3 };
            int[] ids = { mesh.triangles[i], mesh.triangles[i + 1], mesh.triangles[i + 2] };
            Vector2[] uvs = { mesh.uv[ids[0]], mesh.uv[ids[1]], mesh.uv[ids[2]] };

            float dot1 = Vector3.Dot(corner1, cutting_normal);
            float dot2 = Vector3.Dot(corner2, cutting_normal);
            float dot3 = Vector3.Dot(corner3, cutting_normal);
            float sign1 = Mathf.Sign(dot1);
            float sign2 = Mathf.Sign(dot2);
            float sign3 = Mathf.Sign(dot3);

            float[] signs = { sign1, sign2, sign3 };

            if (sign1 == sign2 && sign1 == sign3)
            {
                int active_component = component_prediction.vertexComponentIds[ids[0]];
                // add new vertices
                for (int j = 0; j < 3; j++)
                {
                    if (old_to_new_ids[ids[j]] == -1)
                    {
                        old_to_new_ids[ids[j]] = mesh_data[active_component].next_id;
                        mesh_data[active_component].next_id++;
                        // create vertex
                        mesh_data[active_component].verts.Add(corners[j] + cutting_point);
                        // create uvs
                        mesh_data[active_component].uvs.Add(uvs[j]);
                    }
                    // create triangle
                    mesh_data[active_component].triangle_indices.Add(old_to_new_ids[ids[j]]);
                }
                continue;
            }

            int one_side_index = 0;

            if (sign1 == sign2)
            {
                one_side_index = 2;
            }

            if (sign1 == sign3)
            {
                one_side_index = 1;
            }

            int smaller_component_id = component_prediction.vertexComponentIds[ids[one_side_index]];
            int larger_component_id = component_prediction.vertexComponentIds[ids[(one_side_index + 1) % 3]];

            int c1 = one_side_index;
            int c2 = (one_side_index + 1) % 3;
            int c3 = (one_side_index + 2) % 3;

            var i1key = new Tuple<int, int>(math.min(ids[c1], ids[c2]), math.max(ids[c1], ids[c2]));
            var i2key = new Tuple<int, int>(math.min(ids[c1], ids[c3]), math.max(ids[c1], ids[c3]));

            if (!intersection_dict.ContainsKey(i1key))
            {
                float intersection1 = ComputeIntersection(corners[c1], corners[c2], cutting_normal);
                // compute new pos and uv
                Vector3 cutPos = Vector3.Lerp(corners[c1], corners[c2], intersection1);
                Vector2 cutUV = Vector2.Lerp(uvs[c1], uvs[c2], intersection1);
                // add vertices
                mesh_data[smaller_component_id].verts.Add(cutPos);
                mesh_data[smaller_component_id].next_id++;
                mesh_data[larger_component_id].verts.Add(cutPos);
                mesh_data[larger_component_id].next_id++;
                // add uvs in both meshes
                mesh_data[smaller_component_id].uvs.Add(cutUV);
                mesh_data[larger_component_id].uvs.Add(cutUV);
                // store reference to ids
                IntersectionData i1Data = new IntersectionData();
                i1Data.class_id_to_vertex_id.Add(smaller_component_id, mesh_data[smaller_component_id].next_id - 1);
                i1Data.class_id_to_vertex_id.Add(larger_component_id, mesh_data[larger_component_id].next_id - 1);
                intersection_dict.Add(i1key, i1Data);
            }
            if (!intersection_dict.ContainsKey(i2key))
            {
                float intersection2 = ComputeIntersection(corners[c1], corners[c3], cutting_normal);
                // compute new pos and uv
                Vector3 cutPos = Vector3.Lerp(corners[c1], corners[c3], intersection2);
                Vector2 cutUV = Vector2.Lerp(uvs[c1], uvs[c3], intersection2);
                // add vertices
                mesh_data[smaller_component_id].verts.Add(cutPos);
                mesh_data[smaller_component_id].next_id++;
                mesh_data[larger_component_id].verts.Add(cutPos);
                mesh_data[larger_component_id].next_id++;
                // add uvs in both meshes
                mesh_data[smaller_component_id].uvs.Add(cutUV);
                mesh_data[larger_component_id].uvs.Add(cutUV);
                // store reference to ids
                IntersectionData i2Data = new IntersectionData();
                i2Data.class_id_to_vertex_id.Add(smaller_component_id, mesh_data[smaller_component_id].next_id - 1);
                i2Data.class_id_to_vertex_id.Add(larger_component_id, mesh_data[larger_component_id].next_id - 1);
                intersection_dict.Add(i2key, i2Data);
            }
            
            // Erstelle neue Dreiecke für genau zwei Meshes

            // kleine Seite
            if (old_to_new_ids[ids[one_side_index]] == -1)
            {
                old_to_new_ids[ids[one_side_index]] = mesh_data[smaller_component_id].next_id;
                mesh_data[smaller_component_id].next_id++;
                // create vertex
                mesh_data[smaller_component_id].verts.Add(corners[one_side_index]);
                // create uvs
                mesh_data[smaller_component_id].uvs.Add(uvs[one_side_index]);
            }
            // zwei cut vertices sind schon da :O
            mesh_data[smaller_component_id].triangle_indices.Add(old_to_new_ids[ids[c1]]);
            mesh_data[smaller_component_id].triangle_indices.Add(intersection_dict[i1key].class_id_to_vertex_id[smaller_component_id]);
            mesh_data[smaller_component_id].triangle_indices.Add(intersection_dict[i2key].class_id_to_vertex_id[smaller_component_id]);

            Debug.Log("One Triangle on small side");
            Debug.Log(old_to_new_ids[ids[c1]]);
            Debug.Log(intersection_dict[i1key].class_id_to_vertex_id[smaller_component_id]);
			Debug.Log(intersection_dict[i2key].class_id_to_vertex_id[smaller_component_id]);


			// gro�e Seite
			if (old_to_new_ids[ids[c2]] == -1)
            {
                old_to_new_ids[ids[c2]] = mesh_data[larger_component_id].next_id;
                mesh_data[larger_component_id].next_id++;
                // create vertex
                mesh_data[larger_component_id].verts.Add(corners[c2]);
                // create uvs
                mesh_data[larger_component_id].uvs.Add(uvs[c2]);
            }
            if (old_to_new_ids[ids[c3]] == -1)
            {
                old_to_new_ids[ids[c3]] = mesh_data[larger_component_id].next_id;
                mesh_data[larger_component_id].next_id++;
                // create vertex
                mesh_data[larger_component_id].verts.Add(corners[c3]);
                // create uvs
                mesh_data[larger_component_id].uvs.Add(uvs[c3]);
            }
            // zwei cut vertices ... sind schon da! :OO
            mesh_data[larger_component_id].triangle_indices.Add(old_to_new_ids[ids[c2]]);
            mesh_data[larger_component_id].triangle_indices.Add(old_to_new_ids[ids[c3]]);
            mesh_data[larger_component_id].triangle_indices.Add(intersection_dict[i1key].class_id_to_vertex_id[larger_component_id]);
            mesh_data[larger_component_id].triangle_indices.Add(intersection_dict[i1key].class_id_to_vertex_id[larger_component_id]);
            mesh_data[larger_component_id].triangle_indices.Add(old_to_new_ids[ids[c3]]);
            mesh_data[larger_component_id].triangle_indices.Add(intersection_dict[i2key].class_id_to_vertex_id[larger_component_id]);

			Debug.Log("Two Triangles on large side");
			Debug.Log(old_to_new_ids[ids[c2]]);
			Debug.Log(old_to_new_ids[ids[c3]]);
			Debug.Log(intersection_dict[i1key].class_id_to_vertex_id[larger_component_id]);
			Debug.Log(intersection_dict[i1key].class_id_to_vertex_id[larger_component_id]);
			Debug.Log(old_to_new_ids[ids[c3]]);
			Debug.Log(intersection_dict[i2key].class_id_to_vertex_id[larger_component_id]);

		}

        // create meshes
        var meshes = new List<Mesh>();
        foreach (var meshdata in mesh_data)
        {
            Mesh m = new Mesh();
            m.name = "CutMesh";
            m.vertices = meshdata.verts.ToArray();
            m.uv = meshdata.uvs.ToArray();
            m.triangles = meshdata.triangle_indices.ToArray();
            m.RecalculateNormals();
            meshes.Add(m);
        }
        return meshes;
    }

    private float ComputeIntersection(Vector3 point1, Vector3 point2, Vector3 normal)
    {
        Vector3 distance = Vector3.Normalize(point2 - point1);
        return -Vector3.Dot(point1, normal) / Vector3.Dot(distance, normal);
    }
}
