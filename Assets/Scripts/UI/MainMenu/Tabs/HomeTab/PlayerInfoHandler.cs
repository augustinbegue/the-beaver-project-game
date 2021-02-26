﻿using System;
using Firebase.Data;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI.MainMenu.Tabs.HomeTab
{
    public class PlayerInfoHandler : MonoBehaviour
    {
        public TextMeshProUGUI PlayerName;
        public TextMeshProUGUI PlayerElo;
        public TextMeshProUGUI PlayerLevel;
        
        // TODO: Profile picture handling
        public UnityEngine.UI.Image PlayerProfilePicture;

        public User user = Firebase.AuthHandler.loggedinUser;

        private void OnEnable()
        {
            if (user != null)
            {
                PlayerElo.text = $"Elo: {user.Elo}";
                PlayerLevel.text = $"Level: {user.Level}";
                PlayerName.text = user.Username;
            }
            else
            {
                Debug.Log("UI.MainMenu.Tabs.HomeTab.PlayerInfoHandler: No logged User! Going back to auth form");
                SceneManager.LoadScene("Authentication");
            }
        }
    }
}