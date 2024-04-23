using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Net;
using System.Runtime.InteropServices;
using System.Transactions;
using Unity.VisualScripting;
using UnityEngine;

public class CardboardDestroyer : MonoBehaviour
{
    public GameObject cardboardPiece;
    public Material cardboardMaterial;
    public Material cardboardSideMaterial;
    public float cardboardWidth = 0.1f;
	public float CardboardTexScale = 1f;
	public Color cardboardColor;

    public void SpawnDestroyedCardboard(int targetNumPieces, Vector3 killingKnockback, float _mass = 1.0f)
    {
        // Get object with correct mesh
        var frontFaceChild = transform.Find("renderer/CardboardFace");
        if (frontFaceChild == null)
        {
            Debug.Log("No Cardboard to destroy");
            return;
        }
        Mesh cutMesh = frontFaceChild.GetComponent<MeshFilter>().mesh;
        // cut the mesh
        List<Mesh> pieces = CutterMesh.RecursiveCutting(cutMesh, targetNumPieces, new Vector3(0, 0, 1).normalized);
        // create game objects for each cut mesh
        Transform coinContainer = null;
        if (ArenaController.Instance != null)
        {
            coinContainer = ArenaController.Instance.coinContainer;
        }
        if (coinContainer == null)
        {
            Debug.Log("no coin container found!");
            coinContainer = GameObject.Find("CoinContainer").transform;
        }

        foreach (Mesh piece in pieces)
        {
            if (piece.vertexCount < 3)
            {
                continue;
            }

            Mesh pieceInv = CutterMesh.InvertedMesh(piece);
            Mesh boundary = CreateBoundaryMesh(piece, transform.TransformDirection(new Vector3(0, 0, cardboardWidth)));

            GameObject xpPiece = GameObject.Instantiate(cardboardPiece, coinContainer);
            MeshCollider xpCollider = xpPiece.GetComponent<MeshCollider>();
            Rigidbody xpBody = xpPiece.GetComponent<Rigidbody>();
            Debris xpBodyControlScript = xpPiece.GetComponent<Debris>();
            xpPiece.transform.position = transform.position;
            xpPiece.transform.rotation = transform.rotation;
            xpPiece.transform.localScale = transform.localScale;

            GameObject frontFace = new GameObject("FrontFace");
            GameObject backFace = new GameObject("BackFace");
            GameObject side = new GameObject("Side");

            frontFace.transform.parent = xpPiece.transform;
            frontFace.transform.localPosition = new Vector3(0,0,0);
			frontFace.transform.localEulerAngles = Vector3.zero;
            frontFace.transform.localScale = Vector3.one;
			MeshFilter frontFilter = frontFace.AddComponent<MeshFilter>();
            MeshRenderer frontRenderer = frontFace.AddComponent<MeshRenderer>();
            frontFilter.mesh = piece;
            frontRenderer.material = cardboardMaterial;

            backFace.transform.parent = xpPiece.transform;
            backFace.transform.localPosition = new Vector3(0,0, cardboardWidth);
            backFace.transform.localEulerAngles = Vector3.zero;
            backFace.transform.localScale = Vector3.one;
			MeshFilter frontFilter_back = backFace.AddComponent<MeshFilter>();
			MeshRenderer frontRenderer_back = backFace.AddComponent<MeshRenderer>();
			frontFilter_back.mesh = pieceInv;
			frontRenderer_back.material = cardboardMaterial;

			side.transform.parent = xpPiece.transform;
			side.transform.localPosition = new Vector3(0, 0, 0);
			side.transform.localEulerAngles = Vector3.zero;
			side.transform.localScale = Vector3.one;
			MeshFilter frontFilter_side = side.AddComponent<MeshFilter>();
			MeshRenderer frontRenderer_side = side.AddComponent<MeshRenderer>();
			frontFilter_side.mesh = boundary;
			frontRenderer_side.material = cardboardSideMaterial;

			// Set xp collider and add random force
			xpCollider.sharedMesh = null;
			xpCollider.sharedMesh = frontFilter_side.mesh;
            float forceIntensity = UnityEngine.Random.Range(0.5f, 2f);
            xpBody.mass = _mass;
            Vector3 force = forceIntensity * UnityEngine.Random.onUnitSphere + killingKnockback;
            xpBody.AddForce(force, ForceMode.Force);
            xpBody.AddTorque(forceIntensity * (UnityEngine.Random.onUnitSphere), ForceMode.Force);

			// set debris center for particle effect
			Vector3 cog = Vector3.zero;
			foreach (Vector3 v in boundary.vertices)
			{
				cog += v;
			}
			cog /= boundary.vertexCount;
            xpBodyControlScript.debrisCenter = cog;
		}
        // I dont need to be rendered anymore
        transform.gameObject.SetActive(false);
    }

