using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using Unity.VisualScripting;
using UnityEngine;

public class CardboardDestroyer : MonoBehaviour
{
    public GameObject cardboardPiece;

    private float destroytimer = 0;

    // Start is called before the first frame update
    void Start()
    {
        destroytimer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        destroytimer += Time.deltaTime;
        if (destroytimer > 2)
        {
			SpawnDestroyedCardboard(10, new Vector3(0,0,0.5f));
		}
    }

    public void SpawnDestroyedCardboard(int targetNumPieces, Vector3 spawnOffset)
    {
        // Get object with correct mesh
        var frontFaceChild = transform.Find("CardboardFace");
        if (frontFaceChild == null)
        {
            Debug.Log("No Cardboard to destroy");
            return;
        }
        Mesh cutMesh = frontFaceChild.GetComponent<MeshFilter>().mesh;
        // Translate uvs from sprite mesh to the mesh to be cut TODO
        var mySpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (mySpriteRenderer == null)
        {
            Debug.Log("No sprite to render for destroyed cardboard!");
            return;
        }
        // cut the mesh
        List<Mesh> pieces = CutterMesh.RecursiveCutting(cutMesh, targetNumPieces, new Vector3(0, 0, 1));

        // create game objects for each cut mesh
        Transform coinContainer = ArenaController.Instance.coinContainer;


        foreach (Mesh piece in pieces)
        {

        }
            // cardboard, texture front and backface, colliders
        


    }

}
