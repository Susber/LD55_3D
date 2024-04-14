
using System;
using System.Collections;
using System.Collections.Generic;
using Components;
using UnityEngine;
using UnityEngine.UI;

public class FoxMovementController : MonoBehaviour
{
    public enum FoxState
    {
        Idle, MoveTowardsPlayer
    }

    public float speed;

    public FoxState currentState;
    public float stateTime;

    public float stateMaxTime;

    public Rigidbody foxRigidbody;

    public UnitController unitcontroller;

    private void Start()
    {
        currentState = FoxState.Idle;
    }

    void FixedUpdate()
    {    
        foxRigidbody.velocity *= 0.99f;
        stateTime += Time.fixedDeltaTime;
        if (stateTime > stateMaxTime)
        {
            stateTime = 0;
            switch (currentState)
            {
                case FoxState.Idle:
                    currentState = FoxState.MoveTowardsPlayer;
                    break;
                case FoxState.MoveTowardsPlayer:
                    currentState = FoxState.Idle;
                    break;
            }
        }
        
        switch (currentState)
        {
            case FoxState.Idle:
                break;
            case FoxState.MoveTowardsPlayer:
                var from = this.transform.position;
                var to = PlayerController.Instance.transform.position;
                if ((to - from).sqrMagnitude < 1)
                {
                    return;
                }
                var dir = (to - from).normalized;
                foxRigidbody.AddForce(speed * dir);
                break;
        }
    }
}
