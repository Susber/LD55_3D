using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debris : MonoBehaviour
{
	float myTimer = 0;
	float explosionTime = 3;
	float selfEndTime = 6;
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
			ParticleSystem ps = GetComponent<ParticleSystem>();
			ps.Play();
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
	}
}
