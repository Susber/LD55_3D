using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GunController : MonoBehaviour
{
    public PlayerController holder;
    
    public GameObject bulletPrefab;
    public GameObject explosionPrefab;

    public float cooldown = 0.50f;
    public float knockback;
    
    private float max_recoil = 0.4f;
    private Vector3 lastShotDir = new Vector3(0, 0, 0); //direction where the gun is pushed

    private Vector3 playerGunPosition = new Vector3(0, 1, -0.5f); // default gun position

    public float damage;
    public int shootAmount;
    public float shootHalfAngle;  // in degrees

    public float bulletLifetime;

    private float timeout = 0; //current cooldown, 0 means shooting is possible
    // Start is called before the first frame update
    void Start()
    {
        UpdateGunPosition();
    }

    void ShootAt(Vector3 worldpos)
    {
        var direction = worldpos - transform.position;
        direction.y = 0;
        direction = Vector3.Normalize(direction);
        //print("normalized: " + direction);
        lastShotDir = direction;

        float shootHalfAngleInRad = shootHalfAngle / 360 * 2 * Mathf.PI;
        for (var i = 0; i < shootAmount; i++)
        {
            float shotAngle = shootHalfAngleInRad * ((float) ArenaController.Instance.rnd.NextDouble() * 2 - 1);
            var spreadDirection2d = Util.Rotate2d(new Vector2(direction.x, direction.z), shotAngle);
            var spreadDirection = new Vector3(spreadDirection2d.x, 0, spreadDirection2d.y);
            var bullet = Instantiate(bulletPrefab,transform.position, new Quaternion()).GetComponent<BulletController>();
            bullet.SetTeam(false);
            bullet.bulletRigidbody.velocity = spreadDirection * 100;
            bullet.lifetime = bulletLifetime;
        }

        timeout = cooldown;
    }

    // Update is called once per frame
    void Update()
    {
        if(timeout <= 0){
            bool left = Input.GetKey(KeyCode.Mouse0);
            bool right = Input.GetKey(KeyCode.Mouse1);
            if (left || right) {
                float distance;
                Plane plane = new Plane(Vector3.up, 0);
                Ray ray = holder.playercamera.ScreenPointToRay(Input.mousePosition);
                if (plane.Raycast(ray, out distance))
                {
                    Vector3 worldpos = ray.GetPoint(distance);
                    ShootAt(worldpos);
                    if (right)
                    {
                        var explosion = Instantiate(explosionPrefab).GetComponent<ExplosionController>();
                        explosion.Init(worldpos, 5, Color.red);
                    }
                }
            }
        }
        else
        {
            timeout -= Time.deltaTime;
            UpdateGunPosition();
        }
    }

    private void UpdateGunPosition()
    {
        var displacement = max_recoil * timeout / cooldown;
        transform.localPosition = playerGunPosition - displacement * lastShotDir;
        // recoil looks weird and changes depending on angle, reason is scale of player...
    }
}
