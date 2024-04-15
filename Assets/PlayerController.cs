using System;
using System.Collections;
using System.Collections.Generic;
using Components;
using Unity.VisualScripting;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;
    
    public float speed;
    public Camera playercamera;
    public GameObject gunPrefab;
    public GunController gun;
    public float suckCoinDistance;

    public Rigidbody playerrigidbody;

    public int coins = 0;

    public float invulnerableTimeLeft = 0;
    public float invulnerableTimeAfterHit;

    public PlayerController()
    {
        Instance = this;
    }

    public void Start()
    {
        gun = Instantiate(gunPrefab,this.transform).GetComponent<GunController>();
        gun.holder = this;
    }

    private void FixedUpdate()
    {
        playerrigidbody.velocity *= 0.8f;
        if (invulnerableTimeLeft > 0)
            invulnerableTimeLeft -= Time.fixedDeltaTime;
    }

    void Update()
    {
        Vector3 force = new Vector3(0,0,0);
        if (Input.GetKey(KeyCode.W))
        {
            force += speed * new Vector3(0,0,1);
        }
        if (Input.GetKey(KeyCode.A))
        {
            force += -speed * new Vector3(1,0,0);
        }
        if (Input.GetKey(KeyCode.D))
        {
            force += +speed * new Vector3(1,0,0);
        }
        if (Input.GetKey(KeyCode.S))
        {
            force += -speed * new Vector3(0,0,1);
        }

        this.transform.position += Time.deltaTime * force;
        // playerrigidbody.AddForce(force);  // a bit cursed?
        playercamera.transform.position = new Vector3(this.transform.position.x, this.playercamera.transform.position.y, this.transform.position.z-12);
    }


    public void Damage()
    {
        if (invulnerableTimeLeft > 0)
        {
            return;
        }
        var upgrades = ArenaController.Instance.upgradeUi;
        upgrades.stats[UpgradeUIComponent.Health] -= 1;
        if (upgrades.stats[UpgradeUIComponent.Health] <= 0)
        {
            upgrades.stats[UpgradeUIComponent.Health] = 0;
            // todo, death sequence!
        }
        invulnerableTimeLeft = invulnerableTimeAfterHit;
        ArenaController.Instance.UpdateHud();
    }

    public int GetHealth()
    {
        return ArenaController.Instance.upgradeUi.stats[UpgradeUIComponent.Health];
    }
}
