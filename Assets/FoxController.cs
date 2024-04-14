
using System;
using System.Collections;
using System.Collections.Generic;
using Components;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using Unity.VisualScripting;

public class FoxMovementController : MonoBehaviour
{
    public enum FoxState
    {
        Attack, MoveTowardsPlayer
    }
    
    public float speed;

    public FoxState currentState;

    public bool attackFinished = false;
    public float shootDistance;

    public Rigidbody foxRigidbody;

    public UnitController unitcontroller;

    public GameObject[] tails;

    private void Start()
    {
        currentState = FoxState.MoveTowardsPlayer;
        SetTail(0);
    }

    void SetTail(int tailIndex)
    {
        for (var i = 0; i < tails.Length; i++)
        {
            tails[i].SetActive(i == tailIndex);
        }
        
    }

    void FixedUpdate()
    {    
        foxRigidbody.velocity *= 0.99f;
        switch (currentState)
        {
            case FoxState.MoveTowardsPlayer:
                var from = this.transform.position;
                var to = PlayerController.Instance.transform.position;
                if ((to - from).sqrMagnitude < shootDistance)
                {
                    currentState = FoxState.Attack;
                    attackFinished = false;
                    DoAttack();
                    return;
                }
                var dir = (to - from).normalized;
                foxRigidbody.AddForce(speed * dir);
                break;
            case FoxState.Attack:
                if (attackFinished)
                {
                    currentState = FoxState.MoveTowardsPlayer;
                }
                break;
        }
    }

    public async void DoAttack()
    {
        SetTail(1);
        await Task.Delay(300);
        SetTail(2);
        await Task.Delay(300);
        SetTail(3);
        // shoot ...
        await Task.Delay(300);
        attackFinished = true;
        SetTail(2);
        await Task.Delay(300);
        SetTail(1);
        await Task.Delay(300);
        SetTail(0);
        await Task.Delay(300);

    }
}
