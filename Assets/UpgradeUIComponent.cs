using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeUIComponent : MonoBehaviour
{
    public int[] stats;
    public GameObject[] uiRows;

    public TextMeshProUGUI titleText;
    public Text moneyText;

    public const int Health = 0;
    public const int Speed = 1;
    public const int Weapon = 2;
    public const int SummonGiant = 3;
    public const int SummonBomb = 4;
    public const int SummonShotgun = 5;

    public void UpdateUI()
    {
        for (var statNum = 0; statNum < stats.Length; statNum++)
        {
            foreach (var upgradeButton in uiRows[statNum].GetComponentsInChildren<UpgradeButtonComponent>(true))
            {
                upgradeButton.GetComponent<Button>().interactable = upgradeButton.upgradeLevel == stats[statNum];
                upgradeButton.gameObject.SetActive(upgradeButton.upgradeLevel <= stats[statNum]);
            }
        }

        var player = PlayerController.Instance;
        int currLevel = ArenaController.Instance.currentLevel;
        titleText.text = "Congratulations, you completed level " + (currLevel < 9 ? "0" : "") + currLevel + " / " + ArenaController.Instance.maxLevel + "!";
    }

    public void HandleClick(UpgradeButtonComponent button)
    {
        var player = PlayerController.Instance;
        if (player.coins < button.cost)
        {
            return;
        }
        var myStatNum = -1;
        for (var statNum = 0; statNum < stats.Length; statNum++)
        {
            if (uiRows[statNum].GetComponentsInChildren<UpgradeButtonComponent>().Contains(button))
            {
                myStatNum = statNum;
                break;
            }
        }
        stats[myStatNum] += 1;
        DoUpdateStats();
        player.coins -= button.cost;
        UpdateUI();
        ArenaController.Instance.UpdateHud();
    }

    public void DoUpdateStats()
    {
        PlayerController.Instance.speed = Mathf.Lerp(4, 8.5f, (ArenaController.Instance.upgradeUi.stats[UpgradeUIComponent.Speed] - 1) / 5f);
        if (PlayerController.Instance.gun != null)
            PlayerController.Instance.gun.SetLevel(stats[Weapon]);//cooldown = Mathf.Lerp(0.5f, 0.1f, (stats[Weapon] - 1) / 5f));
    }
}
