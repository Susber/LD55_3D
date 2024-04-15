using System;
using System.Collections;
using System.Collections.Generic;
using Components;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class MinionController : MonoBehaviour
{
    
    public GameObject gunPrefab;
    public Transform renderingContainer;
    public SpriteRenderer spriteRenderer;

    private float speed; // set by player speed

    public float stateTime;

    public float stateMaxTime;

    public Rigidbody minionrigidbody;

    public UnitController unitcontroller;


    public Rigidbody playerrigidbody;
    private float strength;
    public GameObject target;
    public GunController gun;

    private void Start()
    {
        spriteRenderer = GetComponentsInChildren < SpriteRenderer>()[0];
        unitcontroller = GetComponent<UnitController>();
        target = null;
        
        gun = Instantiate(gunPrefab, renderingContainer).GetComponent<GunController>();
        //gun.SetGuntype(GunController.Guntype.Rocketlauncher);
        gun.Init(this.minionrigidbody);
    }

    private void Init(PlayerController playerController, float strength2)
    {
        playerrigidbody = playerController.playerrigidbody;
        strength = strength2;
        speed = playerController.speed * 1.2f;
    }
    
    void FixedUpdate()
    {    
            var from = this.transform.position;
            var to = playerrigidbody.position;
            
            //FlipSprite(sheepRigidbody.velocity.x > 0);

            if ((to - from).sqrMagnitude < 1)
            {
                return;
            }
            var dir = (to - from).normalized;
            unitcontroller.Walk(speed * dir, 0.5f);
    }
}

