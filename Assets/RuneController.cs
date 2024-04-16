using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class RuneController : MonoBehaviour
{
    public Transform linesContainer;
    public GameObject linePrefab;

    public bool startedDrawing = false;
    public bool needsToStartAtEnd = false;

    public SummonEffect summonEffect;
    public RuneLineController[] lineSegments = null;

    private float prepareTimeLeft = 0;
    public float maxPrepareTime = 1;
    private bool summonEffectFinished = false;

    public float runeScale;

    public RuneState currentState = RuneState.DRAWING;

    public class RuneEdges
    {
        public Vector3[] from;
        public Vector3[] to;
        public bool closedLoop;
        public float runeScale;

        public RuneEdges(int n, bool closedLoop, float runeScale)
        {
            from = new Vector3[n];
            to = new Vector3[n];
            this.closedLoop = closedLoop;
            this.runeScale = runeScale;
        }
    }

    public interface RuneType
    {
        public RuneEdges MakeEdges();
    }

    public enum RuneState
    {
        DRAWING,
        PREPARE_SUMMON,
        SUMMONING,
        FINISHED
    }

    public interface SummonEffect
    {
        public void PlayEffect(RuneController rune);
    }
    
    public class SummonBombEffect : SummonEffect
    {
        public void PlayEffect(RuneController rune)
        {
            // todo, animations!
            int level = ArenaController.Instance.upgradeUi.stats[UpgradeUIComponent.SummonBomb];
            var bombPrefab = ArenaController.Instance.bombPrefab;
            var mainExplosion = Instantiate(bombPrefab).GetComponent<BombController>();
            mainExplosion.transform.position = rune.transform.position;
            mainExplosion.Init(Mathf.Lerp(5f, 10f, (level - 1) / 5f), 3f);
            if (level > 2) {
                foreach (var line in rune.lineSegments)
                {
                    var cornerExplosion = Instantiate(bombPrefab).GetComponent<BombController>();
                    cornerExplosion.transform.position = line.left;
                    cornerExplosion.Init(Mathf.Lerp(3f, 8f, (level - 1) / 5f), 3f);
                }
            }
            rune.summonEffectFinished = true;
        }
    }

    public class SummonExplosionsEffect : SummonEffect
    {
        public void PlayEffect(RuneController rune)
        {
            var explosionPrefab = PlayerController.Instance.gun.explosionPrefab;
            foreach (var line in rune.lineSegments)
            {
                var explosion = Instantiate(explosionPrefab).GetComponent<ExplosionController>();
                explosion.Init(line.left, 5, Color.yellow);
            }
            rune.summonEffectFinished = true;
        }
    }
    
    public class SummonGiantEffect : SummonEffect
    {
        public void PlayEffect(RuneController rune)
        {
            var pos = rune.transform.position;
            float radius = rune.runeScale/2;
            int level = ArenaController.Instance.upgradeUi.stats[UpgradeUIComponent.SummonGiant];
            int n_minions = ((int)level/2)+1;

            for (int i = 0; i < level; i++)
            {
                float angle = 360f / n_minions * i;
                var vec = pos + radius * new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));
                MinionController minion = Instantiate(ArenaController.Instance.minionPrefab, ArenaController.Instance.friendContainer).GetComponent<MinionController>();
                minion.Init(level, vec, 10 + level * 5);
            }
            
            
            rune.summonEffectFinished = true;
        }
    }
    
    public class SummonRocketsEffect : SummonEffect
    {
        public void PlayEffect(RuneController rune)
        {
            int level = ArenaController.Instance.upgradeUi.stats[UpgradeUIComponent.SummonShotgun];
            PlayerController.Instance.gun.ChargeWithRockets(3 + level * 4);
            
            rune.summonEffectFinished = true;
        }
    }

    public class TutorialRuneEffect : SummonEffect
    {
        public void PlayEffect(RuneController rune)
        {
            rune.summonEffectFinished = true;
            ArenaController.Instance.SetStage(ArenaController.GameStage.IN_LEVEL);
        }
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

            var edges = new RuneEdges(n, true, radius);
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
    public class Triangle : RuneType {
        int n;
        float radius;

        public Triangle(int n, float radius)
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

            var edges = new RuneEdges(n, true, radius);
            for (var i = 0; i < n; i++)
            {
                var from = circlePos[i];
                var to = circlePos[(i + 1) % n];
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
            var edges = new RuneEdges(4, false, radius);
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
            var edges = new RuneEdges(1, false, radius);
            edges.from[0] = new Vector3(-1 * l, 0, 0);
            edges.to[0] = new Vector3(1 * l, 0, 0);
            return edges;
        }
    }

    public void MakeRuneFromEdges(RuneEdges edges, Vector3 offset) {
        lineSegments = new RuneLineController[edges.from.Length];
        runeScale = edges.runeScale;
        for (var i = 0; i < edges.from.Length; i++)
        {
            var from = edges.from[i] + offset;
            var to = edges.to[i] + offset;
            from.y += 0.001f * (i + 1);  // to prevent y-fighting
            to.y += 0.001f * (i + 1);

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

    public void SetState(RuneState newState)
    {
        switch (newState)
        {
            case RuneState.PREPARE_SUMMON:
                if (AudioManager.Instance is not null)
                    AudioManager.Instance.PlaySoundPentagram();
                prepareTimeLeft = maxPrepareTime;
                break;
            case RuneState.SUMMONING:
                if (AudioManager.Instance is not null)
                    AudioManager.Instance.PlaySoundDeath();
                summonEffectFinished = false;
                summonEffect.PlayEffect(this);
                break; 
            case RuneState.FINISHED:

                Destroy(this.gameObject);
                break;
        }
        this.currentState = newState;
    }

    public void Update()
    {
        switch (currentState)
        {
            case RuneState.DRAWING:
            {
                if (lineSegments != null)
                {
                    var drawingComplete = true;
                    foreach (var segment in lineSegments)
                    {
                        drawingComplete &= segment.IsComplete();
                    }

                    if (drawingComplete)
                    {
                        SetState(RuneState.PREPARE_SUMMON);
                    }
                }

                break;
            }
            case RuneState.PREPARE_SUMMON:
            {
                prepareTimeLeft -= Time.deltaTime;
                if (prepareTimeLeft < 0)
                    SetState(RuneState.SUMMONING);
                break;
            }
            case RuneState.SUMMONING:
            {
                if (summonEffectFinished)
                    SetState(RuneState.FINISHED);
                break;
            }
        }

    }

    public void MaybeDestroyOnWaveBegin()
    {
        if (!startedDrawing)
        {
            Destroy(this.gameObject);
        }
    }
}
