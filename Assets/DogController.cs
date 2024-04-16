
using System;
using System.Collections;
using System.Collections.Generic;
using Components;
using UnityEngine;
using UnityEngine.UI;
using Unity.VisualScripting;

public class DogController : MonoBehaviour
{
    public float normalSpeed;
    public float rageSpeed;
    public bool inRage = false;
    
    public GameObject[] graphicStates;

    public float rageDistance;

    private void Start()
    {
        SetGraphicState(0);
    }

    void SetGraphicState(int stateIndex)
    {
        for (var i = 0; i < graphicStates.Length; i++)
        {
            graphicStates[i].SetActive(i == stateIndex);
        }
    }
    
    void FixedUpdate()
    {    
        GetComponent<Rigidbody>().velocity *= 0.99f;
        var from = this.transform.position;
        var to = PlayerController.Instance.transform.position;
        inRage = (to - from).magnitude < rageDistance;
        SetGraphicState(inRage ? 1 : 0);
        var currentSpeed = inRage ? rageSpeed : normalSpeed;
        
        var dir = (to - from).normalized;
        GetComponent<Rigidbody>().AddForce(currentSpeed * dir);
    }
}
