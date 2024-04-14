using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class RuneLineController : MonoBehaviour
{
    public LineRenderer lineRenderer;

    public RuneController rune;
    public Vector3 left;
    public Vector3 right;
    public float leftAlphaActive = 0;
    public float rightAlphaActive = 1;

    public RuneLineController leftNeighbor;
    public RuneLineController rightNeighbor;

    public Color inactiveColor;
    public Color activeColor;

    public float drawThreshold;

    void Update()
    {
        UpdateGradient();
    }

    void FixedUpdate()
    {
        var playerPos = PlayerController.Instance.transform.position;
        playerPos = new Vector3(playerPos.x, 0, playerPos.z);
        var normal = (right - left).normalized;

        var lineLength = (right - left).magnitude;
        var drawAtAlpha = Vector3.Dot(playerPos - left, normal) / lineLength;
        if (drawAtAlpha < -drawThreshold / lineLength || drawAtAlpha > 1 + drawThreshold / lineLength)
        {
            return;
        }

        var distToLineSqr = (playerPos - (left + normal * lineLength * drawAtAlpha)).sqrMagnitude;
        if (distToLineSqr > drawThreshold * drawThreshold)
        {
            return;
        }

        var canDrawLeft = leftAlphaActive > 0 || (leftNeighbor != null && (leftNeighbor.rightAlphaActive < 1 || leftNeighbor.IsComplete()));
        var canDrawRight = rightAlphaActive < 1 || (rightNeighbor != null && (rightNeighbor.leftAlphaActive > 0 || rightNeighbor.IsComplete()));
        if (!rune.startedDrawing)
        {
            if (rune.needsToStartAtEnd)
            {
                canDrawLeft = leftNeighbor == null;
                canDrawRight = rightNeighbor == null;
            }
            else
            {
                canDrawLeft = true;
                canDrawRight = true;
            }
        }
        
        if (canDrawLeft && drawAtAlpha > leftAlphaActive && drawAtAlpha - leftAlphaActive < drawThreshold / lineLength)
        {
            leftAlphaActive = drawAtAlpha;
            rune.startedDrawing = true;
        }
        if (canDrawRight && drawAtAlpha < rightAlphaActive && rightAlphaActive - drawAtAlpha < drawThreshold / lineLength)
        {
            rightAlphaActive = drawAtAlpha;
            rune.startedDrawing = true;
        }

        UpdateGradient();
    }

    public bool IsComplete()
    {
        return rightAlphaActive - leftAlphaActive < 0.9f;
    }

    public void UpdateGradient()
    {
        GradientColorKey[] colorList = new GradientColorKey[3];
        GradientAlphaKey[] colorAlphasList = new GradientAlphaKey[2];

        var complete = IsComplete();

        colorList[0] = new GradientColorKey(complete || leftAlphaActive > 0 ? activeColor : inactiveColor, leftAlphaActive);
        colorList[1] = new GradientColorKey(!complete ? inactiveColor : activeColor, rightAlphaActive);
        colorList[2] = new GradientColorKey(activeColor, 1.0f);
        
        colorAlphasList[0] = new GradientAlphaKey(1.0f, 0.0f);
        colorAlphasList[1] = new GradientAlphaKey(1.0f, 1.0f);

        Gradient newGradient = new Gradient();
        newGradient.mode = GradientMode.Fixed;
        newGradient.SetKeys(colorList, colorAlphasList);
        lineRenderer.colorGradient = newGradient;
    }
}
