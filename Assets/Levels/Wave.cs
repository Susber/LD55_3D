using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Win32.SafeHandles;
using UnityEngine;

    public class Wave
    {
        public GameObject enemyPrefab;
        public int enemyAmount;
        public float spawnTime;

        public Wave(GameObject enemyPrefab, int enemyAmount, float spawnTime)
        {
            this.enemyPrefab = enemyPrefab;
            this.enemyAmount = enemyAmount;
            this.spawnTime = spawnTime;
        }

        // public async void DoSpawn()
        // {
        //     var spawnCenter = ArenaController.Instance.RandomEmptyPos(4, 10);
        //     
        //     var dir = spawnCenter - (Vector2) PlayerController.Instance.transform.position;
        //     var angle = Mathf.Atan2(dir.y, dir.x);
        //
        //     var n = 0;
        //     var dist = 2;
        //     for (var row = 0; n < enemyAmount; row++)
        //     {
        //         for (var height = 0; height < row && n < enemyAmount; height++)
        //         {
        //             var spawnPos = new Vector2(row * dist, ((row - 1) - 2 * height) * dist);
        //             spawnPos = spawnCenter + Util.Rotate2d(spawnPos, angle);
        //             ArenaController.Instance.SpawnEnemy(enemyPrefab, spawnPos);
        //             n++;
        //             await Task.Delay(200);
        //         }
        //     }
        // }
    }