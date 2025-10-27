using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using Firebase.Database;
using System.Threading.Tasks;
using System;


public class ScoreManager : MonoBehaviour
{
    private static ScoreManager instance;
    public static ScoreManager Instance => instance;

    private DatabaseReference scoreRef;

    private int cachedBestScore = 0;
    public int CachedBestScore => cachedBestScore;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private async UniTaskVoid Start()
    {
        await FireBaseInitializer.Instance.WaitForInitializationAsync();

        scoreRef = FirebaseDatabase.DefaultInstance.RootReference.Child("scores");

        Debug.Log("[Score] End Init");

        await LoadBestScoreAsync();
    }

    private async UniTask<int> LoadBestScoreAsync()
    {
        if (!AuthManager.Instance.IsLoggedIn)
            return 0;

        string uid = AuthManager.Instance.UserId;

        try
        {
            DataSnapshot snapshot = await scoreRef.Child(uid).Child("bestScore").GetValueAsync().AsUniTask();

            if (snapshot.Exists)
            {
                cachedBestScore = int.Parse(snapshot.Value.ToString());
                Debug.Log($"[Score] Load Best Score : {cachedBestScore}");
            }
            else
            {
                cachedBestScore = 0;
                Debug.Log($"[Score] No Best Score");
            }
        }
        catch (System.Exception ex)
        {
            Debug.Log($"[Score] Load Error : {ex.Message}");
        }

        return 0;
    }

    public async UniTask<(bool success, string error)> SaveScoreAsync(int score)
    {
        if (!AuthManager.Instance.IsLoggedIn)
            return (false, "[Score] Need Login");

        string uid = AuthManager.Instance.UserId;

        try
        {
            Debug.Log("[Score] Save Start");

            DatabaseReference historyRef = scoreRef.Child(uid).Child("history");
            DatabaseReference newHistoryRef = historyRef.Push();

            var scoreData = new Dictionary<string, object>();
            scoreData.Add("score", score);
            scoreData.Add("timestamp", ServerValue.Timestamp);

            await newHistoryRef.UpdateChildrenAsync(scoreData).AsUniTask();

            bool shouldUpdateBestScore = false;
            if (cachedBestScore == 0)
            {
                var bestScoreSnapshot = await scoreRef.Child(uid).Child("bestScore").GetValueAsync().AsUniTask();

                if (bestScoreSnapshot.Exists)
                {
                    shouldUpdateBestScore = true;
                }
                else if (score > cachedBestScore)
                {
                    shouldUpdateBestScore = true;
                }
            }
            else if (score > cachedBestScore)
            {
                shouldUpdateBestScore = true;
            }

            if (shouldUpdateBestScore)
            {
                await UpdateBestScoreAsync(score);
            }

            Debug.Log($"[Score] Score Save Success");
            return (true, "[Score] Score Save Success");
        }
        catch (System.Exception ex)
        {
            Debug.Log($"[Score] Save Error : {ex.Message}");
            return (false, ex.Message);
        }
    }

    private async UniTask UpdateBestScoreAsync(int newBestScore)
    {
        if (!AuthManager.Instance.IsLoggedIn)
            return;

        string uid = AuthManager.Instance.UserId;

        try
        {
            await scoreRef.Child(uid).Child("bestScore").SetValueAsync(newBestScore).AsUniTask();
            cachedBestScore = newBestScore;

            Debug.Log($"[Score] New Best Score!! : {newBestScore}");
        }
        catch (System.Exception ex)
        {
            Debug.LogErrorFormat("[Score] Fail Load Best Score : {0}", ex.Message);

        }
    }

    public async UniTask<List<ScoreData>> LoadHistoryAsync(int limit = 10)
    {
        var list = new List<ScoreData>();

        if (!AuthManager.Instance.IsLoggedIn)
            return list;

        string uid = AuthManager.Instance.UserId;

        try
        {
            Debug.Log($"[Score] Load History Start");

            DatabaseReference historyRef = scoreRef.Child(uid).Child("history");
            Query query = historyRef.OrderByChild("timestamp").LimitToLast(limit);

            DataSnapshot snapshot = await query.GetValueAsync().AsUniTask();

            if (snapshot.Exists)
            {
                foreach (DataSnapshot child in snapshot.Children)
                {
                    string json = child.GetRawJsonValue();
                    ScoreData data = ScoreData.FromJson(json);
                    list.Add(data);
                }
            }
            else
            {

            }

            Debug.Log($"[Score] Load History Success");
        }
        catch (System.Exception ex)
        {
            Debug.LogErrorFormat("[Score] Fail Load History : {0}", ex.Message);
        }

        list.Reverse();
        return list;
    }
}
