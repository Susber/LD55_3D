using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debris : MonoBehaviour
{
	public GameObject poofParticles;
	public Vector3 debrisCenter;

	float myTimer = 0;
	float explosionTime = 3f;
	float selfEndTime = 6f;
	bool exploded = false;
	bool deleted = false;

	private void Update()
	{
		myTimer += Time.deltaTime;
		if (myTimer > explosionTime && !exploded)
		{
			exploded = true;
			// delete all children
			for (int i = transform.childCount - 1; i >= 0; i--)
			{
				Destroy(transform.GetChild(i).gameObject);
			}
			var poof = Instantiate(poofParticles, transform);
			poof.transform.localPosition = debrisCenter;
			poof.transform.rotation = Quaternion.identity;
		}

		if (myTimer > selfEndTime && !deleted)
		{
			deleted = true;
			// boom
			GameObject.Destroy(transform.gameObject);
		}
	}

	private void Awake()
	{
		myTimer = 0;
		explosionTime = explosionTime + Random.Range(-0.1f, 0.1f);
		selfEndTime = explosionTime + 2;
	}
}
