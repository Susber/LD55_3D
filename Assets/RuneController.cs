using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuneController : MonoBehaviour
{
    public Transform linesContainer;
    public GameObject linePrefab;

    public bool startedDrawing = false;
    public bool needsToStartAtEnd = false;

    public class RuneEdges
    {
        public Vector3[] from;
        public Vector3[] to;
        public bool closedLoop;

        public RuneEdges(int n, bool closedLoop)
        {
            from = new Vector3[n];
            to = new Vector3[n];
            this.closedLoop = closedLoop;
        }
    }

    public interface RuneType
    {
        public RuneEdges MakeEdges();
    }

    public class Pentagram : RuneType {
        int n;
        float radius;

        public Pentagram(int n, float radius)
        {
            this.n = n;
            this.radius = radius;
        }
        public RuneEdges MakeEdges()
        {
            Vector3[] circlePos = new Vector3[n];
            for (var i = 0; i < n; i++)
            {
                var angle = 2 * Mathf.PI * i / n;
                circlePos[i] = new Vector3(radius * Mathf.Cos(angle), 0, radius * Mathf.Sin(angle));
            }

            var edges = new RuneEdges(n, true);
            for (var i = 0; i < n; i++)
            {
                var from = circlePos[(2 * i) % n];
                var to = circlePos[(2 * i + 2) % n];
                edges.from[i] = from;
                edges.to[i] = to;
            }

            return edges;
        }
    }

    public class Estate : RuneType
    {
        float radius;
        public Estate(float radius)
        {
            this.radius = radius;
        }

        public RuneEdges MakeEdges()
        {
            float l = radius;
            var edges = new RuneEdges(4, false);
            edges.from[0] = new Vector3(-1 * l, 0, -1 * l);
            edges.to[0] = new Vector3(1 * l, 0, 1 * l);
            edges.from[1] = new Vector3(1 * l, 0, 1 * l);
            edges.to[1] = new Vector3(0, 0, 2 * l);
            edges.from[2] = new Vector3(0, 0, 2 * l);
            edges.to[2] = new Vector3(-1 * l, 0, 1 * l);
            edges.from[3] = new Vector3(-1 * l, 0, 1 * l);
            edges.to[3] = new Vector3(1 * l, 0, -1 * l);
            return edges;
        }
    }
    
    public class Line : RuneType
    {
        float radius;
        public Line(float radius)
        {
            this.radius = radius;
        }

        public RuneEdges MakeEdges()
        {
            float l = radius;
            var edges = new RuneEdges(1, false);
            edges.from[0] = new Vector3(-1 * l, 0, 0);
            edges.to[0] = new Vector3(1 * l, 0, 0);
            return edges;
        }
    }

    public void MakeRuneFromEdges(RuneEdges edges) {
        RuneLineController[] lineSegments = new RuneLineController[edges.from.Length];
        for (var i = 0; i < edges.from.Length; i++)
        {
            var from = edges.from[i];
            var to = edges.to[i];
            from.y += 0.1f;
            to.y += 0.1f;

            var segmentLength = 0.2f;
            var numPoints = (int) ((to - from).magnitude / segmentLength + 1);
            Vector3[] segmentList = new Vector3[numPoints];

            for (var j = 0; j < numPoints; j++)
            {
                var alpha = (float) j / (numPoints - 1);
                segmentList[j] = (1 - alpha) * from + alpha * to;
            }

            var line = Instantiate(linePrefab, linesContainer);
            line.transform.localPosition = Vector3.zero;
            var lineRenderer = line.GetComponent<LineRenderer>();
            
            lineRenderer.positionCount = segmentList.Length;
            lineRenderer.SetPositions(segmentList);

            var lineController = line.GetComponent<RuneLineController>();
            lineController.rune = this;
            lineController.left = from;
            lineController.right = to;
            lineController.UpdateGradient();
            lineSegments[i] = lineController;
        }

        for (var i = 0; i < edges.from.Length; i++)
        {
            if (edges.closedLoop || i > 0)
                lineSegments[i].leftNeighbor = lineSegments[(i + edges.from.Length - 1) % edges.from.Length];
            if (edges.closedLoop || i < edges.from.Length - 1)
                lineSegments[i].rightNeighbor = lineSegments[(i + 1) % edges.from.Length];
        }
    }
}
