using System;
using System.Collections;
using System.Collections.Generic;
using Components;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SheepMovementController : MonoBehaviour
{
    public enum SheepState
    {
        Idle, MoveTowardsPlayer
    }

    public float speed;

    public SheepState currentState;
    public float stateTime;

    public float stateMaxTime;

    public Rigidbody sheepRigidbody;

    public UnitController unitcontroller;

    public SpriteRenderer spriteRenderer;

    private void Start()
    {
        currentState = SheepState.Idle;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    
    void FlipSprite(bool right)
    {
        // default: left
        spriteRenderer.flipX = right;
        
    }
    void FixedUpdate()
    {    
        sheepRigidbody.velocity *= 0.99f;
        stateTime += Time.fixedDeltaTime;
        if (stateTime > stateMaxTime)
        {
            stateTime = 0;
            switch (currentState)
            {
                case SheepState.Idle:
                    currentState = SheepState.MoveTowardsPlayer;
                    break;
                case SheepState.MoveTowardsPlayer:
                    currentState = SheepState.Idle;
                    break;
            }
        }
        
        switch (currentState)
        {
            case SheepState.Idle:
                break;
            case SheepState.MoveTowardsPlayer:
                var from = this.transform.position;
                var to = PlayerController.Instance.transform.position;


                FlipSprite(from.x < to.x);

                if ((to - from).sqrMagnitude < 1)
                {
                    return;
                }
                var dir = (to - from).normalized;
                sheepRigidbody.AddForce(speed * dir);
                break;
        }
    }
}
