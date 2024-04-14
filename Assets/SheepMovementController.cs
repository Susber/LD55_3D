using System;
using System.Collections;
using System.Collections.Generic;
using Components;
using UnityEngine;

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

    private void Start()
    {
        currentState = SheepState.Idle;
    }

    void FixedUpdate()
    {
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
                if ((to - from).sqrMagnitude < 1)
                {
                    return;
                }
                var dir = (to - from).normalized;
                GetComponent<UnitController>().Push(speed * dir);
                break;
        }
    }
}
