using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using Firebase.Database;
using System.Threading.Tasks;
using Unity.VisualScripting;
using System;

public class LeaderboardManager : MonoBehaviour
{
    private static LeaderboardManager instance;
    public static LeaderboardManager Instance => instance;

    private DatabaseReference leaderboardRef;

    private List<LeaderboardData> cachedLeaderboard;
    public List<LeaderboardData> CachedLeaderboard => cachedLeaderboard;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private async UniTaskVoid Start()
    {
        await FireBaseInitializer.Instance.WaitForInitializationAsync();

        leaderboardRef = FirebaseDatabase.DefaultInstance.RootReference.Child("leaderboard");

        Debug.Log("[Leaderboard] End Init");

        await LoadLeaderboardAsync();

        leaderboardRef.ValueChanged += OnLeaderboardChanged;
    }

    private void OnLeaderboardChanged(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError("[Leaderboard] leaderboard error");
        }

        cachedLeaderboard = new List<LeaderboardData>();

        foreach (DataSnapshot child in args.Snapshot.Children)
        {
            string json = child.GetRawJsonValue();
            var data = LeaderboardData.FromJson(json);

            cachedLeaderboard.Add(data);
        }

        Debug.Log("[Leaderboard] Live Update Leaderboard");
    }

    public async UniTask UpdateLeaderboardAsync(int score)
    {
        if (!AuthManager.Instance.IsLoggedIn)
        {
            return;
        }

        var profile = ProfileManager.Instance.CachedProfile;
        var uid = AuthManager.Instance.UserId;

        try
        {
            DataSnapshot snapshot = await leaderboardRef.Child(uid).GetValueAsync().AsUniTask();

            if(snapshot.Exists)
            {
                var newScoreData = new Dictionary<string, object>();
                newScoreData.Add("nickname", profile.nickname);
                newScoreData.Add("score", score);
                await leaderboardRef.Child(uid).UpdateChildrenAsync(newScoreData).AsUniTask();
            }
            else
            {
                DatabaseReference newLeaderBoardRef = leaderboardRef.Push();

                var boardData = new Dictionary<string, object>();
                boardData.Add("nickname", profile.nickname);
                boardData.Add("score", score);
                boardData.Add("timestamp", ServerValue.Timestamp);

                await newLeaderBoardRef.UpdateChildrenAsync(boardData).AsUniTask();
            }
        }
        catch (System.Exception ex)
        {
            Debug.Log(ex.Message);
        }
    }

    public async UniTask<int> LoadLeaderboardAsync(int limit = 10)
    {
        if (!AuthManager.Instance.IsLoggedIn)
        {
            return 0;
        }

        cachedLeaderboard = new List<LeaderboardData>();

        try
        {
            Debug.Log("[Leaderboard] Load Leaderboard Start");

            Query query = leaderboardRef.OrderByChild("score").LimitToLast(limit);
            DataSnapshot snapshot = await query.GetValueAsync().AsUniTask();

            if (snapshot.Exists)
            {
                foreach (DataSnapshot child in snapshot.Children)
                {
                    string json = child.GetRawJsonValue();
                    var data = LeaderboardData.FromJson(json);

                    cachedLeaderboard.Add(data);
                }
            }

            Debug.Log("[Leaderboard] Load Leaderboard End");
        }
        catch (System.Exception ex)
        {
            Debug.Log($"[Leaderboard] Fail Load {ex.Message}");
        }

        return 0;
    }
}
