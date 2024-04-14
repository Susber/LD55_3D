using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeButtonComponent : MonoBehaviour
{
    public int upgradeLevel;

    void Start()
    {
        GetComponent<Button>().onClick.AddListener(DoClick);
    }

    void DoClick()
    {
        ArenaController.Instance.upgradeUi.HandleClick(this);
    }
}
