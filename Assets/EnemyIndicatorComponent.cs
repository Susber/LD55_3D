using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Components;
using UnityEditor.Il2Cpp;
using UnityEngine;
using UnityEngine.UI;
using Plane = UnityEngine.Plane;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class EnemyIndicatorComponent : MonoBehaviour
{
    public GameObject indicatorPrefab;
    public GameObject indicator;

    void Start()
    {
        indicator = Instantiate(indicatorPrefab, GameObject.Find("IndicatorPanel").transform);
    }

    public float fullAlpha = 1f;
    public float lastAlpha = 1f;

    public bool HasEnemyOnScreen()
    {
        var playerCamera = PlayerController.Instance.playercamera;
        foreach (var entity in ArenaController.Instance.enemyContainer.GetComponentsInChildren<UnitController>())
        {
            var vec = playerCamera.WorldToScreenPoint(entity.transform.position);
            Debug.Log(vec);
            if (vec.x >= 0 && vec.y >= 0 && vec.x <= playerCamera.pixelWidth && vec.y <= playerCamera.pixelHeight)
            {
                return true;
            }
        }

        return false;
    }
    
    void Update()
    {
        var playerCamera = PlayerController.Instance.playercamera;

        var playerScreen = playerCamera.WorldToScreenPoint(PlayerController.Instance.transform.position);
        var toOrig = playerCamera.WorldToScreenPoint(transform.position) - playerScreen;
        var to = toOrig + Vector3.zero;
        to.x /= 0.5f * playerCamera.pixelWidth;
        to.y /= 0.5f * playerCamera.pixelHeight;
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
            // var pixelPos = new Vector3((renderAt.x / 2 + 0.5f) * playerCamera.pixelWidth,
            // (renderAt.y / 2 + 0.5f) * playerCamera.pixelHeight, 0);
            var pixelPos = playerScreen + 200 * toOrig.normalized;

            indicator.transform.position = pixelPos;
            var angle = Mathf.Atan2(toOrig.y, toOrig.x);
            var image = indicator.GetComponentInChildren<Image>();
            image.transform.rotation = Quaternion.Euler(0, 0, angle * 180 / Mathf.PI);

            var alpha = HasEnemyOnScreen() ? 0f : fullAlpha;
            alpha = Mathf.MoveTowards(lastAlpha, alpha, 4f * Time.deltaTime);
            lastAlpha = alpha;
            image.color = new Color(image.color.r, image.color.g, image.color.b, alpha);
        }
    }

    private void OnDestroy()
    {
        Destroy(indicator);
    }
}
