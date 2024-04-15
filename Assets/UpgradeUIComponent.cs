using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
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
        DoIncreaseStat(myStatNum);
        player.coins -= button.cost;
        UpdateUI();
        ArenaController.Instance.UpdateHud();
    }

    public void DoIncreaseStat(int statNum)
    {
        switch (statNum)
        {
            case Health:
            case SummonGiant:
            case SummonBomb:
                break; // these are handled indirectly
            case Speed:
                PlayerController.Instance.speed += 1.2f;
                break;
            case Weapon:
                PlayerController.Instance.gun.cooldown = Mathf.Lerp(0.5f, 0.1f, (stats[Weapon] - 1) / 5f);
                break;
            default:
                print("unknown stat? " + statNum);
                break;
        }
        stats[statNum] += 1;
    }
}
