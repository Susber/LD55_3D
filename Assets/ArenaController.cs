using System;
using System.Collections;
using System.Collections.Generic;
using Components;
using Random = System.Random;
using UnityEngine;
using UnityEngine.XR;

public class ArenaController : MonoBehaviour
{
    public enum GameStage
    {
        IN_LEVEL,
        UPGRADE
    }
    
    public static ArenaController Instance;
    
    public Transform enemyContainer;
    public GameObject decorationContainer;
    public GameStage currentStage = GameStage.IN_LEVEL;

    public Random rnd = new Random();

    // prefabs
    public GameObject sheepPrefab;
    public Component grassPrefab;

    public List<Wave> levelWaveQueue = new List<Wave>();
    public float spawnNextWaveTime;

    private void Start()
    {
        SetStage(GameStage.IN_LEVEL);
        
        for (var x = 0; x < 5000; x++)
        {
            var enemy = Instantiate(grassPrefab, decorationContainer.transform);
            var randomPos = RandomPos();
            enemy.transform.localPosition = new Vector3(randomPos.x, 0.8f, randomPos.z);
        }

        foreach (Transform t in enemyContainer)
        {
            Destroy(t.gameObject);
        }
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
    
    
    public Vector3 RandomPos()
    {
        //var origin = (Vector3) (transform.localPosition - this.transform.localScale / 2);
        //return origin + new Vector3((float)rnd.NextDouble() * this.transform.localScale.x,0, (float)rnd.NextDouble() * this.transform.localScale.y);
        return new Vector3((float)rnd.NextDouble() * 200,0, (float)rnd.NextDouble() * 200)- new Vector3(100,0,100);
    }
/*
    public Vector3 RandomEmptyPos(float freeCircleRadius, float distToPlayer)
    {
        var freeCircleRadiusSqr = freeCircleRadius * freeCircleRadius;
        for (var numTry = 0; numTry < 20; numTry++)
        {
            var rndPos = RandomPos();
            var minDistSquared = Mathf.Infinity;
            foreach (Transform child in enemyContainer.transform)
            {
                minDistSquared = Mathf.Min(minDistSquared, ((Vector2) child.localPosition - rndPos).sqrMagnitude);
            }

            var playerDist = ((Vector2)PlayerController.Instance.transform.localPosition - rndPos).sqrMagnitude;
            if (minDistSquared >= freeCircleRadiusSqr && playerDist >= distToPlayer)
            {
                return rndPos;
            }
        }
        // fail, relax.
        return RandomEmptyPos(freeCircleRadius - 3, distToPlayer - 3);
    }*/

    private void FixedUpdate()
    {
        // switch (currentStage)
        // {
        //     case GameStage.IN_LEVEL:
        //     {
        //         var hasNextWave = levelWaveQueue.Count > 0;
        //         if (hasNextWave)
        //         {
        //             var nextWave = levelWaveQueue[0];
        //             spawnNextWaveTime += Time.fixedDeltaTime;
        //             if (spawnNextWaveTime >= nextWave.spawnTime)
        //             {
        //                 // spawn next wave
        //                 levelWaveQueue.RemoveAt(0);
        //                 nextWave.DoSpawn();
        //                 spawnNextWaveTime = 0;
        //             }
        //         }
        //         break;
        //     }
        //     case GameStage.UPGRADE:
        //     {
        //         break;
        //     }
        // }
    }

    public GameObject SpawnEnemy(GameObject enemyPrefab, Vector2 pos)
    {
        var enemy = Instantiate(enemyPrefab, enemyContainer);
        enemy.transform.localPosition = pos;
        return enemy;
    }

    public void SetStage(GameStage newStage)
    {
        if (newStage == GameStage.IN_LEVEL) {
            levelWaveQueue.Clear();
            for (var n = 0; n < 5; n++)
            {
                levelWaveQueue.Add(new Wave(sheepPrefab, 10, 5));
            }
            spawnNextWaveTime = 0;
        }

        currentStage = newStage;
    }
}
