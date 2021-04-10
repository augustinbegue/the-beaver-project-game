﻿using System.Collections.Generic;
using Photon.Realtime;
using Scripts.Gamemodes;

namespace Scripts
{
    public class GameData
    {
        public Dictionary<Player, PlayerData> Dictionary;

        /// <summary>
        /// Creates a new GameData instance
        /// </summary>
        public GameData()
        {
            Dictionary = new Dictionary<Player, PlayerData>();
        }
        
        /// <summary>
        /// Setup the game data instance with the current players in the room
        /// </summary>
        /// <param name="players">Dictionnary containing the current players in the room</param>
        public void SetupData(Dictionary<int,Player> players)
        {
            Dictionary = new Dictionary<Player, PlayerData>();
            
            foreach (var roomPlayerKVP in players)
            {
                var roomPlayer = roomPlayerKVP.Value;
                var playerData = new PlayerData();
                playerData.name = roomPlayer.NickName;
                
                Dictionary.Add(roomPlayer, playerData);
            }
        }

        /// <summary>
        /// Adds a player to the Dictionnary if it doesn't already exists
        /// </summary>
        /// <param name="roomPlayer">player to add</param>
        public void AddPlayerToDataIfNotExists(Player roomPlayer)
        {
            var playerData = new PlayerData();
            playerData.name = roomPlayer.NickName;

            foreach (var PlayerData in Dictionary)
            {
                if (PlayerData.Key.ActorNumber == roomPlayer.ActorNumber)
                {
                    return;
                }
            }
                
            Dictionary.Add(roomPlayer, playerData);
        }

        /// <summary>
        /// Removes a player from the Dictionnary
        /// </summary>
        /// <param name="roomPlayer"></param>
        public void RemovePlayerFromData(Player roomPlayer)
        {
            Player toRemove = null;

            foreach (var PlayerData in Dictionary)
            {
                if (PlayerData.Key.ActorNumber == roomPlayer.ActorNumber)
                {
                    toRemove = PlayerData.Key;
                }
            }

            Dictionary.Remove(toRemove);
        }

        /// <summary>
        /// Updates the data of a player from its actor number
        /// </summary>
        /// <param name="playerActorNumber">actor number of the player</param>
        /// <param name="kills">new number of kills</param>
        /// <param name="assists">new number of assists</param>
        /// <param name="deaths">new number of deaths</param>
        public void UpdateDataByPlayer(int playerActorNumber, int kills = -1, int assists = -1, int deaths = -1)
        {
            KeyValuePair<Player, PlayerData> toUpdate;

            foreach (var PlayerData in Dictionary)
            {
                var Player = PlayerData.Key;

                if (Player.ActorNumber == playerActorNumber)
                {
                    toUpdate = PlayerData;
                }
            }
            
            var playerData = new PlayerData();
            playerData.name = toUpdate.Key.NickName;
            
            playerData.kills = kills == -1 ? toUpdate.Value.kills : kills;
            playerData.deaths = deaths == -1 ? toUpdate.Value.deaths : deaths;
            playerData.assists = assists == -1 ? toUpdate.Value.assists : assists;

            Dictionary[toUpdate.Key] = playerData;
        }
        
        
        /// <summary>
        /// Increments by the specified number the data of the player
        /// </summary>
        /// <param name="playerActorNumber">actor number of the player</param>
        /// <param name="kills">number of kills to add</param>
        /// <param name="assists">number of assists to add</param>
        /// <param name="deaths">number of deaths to add</param>
        public void IncrementDataByPlayer(int playerActorNumber, int kills = -1, int assists = -1, int deaths = -1)
        {
            if (playerActorNumber == -1)
            {
                return;
            }
            
            KeyValuePair<Player, PlayerData> toUpdate;

            foreach (var PlayerData in Dictionary)
            {
                var Player = PlayerData.Key;

                if (Player.ActorNumber == playerActorNumber)
                {
                    toUpdate = PlayerData;
                }
            }
            
            var playerData = new PlayerData();
            playerData.name = toUpdate.Key.NickName;
            playerData.kills = kills == -1 ? toUpdate.Value.kills : toUpdate.Value.kills + kills;
            playerData.deaths = deaths == -1 ? toUpdate.Value.deaths : toUpdate.Value.deaths + deaths;
            playerData.assists = assists == -1 ? toUpdate.Value.assists : toUpdate.Value.assists + assists;

            Dictionary[toUpdate.Key] = playerData;
        }
    }
    
    /// <summary>
    /// Structure used to hold the data of the players during a game
    /// </summary>
    public struct PlayerData
    {
        public string name;
        public int kills;
        public int assists;
        public int deaths;
    }
}