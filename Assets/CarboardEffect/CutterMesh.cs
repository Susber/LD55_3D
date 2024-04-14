using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;

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

public class CutterMesh : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private MeshConnectedComponents ComputeConnectedComponentsAfterCut(Mesh mesh, Vector3 cutting_point,
        Vector3 cutting_normal)
    {
        MeshConnectedComponents res = new MeshConnectedComponents();
        // todo: implement graph search

        return res;
    }

    private void cutMesh(Mesh mesh, Vector3 cutting_point, Vector3 cutting_normal)
    {
        MeshConnectedComponents component_prediction = ComputeConnectedComponentsAfterCut(mesh, cutting_point, cutting_normal);

        List<MeshBuildingData> mesh_data = new List<MeshBuildingData>(component_prediction.numComponents);

        // wtf why so bad c#
        List<int> old_to_new_ids = new List<int>();
        for (int i = 0; i < mesh.vertexCount; i++)
        {
            old_to_new_ids.Add(-1);
        }

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

			// add the triangle to one of the meshes
			if (sign1 == sign2 && sign1 == sign3) 
            {
                int active_component = component_prediction.vertexComponentIds[ids[0]];
                // add new vertices
                for (int j = 0; j < 3; j++)
                {
                    if (old_to_new_ids[ids[j]] == -1)
                    {
                        old_to_new_ids[ids[j]] = mesh_data[active_component].next_id;
                        mesh_data[active_component].next_id += 1;
						// create vertex
						mesh_data[active_component].verts.Add(corners[j]);
					}
                    // create triangle
                    mesh_data[active_component].triangle_indices.Add(old_to_new_ids[ids[j]]);
                    // create uvs
                    mesh_data[active_component].uvs.Add(uvs[j]);
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

            Vector3 intersection1 = compute_intersection(corners[one_side_index], corners[one_side_index + 2 % 3], cutting_normal);
            Vector3 intersection2 = compute_intersection(corners[one_side_index], corners[one_side_index + 1 % 3], cutting_normal);

            if (signs[one_side_index] < 0)
            {
                continue;
            }



            // erstelle neue Dreiecke mit den Schnittpunkten
        }
    }

    private Vector3 compute_intersection(Vector3 point1, Vector3 point2, Vector3 normal)
    {
        Vector3 distance = Vector3.Normalize(point1 - point2);
        return point1 + distance * (-Vector3.Dot(point1, normal) / Vector3.Dot(distance, normal));
    }

}
