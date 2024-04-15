using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Components;
using Components.Levels;
using UnityEditor.Rendering.Universal;
using Random = System.Random;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using UnityEngine.XR;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class ArenaController : MonoBehaviour
{
    public enum GameStage
    {
        IN_LEVEL,
        UPGRADE
    }
    
    public static ArenaController Instance;

    public Vector3 arenaRadius;
    
    public Transform enemyContainer;
    public Transform decorationContainer;
    public Transform runeContainer;
    public Transform coinContainer;
    
    public GameStage currentStage = GameStage.IN_LEVEL;

    public Random rnd = new Random();

    // prefabs
    public GameObject sheepPrefab;
    public GameObject foxPrefab;
    public GameObject grassPrefab;

    public GameObject runePrefab;
    public GameObject coinPrefab;

    public List<AbstractWave> levelWaveQueue = new List<AbstractWave>();
    public AbstractWave waitForWaveToFinish = null;
    public float spawnNextWaveTime;

    public int currentLevel;
    public int maxLevel = 10;

    public UpgradeUIComponent upgradeUi;

    public Text healthText;
    public Text moneyText;
    public Text levelText;

    public int numRunes;
    
    private void Start()
    {
        SetStage(GameStage.IN_LEVEL);
        
        for (var x = 0; x < 5000; x++)
        {
            var enemy = Instantiate(grassPrefab, decorationContainer.transform);
            var randomPos = RandomPosOnArena(1f);
            enemy.transform.localPosition = new Vector3(randomPos.x, 0.2f, randomPos.z);
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
        var bottomLeft = (Vector2) (transform.localPosition - this.transform.localScale / 2);
        var topRight = (Vector2) (transform.localPosition + this.transform.localScale / 2);
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
            distance = (topRight.x - bottomLeft.x) * (float) multiplier;
            randomPos.x = bottomLeft.x + distance;
        }

        return randomPos;
    }
    
    
    public Vector3 RandomPosOnArena(float distanceToBorder)
    {
        var origin = transform.position - this.arenaRadius + new Vector3(distanceToBorder, 0, distanceToBorder);
        var radius = arenaRadius - 2 * new Vector3(distanceToBorder, 0, distanceToBorder);
        return origin + new Vector3((float)rnd.NextDouble() * radius.x * 2,0, (float)rnd.NextDouble() * radius.z * 2);
    }

    public Vector3 RandomEmptyPos(float freeCircleRadius, float distToPlayer)
    {
        var freeCircleRadiusSqr = freeCircleRadius * freeCircleRadius;
        for (var numTry = 0; numTry < 20; numTry++)
        {
            var rndPos = RandomPosOnArena(freeCircleRadius);
            var minDistSquared = Mathf.Infinity;
            foreach (Transform child in enemyContainer.transform)
            {
                minDistSquared = Mathf.Min(minDistSquared, (child.localPosition - rndPos).sqrMagnitude);
            }

            var playerDist = (PlayerController.Instance.transform.localPosition - rndPos).sqrMagnitude;
            if (minDistSquared >= freeCircleRadiusSqr && playerDist >= distToPlayer)
            {
                return rndPos;
            }
        }
        // fail, relax.
        return RandomEmptyPos(freeCircleRadius - 3, distToPlayer - 3);
    }

    private void FixedUpdate()
    {
        switch (currentStage)
        {
            case GameStage.IN_LEVEL:
            {
                // runes
                if (runeContainer.childCount < numRunes)
                {
                    // spawn another rune.
                    SpawnRune();
                }
                
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
                        UpdateHud();
                        SetStage(GameStage.UPGRADE);
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

    public void SpawnRune()
    {
        // bit hacky, but okay.
        int spawnType = UpgradeUIComponent.Meteor;
        if (upgradeUi.stats[UpgradeUIComponent.Minions] > 0 && upgradeUi.stats[UpgradeUIComponent.Poop] > 0)
        {
            var r = rnd.NextDouble();
            if (r > 0.66666)
                spawnType = UpgradeUIComponent.Poop;
            else if (r > 0.333333)
                spawnType = UpgradeUIComponent.Minions;
            else
                spawnType = UpgradeUIComponent.Meteor;
        } else if (upgradeUi.stats[UpgradeUIComponent.Minions] > 0)
        {
            spawnType = rnd.NextDouble() > 0.5 ? UpgradeUIComponent.Minions : UpgradeUIComponent.Meteor;
        } else if (upgradeUi.stats[UpgradeUIComponent.Poop] > 0)
        {
            spawnType = rnd.NextDouble() > 0.5 ? UpgradeUIComponent.Poop : UpgradeUIComponent.Meteor;
        }
        else
        {
            spawnType = UpgradeUIComponent.Meteor;
        }

        float rad = 3f + (float)(rnd.NextDouble() * (upgradeUi.stats[spawnType] - 1) * 3 / 5f);

        RuneController.RuneType runeType = null;
        switch (spawnType)
        {
            case UpgradeUIComponent.Meteor:
            {
                runeType = new RuneController.Pentagram(5, rad);
                break;
            }
            case UpgradeUIComponent.Minions:
            {
                runeType = new RuneController.Estate(rad);
                break;
            }
            case UpgradeUIComponent.Poop:
            {
                // todooooo!
                runeType = new RuneController.Estate(rad);
                break;
            }
        }
        new SpawnRune(0, runeType, new RuneController.SummonMeteoroidEffect()).DoSpawn();
    }

    public Vector3 RandomRunePos()
    {
        var existingRunes = new List<Vector3>();
        foreach (var rune in runeContainer.GetComponentsInChildren<RuneController>())
        {
            existingRunes.Add(rune.gameObject.transform.position);
        }
        var origin = transform.position - this.arenaRadius;
        existingRunes.Add(origin);
        existingRunes.Add(origin + 2 * arenaRadius.x * Vector3.left);
        existingRunes.Add(origin + 2 * arenaRadius.y * Vector3.forward);
        existingRunes.Add(origin + 2 * arenaRadius.x * Vector3.left + 2 * arenaRadius.x * Vector3.left);

        Vector3 best = origin;
        var bestDistSquared = 0f;
        for (var i = 0; i < 100; i++)
        {
            var rndPos = RandomPosOnArena(5f);
            var minDistSquared = Mathf.Infinity;
            foreach (var existingPos in existingRunes)
            {
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

    public void SetStage(GameStage newStage)
    {
        switch (newStage)
        {
            case GameStage.IN_LEVEL:
            {
                foreach (var rune in runeContainer.GetComponentsInChildren<RuneController>())
                {
                    Destroy(rune.gameObject);
                }
                levelWaveQueue.Clear();
                for (var n = 0; n < 1; n++)
                {
                    levelWaveQueue.Add(new Wave(5, sheepPrefab, 10));
                    levelWaveQueue.Add(new Wave(2, foxPrefab, 1));
                }

                spawnNextWaveTime = 0;
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
}
