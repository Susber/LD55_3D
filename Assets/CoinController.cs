using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinController : MonoBehaviour
{
    public float suckForce;

    public float aliveTicks = 0;
    public float maxLifetime;
    
    void FixedUpdate()
    {
        var player = PlayerController.Instance;
        if ((transform.position - player.transform.position).magnitude <= player.suckCoinDistance)
        {
            var rigidbody = this.GetComponent<Rigidbody>();
            rigidbody.AddForce(suckForce * (player.transform.position - transform.position).normalized);
            rigidbody.velocity *= 0.98f;
        }

        aliveTicks += Time.deltaTime;
        if (aliveTicks >= maxLifetime)
        {
            Destroy(this.gameObject);
        }
    }
    
    void OnTriggerEnter(Collider coll)
    {
        var incomingRigidbody = coll.GetComponent<Rigidbody>();
        if (incomingRigidbody == null)
            return;
        GameObject obstracle = incomingRigidbody.gameObject;
        var player = obstracle.GetComponent<PlayerController>();
        if (player == null)
            return;
        if (AudioManager.Instance is not null)
            AudioManager.Instance.PlaySoundExp();
        player.coins += 1;
        ArenaController.Instance.UpdateHud();
        Destroy(this.gameObject);
    }
}
