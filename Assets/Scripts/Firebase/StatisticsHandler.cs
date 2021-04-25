﻿using System;
using System.Collections.Generic;
using Firebase.Data;
using Photon.Pun.UtilityScripts;
using Proyecto26;
using Scripts;
using UnityEngine;
using String = Firebase.Data.String;

namespace Firebase
{
    public class StatisticsHandler : DatabaseHandler
    {
        public delegate void PostNewMatchCallback(bool success, FirebaseMatchDocument responseDocument = null);

        /// <summary>
        /// Inserts a new Match document in the Firestore database
        /// </summary>
        /// <param name="gameMode">gamemode of the match to be inserted</param>
        /// <param name="winner">winner of the match to be inserted</param>
        /// <param name="gameData">GameData from the match</param>
        /// <param name="callback">callaback with the inserted document and status</param>
        public static void PostNewMatch(string gameMode, string winner, GameData gameData, PostNewMatchCallback callback)
        {
            FirebaseMatchDocument document = new FirebaseMatchDocument
            {
                Name = "",
                CreateTime = DateTimeOffset.Now,
                UpdateTime = DateTimeOffset.Now,
                Fields = new FirebaseMatchDocumentFields
                {
                    Type = new String{ StringValue = gameMode },
                    Winner = new String{ StringValue = winner },
                    EndDate = new Date{ TimestampValue = DateTimeOffset.Now },
                    Players = new Players
                    {
                        ArrayValue = new ArrayValue
                        {
                            Values = new List<Value>()
                        }
                    }
                }
            };

            foreach (var kvp in gameData.Dictionary)
            {
                var team = kvp.Key.GetPhotonTeam();
                
                var value = new Value
                {
                    MapValue = new MapValue
                    {
                        Fields = new MapValueFields
                        {
                            Uid = new Data.String{ StringValue = "" },
                            Name = new Data.String{ StringValue = kvp.Key.NickName },
                            Assists = new Number{ IntegerValue = kvp.Value.assists },
                            Deaths = new Number{ IntegerValue = kvp.Value.deaths },
                            Kills = new Number{ IntegerValue = kvp.Value.kills },
                            Points = new Number{ IntegerValue = kvp.Value.points },
                            Team = team != null ? new Number {IntegerValue = team.Code} : new Number {IntegerValue = 0},
                        }
                    }
                };

                document.Fields.Players.ArrayValue.Values.Add(value);
            }
            
            string documentStr = Serialize<FirebaseMatchDocument>.ToJson(document);

            AuthHandler.GetIdToken(token =>
            {
                var options = new RequestHelper();
                options.BodyString = documentStr;
                options.Headers.Add("Authorization", "Bearer " + token);
                options.Uri = $"{DatabaseUrl}/projects/{ProjectId}/databases/{DatabaseId}/documents/matches";
                
                RestClient.Post(options)
                    .Then(res =>
                    {
                        if (res.StatusCode == 200)
                        {
                            callback(true, FirebaseMatchDocument.FromJson(res.Text));
                        }
                        else
                        {
                            Debug.Log(res.StatusCode);
                            Debug.Log(res.Error);
                            Debug.Log(res.Text);
                        }
                    });
            });
        }


        public delegate void RegisterMatchCallback(bool success);

        /// <summary>
        /// Associate a match with a user in its document
        /// References the matchDocument in the userDocument
        /// </summary>
        /// <param name="matchDocumentId">match document to reference</param>
        /// <param name="callback">callback with status</param>
        public static void RegisterMatch(string matchDocumentId, RegisterMatchCallback callback)
        {
            AuthHandler.GetIdToken(token =>
            {
                var userId = PlayerPrefs.GetString(PlayerPrefKeys.LoggedUserId);
                
                GetUserById(userId, user =>
                {
                    var document = new FirebaseUserDocument();

                    // Add current matches to new array value
                    foreach (var str in user.MatchHistory)
                    {
                        document.Fields.MatchHistory.ArrayValue.Values.Add(
                            new String{ StringValue = str });
                    }
                    
                    // add new match
                    document.Fields.MatchHistory.ArrayValue.Values.Add(
                        new String{ StringValue = matchDocumentId });

                    var documentStr = Serialize<FirebaseUserDocument>.ToJson(document);
                    
                    Debug.Log(documentStr);
                    
                    RestClient.Request(new RequestHelper
                    {
                        Uri = $"{DatabaseUrl}/projects/{ProjectId}/databases/{DatabaseId}/documents/users/{userId}" +
                              $"?updateMask.fieldPaths=matchHistory",
                        Method = "PATCH",
                        BodyString = documentStr,
                        Headers = new Dictionary<string, string>
                        {
                            {"Authorization", "Bearer " + token}
                        }
                    }).Then(res =>
                    {
                        if (res.StatusCode == 200)
                        {
                            callback(true);
                        }
                        else
                        {
                            Debug.Log(res.StatusCode);
                            Debug.Log(res.Error);
                            Debug.Log(res.Text);
                            callback(false);
                        }
                    }).Catch(err =>
                    {
                        Debug.LogErrorFormat($"Firebase.StatisticsHandler: Exception when trying to register a new match for the user {userId}: {err}");
                        callback(false);
                    });
                });
            });
        }
    }
}