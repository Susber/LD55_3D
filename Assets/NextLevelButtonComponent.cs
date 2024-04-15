using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NextLevelButtonComponent : MonoBehaviour
{
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(DoClick);
    }

    void DoClick()
    {
        ArenaController.Instance.upgradeUi.gameObject.SetActive(false);
        ArenaController.Instance.StartPlayingNextLevel();
    }
}
