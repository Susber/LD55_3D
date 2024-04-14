
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
        Attack, MoveTowardsPlayer
    }

    public float speed;

    public FoxState currentState;
    public float stateTime;

    public float stateMaxTime;

    public float shootDistance;

    public Rigidbody foxRigidbody;

    public UnitController unitcontroller;

    private void Start()
    {
        currentState = FoxState.MoveTowardsPlayer;
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
                case FoxState.Attack:
                    currentState = FoxState.MoveTowardsPlayer;
                    break;
                case FoxState.MoveTowardsPlayer:
                    currentState = FoxState.Attack;
                    break;
            }
        }
        
        switch (currentState)
        {
            case FoxState.Attack:
                break;
            case FoxState.MoveTowardsPlayer:
                var from = this.transform.position;
                var to = PlayerController.Instance.transform.position;
                if ((to - from).sqrMagnitude < shootDistance)
                {
                    return;
                }
                var dir = (to - from).normalized;
                foxRigidbody.AddForce(speed * dir);
                break;
        }
    }
}
