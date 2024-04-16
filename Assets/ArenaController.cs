using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Components;
using UnityEditor.Rendering.Universal;
using Random = System.Random;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using UnityEngine.XR;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;
using UnityEngine.SceneManagement;


public class ArenaController : MonoBehaviour
{
    public enum GameStage
    {
        TUTORIAL,
        IN_LEVEL,
        UPGRADE
    }

    public static ArenaController Instance;

    public Vector3 arenaRadius;

    public Transform enemyContainer;
    public Transform friendContainer;
    public Transform decorationContainer;
    public Transform bigRuneContainer;
    public Transform smallRuneContainer;
    public Transform coinContainer;

    public GameStage currentStage = GameStage.TUTORIAL;

    public Random rnd = new Random();

    // prefabs
    public GameObject sheepPrefab;
    public GameObject foxPrefab;
    public GameObject dogPrefab;
    public GameObject grassPrefab;
    public GameObject treePrefab;
    public GameObject stonePrefab;
    public GameObject bombPrefab;
    public GameObject gunPrefab;
    public GameObject minionPrefab;

    public int num_grass = 5000;
    public int num_trees = 100;
    public int num_stones = 200;

    public GameObject runePrefab;
    public GameObject coinPrefab;

    public List<AbstractWave> levelWaveQueue = new List<AbstractWave>();
    public AbstractWave waitForWaveToFinish = null;
    public float spawnNextWaveTime;

    public int currentLevel = 0;
    public int maxLevel = 10;

    public UpgradeUIComponent upgradeUi;

    public Text healthText;
    public Text moneyText;
    public Text levelText;

    public int numBigRunes;
    public int numSmallRunes;

    public float tutorialRuneRadius;

    public float spawnDistanceToPlayer;

    public GameObject tutorialHud;
    public GameObject inLevelHud;

    private void Start()
    {
        SetStage(GameStage.TUTORIAL); // todo, change back to TUTORIAL
        upgradeUi.DoUpdateStats();

        for (var x = 0; x < num_grass; x++)
        {
            var enemy = Instantiate(grassPrefab, decorationContainer.transform);
            var randomPos = RandomPosOnArena(1f);
            enemy.transform.localPosition = new Vector3(randomPos.x, 0, randomPos.z);
        }
        for (int i = 0; i < num_trees; i++)
        {
            var randomPos = RandomPosOnArena(2f);
            if ((randomPos - transform.position).magnitude >= tutorialRuneRadius * 1.5f)
            {
                var tree = Instantiate(treePrefab, decorationContainer.transform);
                tree.transform.localPosition = new Vector3(randomPos.x, 0, randomPos.z);
            }
        }
        for (int i = 0; i < num_stones; i++)
        {
            var stone = Instantiate(stonePrefab, decorationContainer.transform);
            var randomPos = RandomPosOnArena(2f);
            stone.transform.localPosition = new Vector3(randomPos.x, 0, randomPos.z);
        }

        foreach (Transform t in enemyContainer)
        {
            Destroy(t.gameObject);
        }

        upgradeUi.UpdateUI();
        UpdateHud();
        upgradeUi.gameObject.SetActive(false);
    }


    public ArenaController()
    {
        Instance = this;
    }

    Vector3 RandomBorderPos()
    {
        var bottomLeft = (Vector2)(transform.localPosition - this.transform.localScale / 2);
        var topRight = (Vector2)(transform.localPosition + this.transform.localScale / 2);
        var randomPos = new Vector2(0, 0);
        int numX = rnd.Next(0, 2);
        int numY = rnd.Next(0, 2);
        var multiplier = rnd.NextDouble();
        float distance;
        if (numX == 1)
        {
            if (numY == 1) // left side
            {
                randomPos.x = bottomLeft.x;
            }
            else
            {
                randomPos.x = topRight.x;
            }
            distance = (topRight.y - bottomLeft.y) * (float)multiplier;
            randomPos.y = bottomLeft.y + distance;
        }
        else
        {
            if (numY == 1)  // top side
            {
                randomPos.y = bottomLeft.y;
            }
            else  // bottom side
            {
                randomPos.y = topRight.y;
            }
            distance = (topRight.x - bottomLeft.x) * (float)multiplier;
            randomPos.x = bottomLeft.x + distance;
        }

        return randomPos;
    }


    public Vector3 RandomPosOnArena(float distanceToBorder)
    {
        var origin = transform.position - this.arenaRadius + new Vector3(distanceToBorder, 0, distanceToBorder);
        var radius = arenaRadius - 2 * new Vector3(distanceToBorder, 0, distanceToBorder);
        var vec = origin + new Vector3((float)rnd.NextDouble() * radius.x * 2, 0, (float)rnd.NextDouble() * radius.z * 2);
        return vec;
    }

