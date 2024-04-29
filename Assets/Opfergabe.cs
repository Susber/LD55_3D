using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Opfergabe : MonoBehaviour
{
    CardboardDestroyer myDestroyer = null;
    private float deleteTimer = 5.0f;
    bool deleteFlag = false;


    // Start is called before the first frame update
    void Start()
    {
		myDestroyer = transform.GetComponentInChildren<CardboardDestroyer>();
        deleteTimer = 5.0f;
        deleteFlag = false;
    }

	private void Update()
	{
	    if (!deleteFlag) { return; }
        if ( deleteTimer < 0)
        {
            Destroy(transform.gameObject);
        }
        deleteTimer -= Time.deltaTime;
    }

	private void OnCollisionEnter(Collision collision)
	{
        myDestroyer.SpawnDestroyedCardboard(20, 50 * Vector3.up);
        this.gameObject.SetActive(false);
		Destroy(transform.gameObject);
	}
}
