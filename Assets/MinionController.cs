using System;
using System.Collections;
using System.Collections.Generic;
using Components;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class MinionController : MonoBehaviour
{
    
    public Transform renderingContainer;
    public SpriteRenderer spriteRenderer;

    private float speed; // set by player speed

    public float stateTime;

    public float stateMaxTime;

    public Rigidbody minionrigidbody;

    public UnitController unitcontroller;


    public Rigidbody playerrigidbody;
    private int strength;
    public UnitController target = null;
    public GunController gun;

    private float lifetime = 100;
    private Boolean invincible = false;

    private void Start()
    {
        
    }

    public void Init(int strength2, Vector3 pos, float lifetime)
    {
        this.minionrigidbody = GetComponent<Rigidbody>();
        spriteRenderer = GetComponentsInChildren < SpriteRenderer>()[0];
        unitcontroller = GetComponent<UnitController>();
        speed = PlayerController.Instance.speed * 1.2f;
        strength = strength2;
        unitcontroller.life = strength2 * 1000;
        
        this.minionrigidbody.position = pos;
        gun = Instantiate(ArenaController.Instance.gunPrefab, renderingContainer).GetComponent<GunController>();
        gun.Init(this.minionrigidbody, false, GunController.Guntype.Rocketlauncher);
        gun.SetGuntype(GunController.Guntype.Rocketlauncher);
        gun.SetLevel(strength);
        if (this.lifetime < 0)
            invincible = true;
    }
    
    void FixedUpdate()
    {    
        
        if (PlayerController.Instance is not null)
        {
            var from = this.transform.position;
            var to = PlayerController.Instance.playerrigidbody.position;
            
            //FlipSprite(sheepRigidbody.velocity.x > 0);

            if ((to - from).sqrMagnitude < 1)
            {
                return;
            }
            var dif = (to - from);
            var dir = dif.normalized;
            var dist_sqr = dif.sqrMagnitude;
            
            var walkspeed = new Vector3(0, 0, 0);
            if(dist_sqr > 5*5)
                walkspeed = speed * dir;
            unitcontroller.Walk(walkspeed, 0.3f);
        }

        if (target.IsDestroyed())
            target = null;
        
        if (target is not null)
        {
            var targetposition = target.transform.position;
            gun.TryShootAt(targetposition);
            float distance = (this.minionrigidbody.position - targetposition).sqrMagnitude;
            if (distance > 20 * 20)
                target = null;
        }
        else
        {
            target = ArenaController.Instance.GetClosestEnemyTo(minionrigidbody.position);
        }

        lifetime -= Time.deltaTime;
        if(lifetime <= 0)
            Destroy(this.gameObject);

    }
}