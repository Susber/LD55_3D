using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{

    [SerializeField] private Image[] hearts;
    [SerializeField] private Sprite healthyHeart;
	[SerializeField] private Sprite deadHeart;

    public void updateHealthBar(int playerHealth)
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < playerHealth)
            {
                hearts[i].sprite = healthyHeart;
                continue;
            }
            hearts[i].sprite = deadHeart;
        }
    }

}
