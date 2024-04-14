using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuneController : MonoBehaviour
{
    public Transform linesContainer;
    public GameObject linePrefab;
    
    public void MakePentagram(int n, float radius, float segmentLength)
    {
        Vector3[] circlePos = new Vector3[n];
        for (var i = 0; i < n; i++)
        {
            var angle = 2 * Mathf.PI * i / n;
            circlePos[i] = new Vector3(radius * Mathf.Cos(angle), 0, radius * Mathf.Sin(angle));
        }

        for (var i = 0; i < n; i++)
        {
            var from = circlePos[i];
            var to = circlePos[(i + 2) % n];
            
            var numPoints = (int) ((to - from).magnitude / segmentLength + 1);
            if (numPoints > 9)
            {
                numPoints = 9;  // stupid unity limit
            }
            Vector3[] segmentList = new Vector3[numPoints];

            Vector3[] segmentCenters = new Vector3[numPoints - 1];
            
            for (var j = 0; j < numPoints; j++)
            {
                var alpha = (float) j / (numPoints - 1);
                segmentList[j] = (1 - alpha) * from + alpha * to;

                if (j < numPoints - 1)
                {
                    var centerAlpha = (float)(j + 0.5f) / (numPoints - 1);
                    segmentCenters[j] = (1 - centerAlpha) * from + centerAlpha * to;
                }
            }

            var line = Instantiate(linePrefab, linesContainer);
            line.transform.localPosition = Vector3.zero;
            var lineRenderer = line.GetComponent<LineRenderer>();
            
            lineRenderer.positionCount = numPoints;
            lineRenderer.SetPositions(segmentList);

            line.GetComponent<RuneLineController>().rune = this;
            line.GetComponent<RuneLineController>().segmentsActive = new bool[numPoints - 1];
            line.GetComponent<RuneLineController>().segmentCenters = segmentCenters;
            line.GetComponent<RuneLineController>().UpdateGradient();
        }
    }
}
