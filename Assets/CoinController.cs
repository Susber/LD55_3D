using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinController : MonoBehaviour
{
    public float suckForce;
    
    void FixedUpdate()
    {
        var player = PlayerController.Instance;
        if ((transform.position - player.transform.position).magnitude <= player.suckCoinDistance)
        {
            this.GetComponent<Rigidbody>().AddForce(suckForce * (player.transform.position - transform.position).normalized);
        }
    }
    
    void OnTriggerEnter(Collider coll)
    {
        var incomingRigidbody = coll.GetComponent<Rigidbody>();
        if (incomingRigidbody is null)
            return;
        GameObject obstracle = incomingRigidbody.gameObject;
        var player = obstracle.GetComponent<PlayerController>();
        if (player == null)
            return;
        player.coins += 1;
        Destroy(this.gameObject);
    }
}
