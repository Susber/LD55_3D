using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Transactions;
using Unity.VisualScripting;
using UnityEngine;

public class CardboardDestroyer : MonoBehaviour
{
    public GameObject cardboardPiece;
    public Material cardboardMaterial;
    public Material cardboardSideMaterial;
    public Color cardboardColor;

    private float destroytimer = 0;
    private bool destroyed = false;

    // Start is called before the first frame update
    void Start()
    {
        destroytimer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (destroyed)
        {
            return;
        }
        destroytimer += Time.deltaTime;
        if (destroytimer > 1)
        {
            Debug.Log("Destrooyyy!!");
			destroyed = true;
			SpawnDestroyedCardboard(10);
            transform.gameObject.SetActive(false);
		}
    }

    public void SpawnDestroyedCardboard(int targetNumPieces)
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
        List<Mesh> pieces = CutterMesh.RecursiveCutting(cutMesh, targetNumPieces, new Vector3(0, 0, 1).normalized);

        // create game objects for each cut mesh
        Transform coinContainer = null; //ArenaController.Instance.coinContainer;

        if (coinContainer == null)
        {
            Debug.Log("no coin container found!");
            coinContainer = GameObject.Find("CoinContainer").transform;
        }

        foreach (Mesh piece in pieces)
        {
            Mesh pieceInv = CutterMesh.InvertedMesh(piece);

            GameObject xpPiece = GameObject.Instantiate(cardboardPiece, coinContainer);
            MeshCollider xpCollider = xpPiece.GetComponent<MeshCollider>();
            Rigidbody xpBody = xpPiece.GetComponent<Rigidbody>();
            xpPiece.transform.position = frontFaceChild.position;
            xpPiece.transform.rotation = frontFaceChild.rotation;
            xpPiece.transform.localScale = frontFaceChild.localScale;

            GameObject frontFace = new GameObject("FrontFace");

            frontFace.transform.parent = xpPiece.transform;
            frontFace.transform.localPosition = Vector3.zero + new Vector3(0,0,0.1f);
			frontFace.transform.localEulerAngles = Vector3.zero;
            frontFace.transform.localScale = Vector3.one;
			MeshFilter frontFilter = frontFace.AddComponent<MeshFilter>();
            MeshRenderer frontRenderer = frontFace.AddComponent<MeshRenderer>();
            frontFilter.mesh = pieceInv;
            frontRenderer.material = cardboardMaterial;

            // Set xp collider and add random force
            xpCollider.sharedMesh = null;
			xpCollider.sharedMesh = frontFilter.mesh;
            float forceIntensity = Random.Range(0.1f, 1f);
            xpBody.mass = 0.5f;
            xpBody.AddForce(forceIntensity * (Vector3.up + Random.onUnitSphere).normalized, ForceMode.Impulse);
		}
            // cardboard, texture front and backface, colliders
        


    }

}
