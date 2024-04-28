using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeButtonComponent : MonoBehaviour
{
    public int upgradeLevel;
    public int cost;
    public TextMeshProUGUI text;

    void Start()
    {
        GetComponent<Button>().onClick.AddListener(DoClick);
        text.text = "" + cost;
    }

    void DoClick()
    {
        ArenaController.Instance.upgradeUi.HandleClick(this);
    }
}