    private Mesh CreateBoundaryMesh(Mesh mesh, Vector3 offsetVec)
    {
        List<Vector3> boundary = MeshBoundarySequence(mesh);

		List<Vector3> vertices = new List<Vector3>();
		List<Vector2> uvs = new List<Vector2>();
		List<int> triangles = new List<int>();

		float texture_w = cardboardSideMaterial.mainTexture.width;
		float texture_h = cardboardSideMaterial.mainTexture.height;
		float aspect_ratio = texture_w / texture_h;

		float totalDistance = 0;
		for (int i = 0; i < boundary.Count; i++)
		{
			Vector3 current = boundary[i];
			Vector3 next = boundary[(i + 1) % boundary.Count];

			Vector3 current_world = transform.TransformPoint(current);
			Vector3 next_world = transform.TransformPoint(next);

			vertices.Add(transform.InverseTransformPoint(current_world));
			vertices.Add(transform.InverseTransformPoint(next_world));
			vertices.Add(transform.InverseTransformPoint(next_world + offsetVec));
			vertices.Add(transform.InverseTransformPoint(current_world + offsetVec));

			float distance = CardboardTexScale * Vector2.Distance(current_world, next_world) / (cardboardWidth * aspect_ratio);
			float rest = totalDistance - Mathf.Floor(totalDistance);
			totalDistance += distance;

			uvs.Add(new Vector2(rest, 0));
			uvs.Add(new Vector2(rest + distance, 0));
			uvs.Add(new Vector2(rest + distance, 1));
			uvs.Add(new Vector2(rest, 1));

			int startIndex = i * 4;
			triangles.Add(startIndex);
			triangles.Add(startIndex + 1);
			triangles.Add(startIndex + 2);
			triangles.Add(startIndex);
			triangles.Add(startIndex + 2);
			triangles.Add(startIndex + 3);
		}

		Mesh m = new Mesh();
		m.name = "cbBoundaryMesh";
		m.vertices = vertices.ToArray();
		m.uv = uvs.ToArray();
		m.triangles = triangles.ToArray();
		m.RecalculateNormals();

        return m;
	}

    private List<Vector3> MeshBoundarySequence(Mesh mesh)
    {
        List<Vector3> res = new List<Vector3>();

        Dictionary<Tuple<int, int>, int> edge_count = new Dictionary<Tuple<int, int>, int>();
        List<Tuple<int, int>> dirEdges = new List<Tuple<int, int>>();

        // Count edges...
		for (int i = 0; i < mesh.triangles.Length; i += 3)
        {
            int id1 = mesh.triangles[i];
            int id2 = mesh.triangles[i + 1];
            int id3 = mesh.triangles[i + 2];

            dirEdges.Add(new Tuple<int, int>(id1, id2));
			dirEdges.Add(new Tuple<int, int>(id2, id3));
			dirEdges.Add(new Tuple<int, int>(id3, id1));

			// count all edges
			Tuple<int, int> e1 = new Tuple<int, int>(Math.Min(id1, id2), Math.Max(id1, id2));
			Tuple<int, int> e2 = new Tuple<int, int>(Math.Min(id2, id3), Math.Max(id2, id3));
			Tuple<int, int> e3 = new Tuple<int, int>(Math.Min(id3, id1), Math.Max(id3, id1));

            Tuple<int, int>[] edges = { e1, e2, e3 };
            
            foreach (var e in edges)
            {
                if (!edge_count.ContainsKey(e))
                {
                    edge_count.Add(e, 0);
                }
                edge_count[e]++;
            }
		}
        
        // All boundary vertices
        Dictionary<int, int> boundary_verts_with_neighbours = new Dictionary<int, int>();

        int first_vert = -1;

        foreach (var e in dirEdges)
        {
            Tuple<int, int> e_key = new Tuple<int, int>(Math.Min(e.Item1, e.Item2), Math.Max(e.Item1, e.Item2));
            int count = edge_count[e_key];
            if (count > 1) { continue; };

            if (!boundary_verts_with_neighbours.ContainsKey(e.Item1))
            {
                boundary_verts_with_neighbours.Add(e.Item1, e.Item2);
            }

            if (first_vert == -1) { first_vert = e.Item1; };
		}

        // Start at one boundary vertex and create a loop
        int curr_vertex = first_vert;
        res.Add(mesh.vertices[curr_vertex]);
        curr_vertex = boundary_verts_with_neighbours[curr_vertex];
        while (curr_vertex != first_vert)
        {
            res.Add(mesh.vertices[curr_vertex]);
            curr_vertex = boundary_verts_with_neighbours[curr_vertex];
        }
        return res;
    }

}