    public Vector3 RandomPosOnCircle(Vector3 origin, float radius, float distanceToBorder)
    {
        var maxTries = 0;
        while (true)
        {
            maxTries += 1;
            float angle = (float)rnd.NextDouble() * 2 * Mathf.PI;
            var vec = origin + radius * new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));
            var vecD = vec - transform.position;
            if ((Mathf.Abs(vecD.x) >= arenaRadius.x - distanceToBorder ||
                Mathf.Abs(vecD.z) >= arenaRadius.z - distanceToBorder) && maxTries < 1000)
                continue;  // retry, not in arena.
            return vec;
        }
    }

    public Vector3 RandomWavePos(float distToPlayer)
    {
        var existingEnemies = enemyContainer.GetComponentsInChildren<UnitController>();

        Vector3 best = this.transform.position;
        var bestDistSquared = 0f;
        for (var i = 0; i < 20; i++)
        {
            var rndPos = RandomPosOnCircle(PlayerController.Instance.transform.position, distToPlayer, 20f);

            var minDistSquared = Mathf.Infinity;
            foreach (var rune in existingEnemies)
            {
                var existingPos = rune.gameObject.transform.position;
                minDistSquared = Mathf.Min((existingPos - rndPos).sqrMagnitude, minDistSquared);
            }

            if (minDistSquared > bestDistSquared)
            {
                bestDistSquared = minDistSquared;
                best = rndPos;
            }
        }

        return best;
    }

    private void FixedUpdate()
    {
        switch (currentStage)
        {
            case GameStage.TUTORIAL:
                {
                    break;
                }
            case GameStage.IN_LEVEL:
                {
                    // runes
                    if (bigRuneContainer.childCount < numBigRunes)
                        SpawnRune(true);
                    if (smallRuneContainer.childCount < numSmallRunes)
                        SpawnRune(false);

                    // waves
                    var hasNextWave = levelWaveQueue.Count > 0;
                    if (hasNextWave)
                    {
                        var nextWave = levelWaveQueue[0];
                        if (waitForWaveToFinish != null && !waitForWaveToFinish.finished)
                        {
                            return;
                        }
                        spawnNextWaveTime += Time.fixedDeltaTime;
                        if (spawnNextWaveTime >= nextWave.spawnTime)
                        {
                            // spawn next wave
                            levelWaveQueue.RemoveAt(0);
                            nextWave.DoSpawn();
                            waitForWaveToFinish = nextWave;
                            spawnNextWaveTime = 0;
                        }
                    }
                    else
                    {
                        // all waves finished, wait for all enemies to be dead and all coins to be collected.
                        if (enemyContainer.childCount == 0 && coinContainer.childCount == 0)
                        {
                            currentLevel += 1;
                            // todo, if level is >= 10, then play win screen!
                            if (currentLevel >= 10)
                            {
                                SceneManager.LoadScene(3);
                            }
                            else
                            {
                                UpdateHud();
                                SetStage(GameStage.UPGRADE);
                            }


                        }
                    }
                    break;
                }
            case GameStage.UPGRADE:
                {
                    break;
                }
        }
    }

    public GameObject SpawnEnemy(GameObject enemyPrefab, Vector3 pos)
    {
        var enemy = Instantiate(enemyPrefab, enemyContainer);
        enemy.transform.localPosition = new Vector3(pos.x, 0, pos.z);
        return enemy;
    }

    public void SpawnRune(bool big)
    {
        int spawnType;
        RuneController.RuneType runeType;
        RuneController.SummonEffect summonEffect;

        if (big)
        {
            runeType = new RuneController.Pentagram(5, 8f);
            spawnType = UpgradeUIComponent.SummonGiant;
            summonEffect = new RuneController.SummonGiantEffect(upgradeUi.stats[spawnType]);
        }
        else
        {
            if (rnd.NextDouble() < 0.5)
            {
                runeType = new RuneController.Triangle(3, 3f);
                spawnType = UpgradeUIComponent.SummonBomb;
                summonEffect = new RuneController.SummonBombEffect(upgradeUi.stats[spawnType]);
            }
            else
            {
                runeType = new RuneController.Estate(3f);
                spawnType = UpgradeUIComponent.SummonShotgun;
                summonEffect = new RuneController.SummonRocketsEffect(upgradeUi.stats[spawnType]);
            }
        }

        // check if unlocked
        if (upgradeUi.stats[spawnType] == 0)
            return;

        // the actual spawning!!!
        var edges = runeType.MakeEdges();
        var position = ArenaController.Instance.RandomRunePos(edges.runeScale);
        var runeContainer =
            big ? ArenaController.Instance.bigRuneContainer : ArenaController.Instance.smallRuneContainer;
        ActualSpawnRune(runeContainer, position, edges, summonEffect);
    }

    public static void ActualSpawnRune(Transform runeContainer, Vector3 position, RuneController.RuneEdges edges, RuneController.SummonEffect summonEffect)
    {
        var prefab = ArenaController.Instance.runePrefab;
        var rune = Instantiate(prefab, runeContainer);
        rune.transform.localPosition = position;
        var runeController = rune.GetComponent<RuneController>();
        runeController.MakeRuneFromEdges(edges, position);
        runeController.needsToStartAtEnd = !edges.closedLoop;
        runeController.summonEffect = summonEffect;
    }

    public Vector3 RandomRunePos(float newRuneScale)
    {
        var existingRunes = new List<RuneController>();
        foreach (var rune in bigRuneContainer.GetComponentsInChildren<RuneController>())
            existingRunes.Add(rune);
        foreach (var rune in smallRuneContainer.GetComponentsInChildren<RuneController>())
            existingRunes.Add(rune);

        Vector3 best = this.transform.position;
        var bestDistSquared = 0f;
        for (var i = 0; i < 100; i++)
        {
            var rndPos = RandomPosOnArena(5f);
            var minDistSquared = Mathf.Infinity;
            foreach (var rune in existingRunes)
            {
                var existingPos = rune.gameObject.transform.position;
                var scale = rune.runeScale;
                minDistSquared = Mathf.Min((existingPos - rndPos).sqrMagnitude - scale - newRuneScale, minDistSquared);
            }
            minDistSquared = Mathf.Min((PlayerController.Instance.transform.position - rndPos).sqrMagnitude,
                minDistSquared);

            if (minDistSquared > bestDistSquared)
            {
                bestDistSquared = minDistSquared;
                best = rndPos;
            }
        }

        return best;
    }

    public void SetStage(GameStage newStage)
    {
        inLevelHud.SetActive(false);
        tutorialHud.SetActive(false);

        switch (newStage)
        {
            case GameStage.TUTORIAL:
                {
                    tutorialHud.SetActive(true);
                    var runeType = new RuneController.Pentagram(5, tutorialRuneRadius);
                    var edges = runeType.MakeEdges();
                    ActualSpawnRune(
                        this.transform, this.transform.position, edges, new RuneController.TutorialRuneEffect());
                    break;
                }
            case GameStage.IN_LEVEL:
            {
                inLevelHud.SetActive(true);
                foreach (var rune in bigRuneContainer.GetComponentsInChildren<RuneController>())
                    rune.MaybeDestroyOnWaveBegin();
                foreach (var rune in smallRuneContainer.GetComponentsInChildren<RuneController>())
                    rune.MaybeDestroyOnWaveBegin();
                
                // runes
                for (var i = 0 ; i < numBigRunes - bigRuneContainer.childCount; i++)
                    SpawnRune(true);
                for (var i = 0 ; i < numSmallRunes - smallRuneContainer.childCount; i++)
                    SpawnRune(false);
                levelWaveQueue.Clear();
                
                levelWaveQueue.Add(new Wave(0, sheepPrefab, 10));
                for (var n = 0; n < currentLevel + 1; n++)
                {
                    levelWaveQueue.Add(new Wave(5, sheepPrefab, 10));
                }
                if (currentLevel >= 1)
                {
                    levelWaveQueue.Add(new Wave(10, foxPrefab, currentLevel + 2));
                    for (var n = 0; n < currentLevel + 1; n++)
                    {
                        levelWaveQueue.Add(new Wave(5, sheepPrefab, 10));
                        if (currentLevel >= 4)
                            levelWaveQueue.Add(new Wave(10, dogPrefab, currentLevel));
                    }
                }
                break;
            }
            case GameStage.UPGRADE:
                {
                    upgradeUi.gameObject.SetActive(true);
                    upgradeUi.UpdateUI();
                    break;
                }
        }

        currentStage = newStage;
    }

    public void StartPlayingNextLevel()
    {
        ArenaController.Instance.upgradeUi.gameObject.SetActive(false);
        SetStage(GameStage.IN_LEVEL);
    }

    public void UpdateHud()
    {
        healthText.text = "Health: " + PlayerController.Instance.GetHealth();
        moneyText.text = "Money: " + PlayerController.Instance.coins;
        levelText.text = "Level: " + (currentLevel + 1) + "/" + maxLevel;
    }

    public UnitController GetClosestEnemyTo(Vector3 searchCenter)
    {
        float bestDistSqr = Mathf.Infinity;
        UnitController closest = null;
        foreach (var enemy in enemyContainer.GetComponentsInChildren<UnitController>())
        {
            var currDistSqr = (enemy.transform.position - searchCenter).sqrMagnitude;
            if (currDistSqr < bestDistSqr)
            {
                bestDistSqr = currDistSqr;
                closest = enemy;
            }
        }
        return closest;
    }
}
