using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class RuneLineController : MonoBehaviour
{
    public LineRenderer lineRenderer;

    [FormerlySerializedAs("pentagram")] public RuneController rune;
    public bool[] segmentsActive;
    public Vector3[] segmentCenters;

    public Color inactiveColor;
    public Color activeColor;


    void FixedUpdate()
    {
        var playerPos = PlayerController.Instance.transform.position;
        for (var i = 0; i < segmentCenters.Length; i++)
        {
            if ((playerPos - segmentCenters[i]).sqrMagnitude < 2)
            {
                segmentsActive[i] = true;
                UpdateGradient();
            }
        }
    }

    public void UpdateGradient()
    {
        var numPoints = segmentsActive.Length;
        GradientColorKey[] colorList = new GradientColorKey[numPoints];
        GradientAlphaKey[] colorAlphasList = new GradientAlphaKey[numPoints];

        for (var j = 0; j < numPoints; j++)
        {
            var alpha = (float) j / (numPoints - 1);
            colorList[j] = new GradientColorKey(segmentsActive[j] ? activeColor : inactiveColor, alpha);
            colorAlphasList[j] = new GradientAlphaKey(1.0f, alpha);
        }

        Gradient newGradient = new Gradient();
        newGradient.mode = GradientMode.Fixed;
        newGradient.SetKeys(colorList, colorAlphasList);
        lineRenderer.colorGradient = newGradient;
    }
}
