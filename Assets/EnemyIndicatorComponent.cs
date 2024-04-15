using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Components;
using UnityEditor.Il2Cpp;
using UnityEngine;
using UnityEngine.UI;
using Plane = UnityEngine.Plane;
using Vector3 = UnityEngine.Vector3;

public class EnemyIndicatorComponent : MonoBehaviour
{
    public GameObject indicatorPrefab;
    public GameObject indicator;

    void Start()
    {
        indicator = Instantiate(indicatorPrefab, GameObject.Find("IndicatorPanel").transform);
    }
    
    void Update()
    {
        var playerCamera = PlayerController.Instance.playercamera;

        var to = new Vector3(playerCamera.pixelWidth, playerCamera.pixelHeight, 0) - playerCamera.WorldToScreenPoint(transform.position);
        to.x /= 0.5f * playerCamera.pixelWidth;
        to.y /= 0.5f * playerCamera.pixelHeight;
        to.y *= -1;  // y is flipped
        to.z = 0;

        var renderAt = new Vector3();
        // screen is now within [-1, 1] x [-1, 1]
        if (Mathf.Abs(to.x) > Mathf.Abs(to.y))
        {
            // top or bottom
            renderAt.x = to.y / to.x; // not sure about the sign ...
            renderAt.y = Mathf.Sign(to.x) * 0.95f;
        }
        else
        {
            // left or right
            renderAt.x = Mathf.Sign(to.y) * 0.95f;
            renderAt.y = to.x / to.y; // not sure about the sign ...
        }

        if (indicator != null)
        {
            var pixelPos = new Vector3((renderAt.x / 2 + 0.5f) * playerCamera.pixelWidth,
            (renderAt.y / 2 + 0.5f) * playerCamera.pixelHeight, 0);
            indicator.transform.position = pixelPos;
            // indicator.GetComponent<RectTransform>().anchoredPosition = ;
        }
    }
}
