using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LetterWiggle : MonoBehaviour
{
    // Start is called before the first frame update

    private float randomOffset;
    private float randomAmp;

    void Start()
    {
        randomOffset = Random.Range(0f, 1f);
        randomAmp = Random.Range(0.1f, 0.3f);
    }

    // Update is called once per frame
    void Update()
    {
        transform.localPosition = Vector3.up * randomAmp * Mathf.Sin(randomOffset + Time.time);
    }
}
