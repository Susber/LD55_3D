using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LooseScreenManager : MonoBehaviour
{
	public GameObject GrassPrefab;
	public int numGrass;
	public Transform chicki;
	public Transform[] sheeps;

	public float leftRightSpawn = 10;
	public float frontBackSpawn = 15;
	public float zSpawnOffset = 5.0f;
	public float xSpawnOffset = 50.0f;

	public float chickiSpeed = 10;
	public float sheepSpeed = 8;
	public float sheepSpeedVariance = 2;
	public float sheepSpawnDist = 2;
	public float chaseDelay = 2;
	public float chaseDelayVariance = 1;
	public float chaseSceneTime = 20;

	private float[] sheepSpeeds;
	private float sheepWaitTime = 0;
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
		if (sheepWaitTime > 0)
		{
			sheepWaitTime -= Time.deltaTime;
		}
		else
		{
			// move sheep
			for (int i = 0; i < sheeps.Length; i++)
			{
				sheeps[i].position = sheeps[i].position + Time.deltaTime * sheepSpeeds[i] * chaseDir;
			}
			
		}
		// move chickii
		chicki.position = chicki.position + Time.deltaTime * chickiSpeed * chaseDir;

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

		chicki.position = startPos;
		if (right)
		{
			chicki.rotation = Quaternion.identity;
		}
		else
		{
			chicki.rotation = Quaternion.Euler(0f, 180f, 0f);
		}

		// Initialize sheep
		int i = 0;
		foreach (Transform sheep in sheeps)
		{
			// Find sheeps gun
			Transform gunTransform = sheep.Find("renderables/gunrenderer");

			float sheepZoffset = Random.Range(-sheepSpawnDist, sheepSpawnDist);
			sheep.position = startPos + sheepZoffset * Vector3.forward;
			float mySpeed = sheepSpeed + Random.Range(-sheepSpeedVariance, sheepSpeedVariance);
			sheepSpeeds[i] = mySpeed;

			sheep.rotation = right ? Quaternion.identity : Quaternion.Euler(0f, 180f, 0f);
			gunTransform.localPosition = right ? new Vector3(gunTransform.localPosition.x, gunTransform.localPosition.y, -0.15f) : new Vector3(gunTransform.localPosition.x, gunTransform.localPosition.y, 0.15f);

			i++;
		}

		sheepWaitTime = chaseDelay + Random.Range(-chaseDelayVariance, chaseDelayVariance);
		chaseSceneTimer = chaseSceneTime;
	}
}
