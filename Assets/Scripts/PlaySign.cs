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
			SceneManager.LoadScene(2);
		}
	}

	private void OnMouseDown()
	{
        if (AudioManager.Instance is not null)
		{
            AudioManager.Instance.PlaySoundDestroyEnemy();
            AudioManager.Instance.PlaySoundDeath();
        }
        CardboardDestroyer myDestroyer = transform.GetComponentInChildren<CardboardDestroyer>();
		started = true;
		myDestroyer.SpawnDestroyedCardboard(20, 100 * Vector3.forward);
	}
}
