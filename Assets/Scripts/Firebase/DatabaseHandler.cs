﻿using System.Collections.Generic;
using Firebase.Data;
using Proyecto26;
using UnityEngine;

namespace Firebase
{
    public class DatabaseHandler : MonoBehaviour
    {
        private const string ProjectId = "beaver-ea0ea";
        private const string DatabaseId = "(default)";
        
        public delegate void GetUserCallback(User user);
        
        /// <summary>
        /// Retrieves a user from Firestore with the associated userId
        /// </summary>
        /// <param name="userId">id of the user's document in Firestore</param>
        /// <param name="callback">Callback containing the retrieved data</param>
        /// <returns>Object containing the user's data</returns>
        public static void GetUserById(string userId, GetUserCallback callback)
        {
            RestClient.Get($"https://firestore.googleapis.com/v1/projects/{ProjectId}/databases/{DatabaseId}/documents/users/{userId}")
                .Then(userRes =>
                {
                    var userResponseJson = userRes.Text;

                    var firebaseUserDocument = FirebaseUserDocument.FromJson(userResponseJson);

                    callback(new User(
                        firebaseUserDocument.Fields.Username.StringValue,
                        firebaseUserDocument.Fields.Email.StringValue,
                        firebaseUserDocument.Fields.Birthdate.TimestampValue,
                        null,
                        null,
                        (int) firebaseUserDocument.Fields.Elo.IntegerValue,
                        firebaseUserDocument.Fields.RegisterDate.TimestampValue,
                        (Status) firebaseUserDocument.Fields.Status.IntegerValue,
                        null,
                        (int) firebaseUserDocument.Fields.Level.IntegerValue
                    ));
                })
                .Catch(err =>
                {
                    Debug.LogErrorFormat($"Firebase.DatabaseHandler: Exception when trying to retrieve the user {userId} from the database: {err}");
                    callback(null);
                });
        }

        public delegate void GetAllNewsCallback(List<News> newsList);

        public static void GetAllNews(GetAllNewsCallback callback)
        {
            
            RestClient.Get($"https://firestore.googleapis.com/v1/projects/{ProjectId}/databases/{DatabaseId}/documents/news")
                .Then(userRes =>
                {
                    var userResponseJson = userRes.Text;

                    var firebaseNewsDocumentAll = FirebaseNewsDocumentAll.FromJson(userResponseJson);

                    var newsList = new List<News>();

                    foreach (var firebaseNewsDocument in firebaseNewsDocumentAll.Documents)
                    {
                        var previewImage = firebaseNewsDocument.Fields.PreviewImage?.StringValue != null
                            ? firebaseNewsDocument.Fields.PreviewImage.StringValue
                            : "";
                        
                        newsList.Add(new News(
                            firebaseNewsDocument.Fields.Author.StringValue,
                            firebaseNewsDocument.Fields.Title.StringValue,
                            firebaseNewsDocument.Fields.Content.StringValue,
                            previewImage,
                            firebaseNewsDocument.Fields.Url.StringValue,
                            (int) firebaseNewsDocument.Fields.Likes.IntegerValue));
                    }
                    
                    callback(newsList);
                })
                .Catch(err =>
                {
                    Debug.LogErrorFormat($"Firebase.DatabaseHandler: Exception when trying to get the news: {err}");
                    callback(null);
                });
        }
    }
}