using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinScreenManager : MonoBehaviour
{
	public GameObject GrassPrefab;
	public int numGrass;
	public GameObject opfergabe;

    // Start is called before the first frame update
    void Start()
    {
		for (int i = 0; i < numGrass; i++)
		{
			Vector3 grassPos = new Vector3(Random.Range(-30.0f, 30.0f), 0, Random.Range(-10.0f, 30.0f));
			GameObject grass = Instantiate(GrassPrefab, transform);
			grass.transform.position = grassPos;
		}
	}

    // Update is called once per frame
    void Update()
    {
        int spawnThisFrame = Random.Range(0, 15);
        if (spawnThisFrame == 0 )
        {
            Vector3 spawnPos = new Vector3(Random.Range(-25.0f, 25.0f), Random.Range(10.0f, 30.0f), Random.Range(-5.0f, 20.0f));
			Instantiate(opfergabe, spawnPos, Quaternion.Euler(0,0,Random.Range(135.0f, 225.0f)));
        }
    }
}
