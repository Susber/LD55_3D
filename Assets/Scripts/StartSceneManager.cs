using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class StartSceneManager : MonoBehaviour
{

    public GameObject GrassPrefab;
    public int numGrass;
    public Transform chicki;
    public Transform[] sheeps;

    public float leftRightSpawn;
    public float frontBackSpawn;
    public float zSpawnOffset = 10.0f;
    public float xSpawnOffset = 50.0f;

    public float chickiSpeed;
    public float sheepSpeed;
    public float sheepSpeedVariance;
    public float sheepSpawnDist;
    public float chaseDelay;
    public float chaseDelayVariance;
    public float chaseSceneTime = 15;

    private float[] sheepSpeeds;
    private float chickenWaitTime = 0;
    private float chaseSceneTimer = 0;
    private Vector3 chaseDir = Vector3.left;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < numGrass; i++)
        {
            Vector3 grassPos = new Vector3(Random.Range(-30.0f, 30.0f), 0, Random.Range(-10.0f, 30.0f));
            GameObject grass = Instantiate(GrassPrefab, transform);
            grass.transform.position = grassPos;
        }
		sheepSpeeds = new float[sheeps.Length];
		ChaseSceneStart();   
    }

    // Update is called once per frame
    void Update()
    {
        // move chicken
        if (chickenWaitTime > 0)
        {
			chickenWaitTime -= Time.deltaTime;
		}
        else
        {
			chicki.position = chicki.position + Time.deltaTime * chickiSpeed * chaseDir;
		}
        // move sheep
        for (int i = 0; i < sheeps.Length; i++)
        {
            sheeps[i].position = sheeps[i].position + Time.deltaTime * sheepSpeeds[i] * chaseDir;
        }

        if (chaseSceneTimer > 0)
        {
            chaseSceneTimer -= Time.deltaTime;
        }
        else
        {
            chaseSceneTimer = chaseSceneTime;
            ChaseSceneStart(chaseDir.x > 0);
        }
    }

    private void ChaseSceneStart(bool right = true)
    {
        chaseDir = right ? Vector3.left : Vector3.right;
        float lr = Random.Range(0, leftRightSpawn) + xSpawnOffset;
        if (!right) lr = -lr;
        Vector3 startPos = new Vector3(lr, 0, zSpawnOffset + Random.Range(-frontBackSpawn, frontBackSpawn));

        // Initialize Chickii
        Transform gunTransform = chicki.Find("renderobjects/gunrenderer");

        chicki.position = startPos;
        if (right)
        {
            chicki.rotation = Quaternion.identity;
            gunTransform.localPosition = new Vector3(gunTransform.localPosition.x, gunTransform.localPosition.y, -0.15f);
        }else
        {
			chicki.rotation = Quaternion.Euler(0f, 180f, 0f);
			gunTransform.localPosition = new Vector3(gunTransform.localPosition.x, gunTransform.localPosition.y, 0.15f);
		}

        // Initialize sheep
        int i = 0;
        foreach (Transform sheep in sheeps)
        {
            float sheepZoffset = Random.Range(-sheepSpawnDist, sheepSpawnDist);
            sheep.position = startPos + sheepZoffset * Vector3.forward;
            float mySpeed = sheepSpeed + Random.Range(-sheepSpeedVariance, sheepSpeedVariance);
            sheepSpeeds[i] = mySpeed;

            sheep.rotation = right ? Quaternion.identity : Quaternion.Euler(0f, 180f, 0f);


			i++;
        }

        chickenWaitTime = chaseDelay + Random.Range(-chaseDelayVariance, chaseDelayVariance);
        chaseSceneTimer = chaseSceneTime;
	}



}
