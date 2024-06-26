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

    public GameObject drawnParticlesPrefab;
    public float particlesSystemsPerMeter = 2.0f;
    private int numParticles;
    private List<RuneParticleController> drawnParticles;

	private void Start()
	{
        numParticles = Mathf.CeilToInt( (right - left).magnitude * particlesSystemsPerMeter );
		drawnParticles = new List<RuneParticleController>();
        for (int i = 0; i < numParticles; i++)
        {
            drawnParticles.Add(null);
        }
	}

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
        UpdateDrawParticles();

	}

    public bool IsComplete()
    {
        return rightAlphaActive - leftAlphaActive < 0.15f;
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

    private void TrySpawnParticles(int index)
    {
        if (drawnParticles[index] == null) 
        {
			GameObject currParticles = Instantiate(drawnParticlesPrefab, Vector3.Lerp(left, right, (float)(index) / ((float)(numParticles - 1))), Quaternion.identity);
			currParticles.transform.parent = rune.transform;
            drawnParticles[index] = currParticles.GetComponent<RuneParticleController>();
		}
    }

	public void UpdateDrawParticles()
	{
		var complete = IsComplete();
		
		if (complete)
        {
            for(int i = 0; i < numParticles; i++) { TrySpawnParticles(i); };
			foreach (RuneParticleController pc in drawnParticles)
			{
				pc.ActivateDrawParticles();
			}
			return;
        }
        // activate left part
        int l_idx = 0;
        while (l_idx < Mathf.FloorToInt(leftAlphaActive * (numParticles - 1)))
        {
            TrySpawnParticles(l_idx);
            drawnParticles[l_idx].ActivateDrawParticles();
            l_idx++;
        }

		// activate right part
		int r_idx = numParticles  - 1;
		while (r_idx > Mathf.CeilToInt(rightAlphaActive * (numParticles - 1)))
		{
            TrySpawnParticles(r_idx);
			drawnParticles[r_idx].ActivateDrawParticles();
			r_idx--;
		}

	}

    public void PLayParticleBurst()
    {
		for (int i = 0; i < numParticles; i++) { TrySpawnParticles(i); };
		foreach (RuneParticleController pc in drawnParticles)
		{
            pc.ActivateDrawParticles();
			pc.StopDrawParticleEmission();
            pc.ActivateBurst();
		}
	}

    public void HideLine()
    {
        lineRenderer.enabled = false;
    }
}
