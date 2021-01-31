﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace src.UI.HUD
{
    public class TeamDisplay : MonoBehaviour
    {
        public GameObject[] playerContainers = new GameObject[4];

        public void UpdatePlayerHealth(int id, int newHealth)
        {
            if (id < 1 || id > playerContainers.Length)
                return;
            Slider slider = playerContainers[id - 1].GetComponentInChildren<Slider>();
            Controllers.HealthBarController.UpdateHealth(slider, newHealth);
        }
        
        public void UpdatePlayerName(int id, string newName)
        {
            if (id < 1 || id > playerContainers.Length)
                return;
            playerContainers[id - 1].GetComponentInChildren<TextMeshProUGUI>().text = newName.ToUpper();
        }
        
        public void UpdatePlayerIcon(int id, Sprite sprite)
        {
            if (id < 1 || id > playerContainers.Length)
                return;
            playerContainers[id - 1].GetComponentInChildren<UnityEngine.UI.Image>().sprite = sprite;
        }
    }
}

