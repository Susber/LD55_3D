using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlaySign : MonoBehaviour
{
	public float timeUntilGameStart = 3.0f;
	private bool started = false;

	private void Update()
	{
		if (started)
		{
			timeUntilGameStart -= Time.deltaTime;
		}
		if (timeUntilGameStart < 0 )
		{
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
		}
	}

	private void OnMouseDown()
	{
        Debug.Log("Mouse clicked on start sign");
		CardboardDestroyer myDestroyer = transform.GetComponentInChildren<CardboardDestroyer>();
		Debug.Log(myDestroyer);
		started = true;
		myDestroyer.SpawnDestroyedCardboard(20, 100 * Vector3.forward);
	}
}
