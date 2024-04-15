using System;
using System.Collections;
using System.Collections.Generic;
using Components;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class BombController : MonoBehaviour
{
    public float bombScale;

    public float maxTimeToExplode;
    public float timeToExplode;
    public bool exploded = false;

    public Transform renderingComponent;

    public void Init(float bombScale, float maxTimeToExplode)
    {
        this.bombScale = bombScale;
        this.maxTimeToExplode = maxTimeToExplode;
        this.timeToExplode = maxTimeToExplode;
    }
    
    void FixedUpdate()
    {
        if (exploded)
            return;
        timeToExplode -= Time.fixedDeltaTime;
        float scale = Mathf.Lerp(1, this.bombScale, Mathf.Pow(1f - timeToExplode / maxTimeToExplode, 2));
        renderingComponent.localScale = new Vector3(scale, scale, scale);
        if (timeToExplode < 0)
        {
            var explosionPrefab = PlayerController.Instance.gun.explosionPrefab;
            var explosion = Instantiate(explosionPrefab).GetComponent<ExplosionController>();
            explosion.Init(transform.position, bombScale * 5, Color.yellow);
            Destroy(this.gameObject);
            exploded = true;
        }
    }
}
