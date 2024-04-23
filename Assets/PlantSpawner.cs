using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class PlantSpawner : MonoBehaviour
{
    public Sprite[] plantSprites;
    public int[] spawnOdds;

    // Start is called before the first frame update
    void Start()
    {
        // Find plant renderer
        var myrenderer = GetComponentInChildren<SpriteRenderer>();

        if (spawnOdds.Length == plantSprites.Length)
        {
            int oddsSum = 0;
            foreach (int odd in  spawnOdds)
            {
                oddsSum += Mathf.Max(0, odd);
            }
            if (oddsSum != 0)
            {
                int randomPlantNum = Random.Range(0, oddsSum);
				oddsSum = 0;
                int curr_i = 0;
				foreach (int odd in spawnOdds)
				{
					oddsSum += Mathf.Max(0, odd);
					if (randomPlantNum < oddsSum)
                    {
                        myrenderer.sprite = plantSprites[curr_i];
                        break;
                    }
                    curr_i++;
                }
			}
		}
		else
        {
			// select random sprite (equally distributed)
			myrenderer.sprite = plantSprites[Random.Range(0, plantSprites.Length)];
		}
        // if enabled, compute cardboard effect
        var myCardboarder = GetComponentInChildren<Cardboarder>();
        if (myCardboarder.isActiveAndEnabled)
        {
			myCardboarder.RedrawCardboard();
		}
	}
}
