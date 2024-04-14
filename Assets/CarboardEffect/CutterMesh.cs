using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }
    // Update is called once per frame
    void Update()
    {

    }


    private void cutMesh(Mesh mesh, Vector3 cutting_point, Vector3 cutting_normal)
    {
        List<int> triangles = mesh.triangles;
        Vector3[] meshVertices = mesh.vertices;

        for (int i = 0; i < triangles.Length; i += 3)
        {
            Vector3 corner1 = meshVertices[triangles[i]] - cutting_point;
            Vector3 corner2 = meshVertices[triangles[i + 1]] - cutting_point;
            Vector3 corner3 = meshVertices[triangles[i + 2]] - cutting_point;

            float dot1 = Vector3.Dot()
            // auf welcher Seite liegen die Punkte eines Dreiecks?
            // wenn alle jeweils nur auf einer Seite, nichts
            // sonst berechne Schnittpunkte
            // erstelle neue Dreiecke mit den Schnittpunkten
        }
    }
