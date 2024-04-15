using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Win32.SafeHandles;
using Unity.VisualScripting;
using UnityEngine;

public class Wave : AbstractWave
{
    public GameObject enemyPrefab;
    public int enemyAmount;

    public Wave(float spawnTime, GameObject enemyPrefab, int enemyAmount) : base(spawnTime)
    {
        this.enemyPrefab = enemyPrefab;
        this.enemyAmount = enemyAmount;
    }

    public override async void DoSpawn()
    {
        var spawnCenter3d = ArenaController.Instance.RandomWavePos(ArenaController.Instance.spawnDistanceToPlayer);
        var spawnCenter = new Vector2(spawnCenter3d.x, spawnCenter3d.z);

        var playerPos3d = PlayerController.Instance.transform.position;
        var dir = spawnCenter - new Vector2(playerPos3d.x, playerPos3d.z);
        var angle = Mathf.Atan2(dir.y, dir.x);
    
        var n = 0;
        var dist = 2;
        for (var row = 0; n < enemyAmount; row++)
        {
            for (var height = 0; height < row && n < enemyAmount; height++)
            {
                var spawnPos = new Vector2(row * dist, ((row - 1) - 2 * height) * dist);
                spawnPos = spawnCenter + Util.Rotate2d(spawnPos, angle);
                ArenaController.Instance.SpawnEnemy(enemyPrefab, new Vector3(spawnPos.x, 0, spawnPos.y));
                n++;
                await Task.Delay(200);
            }
        }

        finished = true;
    }
}