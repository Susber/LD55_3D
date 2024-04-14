using System;
using System.Collections;
using System.Collections.Generic;
using Components;
using Unity.VisualScripting;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;
    public Component bulletPrefab;
    
    public float speed;
    public Camera playercamera;

    public Rigidbody playerrigidbody;

    public PlayerController()
    {
        Instance = this;
    }

    private void FixedUpdate()
    {
        playerrigidbody.velocity *= 0.8f;
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
        
        /* sorry david!
        bool left = Input.GetKeyDown(KeyCode.Mouse0);
        bool right = Input.GetKeyDown(KeyCode.Mouse1);

        if (left || right) {
            Vector3 worldPosition;
            float distance;
            Plane plane = new Plane(Vector3.down, 0);
            Ray ray = playercamera.ScreenPointToRay(Input.mousePosition);
            if (plane.Raycast(ray, out distance))
            {
                worldPosition = ray.GetPoint(distance);

                var bullet2 = Instantiate(bulletPrefab);
                bullet2.GetComponent<BulletController>().bulletRigidbody.position = worldPosition;
                
                var direction = Vector3.Normalize(transform.position - worldPosition);
                direction.z = 0;
                var bullet = Instantiate(bulletPrefab,unitRigidbody.position, new Quaternion()).GetComponent<BulletController>();
                bullet.bulletRigidbody.velocity = direction * 10;
            }
        }
        */

    }
    
    
}
