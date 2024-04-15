using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class MeshConnectedComponents
{
    public List<int> vertexComponentIds;
    public int numComponents;
}

public class MeshBuildingData
{
    public List<Vector3> verts;
    public List<Vector2> uvs;
    public List<int> triangle_indices;
    public int next_id = 0;
}

public class IntersectionData
{
    public Vector3 pos;
    public Vector2 uv;
    public Dictionary<int, int> class_id_to_vertex_id;
}

public class CutterMesh : MonoBehaviour
{
    /*
    
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public class AdjacencyList<K>
    {
        private List<List<K>> _vertexList = new List<List<K>>();
        private Dictionary<K, List<K>> _vertexDict = new Dictionary<K, List<K>>();

        public AdjacencyList(K rootVertexKey)
        {
            AddVertex(rootVertexKey);
        }

        private List<K> AddVertex(K key)
        {
            List<K> vertex = new List<K>();
            _vertexList.Add(vertex);
            _vertexDict.Add(key, vertex);

            return vertex;
        }

        public void AddEdge(K startKey, K endKey)
        {
            List<K> startVertex = _vertexDict.ContainsKey(startKey) ? _vertexDict[startKey] : null;
            List<K> endVertex = _vertexDict.ContainsKey(endKey) ? _vertexDict[endKey] : null;

            if (startVertex == null)
                throw new ArgumentException("Cannot create edge from a non-existent start vertex.");

            if (endVertex == null)
                endVertex = AddVertex(endKey);

            startVertex.Add(endKey);
            endVertex.Add(startKey);
        }

        public void RemoveVertex(K key)
        {
            List<K> vertex = _vertexDict[key];

            //First remove the edges / adjacency entries
            int vertexNumAdjacent = vertex.Count;
            for (int i = 0; i < vertexNumAdjacent; i++)
            {
                K neighbourVertexKey = vertex[i];
                RemoveEdge(key, neighbourVertexKey);
            }

            //Lastly remove the vertex / adj. list
            _vertexList.Remove(vertex);
            _vertexDict.Remove(key);
        }

        public void RemoveEdge(K startKey, K endKey)
        {
            ((List<K>)_vertexDict[startKey]).Remove(endKey);
            ((List<K>)_vertexDict[endKey]).Remove(startKey);
        }

        public bool Contains(K key)
        {
            return _vertexDict.ContainsKey(key);
        }

        public int VertexDegree(K key)
        {
            return _vertexDict[key].Count;
        }
    }

    private MeshConnectedComponents ComputeConnectedComponentsAfterCut(Mesh mesh, Vector3 cutting_point,
        Vector3 cutting_normal)
    {
        MeshConnectedComponents res = new MeshConnectedComponents();
        // todo: implement graph search

        AdjacencyList<int> adjacencyList = new AdjacencyList<int>();

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
                adjacencyList.AddEdge(mesh.triangles[i], mesh.triangles[i + 1])
                continue
            }

            if (sign1 == sign3 && sign1 != sign2)
            {
                adjacencyList.AddEdge(mesh.triangles[i], mesh.triangles[i + 2])
                continue
            }

            adjacencyList.AddEdge(mesh.triangles[i], mesh.triangles[i + 1])
            adjacencyList.AddEdge(mesh.triangles[i + 1], mesh.triangles[i + 2])
            adjacencyList.AddEdge(mesh.triangles[i], mesh.triangles[i + 2])
        }

        // do BFS to find components
        // Mark all the vertices as not visited
        bool[] visited = new bool[vertices.Length];
        for (int i = 0; i < vertices.Length; i++)
            visited[i] = false;

        // save vertexComponentindex
        int[] componentIndex = new int[vertices.Length];

        // Create a queue for BFS
        Queue<int> queue = new Queue<int>();


        int index_vertex = 0;
        int component = 0;

        // Mark the current node as
        // visited and enqueue it
        visited[index_vertex] = true;
        queue.Enqueue(index_vertex);

        while (index < mesh.vertices.Length)
        {

            while (queue.Any())
            {

                // Dequeue a vertex from queue
                // and print it
                s = queue.First();
                Console.Write(s + " ");
                queue.Dequeue();

                // Get all adjacent vertices of the
                // dequeued vertex s. If a adjacent
                // has not been visited, then mark it
                // visited and enqueue it
                AdjacencyList<int> list = adjacencyList[s];

                foreach (var val in list)
                {
                    if (!visited[val])
                    {
                        visited[val] = true;
                        queue.Enqueue(val);
                        componentIndex[val] = component;
                    }
                }
            }

            while (visited[index_vertex])
            {
                index_vertex++;
            }
            visited[index_vertex] = true;
            queue.Enqueue(index_vertex);

            component++;

        }

        res.vertexComponentIds = component;
        res.vertexComponentIds = componentIndex;

        return res;
    }

    private List<Mesh> CutMesh(Mesh mesh, Vector3 cutting_point, Vector3 cutting_normal)
    {
        MeshConnectedComponents component_prediction = ComputeConnectedComponentsAfterCut(mesh, cutting_point, cutting_normal);

        List<MeshBuildingData> mesh_data = new List<MeshBuildingData>(component_prediction.numComponents);

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
            var i2key = new Tuple<int, int>(math.min(ids[c1], ids[c2]), math.max(ids[c1], ids[c2]));

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

    */
}
