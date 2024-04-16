
using System;
using System.Collections;
using System.Collections.Generic;
using Components;
using UnityEngine;
using UnityEngine.UI;
using Unity.VisualScripting;

public class FoxMovementController : MonoBehaviour
{
    public enum FoxState
    {
        Attack, MoveTowardsPlayer
    }
    
    public float speed;

    public FoxState currentState;

    public float shootDistance;

    public Rigidbody foxRigidbody;

    public UnitController unitcontroller;

    public GameObject[] tails;
    
    public GameObject bulletPrefab;

    public FoxAttackState currentAttackState;

    public float shouldWaitTicks = 0f;

    public Transform fireballSpawnpoint;

    public enum FoxAttackState
    {
        PRE_ATTACK_1,
        PRE_ATTACK_2,
        ATTACK,
        POST_ATTACK_1,
        POST_ATTACK_2,
        POST_ATTACK_3
    }

    private void Start()
    {
        unitcontroller = GetComponent<UnitController>();
        currentState = FoxState.MoveTowardsPlayer;
        SetTail(0);
    }

    void SetTail(int tailIndex)
    {
        if (tails == null)
            return;
        for (var i = 0; i < tails.Length; i++)
        {
            if (tails[i] != null)
                tails[i].SetActive(i == tailIndex);
        }
    }

    void FixedUpdate()
    {    
        //foxRigidbody.velocity *= 0.95f;
        var from = this.transform.position;
        var to = PlayerController.Instance.transform.position;
        if ((to - from).sqrMagnitude < shootDistance)
        {
            if (shouldWaitTicks >= 0)
            {
                shouldWaitTicks -= Time.fixedDeltaTime;
                return;
            }
            DoAttack();
            return;
        }
        else
        {
            SetTail(0);
        }
        var dir = (to - from).normalized;
        unitcontroller.Walk(speed * dir,0.5f);
    }

    public void DoAttack()
    {
        switch (currentAttackState)
        {
            case FoxAttackState.PRE_ATTACK_1:
                SetTail(1);
                shouldWaitTicks = 0.3f;
                currentAttackState = FoxAttackState.PRE_ATTACK_2;
                break;
            case FoxAttackState.PRE_ATTACK_2:
                SetTail(2);
                shouldWaitTicks = 0.3f;
                currentAttackState = FoxAttackState.ATTACK;
                break;
            case FoxAttackState.ATTACK:
                SetTail(3);
                var fireball = Instantiate(bulletPrefab, ArenaController.Instance.decorationContainer).GetComponent<BulletController>();
                var velocity = PlayerController.Instance.transform.position - this.transform.position;
                velocity = Vector3.Normalize(velocity);
                fireball.Init(BulletController.BulletType.Fireball, fireballSpawnpoint.position, velocity * 10, strength2:1, fromEnemy:true);

                shouldWaitTicks = 0.3f;
                currentAttackState = FoxAttackState.POST_ATTACK_1;
                break;
            case FoxAttackState.POST_ATTACK_1:
                SetTail(2);
                shouldWaitTicks = 0.3f;
                currentAttackState = FoxAttackState.POST_ATTACK_2;
                break;
            case FoxAttackState.POST_ATTACK_2:
                SetTail(1);
                shouldWaitTicks = 0.3f;
                currentAttackState = FoxAttackState.POST_ATTACK_3;
                break;
            case FoxAttackState.POST_ATTACK_3:
                SetTail(0);
                shouldWaitTicks = 0.3f;
                currentAttackState = FoxAttackState.PRE_ATTACK_1;
                break;
        }
    }
}
