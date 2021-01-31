﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace src.UI.HUD.DisplayControllers
{
    public class HealthDisplay : MonoBehaviour
    {
        public TextMeshProUGUI healthText;
        public Slider healthSlider;
        public Image Fill;
        
        public Color maxHealthColor = new Color(31, 235, 248, (float) 255/2);
        public Color minHealthColor = new Color(239, 57, 87, (float) 255/2);

        /*
         * Updates the text and the health bar according to the parameter
         */
        public void SetHUDHealth(int displayHealth)
        {
            Methods.Healthbar.UpdateHealth(healthSlider, displayHealth);
            healthText.text = $"{displayHealth}";
        }
    }
}
