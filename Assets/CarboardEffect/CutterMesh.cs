using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

struct MeshConnectedComponents
{
    public List<int> vertexComponentIds;
	public int numComponents;
}

struct MeshBuildingData
{
	public List<Vector3> verts;
	public List<Vector2> uvs;
	public List<int> triangle_indices;
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

    private MeshConnectedComponents ComputeConnectedComponentsAfterCut(Mesh mesh, Vector3 cutting_point, Vector3 cutting_normal)
    {
        MeshConnectedComponents res = new MeshConnectedComponents();
        // todo: implement graph search
        
        return res;
    }

    private void cutMesh(Mesh mesh, Vector3 cutting_point, Vector3 cutting_normal)
    {
        MeshConnectedComponents component_prediction = ComputeConnectedComponentsAfterCut(mesh, cutting_point, cutting_normal);

        List<MeshBuildingData> data = new List<MeshBuildingData>(component_prediction.numComponents);

		for (int i = 0; i < mesh.triangles.Length; i += 3)
        {
            Vector3 corner1 = mesh.vertices[mesh.triangles[i]] - cutting_point;
            Vector3 corner2 = mesh.vertices[mesh.triangles[i + 1]] - cutting_point;
            Vector3 corner3 = mesh.vertices[mesh.triangles[i + 2]] - cutting_point;

            Vector3[] corners = { corner1, corner2, corner3 };

            float dot1 = Vector3.Dot(corner1, cutting_normal);
			float dot2 = Vector3.Dot(corner2, cutting_normal);
			float dot3 = Vector3.Dot(corner3, cutting_normal);
			
            // auf welcher Seite liegen die Punkte eines Dreiecks?
            if (dot1 < 0 &&  dot2 < 0 && dot3 < 0) 
            {
                continue;   
            }

            if (dot1 >= 0 && dot2 >= 0 && dot3 >=0)
            {
                continue;
            }

			// sonst berechne Schnittpunkte
            float sign1 = Mathf.Sign(dot1);
			float sign2 = Mathf.Sign(dot2);
			float sign3 = Mathf.Sign(dot3);

            float[] signs = { sign1, sign2, sign3 };

            int one_side_index = 0;

            if (sign1 == sign2) 
            {
                one_side_index = 2;
            }
            if (sign1 ==sign3)
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
