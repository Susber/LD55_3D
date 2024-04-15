using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeUIComponent : MonoBehaviour
{
    public int[] stats;
    public GameObject[] uiRows;

    public Text titleText;
    public Text moneyText;

    public const int Health = 0;
    public const int Speed = 1;
    public const int Weapon = 2;
    public const int Meteor = 3;
    public const int Minions = 4;
    public const int Poop = 5;

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
        titleText.text = "Congratulations, you completed level " + ArenaController.Instance.currentLevel + " / " + ArenaController.Instance.maxLevel + "!";
        moneyText.text = "Money left: " + player.coins;
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
        ArenaController.Instance.upgradeUi.stats[myStatNum] += 1;
        player.coins -= button.cost;
        UpdateUI();
        ArenaController.Instance.UpdateHud();
    }
}
