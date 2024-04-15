using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantSpawner : MonoBehaviour
{
    public Sprite[] plantSprites;

    // Start is called before the first frame update
    void Start()
    {
        // Find plant renderer
        var myrenderer = GetComponentInChildren<SpriteRenderer>();
        // select random sprite
        myrenderer.sprite = plantSprites[Random.Range(0, plantSprites.Length)];
        // if enabled, compute cardboard effect
        var myCardboarder = GetComponentInChildren<Cardboarder>();
        if (myCardboarder.isActiveAndEnabled)
        {
			myCardboarder.RedrawCardboard();
		}
	}
}
