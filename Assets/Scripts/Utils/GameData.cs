﻿using System.Collections.Generic;
using System.Linq;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using Scripts.Gamemodes;
using UnityEngine;

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
                AddPlayerToDataIfNotExists(roomPlayer);
            }
        }

        private delegate void PlayerDataCallback(PlayerData playerData);
        private void InitPlayerData(Player roomPlayer, PlayerDataCallback callback)
        {
            PlayerData playerData = new PlayerData();
            playerData.name = roomPlayer.NickName;
            playerData.actorNumber = roomPlayer.ActorNumber;
            playerData.alive = true;

            Utils.GetSpriteFromUrlNoCoroutine((string) roomPlayer.CustomProperties["iconUrl"], (sprite) =>
            {
                playerData.icon = sprite;
                callback(playerData);
            });
        }

        /// <summary>
        /// Adds a player to the Dictionnary if it doesn't already exists
        /// </summary>
        /// <param name="roomPlayer">player to add</param>
        public void AddPlayerToDataIfNotExists(Player roomPlayer)
        {
            InitPlayerData(roomPlayer, (playerData =>
            {
                foreach (var PlayerData in Dictionary)
                {
                    if (PlayerData.Key.ActorNumber == roomPlayer.ActorNumber)
                    {
                        return;
                    }
                }
                
                Dictionary.Add(roomPlayer, playerData);
            }));
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
        public void UpdateDataByPlayer(int playerActorNumber, int kills = -1, int assists = -1, int deaths = -1, int points = -1)
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
            
            // Static data
            var playerData = new PlayerData();
            playerData.name = toUpdate.Key.NickName;
            playerData.actorNumber = toUpdate.Key.ActorNumber;
            playerData.alive = toUpdate.Value.alive;
            playerData.icon = toUpdate.Value.icon;
            
            playerData.kills = kills == -1 ? toUpdate.Value.kills : kills;
            playerData.deaths = deaths == -1 ? toUpdate.Value.deaths : deaths;
            playerData.assists = assists == -1 ? toUpdate.Value.assists : assists;
            playerData.points = points == -1 ? toUpdate.Value.points : points;

            Dictionary[toUpdate.Key] = playerData;
        }
        
        
        /// <summary>
        /// Increments by the specified number the data of the player
        /// </summary>
        /// <param name="playerActorNumber">actor number of the player</param>
        /// <param name="kills">number of kills to add</param>
        /// <param name="assists">number of assists to add</param>
        /// <param name="deaths">number of deaths to add</param>
        public void IncrementDataByPlayer(int playerActorNumber, int kills = -1, int assists = -1, int deaths = -1, int points = -1)
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
            
            // Static data
            var playerData = new PlayerData();
            playerData.name = toUpdate.Key.NickName;
            playerData.actorNumber = toUpdate.Key.ActorNumber;
            playerData.alive = toUpdate.Value.alive;
            playerData.icon = toUpdate.Value.icon;
            
            playerData.kills = kills == -1 ? toUpdate.Value.kills : toUpdate.Value.kills + kills;
            playerData.deaths = deaths == -1 ? toUpdate.Value.deaths : toUpdate.Value.deaths + deaths;
            playerData.assists = assists == -1 ? toUpdate.Value.assists : toUpdate.Value.assists + assists;
            playerData.points = points == -1 ? toUpdate.Value.points : toUpdate.Value.points + points;

            Dictionary[toUpdate.Key] = playerData;
        }


        public List<PlayerData> GetPlayerDataByTeam(byte teamCode)
        {
            var res = new List<PlayerData>();

            foreach (var kvp in Dictionary)
            {
                if (kvp.Key.GetPhotonTeam()?.Code == teamCode)
                {
                    res.Add(kvp.Value);
                }
            }

            return res;
        }

        /// <summary>
        /// Returns a sorted list of the PlayerData belonging to the players of a team
        /// </summary>
        /// <param name="teamCode">teamcode used to retreive players team</param>
        /// <returns>Sorted PlayerData list by points</returns>
        public List<PlayerData> GetSortedPlayerDataByTeam(byte teamCode)
        {
            var res = GetPlayerDataByTeam(teamCode);
            
            res.Sort((p1, p2) => p1.points.CompareTo(p2.points));

            return res;
        }
        
        /// <summary>
        /// Returns a sorted list of every PlayerData
        /// </summary>
        /// <returns>sorted list of PlayerData</returns>
        public List<PlayerData> GetSortedPlayerData()
        {
            var res = new List<PlayerData>(Dictionary.Values);
            
            res.Sort((p1, p2) => p2.points.CompareTo(p1.points));

            return res;
        }

        /// <summary>
        /// Returns the PlayerData of a single player
        /// </summary>
        /// <param name="ActorNumber">ActorNumber or Id corresponding to the player</param>
        /// <returns>PlayerData of the wanted player</returns>
        public PlayerData GetSinglePlayerData(int ActorNumber)
        {
            foreach (var data in Dictionary)
            {
                if (data.Key.ActorNumber == ActorNumber)
                {
                    return data.Value;
                }
            }

            return new PlayerData();
        }

        public void SetPlayerState(int ActorNumber, bool alive)
        {
            KeyValuePair<Player, PlayerData> toUpdate;

            foreach (var PlayerData in Dictionary)
            {
                var Player = PlayerData.Key;

                if (Player.ActorNumber == ActorNumber)
                {
                    toUpdate = PlayerData;
                }
            }

            // Static data
            var playerData = new PlayerData();
            playerData.name = toUpdate.Key.NickName;
            playerData.actorNumber = toUpdate.Key.ActorNumber;
            playerData.alive = alive;
            playerData.icon = toUpdate.Value.icon;
            
            
            playerData.kills = toUpdate.Value.kills;
            playerData.deaths = toUpdate.Value.deaths;
            playerData.assists = toUpdate.Value.assists;
            playerData.points = toUpdate.Value.points;
            
            Dictionary[toUpdate.Key] = playerData;
        }

        public bool IsEveryPlayerDead(byte teamCode = 3)
        {
            if (teamCode == 3)
            {
                return Dictionary.Values.ToList().TrueForAll(data => !data.alive);
            }

            return GetPlayerDataByTeam(teamCode).TrueForAll(data => !data.alive);
        }
        
        public List<PlayerData> GetPlayerDataAlive(byte teamCode = 3)
        {
            if (teamCode == 3)
            {
                return Dictionary.Values.ToList().FindAll(data => !data.alive);
            }

            return GetPlayerDataByTeam(teamCode).FindAll(data => !data.alive);
        }
    }

    /// <summary>
    /// Structure used to hold the data of the players during a game
    /// </summary>
    public struct PlayerData
    {
        public int actorNumber;
        public string name;
        public Sprite icon;
        public bool alive;
        public int kills;
        public int assists;
        public int deaths;
        public int points;
    }
}