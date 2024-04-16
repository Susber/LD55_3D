using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroSystem : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject[] stories;
    private int nextStory = 1;

	void Start()
    {
        stories[0].SetActive(true);
        stories[1].SetActive(false);
		stories[2].SetActive(false);
		stories[3].SetActive(false);
	}

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (nextStory == 4)
            {
                SceneManager.LoadScene(1);
            }
            else {
                stories[nextStory].SetActive(true);
                nextStory++;
            }
        }
    }
}
