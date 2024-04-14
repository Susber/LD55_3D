using System.Collections;
using System.Collections.Generic;
using Components;
using Unity.VisualScripting;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerController : UnitController
{
    public static PlayerController Instance;
    public Component bulletPrefab;
    
    public float speed;
    public Camera playercamera;

    public PlayerController()
    {
        Instance = this;
    }
    
    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();
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

        Push(force);
        playercamera.transform.position = new Vector3(this.transform.position.x, this.playercamera.transform.position.y, this.transform.position.z-5);//tilt 25, shift 5
        
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
